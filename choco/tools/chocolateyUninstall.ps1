$localAppData = [Environment]::GetFolderPath([Environment+SpecialFolder]::LocalApplicationData)
$updateExe = Join-Path $localAppData "Captura" "Update.exe"

Start-Process -FilePath $updateExe -ArgumentList "--uninstall" -Verb "RunAs" -Wait