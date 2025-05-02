using System;
using System.Collections.Generic;

class Program
{
    // Initialisieren von Variablen und Datenstrukturen
    static string aktuellerOrt = "Strand";  // Der Startort des Spiels
    static List<string> inventar = new List<string>();  // Liste für das Inventar des Spielers
    static int punkte = 0;  // Punkte des Spielers
    static bool fluchGebrochen = false;  // Gibt an, ob der Fluch gebrochen wurde
    static bool schatzGefunden = false;  // Gibt an, ob der Schatz gefunden wurde

    // Karte der Welt (Verbindungen zwischen Orten)
    static Dictionary<string, List<string>> karte = new Dictionary<string, List<string>> {
        { "Strand", new List<string> { "Dschungel" } },
        { "Dschungel", new List<string> { "Strand", "Höhle", "Lagune" } },
        { "Höhle", new List<string> { "Dschungel", "Tempel" } },
        { "Lagune", new List<string> { "Dschungel" } },
        { "Tempel", new List<string> { "Höhle", "Schatzkammer" } },
        { "Schatzkammer", new List<string> { "Tempel" } },
    };

    // Liste der Highscores
    static List<int> highscores = new List<int>();

    static void Main()
    {
        bool nochmalSpielen = true;
        // Schleife, um das Spiel wiederholt zu starten
        while (nochmalSpielen)
        {
            StarteSpiel();

            Console.WriteLine("Möchtest du noch eine Runde spielen? (ja/nein)");
            string antwort = Console.ReadLine()?.ToLower();
            nochmalSpielen = (antwort == "ja");

            if (!nochmalSpielen)
            {
                ZeigeHighscores();  // Zeigt die Highscores, wenn das Spiel endet
            }
        }

        Console.WriteLine(" ");
        Console.WriteLine("Danke fürs Spielen! Auf Wiedersehen!");
        Console.ReadKey();
    }

    // Methode zum Starten des Spiels
    static void StarteSpiel()
    {
        // Spielstatus zurücksetzen
        aktuellerOrt = "Strand";
        inventar = new List<string>();
        punkte = 0;
        fluchGebrochen = false;
        schatzGefunden = false;

        Begrüßung();  // Begrüßung des Spielers

        // Endlosschleife für den Spielablauf
        while (true)
        {
            Console.WriteLine(" ");
            Console.WriteLine("==================================");
            Console.WriteLine($"Du bist jetzt bei: {aktuellerOrt}");
            Console.WriteLine("==================================");
            ZeigeOrtBeschreibung();  // Beschreibung des aktuellen Ortes
            ZeigeVerfügbareOrte();  // Zeigt Orte zu denen der Spieler gehen kann

            Console.Write("Was möchtest du tun? ");
            string eingabe = Console.ReadLine()?.ToLower();
            Console.Clear();

            if (string.IsNullOrWhiteSpace(eingabe))  // Validierung der Eingabe
            {
                Console.WriteLine("Ungültige Eingabe.");
                continue;
            }

            // Spielsteuerung Aktionen des Spielers
            if (eingabe == "ende")
            {
                Beenden();
                break;
            }
            else if (eingabe.StartsWith("gehe ")) GeheZu(eingabe.Substring(5));
            else if (eingabe.StartsWith("untersuche ")) Untersuche(eingabe.Substring(11));
            else if (eingabe.StartsWith("nimm ")) Nimm(eingabe.Substring(5));
            else if (eingabe.StartsWith("benutze ")) Benutze(eingabe.Substring(8));
            else if (eingabe == "inventar") ZeigeInventar();
            else if (eingabe == "hilfe") ZeigeHilfe();
            else if (eingabe == "karte") ZeigeKarte();
            else Console.WriteLine("Unbekannter Befehl. Gib 'hilfe' ein für Optionen.");

            if (ÜberprüfeSpielende()) break;  // Überprüft, ob das Spiel zu Ende ist
        }

        highscores.Add(punkte);  // Speichert den Endpunktestand
    }

    // Begrüßung des Spielers
    static void Begrüßung()
    {
        Console.WriteLine("====================================================");
        Console.WriteLine("Willkommen zum Abenteuer: Der verfluchte Piratenschatz!");
        Console.WriteLine("====================================================");
        Console.WriteLine("Tippe 'hilfe' für eine Liste von Befehlen.");
    }

    // Zeigt die Hilfe mit verfügbaren Befehlen
    static void ZeigeHilfe()
    {
        Console.WriteLine(" ");
        Console.WriteLine("Verfügbare Befehle:");
        Console.WriteLine("  gehe [Ort]          → Reise zu einem Ort");
        Console.WriteLine("  untersuche [Objekt] → Untersuche etwas am aktuellen Ort");
        Console.WriteLine("  benutze [Gegenstand]");
        Console.WriteLine("  nimm [Gegenstand]");
        Console.WriteLine("  inventar            → Zeigt dein Inventar");
        Console.WriteLine("  karte               → Zeigt die Karte");
        Console.WriteLine("  ende                → Spiel beenden");
    }

    // Beschreibt den aktuellen Ort je nach Situation
    static void ZeigeOrtBeschreibung()
    {
        switch (aktuellerOrt)
        {
            case "Strand":
                Console.WriteLine("Du siehst ein Schiffswrack. ('untersuche wrack')");
                break;
            case "Dschungel":
                Console.WriteLine("Eine alte Statue steht hier. ('untersuche statue')");
                break;
            case "Höhle":
                Console.WriteLine("Es ist dunkel. Eine 'fackel' könnte helfen.");
                break;
            case "Lagune":
                Console.WriteLine("Du entdeckst einen 'spiegel' und einen 'schlüssel'.");
                break;
            case "Tempel":
                Console.WriteLine("Mysteriöser Tempel. Der Spiegel zeigt hier etwas.");
                break;
            case "Schatzkammer":
                Console.WriteLine("Du siehst den Schatz! ('nimm schatz')");
                break;
        }
    }

    // Zeigt die verfügbaren Orte, die vom aktuellen Ort aus erreichbar sind
    static void ZeigeVerfügbareOrte()
    {
        if (karte.TryGetValue(aktuellerOrt, out List<string> orte))
        {
            Console.WriteLine(" ");
            Console.WriteLine("Von hier aus erreichbare Orte:");
            foreach (var ort in orte)
                Console.WriteLine($"  → {ort}");
        }
    }

    // Die Funktion für das Gehen zu einem neuen Ort
    static void GeheZu(string ort)
    {
        foreach (var ziel in karte[aktuellerOrt])
        {
            if (ziel.Equals(ort, StringComparison.OrdinalIgnoreCase))
            {
                aktuellerOrt = ziel;
                Console.WriteLine(" ");
                Console.WriteLine($"Du gehst nach {ziel}.");
                Console.WriteLine(" ");
                return;
            }
        }
        Console.WriteLine(" ");
        Console.WriteLine("Dorthin kannst du nicht direkt gehen.");
    }

    // Untersuchung von Objekten am aktuellen Ort
    static void Untersuche(string objekt)
    {
        if (aktuellerOrt == "Strand" && objekt == "wrack")
        {
            Console.WriteLine(" ");
            Console.WriteLine("Du findest eine alte Karte und nimmst sie!");
            HinzufügenZumInventar("Karte", 5);
        }
        else if (aktuellerOrt == "Dschungel" && objekt == "statue")
        {
            Console.WriteLine(" ");
            Console.WriteLine("Die Statue flüstert: 'Memento'");
            punkte += 10;
        }
        else if (aktuellerOrt == "Lagune" && objekt == "spiegel")
        {
            Console.WriteLine(" ");
            Console.WriteLine(inventar.Contains("Schlüssel")
                ? "Der Spiegel zeigt einen geheimen Pfad im Tempel."
                : "Der Spiegel ist trüb. Du brauchst etwas...");
        }
        else
        {
            Console.WriteLine(" ");
            Console.WriteLine("Du findest nichts Interessantes.");
        }
    }

    // Funktion, um Gegenstände zu nehmen
    static void Nimm(string gegenstand)
    {
        if (aktuellerOrt == "Höhle" && gegenstand == "amulett")
        {
            Console.WriteLine(" ");
            Console.WriteLine("Du hast ein leuchtendes Amulett gefunden.");
            HinzufügenZumInventar("Amulett", 5);
        }
        else if (aktuellerOrt == "Lagune" && gegenstand == "schlüssel")
        {
            Console.WriteLine(" ");
            Console.WriteLine("Du hebst einen alten Schlüssel aus dem Wasser und nimmst ihn.");
            HinzufügenZumInventar("Schlüssel", 5);
        }
        else if (aktuellerOrt == "Schatzkammer" && gegenstand == "schatz")
        {
            if (inventar.Contains("Amulett"))
            {
                Console.WriteLine(" ");
                Console.WriteLine("Du verwendest das Amulett – der Fluch wird gebrochen!");
                fluchGebrochen = true;
            }
            else
            {
                Console.WriteLine(" ");
                Console.WriteLine("Du nimmst den Schatz – aber der Fluch bleibt!");
            }

            schatzGefunden = true;
            punkte += 50;
        }
        else
        {
            Console.WriteLine(" ");
            Console.WriteLine("Hier gibt es das nicht zu nehmen.");
        }
    }

    // Benutze ein bestimmtes Objekt z.B. Fackel in der Höhle
    static void Benutze(string gegenstand)
    {
        if (gegenstand == "fackel" && aktuellerOrt == "Höhle")
        {
            Console.WriteLine(" ");
            Console.WriteLine("Du erleuchtest die Höhle und siehst ein Amulett.");
        }
        else
        {
            Console.WriteLine(" ");
            Console.WriteLine("Das kannst du hier nicht benutzen.");
        }
    }

    // Zeigt das Inventar des Spielers
    static void ZeigeInventar()
    {
        Console.WriteLine(" ");
        Console.WriteLine("Dein Inventar:");
        if (inventar.Count == 0)
            Console.WriteLine("  (leer)");
        else
            inventar.ForEach(item => Console.WriteLine($"  - {item}"));
    }

    // Zeigt eine einfache Karte des Spiels
    static void ZeigeKarte()
    {
        Console.WriteLine(@"
        Strand
           │
       Dschungel ── Höhle
          │           │
        Lagune      Tempel ─ Schatzkammer
        ");
    }

    // Überprüft ob das Spiel zu Ende ist
    static bool ÜberprüfeSpielende()
    {
        if (schatzGefunden)
        {
            Console.WriteLine("SPIEL BEENDET");
            Console.WriteLine(fluchGebrochen
                ? "Du hast den Schatz geborgen UND den Fluch gebrochen!"
                : "Du bist reich, aber verflucht!");

            Console.WriteLine($"Gesamtpunktzahl: {punkte}");
            return true;
        }
        return false;
    }

    // Beendet das Spiel und gibt den Endpunktestand aus
    static void Beenden()
    {
        Console.WriteLine(" ");
        Console.WriteLine("Das Spiel wird beendet. Danke fürs Spielen!");
        Console.WriteLine($"Endpunktestand: {punkte}");
        Environment.Exit(0);
    }

    // Fügt einen Gegenstand zum Inventar hinzu und erhöht die Punkte
    static void HinzufügenZumInventar(string item, int punkteWert)
    {
        if (!inventar.Contains(item))
        {
            inventar.Add(item);
            punkte += punkteWert;
        }
    }

    // Zeigt die Highscores an
    static void ZeigeHighscores()
    {
        Console.WriteLine(" ");
        Console.WriteLine("Highscores:");
        if (highscores.Count == 0)
        {
            Console.WriteLine("  Keine Highscores vorhanden.");
        }
        else
        {
            highscores.Sort();
            highscores.Reverse();

            for (int i = 0; i < highscores.Count; i++)
            {
                Console.WriteLine($"  {i + 1}. Platz: {highscores[i]} Punkte");
            }
        }
    }
}
