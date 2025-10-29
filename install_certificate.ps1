# Check for administrator privileges
if (-NOT ([Security.Principal.WindowsPrincipal][Security.Principal.WindowsIdentity]::GetCurrent()).IsInRole([Security.Principal.WindowsBuiltInRole] "Administrator")) {
    Write-Warning "This script must be run as an administrator. Please right-click the script and select 'Run as Administrator'."
    Read-Host "Press Enter to exit"
    exit
}

$pfxPassword = ConvertTo-SecureString -String "TicketManager123" -Force -AsPlainText
Import-PfxCertificate -FilePath ".\TicketManager.pfx" -CertStoreLocation "Cert:\LocalMachine\Root" -Password $pfxPassword -Exportable
Write-Host "Certificate installed successfully."
Read-Host "Press Enter to exit"