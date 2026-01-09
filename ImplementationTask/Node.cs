namespace ImplementationTask;

//Klasa węzła drzewa Huffmana
public class Node() : IComparable<Node>
{
    public char Symbol { get; set; }
    public int Frequency { get; set; }
    public Node? Left { get; set; }
    public Node? Right { get; set; }

    public Node(char symbol, int frequency, Node? left = null, Node? right = null) : this()
    {
        Symbol = symbol;
        Frequency = frequency;
        Left = left;
        Right = right;
    }

    public int CompareTo(Node? other)
    {
        if (other == null) return 1;
        if(Frequency < other.Frequency)
            return -1;
        if(Frequency > other.Frequency)
            return 1;
        return 0;
    }
}