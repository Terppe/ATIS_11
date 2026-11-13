# ATIS - V7.8

## Webseiten auf der HomePage

Beim ersten Laden der HomePage kopiert ATIS die mitgelieferte Vorlage
`Defaults/websites.json` nach `%LOCALAPPDATA%\ATIS\websites.json`. Eine
vorhandene Benutzerdatei wird nicht überschrieben, auch nicht bei Updates.

Die Datei kann mit einem Texteditor bearbeitet werden:

```json
{
  "Website.ITIS USA": "https://itis.gov/",
  "Website.Eigener Name": "https://example.org/"
}
```

Jeder Schlüssel mit dem Präfix `Website.` wird angezeigt. Der Text nach dem
Punkt ist die Beschriftung. Unterstützt werden absolute HTTP- und HTTPS-URLs;
das überprüft nicht die Erreichbarkeit oder das Zertifikat einer Website.
Ungültige Einträge werden auf der HomePage gemeldet und übersprungen; eine
ungültige JSON-Datei wird gemeldet und nicht ersetzt. Nach dem Bearbeiten die
HomePage erneut öffnen oder ATIS neu starten.

Die bisherigen Website-Einträge in `App.config` bleiben vorläufig als alte
Standardwerte erhalten, werden von der HomePage aber nicht mehr gelesen.
Individuell angepasste Adressen daraus müssen einmal in die Benutzerdatei
übernommen werden.

Die dateibasierten Prüfungen können mit
`dotnet run --project Tests/WebsiteSettings/WebsiteSettingsChecks.csproj`
ausgeführt werden.