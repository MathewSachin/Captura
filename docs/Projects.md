# Project Structure

## Base
`Captura.Base` contains common interfaces and base classes. This project is referenced by all other projects.

## Localization
`Captura.Loc` contains the localization code.

## Audio
`Captura.Audio` contains the audio interfaces which are implemented by specific libraries like `Captura.NAudio`.

## Screna, Core
`Screna` and `Captura.Core` projects contain the bulk of the code and are depended on by both UI and Console projects.

## Windows
`Captura.Windows` conatins code that is completely specific to the Windows OS.

## Console
`Captura.Console` builds the console application.

## View Core
`Captura.ViewCore` project contains View models and is depended on by the UI project.

## UI
`Captura` is a WPF project containing the UI.

## Other
The remaining projects add specific features like Imgur, SharpAvi, FFmpeg, etc.