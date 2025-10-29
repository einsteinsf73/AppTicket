@echo off
echo This script will install the security certificate for the Ticket Manager application.
echo You will be prompted for administrative privileges.
echo.
certutil -addstore -f "Root" "TicketManager.pfx"
echo.
echo Certificate installed successfully.
pause