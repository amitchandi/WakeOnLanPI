# WakeOnLanPI

**WakeOnLanPI** is a lightweight .NET web application that exposes a simple REST API for sending **Wake-on-LAN (WOL)** magic packets and **pinging servers to monitor their status**.  
Designed for Raspberry Pi (or any .NET-capable machine), it lets you remotely power on and monitor devices on your local network — perfect for home labs, servers, and media PCs.

---

## Features

- ⚡ **Wake-on-LAN API & UI** — Send magic packets to power on devices through REST or the web interface.  
- 🌐 **Web Dashboard** — Clean and lightweight UI to manage and monitor your devices.  
- 🖥️ **Server Monitoring** — Automatically pings servers to show online/offline status.  
- 🧠 **Lightweight & Fast** — Optimized for Raspberry Pi and low-resource environments.  
- 🔧 **Easy Setup** — Simple installation with optional `systemd` service.  
- 🧩 **Cross-Platform** — Runs on any .NET-supported system (Linux, Windows, macOS).

---

## Requirements

- Raspberry Pi (or any device running Linux/Windows)  
- [.NET 9 SDK/Runtime](https://dotnet.microsoft.com/download)  
- A device that supports Wake-on-LAN, with WOL enabled in its BIOS/UEFI and network adapter settings  

---


## Using Prebuilt Release

1. Download the latest `app-linux-arm64.zip` from the [Releases](https://github.com/amitchandi/WakeOnLanPI/releases) page.  
2. Copy or unzip it into a directory on your Raspberry Pi (e.g. `/someuser/wakeonlanpi`).  
3. Make the binary executable (if needed):
```bash
chmod +x WoLPi
```
4. Run
```bash
./WoLPi
```

## Build From Source

### Clone
```bash
git clone https://github.com/amitchandi/WakeOnLanPI.git
cd WakeOnLanPI
```

### Build

```bash
dotnet build
```

### Run
```bash
dotnet run
```
or
```bash
#Hot Reload
dotnet watch
```

### Publish
```bash
dotnet publish -c Release -r linux-arm64 \
  --self-contained false \
  /p:PublishSingleFile=true \
  /p:PublishTrimmed=true \
  -o ./publish
```

### Example systemd service

Runs on port 5000 by default

example_systemd.service
```bash
[Unit]
Description=WakeOnLan Pi Webapp

[Service]
WorkingDirectory=/home/example-user/WoLPi/
ExecStart=/home/example-user/WoLPi/WoLPi
Restart=always
# Restart service after 10 seconds if the dotnet service crashes:
RestartSec=10
KillSignal=SIGINT
SyslogIdentifier=dotnet-WoLPi
User=example-user

Environment=ASPNETCORE_ENVIRONMENT=Production
Environment=DOTNET_NOLOGO=true

# uncomment this to set hostname/IP and port
#Environment=ASPNETCORE_URLS=http://localhost:5000

[Install]
WantedBy=multi-user.target
```

#### Reccomended
Make a designated user for running the app and to also use a reverse proxy for serving the Web App/REST API.

Keep traffic to the app locally or behind a VPN. Exposing the Web App or the REST API is not safe.

```bash
adduser example-user
```
Place the systemd service file in the systemd directory: /etc/systemd/system

*give the service file a memorable name (eg. wakeonlanpi.service)
