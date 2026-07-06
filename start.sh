#!/bin/bash

dotnet run --launch-profile http &

sleep 3

xdg-open http://localhost:5048