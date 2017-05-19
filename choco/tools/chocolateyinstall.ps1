$params = @{
    'PackageName' = 'Captura';
    'Url' = "https://github.com/MathewSachin/Captura/releases/download/$tag/Captura-Release.zip";
    'UnzipLocation' = "$(Split-Path -parent $MyInvocation.MyCommand.Definition)";
    'Checksum' = $checksum;
    'ChecksumType' = 'sha256';
}

Install-ChocolateyZipPackage @params