namespace ImplementationTask.Structures;

/// <summary>
/// Własna implementacja Kolejki Priorytetowej oparta na Kopcu Minimalnym (Min-Heap).
/// Element o najmniejszej wartości (częstości) jest zawsze na szczycie (indeks 0).
/// </summary>
public class PriorityQueue
{
    // Lista przechowująca elementy kopca w strukturze tablicowej.
    // Relacje: dla indeksu 'i', dzieci są pod '2*i + 1' oraz '2*i + 2'.
    private List<Node> Elements { get; set; }
    
    // Zwraca aktualną liczbę elementów w kolejce.
    public int Count => Elements.Count;
    
    public PriorityQueue()
    {
        Elements = new List<Node>();
    }
    
    /// <summary>
    /// Dodaje nowy węzeł do kolejki i naprawia kopiec w górę.
    /// </summary>
    public void Insert(Node node)
    {
        Elements.Add(node); // Krok 1: Dodaj na sam koniec listy
        FixHeap(node);      // Krok 2: "Wypłyń" elementem w górę na właściwe miejsce
    }

    /// <summary>
    /// Algorytm "Bubble Up" (Wypływanie).
    /// Przesuwa dodany element w górę drzewa tak długo, jak jest on mniejszy od swojego rodzica.
    /// </summary>
    private void FixHeap(Node node)
    {
        int childIndex = Elements.Count - 1; // Startujemy od końca (tam dodaliśmy element)
        int parentIndex = (childIndex - 1) / 2; // Wzór na indeks rodzica

        // Pętla działa dopóki nie dotrzemy do korzenia (childIndex > 0)
        // Oraz dopóki dziecko jest ważniejsze (mniejsze) od rodzica.
        while (childIndex > 0 && node.CompareTo(Elements[parentIndex]) < 0)
        {
            // Zamiana miejscami (Swap)
            var temp = Elements[parentIndex];
            Elements[parentIndex] = node;
            Elements[childIndex] = temp;
            
            // Aktualizacja indeksów do kolejnego obrotu pętli (idziemy w górę)
            childIndex = parentIndex;
            parentIndex = (childIndex - 1) / 2;
        }
    }
    
    /// <summary>
    /// Usuwa i zwraca element o najmniejszym priorytecie (korzeń).
    /// Następnie naprawia kopiec w dół (Bubble Down).
    /// </summary>
    public Node ExtractMin()
    {
        if (Elements.Count == 0)
            throw new InvalidOperationException("Queue is empty");

        // 1. Pobieramy korzeń (najmniejszy element) - to nasz wynik.
        Node minNode = Elements[0];

        // 2. Przenosimy ostatni element na miejsce korzenia, aby zachować ciągłość struktury drzewa.
        int lastIndex = Elements.Count - 1;
        Elements[0] = Elements[lastIndex];
        Elements.RemoveAt(lastIndex); // Usuwamy fizycznie ostatni element z końca

        // 3. Naprawiamy kopiec w dół (Bubble Down) - "Spychanie" nowego korzenia
        lastIndex--; // Rozmiar się zmniejszył
        int parentIndex = 0;

        while (true)
        {
            int leftChild = 2 * parentIndex + 1;
            int rightChild = 2 * parentIndex + 2;
            int smallest = parentIndex;

            // Sprawdź czy lewe dziecko istnieje i jest mniejsze od rodzica
            if (leftChild <= lastIndex && Elements[leftChild].CompareTo(Elements[smallest]) < 0)
            {
                smallest = leftChild;
            }

            // Sprawdź czy prawe dziecko istnieje i jest mniejsze od "aktualnie najmniejszego"
            if (rightChild <= lastIndex && Elements[rightChild].CompareTo(Elements[smallest]) < 0)
            {
                smallest = rightChild;
            }

            // Jeśli rodzic nadal jest najmniejszy z trójki, kopiec jest naprawiony. Koniec.
            if (smallest == parentIndex)
                break;

            // Zamiana miejscami z mniejszym dzieckiem
            // ReSharper disable once SwapViaDeconstruction
            var temp = Elements[parentIndex];
            Elements[parentIndex] = Elements[smallest];
            Elements[smallest] = temp;

            // Idziemy dalej w dół drzewa
            parentIndex = smallest;
        }

        return minNode;
    }
    
    /// <summary>
    /// Sprawdza, czy w kolejce znajdują się jakiekolwiek elementy.
    /// </summary>
    public bool IsEmpty()
    {
        return Elements.Count == 0;
    }
    
    /// <summary>
    /// Buduje kolejkę priorytetową na podstawie dostarczonej listy węzłów.
    /// </summary>
    public void Build(IEnumerable<Node> data)
    {
        Elements.Clear();
        foreach (var node in data)
        {
            Insert(node);
        }
    }
    
    /// <summary>
    /// Zmienia priorytet (częstość występowania) węzła reprezentującego dany symbol.
    /// Wymaga to naprawy kopca w górę (Bubble Up), czyli przesunięcia elementu w stronę korzenia.
    /// </summary>
    /// <param name="symbol">Znak, którego wagę chcemy zmienić.</param>
    /// <param name="newFrequency">Nowa, mniejsza częstość występowania.</param>
    public void ChangePriority(char symbol, int newFrequency)
    {
        // 1. Szukamy, pod którym indeksem jest nasz znak
        int index = Elements.FindIndex(n => n.Symbol == symbol);

        if (index == -1)
        {
            Console.WriteLine("Symbol not found in the priority queue.");
            return; 
        }

        // 2. Pobieramy ten węzeł
        Node node = Elements[index];

        // Sprawdzamy, czy nowa waga jest faktycznie mniejsza (wymóg "zmniejsz priorytet")
        if (newFrequency < node.Frequency)
        {
            node.Frequency = newFrequency;

            // 3. Naprawiamy kopiec w górę (Bubble Up)
        
            int childIndex = index;
            int parentIndex = (childIndex - 1) / 2;

            while (childIndex > 0 && Elements[childIndex].CompareTo(Elements[parentIndex]) < 0)
            {
                // Zamiana miejscami (Swap)
                // ReSharper disable once SwapViaDeconstruction
                var temp = Elements[parentIndex];
                Elements[parentIndex] = Elements[childIndex];
                Elements[childIndex] = temp;

                // "Zmniejszamy indeks" - idziemy piętro wyżej
                childIndex = parentIndex;
                parentIndex = (childIndex - 1) / 2;
            }
        }
    }
    
    public void PrintQueueState()
    {
        Console.WriteLine("\n--- STAN WEWNĘTRZNY KOPCA ---");
        if (Elements.Count == 0)
        {
            Console.WriteLine("(Pusta)");
            return;
        }

        for (int i = 0; i < Elements.Count; i++)
        {
            var node = Elements[i];
            // Obliczamy indeksy dzieci, żeby widzieć relacje
            int left = 2 * i + 1;
            int right = 2 * i + 2;
            
            string childrenInfo = "";
            if (left < Elements.Count) childrenInfo += $"L:{Elements[left].Frequency} ";
            if (right < Elements.Count) childrenInfo += $"P:{Elements[right].Frequency}";

            Console.WriteLine($"[{i}] Znak: '{node.Symbol}' Waga: {node.Frequency} \t-> (Dzieci: {childrenInfo})");
        }
        Console.WriteLine("-----------------------------");
    }
}