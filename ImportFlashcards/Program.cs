using System.IO.Compression;
using Microsoft.Data.Sqlite;
using System;
using System.IO;


namespace ImportFlashcards
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("Hello, World!");

            string storagePath = "C:\\Users\\elija\\source\\repos\\ImportFlashcards\\ImportFlashcards\\Storage\\Networking_and_security.apkg";
            string name = "networkingSecurity";
            string extractFolder = "C:\\Users\\elija\\source\\repos\\ImportFlashcards\\ImportFlashcards\\Extract\\";
            string extractToPath = extractFolder + name;

            if (!string.IsNullOrEmpty(name))
            {
                try
                {
                    // Create the directory if it does not exist
                    if (!Directory.Exists(extractToPath))
                    {
                        Directory.CreateDirectory(extractToPath);
                        await ExtractApkg(storagePath, extractToPath);
                        Console.WriteLine("Directory created!");
                    }
                    else
                    {
                        Console.WriteLine("Directory already exists");
                    }

                    string ankiData = Path.Combine(extractToPath, "collection.anki2");
                    await Console.Out.WriteLineAsync(ankiData);

                    await ReadAnkiData(ankiData);
                }
                catch (Exception e)
                {
                    Console.WriteLine($"An error occurred: {e.Message}");
                }
            }

            Console.WriteLine("Done");
        }

        public class Flashcard
        {
            public string Front { get; set; }
            public string Back { get; set; }
        }

        public static async Task ReadAnkiData(string dbPath)
        {
            string connectionString = $"Data Source={dbPath};";

            using (var connection = new SqliteConnection(connectionString))
            {
                connection.Open();

                var command = connection.CreateCommand();
                command.CommandText = "SELECT flds FROM notes;";

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        string flds = reader.GetString(0);
                        var flashcardParts = flds.Split('\x1f'); // '\x1f' is the field separator in Anki

                        if (flashcardParts.Length >= 2)
                        {
                            Flashcard flashcard = new Flashcard
                            {
                                Front = flashcardParts[0],
                                Back = flashcardParts[1]
                            };
                            Console.WriteLine($"Front: {flashcard.Front}\nBack: {flashcard.Back} \n\n" );
                        }
                    }
                }

                connection.Close();
            }
        }

        public static async Task ExtractApkg(string apkgPath, string extractPath)
        {
            ZipFile.ExtractToDirectory(apkgPath, extractPath);
        }
    }
}

/*
        public static async Task step1()
        {
            string ckadFlashcards = "C:\\Users\\elija\\source\\repos\\ImportFlashcards\\ImportFlashcards\\Storage\\CKAD_Exercises.apkg";
            string machineLearningFlashcards = "C:\\Users\\elija\\source\\repos\\ImportFlashcards\\ImportFlashcards\\Storage\\Machine_Learning_Smart_Decisions_cards.apkg";

            string extractionPath = "C:\\Users\\elija\\source\\repos\\ImportFlashcards\\ImportFlashcards\\Extract\\";

            await ExtractApkg(ckadFlashcards, (extractionPath + "ckad"));
            await ExtractApkg(machineLearningFlashcards, (extractionPath + "machineLearning"));
        }
*/