# ATIS - V7.8

## Spracheinstellung

ATIS speichert die im Menü oder in den Einstellungen gewählte Sprache für den
Windows-Benutzer in `%LOCALAPPDATA%\ATIS\settings.json` unter `Culture`. Die
Wahl bleibt beim Neustart erhalten. Bestehende Fenstereinstellungen bleiben
in derselben Datei erhalten.

Fehlt `Culture` in der JSON-Datei, übernimmt ATIS beim ersten Start einmal den
bisherigen Wert aus `App.config`. Fehlt auch dieser, wird `de-DE` verwendet.
Danach hat der JSON-Wert Vorrang. Die Datei kann bei geschlossenem ATIS
bearbeitet werden, z. B. `"Culture": "en-US"`; vorhandene Einträge und eine
gültige JSON-Struktur müssen erhalten bleiben.
Derzeit enthält ATIS Übersetzungsdateien für Deutsch und Englisch. Für andere
eingestellte Kulturen bleiben beim Wechsel zunächst die bisherigen Texte
sichtbar; beim Neustart werden deutsche Texte angezeigt. Die fehlenden
Übersetzungen werden im Protokoll vermerkt.

Die dateibasierten Prüfungen können mit
`dotnet run --project Tests/CultureSettings/CultureSettingsChecks.csproj`
ausgeführt werden.

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