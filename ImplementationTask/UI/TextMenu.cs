using ImplementationTask.IO;
using ImplementationTask.Structures;

namespace ImplementationTask.UI;

public class TextMenu
{
    /// <summary>
    /// Główna pętla aplikacji. Wyświetla opcje i steruje przepływem.
    /// </summary>
    public void RunMainMenu()
    {
        while (true)
        {
            Console.Clear();
            Console.WriteLine("==========================================");
            Console.WriteLine("      KOMPRESOR HUFFMANA - MENU GŁÓWNE    ");
            Console.WriteLine("==========================================");
            Console.WriteLine("1. Kompresuj plik (Tekst -> Huffman)");
            Console.WriteLine("2. Dekompresuj plik (Huffman -> Tekst)");
            Console.WriteLine("3. Narzędzia deweloperskie (Test Kolejki Priorytetowej)");
            Console.WriteLine("0. Wyjście");
            Console.WriteLine("==========================================");
            Console.Write("Wybierz opcję: ");

            var key = Console.ReadKey().KeyChar;
            Console.WriteLine();

            switch (key)
            {
                case '1':
                    ShowCompressionMenu();
                    break;
                case '2':
                    ShowDecompressionMenu();
                    break;
                case '3':
                    ShowQueueDebugMenu();
                    break;
                case '0':
                    Console.WriteLine("Do widzenia!");
                    return;
                default:
                    Console.WriteLine("\nNieznana opcja. Naciśnij dowolny klawisz...");
                    Console.ReadKey();
                    break;
            }
        }
    }

    /// <summary>
    /// Podmenu do testowania samej struktury danych
    /// </summary>
    private void ShowQueueDebugMenu()
    {
        // Tworzymy nową kolejkę na potrzeby testów
        PriorityQueue debugQueue = new PriorityQueue();
        
        bool inDebug = true;
        while (inDebug)
        {
            Console.Clear();
            Console.WriteLine("=== DEBUG: KOLEJKA PRIORYTETOWA ===");
            Console.WriteLine($"Liczba elementów: {debugQueue.Count}");
            Console.WriteLine("1. Insert (Dodaj)");
            Console.WriteLine("2. ExtractMin (Pobierz)");
            Console.WriteLine("3. ChangePriority (Zmień wagę)");
            Console.WriteLine("4. Build (Zbuduj z danych testowych)");
            Console.WriteLine("5. IsEmpty?");
            Console.WriteLine("6. PrintQueueState (Podgląd kopca)");
            Console.WriteLine("0. Powrót do menu głównego");
            Console.Write("\nWybierz opcję: ");

            switch (Console.ReadLine())
            {
                case "1":
                    Console.Write("Znak: ");
                    char s = Console.ReadKey().KeyChar;
                    Console.WriteLine();
                    Console.Write("Waga (int): ");
                    if (int.TryParse(Console.ReadLine(), out int f))
                    {
                        debugQueue.Insert(new Node(s, f));
                        Console.WriteLine("Dodano.");
                    }
                    break;
                case "2":
                    try {
                        var min = debugQueue.ExtractMin();
                        Console.WriteLine($"Pobrano: [{min.Symbol}] waga: {min.Frequency}");
                    } catch (Exception e) { PrintError(e.Message); }
                    break;
                case "3":
                    Console.Write("Znak: ");
                    char c = Console.ReadKey().KeyChar;
                    Console.WriteLine();
                    Console.Write("Nowa waga: ");
                    if (int.TryParse(Console.ReadLine(), out int nf))
                        debugQueue.ChangePriority(c, nf);
                    break;
                case "4":
                    debugQueue.Build(new List<Node> { new('A', 10), new('B', 5), new('C', 20), new('D', 1) });
                    Console.WriteLine("Zbudowano testową kolejkę.");
                    break;
                case "5":
                    Console.WriteLine($"Pusta? {debugQueue.IsEmpty()}");
                    break;
                case "6":
                    debugQueue.PrintQueueState();
                    break;
                case "0":
                    inDebug = false;
                    break;
            }
            if (inDebug) WaitForKey();
        }
    }

    /// <summary>
    /// Interfejs użytkownika dla procesu dekompresji.
    /// </summary>
    private void ShowDecompressionMenu()
    {
        Console.Clear();
        Console.WriteLine("--- DEKOMPRESJA PLIKU ---");

        Console.Write("Podaj ścieżkę do pliku .huff: ");
        string inputPath = Console.ReadLine() ?? "";

        Console.Write("Podaj nazwę pliku wyjściowego (np. odzyskany.txt): ");
        string outputPath = Console.ReadLine() ?? "odzyskany.txt";
        
        Console.Write("Podaj separator użyty przy kompresji (ENTER = domyślny ';'): ");
        string sepInput = Console.ReadLine();
        string separator = string.IsNullOrEmpty(sepInput) ? ";" : sepInput;

        if (!File.Exists(inputPath))
        {
            PrintError($"Plik '{inputPath}' nie istnieje!");
            return;
        }

        try
        {
            Console.WriteLine("\nRozpoczynam dekompresję...");
            var watch = System.Diagnostics.Stopwatch.StartNew();

            // Wywołanie logiki biznesowej
            FileService.Decompress(inputPath, outputPath, separator);

            watch.Stop();
            PrintSuccess($"Gotowe! Plik zapisano jako: {outputPath}");
            Console.WriteLine($"Czas operacji: {watch.ElapsedMilliseconds} ms");
        }
        catch (Exception ex)
        {
            PrintError(ex.Message);
        }

        WaitForKey();
    }

    /// <summary>
    /// Interfejs użytkownika dla procesu kompresji.
    /// </summary>
    private void ShowCompressionMenu()
    {
        Console.Clear();
        Console.WriteLine("--- KOMPRESJA PLIKU ---");
        
        Console.Write("Podaj ścieżkę do pliku wejściowego (np. dane.txt): ");
        string inputPath = Console.ReadLine() ?? "dane.txt";

        Console.Write("Podaj nazwę pliku wynikowego (np. wynik.huff): ");
        string outputPath = Console.ReadLine() ?? "wynik.huff";
        
        Console.Write("Podaj separator nagłówka (ENTER = domyślny ';'): ");
        string sepInput = Console.ReadLine();
        string separator = string.IsNullOrEmpty(sepInput) ? ";" : sepInput;

        if (!File.Exists(inputPath))
        {
            PrintError($"Plik '{inputPath}' nie istnieje!");
            return;
        }

        try
        {
            Console.WriteLine("\nRozpoczynam kompresję...");
            var watch = System.Diagnostics.Stopwatch.StartNew();

            // Wywołanie kompresji
            FileService.Compress(inputPath, outputPath, separator);

            watch.Stop();
            PrintSuccess($"Zakończono w {watch.ElapsedMilliseconds} ms.");
            
            // Statystyki
            long originalSize = new FileInfo(inputPath).Length;
            long compressedSize = new FileInfo(outputPath).Length;
            Console.WriteLine($"Oryginał:   {originalSize} bajtów");
            Console.WriteLine($"Po zmianie: {compressedSize} bajtów");
            if (originalSize > 0)
                Console.WriteLine($"Ratio:      {((double)compressedSize / originalSize):P2}");
        }
        catch (Exception ex)
        {
            PrintError(ex.Message);
        }
        
        WaitForKey();
    }
    
    private void PrintError(string msg)
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine($"\nBŁĄD: {msg}");
        Console.ResetColor();
        WaitForKey();
    }

    private void PrintSuccess(string msg)
    {
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine($"\n{msg}");
        Console.ResetColor();
        WaitForKey();
    }

    private void WaitForKey()
    {
        Console.WriteLine("\nNaciśnij dowolny klawisz...");
        Console.ReadKey();
    }
}