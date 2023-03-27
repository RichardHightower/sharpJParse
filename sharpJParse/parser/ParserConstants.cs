namespace sharpJParse.parser;

public abstract class ParseConstants
{
    public const int TrueBooleanStart = 't';
    public const int NullStart = 'n';
    public const int FalseBooleanStart = 'f';
    public const int ObjectStartToken = '{';
    public const int ObjectEndToken = '}';
    public const int ArrayStartToken = '[';
    public const int ArrayEndToken = ']';
    public const int AttributeSep = ':';
    public const int ArraySep = ',';
    public const int ObjectAttributeSep = ',';

    public const int IndexBracketStartToken = ArrayStartToken;
    public const int IndexBracketEndToken = ArrayEndToken;

    public const int StringStartToken = '"';
    public const int StringEndToken = '"';

    public const int NewLineWs = '\n';
    public const int TabWs = '\t';
    public const int CarriageReturnWs = '\r';
    public const int SpaceWs = ' ';
    public const int Del = 127;
    public const int ControlEscapeToken = '\\';

    public const int Num0 = '0';
    public const int Num1 = '1';
    public const int Num2 = '2';
    public const int Num3 = '3';
    public const int Num4 = '4';
    public const int Num5 = '5';
    public const int Num6 = '6';
    public const int Num7 = '7';
    public const int Num8 = '8';
    public const int Num9 = '9';


    public const int DecimalPoint = '.';
    public const int Minus = '-';
    public const int Plus = '+';
    public const int ExponentMarker = 'e';
    public const int ExponentMarker2 = 'E';


    public const int Dot = '.';
    public const int SingleQuote = '\'';
    public const int A = 'A';
    public const int B = 'B';
    public const int C = 'C';
    public const int D = 'D';
    public const int E = 'E';
    public const int F = 'F';
    public const int G = 'G';
    public const int H = 'H';
    public const int I = 'I';
    public const int J = 'J';
    public const int K = 'K';
    public const int L = 'L';
    public const int M = 'M';
    public const int N = 'N';
    public const int O = 'O';
    public const int P = 'P';
    public const int Q = 'Q';
    public const int R = 'R';
    public const int S = 'S';
    public const int T = 'T';
    public const int U = 'U';
    public const int V = 'V';
    public const int W = 'W';
    public const int X = 'X';
    public const int Y = 'Y';
    public const int Z = 'Z';
    public const int A_ = 'a';
    public const int B_ = 'b';
    public const int C_ = 'c';
    public const int D_ = 'd';
    public const int E_ = 'e';
    public const int F_ = 'f';
    public const int G_ = 'g';
    public const int H_ = 'h';
    public const int I_ = 'i';
    public const int J_ = 'j';
    public const int K_ = 'k';
    public const int L_ = 'l';
    public const int M_ = 'm';
    public const int N_ = 'n';
    public const int O_ = 'o';
    public const int P_ = 'p';
    public const int Q_ = 'q';
    public const int R_ = 'r';
    public const int S_ = 's';
    public const int T_ = 't';
    public const int U_ = 'u';
    public const int V_ = 'v';
    public const int W_ = 'w';
    public const int X_ = 'x';
    public const int Y_ = 'y';
    public const int Z_ = 'z';

    public static readonly int NestLevel = 2_000;


    /**
     * End of text
     */
    public static readonly int Etx = 3;

    public static readonly string MinIntStr = "" + int.MinValue;
    public static readonly string MaxIntStr = "" + int.MaxValue;

    public static readonly string MinLongStr = "" + long.MinValue;
    public static readonly string MaxLongStr = "" + long.MaxValue;

    public static readonly int MinIntStrLength = MinIntStr.Length;
    public static readonly int MaxIntStrLength = MaxIntStr.Length;


    public static readonly int MinLongStrLength = MinLongStr.Length;
    public static readonly int MaxLongStrLength = MaxLongStr.Length;
}