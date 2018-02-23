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
        //Sprachwiedergabe
        SpeechSynthesizer s = new SpeechSynthesizer();
        //Sprachwiedergabe
        SpeechRecognitionEngine h = new SpeechRecognitionEngine();
        // Erzeugung der Befehls- und Antwortliste
        static List<string> Befehle = new List<string>() { };
        static List<string> Answers = new List<string>() { };

        public void initialisation(List<string> Befehle)
        {
            Choices commands = new Choices();
            commands.Add(Befehle.ToArray());
            GrammarBuilder gbuilder = new GrammarBuilder();
            gbuilder.Append(commands);
            Grammar grammar = new Grammar(gbuilder);
            h.LoadGrammar(grammar);
            h.SetInputToDefaultAudioDevice();
            h.SpeechRecognized += recEngine_SpeechRecognized;
            h.RecognizeAsync(RecognizeMode.Multiple);
            s.SelectVoiceByHints(VoiceGender.Female, VoiceAge.Adult);
            Console.WriteLine("Sag ruhig was");
        }

        public static void Main(string[] args)
        {
            Program Start = new Program();
         // Pfad zur Textdatei für die Auflistung aller Befehle und legt neuen IO StreamReader an zur Auslesung der Textdatei
            String path = @"G:\Visual Studio Enterprise codes\KI test 01\KI test 01\commandData.txt";
            System.IO.StreamReader file = new System.IO.StreamReader(path);


            getCommands(file.ReadLine(),Befehle, Answers, file);
            Start.initialisation(Befehle);

            Console.Read();

        }

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
        static private void genLists(String Line, List<string> Bef, List<string> Ans)
        {
            int index = Line.IndexOf(";");
            Bef.Add(Line.Substring(0, index));
            Ans.Add(Line.Substring(index+1, Line.Length-index-1));
        }
    }
}
