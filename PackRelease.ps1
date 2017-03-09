cd src/bin/Release
Get-ChildItem *.exe*, *.dll | Compress-Archive -DestinationPath Captura.zip -Force