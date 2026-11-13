# ATIS - V7.8

## Mail-Zugangsdaten

Das SMTP-Passwort für „Passwort vergessen“ wird unter
`%LOCALAPPDATA%\ATIS\Credentials\smtp-password.dpapi` mit Windows-DPAPI
(`CurrentUser`) verschlüsselt gespeichert, nicht in JSON oder `App.config`.
Als ATIS-Administrator kann man unter **Einstellungen > Mail-Zugangsdaten**
ein neues SMTP-Passwort eingeben und speichern. Es wird nicht wieder angezeigt.
`SmtpHost`, `SmtpPort` und `Sender` bleiben vorerst in `App.config`.

Bei vorhandenen alten Einträgen übernimmt ATIS beim Start
`EncryptMailAccountPassword` einmalig in den lokalen DPAPI-Speicher.
`CryptMailPassword` wird im aktuellen Code nicht verwendet; sein bisheriger Wert
wird unverändert zusätzlich DPAPI-verschlüsselt als `legacy-mail-password.dpapi`
gesichert, nicht als SMTP-Passwort interpretiert. Existierende DPAPI-Dateien haben
Vorrang und werden bei der Übernahme nicht überschrieben. Defekte oder für den
aktuellen Windows-Benutzer unlesbare Dateien werden gemeldet, nicht durch alte
Config-Werte ersetzt. Das SMTP-Passwort kann in den Einstellungen neu gespeichert
werden.

Nach bestätigter Übernahme müssen die beiden alten Passworteinträge manuell aus
der lokalen Quell-`App.config` sowie aus vorhandenen Build-, Publish- und
Installationskopien entfernt werden. ATIS ändert keine Installationsdateien.
Danach neu bauen und für künftige Installationen ausschließlich die bereinigte
Konfiguration verwenden. Bereits veröffentlichte Zugangsdaten beim Mailanbieter
ändern; Entfernen aus einer Datei entfernt sie nicht aus der Git-Historie.

Die DPAPI-Dateien nicht ins Repository übernehmen oder mit dem Installer
verteilen. Sie sind an das Windows-Benutzerprofil gebunden; auf einem anderen
Rechner oder unter einem anderen Benutzer das SMTP-Passwort neu einrichten.
DPAPI schützt nicht vor Programmen, die bereits als derselbe Windows-Benutzer
laufen. Ein kundenseitig gespeichertes gemeinsames SMTP-Passwort kann deshalb
nicht gegenüber diesem Kunden geheim gehalten werden.

Die DPAPI-Prüfungen laufen unter Windows mit
`dotnet run --project Tests/MailCredentials/MailCredentialChecks.csproj`
und im Windows-CI-Build. Unter Linux wird nur kompiliert; DPAPI benötigt Windows.

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