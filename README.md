# GamerJail

GamerJail ist ein Programm, welches die Computerspielzeit kontrollieren und begrenzen soll. Es kann zwischen verschiedenen Kontigentverteilungsmodi (Täglich, Täglich mit Speichern und Wöchentlich) gewählt werden. Das Kontigent wird jeweils für einen Tag festgelegt. Desweiteren kann ein Zeitraum des Tages ausgewählt werden, an dem gespielt werden darf. So können Sie zum Beispiel einstellen, dass ab 24 Uhr Schluss ist. Ist das Kontigent abgelaufen oder der Zeitraum vorbei, kann eine Aktion ausgewählt werden (Nichts, Programm beenden, Computer ausschalten). Der Benutzer wird 30 und nochmal 5 Minuten vorher gewarnt. Dies erfolgt durch eine Ansage und eine Benachrichtigung. Wenn die Zeit vorbei ist, wird erneut eine Warnung ausgesprochen und der Benutzer hat 5 Minuten Zeit, das Spiel zu beenden. Anschließend folgen die Aktionen.

![MainWindow](http://fs2.directupload.net/images/150809/3hhxszdm.png)


## Funktionsweise
Das Programm reagiert auf eine Veränderung des aktuellen Fensters. Der dazugehörige Prozess wird im ersten Schritt in der lokalen Datenbank mithilfe des Prozessnamens gesucht. Ist kein Eintrag vorhanden, wird 60 Sekunden gewartet, damit die Anwendung sicher vollständig geladen wurde. Dann werden zwei Sachen überprüft: "Deckt die Anwendung den kompletten Bildschirm ab?" und "Belegt diese Anwendung mehr als 450 MiB?". Nach vielem geteste hat diese vorgehensweise eine sehr hohe Erfolgsquote. Fehlschläge gab es bei mir noch keine und alle Spiele, die ich besitze, werden erkannt: Call of Duty - AW, ARK, Rocket League, League Of Legends, Counter-Strike: Global Offensive und Serious Sam. Ob Spiel oder nicht, es wird ein neuer Eintrag in der Datenbank gemacht, sodass jedes Programm nur einmal analysiert werden muss.

Trotzdem sollte man ab und an überprüfen, ob alles richtig ist. Dies kann man einfach unter `Administration` -> `Programme`

![Programme](http://fs1.directupload.net/images/150809/ww272tuu.png)

## Features

* Viele Einstellungsmöglichkeiten
* Statistiken                      
![Statistik](http://fs2.directupload.net/images/150809/odyzhgv8.png)
* Automatische Erkennung, selbst neue oder unveröffentlichte Spiele werden angezeigt
* Administration Passwortgeschützt
* Anwendung kann geschützt werden
* Ein Zeitraum kann ausgewählt werden, in dem gespielt werden darf
* Verschiedene Kontigentverteilungsmodi
* Der Verlauf kann eingesehen werden
* Deutliche Warnungen durch eine Ansage
* Keine Administratorberechtigung von Nöten
