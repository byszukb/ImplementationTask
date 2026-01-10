namespace ImplementationTask;

public class BitWriter
{
    private byte _buffer;
    private int _bitCount;
    private Stream _output;
    
    public BitWriter(Stream output)
    {
        _output = output;
        _buffer = 0;
        _bitCount = 0;
    }
    
    public void WriteBit(int bit)
    {
        if (bit != 0 && bit != 1)
            throw new ArgumentException("Bit must be 0 or 1");

        _buffer = (byte)((_buffer << 1) | bit);
        _bitCount++;

        if (_bitCount == 8)
        {
            FlushBuffer();
        }
    }
    
    private void FlushBuffer()
    {
        // 1. Zapisz zawartość bufora (pełny bajt) do strumienia pliku
        _output.WriteByte(_buffer);
    
        // 2. Wyzeruj bufor i licznik, żeby były gotowe na nową partię
        _buffer = 0;
        _bitCount = 0;
    }
    
    public void Finish()
    {
        if (_bitCount > 0)
        {
            // Przesuwamy bity w lewo, żeby znalazły się na początku bajtu
            // Np. mamy 101 (3 bity). Chcemy zapisać 10100000.
            // Przesuwamy o 5 miejsc (8 - 3).
            _buffer = (byte)(_buffer << (8 - _bitCount));
        
            _output.WriteByte(_buffer);
        
            // Resetujemy (dla porządku)
            _bitCount = 0;
            _buffer = 0;
        }
    }
}