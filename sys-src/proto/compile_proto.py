###
# Compiles the Protobuff and GRPC Stubs for  all services
###

import os
import subprocess
import shutil
import json

SRCDIR = "sys-src"
PROTODIR = "proto"

DEPENDENCIES = ["objects.proto"]

# Pairs the Service to the Target Directory
with open(os.path.join(os.getcwd(), SRCDIR, PROTODIR, "ServiceMap.json")) as json_file:
    SERVICES = json.load(json_file)

if __name__ == "__main__":

    try:
        subprocess.call(
            " ".join(["python", "-m", "grpc_tools.protoc"]),
            stdout=subprocess.DEVNULL,
            stderr=subprocess.DEVNULL,
        )
    except:
        print(
            "The python grpc_tools.protoc module is not installed. Please run 'pip install grpcio-tools' and try again!"
        )

    protodir = f'"{os.path.join(os.getcwd(),SRCDIR,PROTODIR)}"'

    for service in SERVICES:
        outdir = os.path.join(os.getcwd(), SRCDIR, SERVICES[service])
        # Check if the Directory Exists
        if os.path.isdir(outdir):
            # Remove it
            shutil.rmtree(outdir)
        # Create the Directory
        os.makedirs(outdir)
        with open(os.path.join(outdir, "__init__.py"), "w") as init_file:
            pass
        outdir = f'"{outdir}"'
        # Compile the Proto File
        try:
            service_name = service.split(".")[0]
            grpc_file = os.path.join(SERVICES[service], f"{service_name}_pb2_grpc.py")
            proto_file = os.path.join(SERVICES[service], f"{service_name}_pb2.py")

            generated_dependencies = []
            # Generate Dependencies
            for dependency in DEPENDENCIES:
                args = [
                    "python",
                    "-m",
                    "grpc_tools.protoc",
                    "-I",
                    protodir,
                    f"--python_out={outdir}",
                    dependency,
                ]
                generated_dependencies.append(f"{dependency.split('.')[0]}_pb2")
                subprocess.call(" ".join(args))

            args = [
                "python",
                "-m",
                "grpc_tools.protoc",
                "-I",
                protodir,
                f"--python_out={outdir}",
                f"--grpc_python_out={outdir}",
                service,
            ]
            subprocess.call(" ".join(args))

            # Adjust the imports to the current location
            def replace_import(file, old_import, new_import):
                lines = []
                with open(file, "r") as f:
                    lines = f.readlines()

                for i, line in enumerate(lines):
                    if old_import in line:
                        lines[i] = line.replace(old_import, new_import)

                with open(file, "w") as f:
                    f.writelines(lines)

            for generated_dependency in generated_dependencies:
                cleaned_outdir = outdir.replace('"', "")
                base_import_path = os.path.basename(os.path.normpath(outdir))
                replace_import(
                    os.path.join(cleaned_outdir, f"{service_name}_pb2_grpc.py"),
                    f"import {generated_dependency} as",
                    f"import {base_import_path[:-1]}.{generated_dependency} as",
                )
                replace_import(
                    os.path.join(cleaned_outdir, f"{service_name}_pb2.py"),
                    f"import {generated_dependency} as",
                    f"import {base_import_path[:-1]}.{generated_dependency} as",
                )

            print(f"Created '{grpc_file}' and '{proto_file}'.")

        except Exception as e:
            print(f"Failed to create '{grpc_file}' and '{proto_file}'!")
            print(e)
