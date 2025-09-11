#!/bin/bash
set -e

# Check for root privileges
if [ "$(id -u)" -ne 0 ]; then
    echo "This script requires root privileges. Rerunning with sudo..."
    # Rerun this script with sudo
    exec sudo "$0" "$@"
fi

echo "Adding demodebcli repository..."

# Add repository
echo "deb [trusted=yes] https://damiensawyer.github.io/demodebcli stable main" > /etc/apt/sources.list.d/demodebcli.list

# Update package list
apt update

echo "Repository added successfully!"
echo "You can now install demodebcli with: sudo apt install demodebcli"
