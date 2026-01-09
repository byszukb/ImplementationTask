namespace ImplementationTask;

public class TextMenu
{
    private readonly PriorityQueue _queue;

    public TextMenu(PriorityQueue queue)
    {
        _queue = queue;
    }

    public void Run()
    {
        bool running = true;
        while (running)
        {
            Console.Clear();
            Console.WriteLine("=== TEST KOLEJKI PRIORYTETOWEJ ===");
            Console.WriteLine($"Liczba elementów: {_queue.Count}");
            Console.WriteLine("1. Dodaj element (Insert)");
            Console.WriteLine("2. Pobierz minimum (ExtractMin)");
            Console.WriteLine("3. Zmień priorytet (ChangePriority)");
            Console.WriteLine("4. Zbuduj z przykładowych danych (Build)");
            Console.WriteLine("5. Sprawdź czy pusta (IsEmpty)");
            Console.WriteLine("6. Uruchom test weryfikacyjny");
            Console.WriteLine("7. Wyświetl stan kolejki");
            Console.WriteLine("0. Wyjście");
            Console.Write("\nWybierz opcję: ");

            switch (Console.ReadLine())
            {
                case "1":
                    Console.Write("Podaj znak: ");
                    char symbol = Console.ReadKey().KeyChar;
                    Console.WriteLine();
                    Console.Write("Podaj częstość (int): ");
                    if (int.TryParse(Console.ReadLine(), out int freq))
                    {
                        _queue.Insert(new Node(symbol, freq));
                        Console.WriteLine("Dodano!");
                    }
                    break;

                case "2":
                    try
                    {
                        var min = _queue.ExtractMin();
                        Console.WriteLine($"\nPobrano: [{min.Symbol}] z wagą {min.Frequency}");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"\nBłąd: {ex.Message}");
                    }
                    break;

                case "3":
                    Console.Write("Podaj znak do zmiany: ");
                    char s = Console.ReadKey().KeyChar;
                    Console.WriteLine();
                    Console.Write("Podaj nową (mniejszą) częstość: ");
                    if (int.TryParse(Console.ReadLine(), out int newFreq))
                    {
                        try
                        {
                            _queue.ChangePriority(s, newFreq);
                            Console.WriteLine("Zmieniono priorytet.");
                        }
                        catch (Exception ex) { Console.WriteLine(ex.Message); }
                    }
                    break;

                case "4":
                    var data = new List<Node>
                    {
                        new Node('A', 10),
                        new Node('B', 5),
                        new Node('C', 20),
                        new Node('D', 1) 
                    };
                    _queue.Build(data);
                    Console.WriteLine("\nZbudowano kolejkę z: A(10), B(5), C(20), D(1)");
                    break;
                
                case "5":
                    Console.WriteLine($"\nCzy pusta? {_queue.IsEmpty()}");
                    break;

                case "6":
                    Console.WriteLine("--- TEST WERYFIKACYJNY ---");
                    // 1. Czyścimy kolejkę (żeby stare dane nie przeszkadzały)
                    // Uwaga: Możesz potrzebować metody Clear() w PriorityQueue (Elements.Clear())
                    // Lub po prostu stwórz nową instancję kolejki na potrzeby testu.
                    var testQueue = new PriorityQueue(); 
    
                    Console.WriteLine("Dodaję losowo: Z(50), A(10), K(5), B(30)");
                    testQueue.Insert(new Node('Z', 50));
                    testQueue.Insert(new Node('A', 10));
                    testQueue.Insert(new Node('K', 5));
                    testQueue.Insert(new Node('B', 30));
    
                    Console.WriteLine("\nTeraz wyciągam (ExtractMin) wszystko po kolei:");
                    while (!testQueue.IsEmpty())
                    {
                        var n = testQueue.ExtractMin();
                        Console.Write($"{n.Frequency}({n.Symbol}) -> ");
                    }
                    Console.WriteLine("KONIEC");
                    Console.WriteLine("\nJeśli widzisz: 5(K) -> 10(A) -> 30(B) -> 50(Z), to DZIAŁA!");
                    break;
                
                case "7":
                    _queue.PrintQueueState();
                    break;

                case "0":
                    running = false;
                    break;
            }
            
            if (running)
            {
                Console.WriteLine("\nNaciśnij dowolny klawisz...");
                Console.ReadKey();
            }
        }
    }
}