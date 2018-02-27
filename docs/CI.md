# Continuous Integration

We use [AppVeyor](https://ci.appveyor.com/project/MathewSachin/Captura) for Continuous Integration.

When a commit is pushed to the GitHub repository, AppVeyor clones the repo and builds, tests and deploys it.

## Build

NuGet packages directory is cached until no `packages.config` files change.

The build output along with dependencies is packaged as a Zip file.
License files, BASS and BassMix dlls are downloaded and included in the build output.

On Tag builds, version is determined from the tag name.
Otherwise, `AssemblyInfo.cs` files are used.

Release builds include the main exe, dll files and config file.
Debug builds include PDB (Symbol Database Files) and XML documentation files in addition to those in Release builds.

## Test

ScreenShots of `captura.ui.exe` are taken using `captura shot`.

~~On Tag builds, the ScreenShots on the website are updated.~~

## Deploy

AppVeyor automates the Release process.

Tags are used to indicate a release.
Tag format is verified to match `v{Major}.{Minor}.{Patch}[-{prerelease}]`. When tag does not match this format, build fails.

`AssemblyInfo.cs` files are updated as per the tag.

Api keys for Imgur are written into the app.

The application is deployed to GitHub Releases as a Draft.
Also, Chocolatey package is prepared corresponding to the GitHub Release.
It can be manually deployed once the GitHub Releases draft is published.

## Getting the Dev Builds

1. Go to [AppVeyor project](https://ci.appveyor.com/project/MathewSachin/Captura) page.

2. Select Build Configuration: **Debug** or **Release**.

3. Open **Artifacts** tab.

4. Download **Zip package**.

**Warning: Dev builds can be unstable and should be used for testing purposes only.**