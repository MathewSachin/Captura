# Verb: start
Starts Recording.

There are two modes.

When Length is specified, recording runs until specified Length.
When Length is not specified, press q to quit message is displayed.

Argument              | Description
----------------------|-------------------------------------------------------------------------
`--cursor`            | Include cursor
`--keys`              | Include keystrokes
`--clicks`            | Include mouse clicks
`--delay`             | Delay before starting recording (in ms)
`-t` or `--length`    | Length of recording (in s)
`--source`            | The source to record from. See [here](Arg-Source.md).
`--mic`               | The microphone index to use. (-1 = none (Default)) (0 is first device).
`--speaker`           | The speaker output index to use. (-1 = none (Default)) (0 is first device).
`--webcam`            | Webcam to use. (-1 = none (Default)) (0 is first webcam).
`-r` or `--framerate` | Frame Rate (Default is 10).
`--encoder`           | The video encoder to use. See below.
`--vq`                | Video Quality (1 to 100) (Default is 70).
`--aq`                | Audio Quality (1 to 100) (Default is 50).
`-f` or `--file`      | Output file path.
`-y`                  | Overwrite existing file.
`--replay`            | Replay recording. Specify duration in seconds as parameter. e.g. `--replay 20`.

e.g. Record 10 seconds with cursor and keystrokes and audio from first speaker output.

```
captura-cli start --length 10 --cursor --keys --speaker=0
```

## Using the Encoder argument

By default, SharpAvi Motion JPEG encoder is used.

### SharpAvi
Use `sharpavi:<index>` as argument. `index` is a zero-based index identifying the encoder.

You can use `captura-cli list` to check encoder indices.

e.g.

```
captura-cli start --encoder sharpavi:0
```

## Media Foundation
Use `mf` as argument.

e.g.

```
captura-cli start --encoder mf
```

### FFmpeg
Use `ffmpeg:<index>` as argument. `index` is a zero-based index identifying the encoder.

You can use `captura-cli list` to check encoder indices.

e.g.

```
captura-cli start --encoder ffmpeg:0
```

### Stream
Use `stream:<url>` as argument. `url` is the rtmp url of the streaming service.

e.g. Stream to Twitch

```
captura-cli start --encoder stream:rtmp://live.twitch.tv/app/TWITCH_KEY
```

### Steps
Use `steps:video` and `steps:images` as `encoder` for Steps recording mode.

#### Record steps to video (avi)

```
captura-cli start --encoder steps:video
```

#### Record steps to a folder containing images (png)

```
captura-cli start --encoder steps:images
```