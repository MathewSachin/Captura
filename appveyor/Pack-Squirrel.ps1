if ($env:configuration -eq 'Release' -and $env:appveyor_repo_tag -eq 'true')
{
    nuget pack "src/Captura.nuspec" -version $env:TagVersion

    . (gci "src/packages/Squirrel.Windows**/Squirrel.exe" -recurse)[0].FullName --releasify "Captura.$env:TagVersion.nupkg" --no-msi
}