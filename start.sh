#!/bin/bash

# Script for launching Stupid Chat projects on Linux/macOS
echo -e "\033[32mStarting Stupid Chat...\033[0m"

# Get script directory
SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"

# Function to stop processes when script terminates
cleanup() {
    echo -e "\033[33mStopping processes...\033[0m"
    if [ ! -z "$API_PID" ]; then
        kill $API_PID 2>/dev/null
    fi
    if [ ! -z "$WEB_PID" ]; then
        kill $WEB_PID 2>/dev/null
    fi
    exit 0
}

# Set up signal handlers
trap cleanup SIGINT SIGTERM

# Launch .NET API in background
echo -e "\033[33mStarting .NET API...\033[0m"
cd "$SCRIPT_DIR/api/StupidChat"
dotnet run &
API_PID=$!

# Wait a bit for API to start
sleep 3

# Navigate to web application directory
cd "$SCRIPT_DIR/web"

# Check if node_modules exists
if [ ! -d "node_modules" ]; then
    echo -e "\033[36mInstalling npm dependencies...\033[0m"
    npm install
fi

# Launch Angular application in background
echo -e "\033[33mStarting Angular application...\033[0m"
npm start &
WEB_PID=$!

echo -e "\033[32mApplications started!\033[0m"
echo -e "\033[36mAPI: https://localhost:5298\033[0m"
echo -e "\033[36mWeb: http://localhost:4200\033[0m"
echo -e "\033[33mPress Ctrl+C to stop all processes\033[0m"

# Wait for processes to complete
wait
