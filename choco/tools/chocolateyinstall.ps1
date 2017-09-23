$params = @{
    'PackageName' = 'Captura';
    'Url' = "https://github.com/MathewSachin/Captura/releases/download/$tag/Captura-Portable.zip";
    'UnzipLocation' = "$(Split-Path -parent $MyInvocation.MyCommand.Definition)";
    'Checksum' = $checksum;
    'ChecksumType' = 'sha256';
};

Install-ChocolateyZipPackage @params