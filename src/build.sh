#!/bin/bash

dotnet publish -r linux-x64 --self-contained -p:PublishSingleFile=true -p:PublishReadyToRun=true -p:StripSymbols=true -c Release -o publish Gamerun/Gamerun.csproj
