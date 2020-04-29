# Cake build script

## Installing Cake

.NET Core 2.1 is required.

```dotnet
dotnet tool install -g Cake.Tool --version 0.32.1
```

## Running the build script

```dotnet
dotnet-cake
```

## Arguments

Option         | Description
---------------|--------------
-build_version | Build version. Should be like v9.0.0 for stable and CI builds <br> and like v9.0.0-beta3 for prerelease builds. AssemblyInfo.csfiles <br> are updated based on this value during build.
-configuration | Configuration: Release or Debug
-target        | The Task to run in the build script. See build.cake

```dotnet
dotnet-cake --target=CI --configuration=Release
```