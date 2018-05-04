# Translation

Captura is localized using language-specific JSON files.
Language can be set on Config tab.

## Currently supported languages

Name                  | Language ID | Contributor
----------------------|-------------|-------------------------------------------------
Arabic                | ar          | [mohi-othman](https://github.com/mohi-othman)
Chinese (Simplified)  | zh-CN       | [Airborne76](https://github.com/Airborne76)
Czech                 | cs          | [nofutur3](https://github.com/nofutur3)
Danish                | da          | [Martin4ndersen](https://github.com/Martin4ndersen)
Dutch                 | nl          | [demichiel](https://github.com/demichiel)
English               | en          | [Mathew Sachin](https://github.com/MathewSachin)
Finnish               | fi          | [Mknsri](https://github.com/Mknsri)
French                | fr          | [baptistecolin](https://github.com/baptistecolin)
German                | de          | [flxn](https://github.com/flxn)
Hebrew                | he          | [yotam180](https://github.com/yotam180)
Icelandic             | is          | [gautsson](https://github.com/gautsson)
Indonesian            | id          | [TheFaR8](https://github.com/TheFaR8)
Italian               | it          | [simocosimo](https://github.com/simocosimo)
Malayalam             | ml          | [Mathew Sachin](https://github.com/MathewSachin)
Norwegian             | no          | [goggenb](https://github.com/goggenb)
Polish                | pl          | [j4nw](https://github.com/j4nw)
Portuguese            | pt          | [igorruckert](https://github.com/igorruckert)
Romanian              | ro          | [AndreeaEne](https://github.com/AndreeaEne)
Russian               | ru          | [rvgulyaev](https://github.com/rvgulyaev)
Spanish               | es          | [KNTRO](https://github.com/KNTRO) and [Jhovany200](https://github.com/Jhovany200)
Swedish               | sv          | [Arrowfan](https://github.com/Arrowfan)
Thai                  | th          | [kerlos](https://github.com/kerlos)
Turkish               | tr          | [sgbasaraner](https://github.com/sgbasaraner)
Ukrainian             | uk          | [Marusyk](https://github.com/Marusyk)

## Contributing

- Copy the `en.json` file in `src/Captura.Core/Languages` folder.
- Rename the copy to `[Language ID].json` where `[Language ID]` is the ID of language you are translating into.
- Do translation in the renamed file.
- Send Pull Request.

## Testing

Place the `[Language ID].json` file in the `Languages` folder of a build.
The Language can be tested by starting Captura and selecting the Language on `Config` tab.