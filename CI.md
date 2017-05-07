---
layout: page
title: Continuous Integration
permalink: CI/
---

We use [AppVeyor](https://ci.appveyor.com) for Continuous Integration.

When a commit is pushed to the GitHub repository, AppVeyor clones the repo and builds it.
The build status can then be seen even from GitHub. This is particularly helpful in preliminary testing of a Pull Request.

On Tag builds, the application is deployed to GitHub Releases.
So, we don't have to manually build and pack the Release.

## Getting the Dev Builds
Dev Builds are available as zip files on the Artifacts tab under a specific configuration like Debug, Release.

**Warning: Dev builds can be unstable and should be used for testing purposes only.**