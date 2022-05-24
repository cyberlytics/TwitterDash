###
# Compiles the Protobuff and GRPC Stubs for  all services   
###

import os 
import subprocess
import shutil
import json

PROTODIR = "proto"

# Pairs the Service to the Target Directory
with open(os.path.join(os.getcwd(),PROTODIR,"ServiceMap.json")) as json_file:
    SERVICES = json.load(json_file)
    
if __name__ == "__main__":
    
    try:
        subprocess.call(" ".join(["python","-m", "grpc_tools.protoc"]), stdout=subprocess.DEVNULL, stderr=subprocess.DEVNULL)
    except:
        print("The python grpc_tools.protoc module is not installed. Please run 'pip install grpcio-tools' and try again!")
    
    protodir =  f'\"{os.path.join(os.getcwd(),PROTODIR)}\"'
    
    for service in SERVICES:
        outdir = os.path.join(os.getcwd(),SERVICES[service])
        #Check if the Directory Exists
        if os.path.isdir(outdir):
            #Remove it
            shutil.rmtree(outdir)
        #Create the Directory
        os.makedirs(outdir)
        
        outdir = f'\"{outdir}\"'
        # Compile the Proto File
        try:
            service_name = service.split(".")[0]
            grpc_file = os.path.join(SERVICES[service],f"{service_name}_pb2_grpc.py")
            proto_file = os.path.join(SERVICES[service],f"{service_name}_pb2.py")
            
            args = ["python","-m", "grpc_tools.protoc", "-I", protodir, f'--python_out={outdir}',  f'--grpc_python_out={outdir}', service]
            subprocess.call(" ".join(args))

            print(f"Created '{grpc_file}' and '{proto_file}'.")
            
        except Exception as e:
            print(f"Failed to create '{grpc_file}' and '{proto_file}'!")
            print(e)
