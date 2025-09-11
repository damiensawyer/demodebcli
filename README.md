# demodebcli

A demonstration CLI application written in C# and compiled with AOT (Ahead of Time) compilation for optimal performance on Linux systems.

## Features

- **AOT Compiled**: Fast startup and minimal dependencies
- **Simple Interface**: Easy to use command-line interface
- **Version Information**: Built-in version and build date display
- **Man Page**: Complete manual page documentation
- **APT Package**: Easy installation on Debian/Ubuntu systems
- **Auto-updates**: Automatic versioning and upgrade support

## Quick Installation

### Option 1: One-line Installation (Recommended)

```bash
curl -fsSL https://damiensawyer.github.io/demodebcli/install.sh | bash && sudo apt install demodebcli
```

### Option 2: Manual APT Repository Setup

1. **Add the repository:**
   ```bash
   echo "deb [trusted=yes] https://damiensawyer.github.io/demodebcli stable main" | sudo tee /etc/apt/sources.list.d/demodebcli.list
   ```

2. **Update package list:**
   ```bash
   sudo apt update
   ```

3. **Install demodebcli:**
   ```bash
   sudo apt install demodebcli
   ```

### Option 3: Direct .deb Download

1. **Download the latest .deb package:**
   ```bash
   wget https://github.com/damiensawyer/demodebcli/releases/latest/download/demodebcli_*_amd64.deb
   ```

2. **Install the package:**
   ```bash
   sudo dpkg -i demodebcli_*_amd64.deb
   ```

## Usage

### Basic Usage
```bash
# Print "Hello, World!"
demodebcli

# Show version information
demodebcli --version
demodebcli -v
```

### Getting Help
```bash
# View the manual page
man demodebcli
```

### Example Output
```
$ demodebcli
Hello, World!

$ demodebcli --version
demodebcli version 1.0.1
Built: 2025-09-11 11:30:00 UTC
Local time: 2025-09-11 21:30:00
```

## Upgrading

The package supports automatic upgrades through APT:

```bash
sudo apt update
sudo apt upgrade demodebcli
```

## Uninstallation

### Remove the package
```bash
sudo apt remove demodebcli
```

### Remove the repository (optional)
```bash
sudo rm /etc/apt/sources.list.d/demodebcli.list
sudo apt update
```

### Complete cleanup
```bash
sudo apt remove demodebcli
sudo rm /etc/apt/sources.list.d/demodebcli.list
sudo apt update
```

## System Requirements

- **OS**: Debian 10+, Ubuntu 18.04+, or compatible distributions
- **Architecture**: x86_64 (amd64)
- **Dependencies**: libc6 (>= 2.17) - automatically installed

## File Locations

After installation, the following files are installed:

- **Binary**: `/usr/bin/demodebcli`
- **Man page**: `/usr/share/man/man1/demodebcli.1.gz`

## Development

### Building from Source

1. **Prerequisites:**
   - .NET 9.0 SDK
   - Linux x64 system

2. **Clone and build:**
   ```bash
   git clone https://github.com/damiensawyer/demodebcli.git
   cd demodebcli
   dotnet publish -c Release --self-contained -r linux-x64
   ```

3. **Run locally:**
   ```bash
   ./bin/Release/net9.0/linux-x64/publish/demodebcli
   ```

### Project Structure

```
demodebcli/
├── .github/workflows/          # CI/CD workflows
│   ├── build-and-package.yml   # Main build and packaging
│   └── update-repository.yml   # APT repository updates
├── Program.cs                  # Main application code
├── demodebcli.csproj          # Project configuration
├── README.md                  # This file
└── CLAUDE.md                  # Project instructions
```

## CI/CD Pipeline

The project uses GitHub Actions for automated:

1. **Building**: AOT compilation for Linux x64
2. **Packaging**: Creation of .deb packages
3. **Testing**: Installation and functionality tests
4. **Releasing**: Automatic GitHub releases
5. **Repository**: APT repository hosting via GitHub Pages

## Versioning

- **Format**: `MAJOR.MINOR.BUILD`
- **Build numbers**: Auto-incremented with each main branch commit
- **Releases**: Automatically created for main branch builds
- **Preview**: Non-main branches get `-preview` suffix

## Contributing

1. Fork the repository
2. Create a feature branch
3. Make your changes
4. Test locally
5. Submit a pull request

## License

This is a demonstration project. Use as you see fit.

## Support

- **Issues**: [GitHub Issues](https://github.com/damiensawyer/demodebcli/issues)
- **Documentation**: `man demodebcli`
- **Repository**: [https://damiensawyer.github.io/demodebcli](https://damiensawyer.github.io/demodebcli)

---

*Generated with [Claude Code](https://claude.ai/code)*