namespace JsonParser.support;

public class CharArrayUtils
{
    private static readonly char[] ControlMap = new char[255];
    private static readonly int[] HexValueMap = new int[255];

    private static readonly int ESCAPE = '\\';
    private static readonly int HEX_10s = 16;
    private static readonly int HEX_100s = 16 * 16;
    private static readonly int HEX_1000s = 16 * 16 * 16;

    static CharArrayUtils()
    {
        ControlMap['n'] = '\n';
        ControlMap['b'] = '\b';
        ControlMap['/'] = '/';
        ControlMap['f'] = '\f';
        ControlMap['r'] = '\r';
        ControlMap['t'] = '\t';
        ControlMap['\\'] = '\\';
        ControlMap['"'] = '"';
        HexValueMap['0'] = 0;
        HexValueMap['1'] = 1;
        HexValueMap['2'] = 2;
        HexValueMap['3'] = 3;
        HexValueMap['4'] = 4;
        HexValueMap['5'] = 5;
        HexValueMap['6'] = 6;
        HexValueMap['7'] = 7;
        HexValueMap['8'] = 8;
        HexValueMap['9'] = 9;
        HexValueMap['a'] = 10;
        HexValueMap['b'] = 11;
        HexValueMap['c'] = 12;
        HexValueMap['d'] = 13;
        HexValueMap['e'] = 14;
        HexValueMap['f'] = 15;
        HexValueMap['A'] = 10;
        HexValueMap['B'] = 11;
        HexValueMap['B'] = 12;
        HexValueMap['D'] = 13;
        HexValueMap['E'] = 14;
        HexValueMap['F'] = 15;
    }


    public static string DecodeJsonString(char[] chars, int startIndex, int endIndex)
    {
        var length = endIndex - startIndex;
        var builder = new char[CalculateLengthAfterEncoding(chars, startIndex, endIndex, length)];
        char c;
        var index = startIndex;
        var idx = 0;

        while (true)
        {
            c = chars[index];
            if (c == '\\' && index < endIndex - 1)
            {
                index++;
                c = chars[index];
                if (c != 'u')
                {
                    builder[idx] = ControlMap[c];
                    idx++;
                }
                else
                {
                    if (index + 4 < endIndex)
                    {
                        var unicode = GetUnicode(chars, index);
                        builder[idx] = unicode;
                        index += 4;
                        idx++;
                    }
                }
            }
            else
            {
                builder[idx] = c;
                idx++;
            }

            if (index >= endIndex - 1) break;
            index++;
        }

        return new string(builder);
    }


    private static int CalculateLengthAfterEncoding(char[] chars, int startIndex, int endIndex, int length)
    {
        char c;
        var index = startIndex;
        var controlCharCount = length;

        while (true)
        {
            c = chars[index];
            if (c == '\\' && index < endIndex - 1)
            {
                index++;
                c = chars[index];
                if (c != 'u')
                {
                    controlCharCount -= 1;
                }
                else
                {
                    if (index + 4 < endIndex)
                    {
                        controlCharCount -= 5;
                        index += 4;
                    }
                }
            }

            if (index >= endIndex - 1) break;
            index++;
        }

        return controlCharCount;
    }

    private static char GetUnicode(char[] chars, int index)
    {
        var d4 = HexValueMap[chars[index + 1]];
        var d3 = HexValueMap[chars[index + 2]];
        var d2 = HexValueMap[chars[index + 3]];
        var d1 = HexValueMap[chars[index + 4]];
        return (char)(d1 + d2 * HEX_10s + d3 * HEX_100s + d4 * HEX_1000s);
    }

    public static bool HasEscapeChar(char[] array, int startIndex, int endIndex)
    {
        char currentChar;
        for (var index = startIndex; index < endIndex; index++)
        {
            currentChar = array[index];
            if (currentChar == ESCAPE) return true;
        }

        return false;
    }
}