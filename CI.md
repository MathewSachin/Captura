---
layout: page
title: Continuous Integration
permalink: CI/
---

We use [AppVeyor](https://ci.appveyor.com) for Continuous Integration.

## Build
When a commit is pushed to the GitHub repository, AppVeyor clones the repo and builds it.
The build status can then be seen even from GitHub. This is particularly helpful in preliminary testing of a Pull Request.

The build output is packaged as a Zip file.
License files, BASS and BassMix dlls are included in the build output.

Release build includes the main exe, dll files and config file.
Debug build includes PDB (Symbol Database Files) and XML documentation files in addition to those in Release build.

## Deploy
On Tag builds, the application is deployed to GitHub Releases as a Draft.
Also, Chocolatey package is prepared corresponding to the GitHub Release which can be manually deployed once the GitHub Releases draft is approved.
So, we don't have to manually build and pack the Release.

## Getting the Dev Builds
1. Go to [AppVeyor project]({{ site.links.appveyor }}) page.

2. Select Build Configuration: **Debug** or **Release**.

3. Open **Artifacts** tab.

4. Download **Zip package**.

Dev Builds are available as zip files on the Artifacts tab under a specific configuration like Debug, Release.

**Warning: Dev builds can be unstable and should be used for testing purposes only.**