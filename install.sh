#!/bin/bash
set -e
echo "Adding demodebcli repository..."
echo "deb [trusted=yes] https://damiensawyer.github.io/demodebcli stable main" | sudo tee /etc/apt/sources.list.d/demodebcli.list
sudo apt update
echo "Repository added! Install with: sudo apt install demodebcli"
