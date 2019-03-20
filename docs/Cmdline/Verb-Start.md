# Verb: start
Starts Recording.

There are two modes.

When Length is specified, recording runs until specified Length.
When Length is not specified, press q to quit message is displayed.

Argument          | Description
------------------|---------------------------------------------------------------------------
--cursor          | Include cursor
--keys            | Include keystrokes
--clicks          | Include mouse clicks
--delay           | Delay before starting recording (in ms)
--length          | Length of recording (in s)
--source          | The source to record from. See [here](Arg-Source.md).
--mic             | The microphone index to use. (-1 = none (Default)) (0 is first device).
--speaker         | The speaker output index to use. (-1 = none (Default)) (0 is first device).
-r or --framerate | Frame Rate (Default is 10).
--encoder         | The video encoder to use. See below.
--vq              | Video Quality (1 to 100) (Default is 70).
--aq              | Audio Quality (1 to 100) (Default is 50).
-f or --file      | Output file path.

e.g. Record 10 seconds with cursor and keystrokes and audio from first speaker output.

```
captura-cli start --length 10 --cursor --keys --speaker=0
```

## Using the Encoder argument

You can use `captura-cli list` to check encoder indices.

### SharpAvi
Use `sharpavi:<index>` as argument. `index` is a zero-based index identifying the encoder.

### FFmpeg
Use `ffmpeg:<index>` as argument. `index` is a zero-based index identifying the encoder.