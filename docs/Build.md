# Building

## Setting up locally

### Prerequisites
- Visual Studio 2019 or newer with .NET desktop development workload.
- .Net Core 2.1 or greater
- Cake tool  
  Install: `dotnet tool install -g Cake.Tool --version 0.32.1`
- Some features have other specific requirements, see [here](https://mathewsachin.github.io/Captura/sys-req).

### Steps
1. Clone  
   `git clone https://github.com/MathewSachin/Captura.git`
2. Setup Api Keys. These are loaded from environment variables during development and embedded into the app on production builds.
   
   Environment Variable | Description
   ---------------------|-------------
   imgur_client_id      | Imgur Client Id
   yt_client_id         | YouTube Client Id
   yt_client_secret     | YouTube Client Secret

   Imgur credentials are only required if you want to upload to Imgur. See [here](https://apidocs.imgur.com/) for more info.

   YouTube credentials are only required if you want to upload to YouTube. See [here](https://developers.google.com/youtube/registering_an_application) for more info.

3. Download FFmpeg from within the app or from https://ffmpeg.zeranoe.com/builds/ or use a custom build.
4. Now, you're good to go. You can build using Visual Studio or the [cake script](Cake.md).