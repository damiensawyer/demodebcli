#!/bin/bash
set -e

echo "Adding demodebcli repository..."

# Add repository
echo "deb [trusted=yes] https://damiensawyer.github.io/demodebcli stable main" | sudo tee /etc/apt/sources.list.d/demodebcli.list

# Update package list
sudo apt update

echo "Repository added successfully!"
echo "You can now install demodebcli with: sudo apt install demodebcli"
