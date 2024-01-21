using System.IO.Compression;
using Microsoft.Data.Sqlite;
using System;
using System.IO;
using System.ComponentModel.DataAnnotations;


namespace ImportFlashcards
{
    internal class Program
    {

        static async Task Main(string[] args)
        {
            await StartConvertion();
        }
        public class ImportDetail
        {
            public string Name { get; set; }
            public string StoragePath { get; set; }
            public string ExtractPath { get; set; }


            public ImportDetail(string name)
            {
                string basePath = @"C:\Users\elija\source\repos\ImportFlashcards\ImportFlashcards\";
                StoragePath = Path.Combine(basePath, "Storage", name); // Storage/name

                Name = RemoveApkg(name); 
                ExtractPath = Path.Combine(basePath, "Extract", Name); // Extract to this path (Storage\Name)
            }
        }

        public class Flashcard
        {
            public string Front { get; set; }
            public string Back { get; set; }
        }

        public static async Task StartConvertion()
        {
            Console.WriteLine("Starting convertion...");
            List<ImportDetail> allFiles = GetAllFiles2();
            int counter = 0;
            foreach (ImportDetail detail in allFiles)
            {
                await Console.Out.WriteLineAsync($"Count: {counter++}\nName: {detail.Name}\nStorage:{detail.StoragePath}\nExtract:{detail.ExtractPath}");

                if (!string.IsNullOrEmpty(detail.Name))
                {
                    try
                    {
                        // Create the directory if it does not exist, then extract
                        if (!Directory.Exists(detail.ExtractPath))
                        {
                            await Console.Out.WriteLineAsync("It doesn't exists...");
                            Directory.CreateDirectory(detail.ExtractPath);
                            await ExtractApkg(detail.StoragePath, detail.ExtractPath);
                            Console.WriteLine("Directory created!");
                        }
                        else
                        {
                            Console.WriteLine("Directory already exists");
                        }

                        string ankiData = Path.Combine(detail.ExtractPath, "collection.anki2");
                        await Console.Out.WriteLineAsync(ankiData);

                        await ReadAnkiData(ankiData);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine($"An error occurred: {e.Message}");
                    }
                }
            }


            Console.WriteLine("Convertion done");
        }

        public static List<ImportDetail> GetAllFiles2()
        {

            string targetFolderPath = @"C:\Users\elija\source\repos\ImportFlashcards\ImportFlashcards\Storage\";//Replace with your directory path

            DirectoryInfo directoryInfo = new DirectoryInfo(targetFolderPath);
            FileInfo[] files = directoryInfo.GetFiles();
            List<ImportDetail> allDetails = new List<ImportDetail>();

            // Print file names
            foreach (FileInfo file in files)
            {
                //Console.WriteLine(file.Name); // Use 'file.FullName' for full path
                //Console.WriteLine(file.FullName); // Use 'file.FullName' for full path
                allDetails.Add(new ImportDetail(file.Name));
            }

            return allDetails;
        }

        public static string RemoveApkg(string originalString)
        {
            int charsToRemove = 5;
            return originalString.Remove(originalString.Length - charsToRemove);
        }


        public static async Task GetAllFiles1()
        {
            string targetFolderPath = @"\Storage";//Replace with your directory path
            string currDir = Directory.GetCurrentDirectory();
            await Console.Out.WriteLineAsync(currDir);

            string fullPath = Path.Combine(currDir, targetFolderPath);
            Console.WriteLine(fullPath);

            DirectoryInfo directoryInfo = new DirectoryInfo(fullPath);
            FileInfo[] files = directoryInfo.GetFiles();

            // Print file names
            foreach (FileInfo file in files)
            {
                Console.WriteLine(file.Name); // Use 'file.FullName' for full path
            }
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