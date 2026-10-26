# Tower Defense

Tower Defense mäng, mis on tehtud **.NET MAUI** abil (C#). See on mu OOP (objektorienteeritud programmeerimine) kooliülesanne. Projekt on tehtud nii, et seda on lihtne edasi arendada.

## Mis mäng see on?

Vaenlased liiguvad mööda rada sinu baasi poole. Sina paned rajast väljapoole torne, mis vaenlasi tulistavad. Kui vaenlane jõuab raja lõppu, kaotad ühe elu. Kui elud saavad otsa, kaotad mängu. Kui elad üle kõik 5 lainet, võidad.

## Kuidas mängida

1. Vali ülalt torn (Vibukütt või Kahur).
2. Puuduta ruutu mänguväljal, et torn sinna panna. Rajale torni panna ei saa.
3. Vajuta **Start**, et mäng algaks. Sama nupuga saab mängu pausile panna.
4. Tapa vaenlasi, et raha teenida, ja osta selle eest rohkem torne.
5. Ülevalt näed elusid ❤️, raha 💰, punkte ⭐ ja lainet 🌊.

Alguses on sul **10 elu** ja **100 raha**.

## Tornid

| Torn        | Hind | Laskeulatus | Kahjustus | Tulistamiskiirus   |
| ----------- | ---- | ----------- | --------- | ------------------ |
| 🏹 Vibukütt | 50   | 2.5 ruutu   | 10        | 2.0 lasku sekundis |
| 💣 Kahur    | 120  | 3.5 ruutu   | 40        | 0.7 lasku sekundis |

Vibukütt on odav ja kiire. Kahur on kallis ja aeglane, aga teeb suure kahjustuse. Torn sihib vaenlast, kes on rajal kõige kaugemal.

## Vaenlased

| Vaenlane    | Elu | Kiirus | Tasu tapmise eest |
| ----------- | --- | ------ | ----------------- |
| 👾 Tavaline | 40  | 1.0    | 10                |
| 🐇 Kiire    | 25  | 2.0    | 15                |
| 🐘 Tank     | 150 | 0.6    | 40                |

Iga laine toob rohkem vaenlasi. Kiired vaenlased tulevad alates 2. lainest ja tankid alates 3. lainest.

## Muud asjad mängus

- 5 lainet
- Mänguväli on 7 × 9 ruudustik
- Vaenlastel on elusriba
- Torni lask on näha kollase joonena
- Võidu korral saad boonuspunkte allesjäänud elude eest
- Parim tulemus salvestatakse telefoni või arvutisse
- Mitu värviteemat, mida saab ülevalt valida

## Projekti ülesehitus (OOP)

Mäng koosneb eraldi klassidest, millel kõigil on oma ülesanne:

| Klass                                  | Mis see teeb                                                   |
| -------------------------------------- | -------------------------------------------------------------- |
| `Game`                                 | Mängu loogika: rada, lained, tornid, vaenlased, võit ja kaotus |
| `Player`                               | Mängija elud, raha ja punktid                                  |
| `Tower`                                | Tornide ühine alus (abstraktne klass)                          |
| `ArcherTower`, `CannonTower`           | Konkreetsed tornid, mis pärivad klassi `Tower`                 |
| `Enemy`                                | Vaenlaste ühine alus (abstraktne klass)                        |
| `BasicEnemy`, `FastEnemy`, `TankEnemy` | Konkreetsed vaenlased, mis pärivad klassi `Enemy`              |
| `GameEnums`                            | Loendid `GameState` (mängu seis) ja `TowerType` (torni tüüp)   |
| `GameDrawable`                         | Joonistab mänguvälja ekraanile                                 |
| `MainPage`                             | Ekraan, nupud ja mängu taimer                                  |

Kasutatud OOP mõisted:

- **Pärimine:** `ArcherTower` ja `CannonTower` pärivad klassi `Tower`, vaenlased pärivad klassi `Enemy`.
- **Abstraktsed klassid:** `Tower` ja `Enemy` on ainult alus, neist ei tehta otse objekte.
- **Polümorfism:** iga alamklass kirjutab üle oma `Symbol` (emoji). Klassis `Tower` on ka `virtual` meetodid `FindTarget` ja `Attack`, mida alamklassid saavad üle kirjutada.
- **Kapseldus:** näiteks vaenlase elu ja asukoht on kaitstud (`private set`), neid muudavad ainult klassi enda meetodid.
- **Sündmused (events):** `Game` annab teada, kui laine algab, torn tulistab või mäng lõpeb, ja ekraan reageerib sellele.

Uue torni või vaenlase lisamiseks tee uus alamklass ja kirjuta konstruktoris väärtused.

## Kuidas käivitada

1. Paigalda [Visual Studio 2022](https://visualstudio.microsoft.com/) koos **.NET Multi-platform App UI development** komponendiga.
2. Klooni repo:
   ```bash
   git clone https://github.com/Kundratsk/TARge25-mobiilirakendused.git
   ```
3. Ava kaustas `TowerDefense` olev `.sln` fail Visual Studios.
4. Vali käivitamise sihtmärk (näiteks Windows Machine või Android Emulator).
5. Vajuta **Start**.

## Plaanid edasiseks

- Tornide müümine
- Rohkem tornitüüpe ja vaenlasi
- Rohkem lainet ja uusi kaarte

## Tehnoloogiad

- C#
- .NET MAUI
- XAML

## Autor

Kristo ([@Kundratsk](https://github.com/Kundratsk))
