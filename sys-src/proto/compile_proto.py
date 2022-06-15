###
# Compiles the Protobuff and GRPC Stubs for  all services
###

import os
import subprocess
import shutil
import json
import pathlib

PROTO = "proto"
SERVICE_MAP = "ServiceMap.json"
DEPENDENCIES = ["objects.proto"]

def overwrite(source_file:str,target_file:str):
    if os.path.isfile(target_file):
        os.remove(target_file)
    shutil.copy(source_file,target_file)
    
def move_proto_files(proto_dir:pathlib.Path,service:str,target:str,root_dir:pathlib.Path):
    
    source_file = proto_dir / service
    target_dir = root_dir / target
    target_file = target_dir / service
    if not target_dir.is_dir():
        target_dir.mkdir(parents=True, exist_ok=True)
        
    
    overwrite(str(source_file),str(target_file))
    
    for dep in DEPENDENCIES:
        dep_source = proto_dir / dep
        dep_target = target_dir / dep
        overwrite(str(dep_source),str(dep_target))

def call_grpc_tools(protodir:pathlib.Path,target_dir:pathlib.Path,service:str,grpc:bool=True):
    
    def escape_str(input:str)->str:
        return f"\"{input}\""
    args = [
    "python",
    "-m",
    "grpc_tools.protoc",
    "-I",
    escape_str(str(protodir)),
    f"--python_out={escape_str(str(target_dir))}"]
    
    if grpc:
        args.append("--grpc_python_out=" + escape_str(str(target_dir)))
        
    args.append(escape_str(service))
    subprocess.call(" ".join(args))
    
       
def compile_proto_python(proto_dir:pathlib.Path,service:str,target_dir:pathlib.Path,root_dir:pathlib.Path):
    
    source_file = proto_dir / service
    target_dir = root_dir / target
    target_file = target_dir / service
    if not target_dir.is_dir():
        target_dir.mkdir(parents=True, exist_ok=True)
    
    #Create __init__.py
    with open(str(target_dir / "__init__.py"), "w") as _:
        pass
      
    
    generated_dependencies = []
    for dependency in DEPENDENCIES:
        call_grpc_tools(proto_dir,target_dir,dependency,grpc=False)
        generated_dependencies.append(f"{dependency.split('.')[0]}_pb2")
        
    generated_dependencies.append(f"{service.split('.')[0]}_pb2")
       
    call_grpc_tools(proto_dir,target_dir,service)
    
    #Rewrite the generated files to include the dependencies
    for generated_dependency in generated_dependencies:
        for file in [f"{service.split('.')[0]}_pb2.py",f"{service.split('.')[0]}_pb2_grpc.py"]:
            file_to_rewrite = target_dir / file
            lines = []
            with open(file_to_rewrite,"r") as f:
                lines = f.readlines()
                
            for i,line in enumerate(lines):
                if f"import {generated_dependency} as" in line:
                    lines[i] = f"from . import {generated_dependency} as {generated_dependency.replace('_','__')}\n"
                    
            with open(file_to_rewrite,"w") as f:
                f.writelines(lines)
                    
                    
if __name__ == "__main__":

    src_dir = pathlib.Path(__file__).parent.parent.resolve()
    proto_dir = src_dir / PROTO
    
    with open(proto_dir / SERVICE_MAP) as json_file:
        services = json.load(json_file)
    
    for csharp_service in services["C#"]:
        for target in services["C#"][csharp_service]:
            move_proto_files(proto_dir,csharp_service,target,src_dir)
            print("Generated C# Protobufs for " + csharp_service + " to " + target)
            
    
    for node_service in services["NodeJs"]:
        for target in services["NodeJs"][node_service]:
            move_proto_files(proto_dir,node_service,target,src_dir)
            print("Generated NodeJs Protobufs for " + node_service + " to " + target)
            
    for python_service in services["Python"]:
        for target in services["Python"][python_service]:
            compile_proto_python(proto_dir,python_service,target,src_dir)
            print("Generated Python Protobufs for " + python_service + " to " + target)