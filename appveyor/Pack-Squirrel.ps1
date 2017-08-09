if ($env:configuration -eq 'Release' -and $env:appveyor_repo_tag -eq 'true' -and $env:prerelease -ne 'true')
{
    nuget pack "src/Captura.nuspec" -version $env:TagVersion

    . (Get-ChildItem "src/packages/Squirrel.Windows**/Squirrel.exe" -Recurse)[0].FullName --releasify "Captura.$env:TagVersion.nupkg" --no-msi
}