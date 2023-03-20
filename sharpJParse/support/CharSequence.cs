namespace sharpJParse;

public interface CharSequence
{
    string ToString();
    char GetCharAt(int index);
    int GetLength();

    CharSequence SubSequence(int start, int end);
}