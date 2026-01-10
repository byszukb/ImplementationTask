using System.Text;
using ImplementationTask.Core;
using ImplementationTask.Structures;

namespace ImplementationTask.IO;

/// <summary>
/// Klasa statyczna odpowiedzialna za fizyczny zapis i odczyt skompresowanych danych z dysku.
/// Łączy logikę drzewa Huffmana z operacjami na plikach.
/// </summary>
public static class FileService
{
    // Separator oddzielający nagłówek od danych binarnych
    private static readonly byte SectionSeparator = (byte)'\n'; 
    
    /// <summary>
    /// Kompresuje plik tekstowy przy użyciu algorytmu Huffmana i zapisuje wynik do pliku binarnego.
    /// </summary>
    /// <param name="inputPath">Ścieżka do pliku tekstowego wejściowego.</param>
    /// <param name="outputPath">Ścieżka, gdzie ma zostać zapisany plik wynikowy.</param>
    /// <param name="separator">Znak oddzielający wpisy w nagłówku (domyślnie średnik).</param>
    /// <exception cref="FileNotFoundException">Rzucany, gdy plik wejściowy nie istnieje.</exception>
    public static void Compress(string inputPath, string outputPath, string separator = ";")
    {
        if (!File.Exists(inputPath))
            throw new FileNotFoundException("Nie znaleziono pliku wejściowego.");

        string content = File.ReadAllText(inputPath);
        content += '\0';

        // 1. Budowa drzewa
        HuffmanTree tree = new HuffmanTree();
        Node root = tree.Build(content);
        var codes = tree.GetCodes(root);
        HuffmanCoder coder = new HuffmanCoder(codes, root);

        using (var fs = new FileStream(outputPath, FileMode.Create))
        {
            // 2. ZAPIS NAGŁÓWKA
            // Format: A:01;B:10;C:11
            //string header = coder.GenerateHeader(separator); 
            string header = GenerateEscapedHeader(codes, separator);
            
            byte[] headerBytes = Encoding.UTF8.GetBytes(header);
            fs.Write(headerBytes);
            
            // 3. ZAPIS SEPARATORA (Nowa linia)
            fs.WriteByte(SectionSeparator);

            // 4. ZAPIS TREŚCI BINARNEJ
            string encodedText = coder.Encode(content);
            
            var bitWriter = new BitWriter(fs);
            foreach (char bit in encodedText)
            {
                // BitWriter przyjmuje int 0 lub 1
                bitWriter.WriteBit(bit == '1' ? 1 : 0);
            }
            
            bitWriter.Finish(); // Dopełnienie ostatnie bajtu, jeśli trzeba
        }
        
        Console.WriteLine($"Skompresowano do: {outputPath}");
    }

    /// <summary>
    /// Dekompresuje plik zakodowany metodą Huffmana i przywraca jego pierwotną treść tekstową.
    /// </summary>
    /// <param name="inputPath">Ścieżka do pliku skompresowanego.</param>
    /// <param name="outputPath">Ścieżka, gdzie ma zostać zapisany odkodowany plik tekstowy.</param>
    /// <param name="separator">Znak separatora użyty w nagłówku podczas kompresji.</param>
    public static void Decompress(string inputPath, string outputPath, string separator = ";")
    {
        using (var fs = new FileStream(inputPath, FileMode.Open))
        {
            // 1. Bezpieczny odczyt nagłówka (Bajt po bajcie)
            string header = ReadHeaderSafe(fs);
            
            // 2. Parsowanie i odtworzenie drzewa Huffmana
            // Musimy odtworzyć strukturę drzewa, aby móc po nim nawigować przy pomocy bitów.
            Node root = RebuildHuffmanTree(header, separator);
            
            // 3. Dekompresja
            var bitReader = new BitReader(fs);
            
            using (var writer = new StreamWriter(outputPath))
            {
                Node current = root;
                
                // Czytamy bit po bicie aż do końca strumienia
                while (true)
                {
                    int? bit = bitReader.ReadBit();
                    if (bit == null) break; // Koniec pliku

                    // Idziemy po drzewie: 0 w lewo, 1 w prawo
                    if (bit == 0)
                    {
                        if (current.Left != null) current = current.Left;
                    }
                    else
                    {
                        if (current.Right != null) current = current.Right;
                    }

                    // Czy dotarliśmy do liścia? - znak odkodowany
                    if (current.Left == null && current.Right == null)
                    {
                        if(current.Symbol == '\0')
                            break; // Koniec tekstu
                        
                        writer.Write(current.Symbol);
                        current = root; // Reset - wracamy na górę drzewa po następny znak
                    }
                }
            }
        }
        Console.WriteLine($"Zdekompresowano: {inputPath} -> {outputPath}");
    }
    
    /// <summary>
    /// Odczytuje bajty ze strumienia aż do napotkania znaku nowej linii (SectionSeparator).
    /// Zapewnia, że wskaźnik strumienia zatrzyma się idealnie na początku danych binarnych.
    /// </summary>
    /// <param name="stream">Strumień pliku wejściowego.</param>
    /// <returns>Treść nagłówka jako string (bez znaku separatora).</returns>
    private static string ReadHeaderSafe(Stream stream)
    {
        List<byte> bytes = new List<byte>();
        int b;
        while ((b = stream.ReadByte()) != -1)
        {
            if (b == SectionSeparator)
                break; // Znaleziono koniec nagłówka
            
            bytes.Add((byte)b);
        }
        return Encoding.UTF8.GetString(bytes.ToArray());
    }

    /// <summary>
    /// Odtwarza strukturę drzewa Huffmana na podstawie tekstowego nagłówka (słownika kodów).
    /// Tworzy ścieżki w drzewie dla każdego kodu (np. 'A' -> "01").
    /// </summary>
    /// <param name="header">Zawartość nagłówka (np. "A:01;B:10").</param>
    /// <param name="separator">Separator użyty do podziału par znak-kod.</param>
    /// <returns>Korzeń (Root) odbudowanego drzewa.</returns>
    private static Node RebuildHuffmanTree(string header, string separator)
    {
        Node root = new Node();
        
        string[] tokens = header.Split(separator, StringSplitOptions.RemoveEmptyEntries);

        foreach (var token in tokens)
        {
            // Szukamy ostatniego dwukropka, żeby oddzielić znak od kodu
            int splitIndex = token.LastIndexOf(':');
            if (splitIndex == -1) continue;

            string symbolStr = token.Substring(0, splitIndex);
            string code = token.Substring(splitIndex + 1);
            
            // Pobieramy znak zakładając, że to pojedynczy znak
            char symbol = UnescapeSymbol(symbolStr, separator);

            // Wstawiamy do drzewa
            Node current = root;
            foreach (char c in code)
            {
                if (c == '0')
                {
                    if (current.Left == null) current.Left = new Node();
                    current = current.Left;
                }
                else if (c == '1')
                {
                    if (current.Right == null) current.Right = new Node();
                    current = current.Right;
                }
            }
            // Na końcu ścieżki ustawiamy symbol
            current.Symbol = symbol;
        }

        return root;
    }
    
    private static string GenerateEscapedHeader(Dictionary<char, string> codes, string separator)
    {
        var entries = new List<string>();
        foreach (var kvp in codes)
        {
            string safeSymbol = EscapeSymbol(kvp.Key, separator);
            entries.Add($"{safeSymbol}:{kvp.Value}");
        }
        return string.Join(separator, entries);
    }

    private static string EscapeSymbol(char symbol, string separator)
    {
        // 1. Najpierw standardowe znaki specjalne
        if (symbol == '\n') return "\\n";
        if (symbol == '\r') return "\\r";
        if (symbol == '\t') return "\\t"; // Tabulacja też się przydaje
        if (symbol == '\\') return "\\\\";
        if (symbol == '\0') return "\\0";

        // 2. LOGIKA DYNAMICZNA:
        // Jeśli ten znak jest taki sam jak separator wybrany przez usera,
        // zamieniamy go na uniwersalny token "\SEP"
        if (symbol.ToString() == separator) return "\\SEP";

        return symbol.ToString();
    }

    private static char UnescapeSymbol(string symbolStr, string separator)
    {
        if (symbolStr == "\\n") return '\n';
        if (symbolStr == "\\r") return '\r';
        if (symbolStr == "\\t") return '\t';
        if (symbolStr == "\\\\") return '\\';
        if (symbolStr == "\\0") return '\0';

        // Jeśli widzimy "\SEP", to znaczy, że tu był ukryty separator
        // Zwracamy pierwszy znak ze stringa separatora (np. ';' lub '|')
        if (symbolStr == "\\SEP") return separator[0];
        
        if (symbolStr.Length > 0) return symbolStr[0];
        return '\0';
    }
}
    