echo ==================================================
echo      PUBLICANDO A VERSAO DE RELEASE (SELF-CONTAINED)
echo ==================================================

dotnet publish "TicketManager.WPF\TicketManager.WPF.csproj" -c Release -o "TicketManager.WPF\publish" --self-contained true -r win-x64 /p:PublishSingleFile=true

echo.
echo ==================================================
echo      PUBLICACAO CONCLUIDA!
echo ==================================================
echo Os arquivos atualizados estao na pasta 'publish'.
