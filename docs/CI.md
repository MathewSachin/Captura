# Continuous Integration

We use [Appveyor](https://www.appveyor.com/) for CI.
When a commit is pushed to the GitHub repository, AppVeyor clones the repo and builds and tests it using the Cake build script.
If it is a tag build, the release is deployed to GitHub Releases as a draft and to Chocolatey.

### Getting dev builds

> Dev builds can be unstable and should be used for testing purposes only.

1. Go to [AppVeyor project](https://ci.appveyor.com/project/MathewSachin/Captura/branch/master) page.

2. Select Build Configuration: **Debug** or **Release**.

   ![img](https://mathewsachin.github.io/Captura/assets/dev-builds/1.png)

3. Open **Artifacts** tab.

   ![img](https://mathewsachin.github.io/Captura/assets/dev-builds/2.png)

4. Download the Portable or Setup version according to your need.

   ![img](https://mathewsachin.github.io/Captura/assets/dev-builds/3.png)