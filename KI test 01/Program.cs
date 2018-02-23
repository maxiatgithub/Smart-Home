using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Speech.Recognition;
using System.Speech.Synthesis;
using System.Data;

namespace KI_test_01
{
    class Program
    {

        SpeechSynthesizer s = new SpeechSynthesizer(); //Instanz zur Sprachwiedergabe
        SpeechRecognitionEngine h = new SpeechRecognitionEngine(); //Instanz zur Spracherkennung

        // Erzeugung der Befehls- und Antwortliste
        static List<string> Befehle = new List<string>() { };
        static List<string> Answers = new List<string>() { };


        public void initialisation(List<string> Befehle)
        {
            Choices commands = new Choices(); //Instanz zur Kommandoerkennung
            commands.Add(Befehle.ToArray()); //Gibt der Instanz zur Kommandoerkennung die Befehlsliste über
            GrammarBuilder gbuilder = new GrammarBuilder(); //Instanz zur Grammatikanalyse
            gbuilder.Append(commands); //Gibt der Instanz zur Grammatikanalyse die Befehlsliste über
            Grammar grammar = new Grammar(gbuilder); //Gibt die Instanz zur Grammatikanalyse weiter zur Erzeugung einer Instanz, die die neue Grammatik darstellt
            h.LoadGrammar(grammar); //Die Instanz zur Spracherkennung (Zeile 16) lernt nun die neue Grammatik
            h.SetInputToDefaultAudioDevice(); //Als Input wird das Standart Aufnahmegerät ausgewählt
            h.SpeechRecognized += recEngine_SpeechRecognized; //Sobald die Spracherkennung ein Wort erkannt hat, wird die Event Methode ausgeführt
            h.RecognizeAsync(RecognizeMode.Multiple); //Die Spracherkennung soll asynchron stattfinden und mehrere Eingaben hintereinander verarbeiten können
            s.SelectVoiceByHints(VoiceGender.Female, VoiceAge.Adult); //Sprachsample Auswahl zur Sprachwiedergabe
            Console.WriteLine("Sag ruhig was");
        }

        public static void Main(string[] args)
        {
            Program Start = new Program();  
            String path = @"G:\Visual Studio Enterprise codes\KI test 01\KI test 01\commandData.txt"; // Pfad zur Textdatei für die Auflistung aller Befehle und legt neuen IO StreamReader an zur Auslesung der Textdatei
            System.IO.StreamReader file = new System.IO.StreamReader(path);


            getCommands(file.ReadLine(),Befehle, Answers, file); //Erzeugen der beiden Listen (Befehl und Antwort)
            Start.initialisation(Befehle); //Hier habe ich alles zusammengefasst, was zur Auswertung der Spracheingaben nötig ist wie etwa die commands und Grammatik, die die Spracherkennung dann mit dem Input vergleicht

            Console.Read();

        }

        //Die Methode sucht in der Befehlsliste nach dem erkannten Kommando, um dann die zugehörige Antwort in der Antwortliste zu erfassen
        private void recEngine_SpeechRecognized(object sender, SpeechRecognizedEventArgs e)
        {
            Console.WriteLine(e.Result.Text);
            int index = 0;
            Befehle.ForEach(delegate (String Data)
            {
                if (e.Result.Text == Data)
                {
                    s.SpeakAsync(Answers[index]);
                }
                else
                {
                    index++;
                }
            });

        }
        // Schreibt alle Textketten aus der Kommando Datei in die Liste der vorhandenen Befehle
        static private void getCommands(String Line, List<string> Bef, List<string> Ans, System.IO.StreamReader file)
        {            
            if (Line != null)
            {
                genLists(Line, Bef, Ans);
                getCommands(file.ReadLine(), Bef, Ans, file);
            }
            else
            {
                file.Close();
            }
        }

        //Fügt die Textelemente als neues Listenelement zur jeweiligen Liste an
        static private void genLists(String Line, List<string> Bef, List<string> Ans)
        {
            int index = Line.IndexOf(";");
            Bef.Add(Line.Substring(0, index));
            Ans.Add(Line.Substring(index+1, Line.Length-index-1));
        }
    }
}
