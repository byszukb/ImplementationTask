namespace ImplementationTask.IO;

public class BitReader
{
    private Stream _input;
    private byte _buffer;
    private int _bitsCount;

    public BitReader(Stream input)
    {
        _input = input;
        _buffer = 0;
        _bitsCount = 0; // Na starcie 0, żeby wymusić pobranie pierwszego bajtu
    }
    
    public int? ReadBit()
    {
        // Jeśli skończyły nam się bity w obecnym bajcie, pobieramy kolejny z pliku
        if (_bitsCount == 0)
        {
            int readByte = _input.ReadByte();
        
            // Jeśli ReadByte zwróci -1, to znaczy, że koniec pliku (EOF)
            if (readByte == -1)
                return null; // Zwracamy null jako sygnał "koniec danych"

            _buffer = (byte)readByte;
            _bitsCount = 8; // Resetujemy licznik, mamy nową świeżą "ósemkę"
        }
    
        // 1. Pobieramy najstarszy bit
        int bit = (_buffer >> 7) & 1;

        // 2. Przesuwamy bufor w lewo, przygotowując kolejny bit na następny odczyt
        _buffer = (byte)(_buffer << 1);

        // 3. Zmniejszamy licznik dostępnych bitów
        _bitsCount--;

        return bit;
    }
}