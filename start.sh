#!/bin/bash

PORT=5048

if lsof -i :$PORT >/dev/null 2>&1; then
    echo "EmployeeDashboard is already running."
else
    echo "Starting EmployeeDashboard..."
    dotnet run --launch-profile EmployeeDashboard &
fi

sleep 3

xdg-open http://localhost:$PORT