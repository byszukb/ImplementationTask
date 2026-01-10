using System.Text;

namespace ImplementationTask;

public class HuffmanCoder
{
    private Dictionary<char, string> _codes;
    private Node _root;

    public HuffmanCoder(Dictionary<char, string> codes, Node root)
    {
        _codes = codes;
        _root = root;
    }

    public string Encode(string text)
    {
        StringBuilder encoded = new StringBuilder();

        foreach (char c in text)
        {
            if (_codes.ContainsKey(c))
            {
                encoded.Append(_codes[c]);
            }
        }
        
        return encoded.ToString();
    }
    
    public string Decode(string encodedText)
    {
        StringBuilder decoded = new StringBuilder();
        Node current = _root;

        foreach (char bit in encodedText)
        {
            if (bit == '0')
            {
                current = current.Left!;
            }
            else if (bit == '1')
            {
                current = current.Right!;
            }

            // Jeśli dotarliśmy do liścia
            if (current.Left == null && current.Right == null)
            {
                decoded.Append(current.Symbol);
                current = _root; // Wracamy do korzenia
            }
        }

        return decoded.ToString();
    }

    public string GenerateHeader(string separator)
    {
        return string.Join(separator, _codes.Select(kvp => $"{kvp.Key}:{kvp.Value}"));
    }
}