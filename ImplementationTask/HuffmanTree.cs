namespace ImplementationTask;

/// <summary>
/// Klasa odpowiedzialna za logikę Drzewa Huffmana: 
/// liczenie częstości znaków, budowanie struktury drzewa i generowanie kodów binarnych.
/// </summary>
public class HuffmanTree
{
    // Zlicza wystąpienia każdego znaku w podanym tekście.
    private Dictionary<char, int> GetFrequencies(string text)
    {
        var frequencies = new Dictionary<char, int>();

        foreach (char c in text)
        {
            if (frequencies.ContainsKey(c))
            {
                frequencies[c]++;
            }
            else
            {
                frequencies[c] = 1;
            }
        }
        
        return frequencies;
    }
    
    /// <summary>
    /// Buduje Drzewo Huffmana na podstawie podanego tekstu.
    /// </summary>
    /// <param name="text">Tekst wejściowy, dla którego chcemy zbudować drzewo.</param>
    /// <returns>Korzeń (Root) utworzonego drzewa.</returns>
    public Node Build(string text)
    {
        var frequencies = GetFrequencies(text);
        
        var priorityQueue = new PriorityQueue();
        
        foreach (var kvp in frequencies)
        {
            priorityQueue.Insert(new Node(kvp.Key, kvp.Value));
        }

        while (priorityQueue.Count > 1)
        {
            Node left = priorityQueue.ExtractMin();
            Node right = priorityQueue.ExtractMin();
            Node merged = new Node('*', left.Frequency + right.Frequency, left, right);
            priorityQueue.Insert(merged);
        }
        return priorityQueue.ExtractMin();
    }
    
    // Metoda pomocnicza (rekurencyjna) do chodzenia po drzewie.
    private void GenerateCodes(Node node, string code, Dictionary<char, string> codes)
    {
        if (node.Left == null && node.Right == null)
        {
            codes[node.Symbol] = code;
            return;
        }

        if (node.Left != null)
        {
            GenerateCodes(node.Left, code + "0", codes);
        }

        if (node.Right != null)
        {
            GenerateCodes(node.Right, code + "1", codes);
        }
    }
    
    /// <summary>
    /// Generuje słownik kodów binarnych dla każdego znaku w drzewie (np. 'A' -> "101").
    /// </summary>
    /// <param name="root">Korzeń drzewa Huffmana.</param>
    /// <returns>Słownik, gdzie kluczem jest znak, a wartością jego kod binarny.</returns>
    public Dictionary<char, string> GetCodes(Node root)
    {
        var codes = new Dictionary<char, string>();
        GenerateCodes(root, "", codes);
        return codes;
    }
}