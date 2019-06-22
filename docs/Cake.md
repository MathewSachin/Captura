# Cake build script

### Installing Cake
.NET Core 2.1 is required.

```
dotnet tool install -g Cake.Tool --version 0.32.1
```

### Running the build script
```
dotnet-cake
```

### Arguments

Option         | Description
---------------|--------------
-appversion    | Build version. AssemblyInfo.cs files are updated with this value during build.
-configuration | Configuration: Release or Debug
-target        | The Task to run in the build script. See build.cake
-tag           | Used for tag builds by AppVeyor

```
dotnet-cake --target=CI --configuration=Release
```