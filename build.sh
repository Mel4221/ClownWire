#dotnet publish -c Release -r linux-x64 --self-contained true --output ./publish/linux/ClownWire/
dotnet build && cp -rvf wwwroot/ bin/Debug/net8.0/