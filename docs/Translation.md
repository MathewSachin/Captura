# Translation

Captura is localized using language-specific JSON files.
Language can be set on Config tab.

## Currently supported languages

See [here](https://mathewsachin.github.io/Captura/translation).

## Contributing

- Copy the `en.json` file in `src/Captura.Core/Languages` folder.
- Rename the copy to `[Language ID].json` where `[Language ID]` is the ID of language you are translating into.
- Do translation in the renamed file.
- Send Pull Request.

## Testing

Place the `[Language ID].json` file in the `Languages` folder of a build.
The Language can be tested by starting Captura and selecting the Language on `Config` tab.