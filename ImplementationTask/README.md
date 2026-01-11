# Kompresor Huffmana

Program do kompresji i dekompresji plików tekstowych przy użyciu algorytmu Huffmana.

## Wymagania

- .NET 9.0

## Uruchomienie

```bash
cd ImplementationTask
dotnet run
```

## Menu główne

```
1. Kompresuj plik (Tekst -> Huffman)
2. Dekompresuj plik (Huffman -> Tekst)
3. Narzędzia deweloperskie (Test Kolejki Priorytetowej)
0. Wyjście
```

## Kompresja (opcja 1)

1. Podaj ścieżkę do pliku tekstowego (np. `dane.txt`)
2. Podaj nazwę pliku wynikowego (np. `wynik.huff`)
3. Opcjonalnie zmień separator (domyślnie `;`)

Program wyświetli statystyki kompresji (rozmiar przed i po).

## Dekompresja (opcja 2)

1. Podaj ścieżkę do pliku `.huff`
2. Podaj nazwę pliku wyjściowego (np. `odzyskany.txt`)
3. Podaj separator użyty przy kompresji (domyślnie `;`)

## Test kolejki priorytetowej (opcja 3)

Pozwala przetestować operacje na kolejce:

| Opcja | Operacja | Opis |
|-------|----------|------|
| 1 | Insert | Dodaj element (znak + waga) |
| 2 | ExtractMin | Pobierz element o najmniejszej wadze |
| 3 | ChangePriority | Zmień wagę elementu (tylko zmniejszanie) |
| 4 | Build | Zbuduj kolejkę z danych testowych |
| 5 | IsEmpty | Sprawdź czy kolejka jest pusta |
| 6 | PrintQueueState | Wyświetl stan kopca |

## Format pliku .huff

```
A:01;B:10;C:11;\n:110
[dane binarne]
```

- **Linia 1:** Słownik kodów (znak:kod, oddzielone separatorem)
- **Od linii 2:** Skompresowane dane binarne

## Pliki wynikowe

Przy uruchomieniu przez `dotnet run`, katalogiem roboczym jest:

```
ImplementationTask/bin/Debug/net9.0/
```

Tam będą zapisywane i odczytywane pliki (np. `dane.txt`, `wynik.huff`).

Aby używać plików z innego katalogu, podaj pełną ścieżkę (np. `/Users/nazwa/dokumenty/plik.txt`).

## Struktura projektu

```
ImplementationTask/
├── Program.cs              # Punkt wejścia
├── Core/
│   ├── HuffmanTree.cs      # Budowanie drzewa i generowanie kodów
│   └── HuffmanCoder.cs     # Kodowanie i dekodowanie tekstu
├── Structures/
│   ├── PriorityQueue.cs    # Kolejka priorytetowa (min-heap)
│   └── Node.cs             # Węzeł drzewa Huffmana
├── IO/
│   ├── FileService.cs      # Operacje na plikach
│   ├── BitWriter.cs        # Zapis bitów do pliku
│   └── BitReader.cs        # Odczyt bitów z pliku
└── UI/
    └── TextMenu.cs         # Menu użytkownika
```

## Autor
Bartłomiej Byszuk
