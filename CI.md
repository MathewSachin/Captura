---
layout: page
title: Continuous Integration
permalink: CI/
reading_time: true
---

We use [AppVeyor](https://ci.appveyor.com) for Continuous Integration.

When a commit is pushed to the GitHub repository, AppVeyor clones the repo and builds/tests/deploys it.

## Build
The build output along with dependencies is packaged as a Zip file.
License files, BASS and BassMix dlls are downloaded and included in the build output.

Release builds include the main exe, dll files and config file.
Debug builds include PDB (Symbol Database Files) and XML documentation files in addition to those in Release builds.

## Test
ScreenShots of `captura.ui.exe` are taken using `captura shot`.

On Tag builds, the ScreenShots on the website are updated.

## Deploy
AppVeyor automates the Release process.

On Tag builds, the application is deployed to GitHub Releases as a Draft.
Also, Chocolatey package is prepared corresponding to the GitHub Release.
It can be manually deployed once the GitHub Releases draft is published.

## Getting the Dev Builds
1. Go to [AppVeyor project]({{ site.links.appveyor }}) page.

2. Select Build Configuration: **Debug** or **Release**.

3. Open **Artifacts** tab.

4. Download **Zip package**.

**Warning: Dev builds can be unstable and should be used for testing purposes only.**