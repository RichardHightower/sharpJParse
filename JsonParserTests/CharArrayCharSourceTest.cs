using System.Diagnostics.CodeAnalysis;
using System.Numerics;
using sharpJParse.JsonParser.parser;
using sharpJParse.JsonParser.source;
using sharpJParse.JsonParser.source.support;

namespace sharpJParse;

#pragma warning disable NUnit2005
[SuppressMessage("ReSharper", "SuggestVarOrType_SimpleTypes")]
public class CharArrayCharSourceTest
{
    [Test]
    public void NextTest()
    {
        //...................01234567890123456789
        const string json = "01";
        ICharSource source =
            Sources.StringSource(Json.NiceJson(json));

        char next = (char)source.Next();
        Assert.AreEqual('0', next);
        Assert.AreEqual(0, source.GetIndex());

        next = (char)source.Next();
        Assert.AreEqual('1', next);
        Assert.AreEqual(1, source.GetIndex());

        next = (char)source.Next();
        Assert.AreEqual(ParseConstants.Etx, next);
        Assert.AreEqual(json.Length, source.GetIndex());
    }

    /* Basic test to test nextSkipWhiteSpace and getIndex */
    [Test]
    public void NextSkipWhiteSpace2()
    {
        //...................01234567890123456789
        const string json = "\t\n01";

        ICharSource source =
            Sources.StringSource(Json.NiceJson(json));
        char next = (char)source.NextSkipWhiteSpace();
        Assert.AreEqual('0', next);
        Assert.AreEqual(2, source.GetIndex());

        next = (char)source.NextSkipWhiteSpace();
        Assert.AreEqual('1', next);
        Assert.AreEqual(3, source.GetIndex());

        next = (char)source.NextSkipWhiteSpace();
        Assert.AreEqual(ParseConstants.Etx, next);
        Assert.AreEqual(4, source.GetIndex());
    }

    [Test]
    public void NextSkipWhiteSpaceSample()
    {
        //.....................01234
        string str = "[ 1 , 3 ]";
        ICharSource charSource = Sources.StringSource(str);


        Assert.AreEqual('[', (char)charSource.NextSkipWhiteSpace());
        Assert.AreEqual('1', (char)charSource.NextSkipWhiteSpace());
        Assert.AreEqual(',', (char)charSource.NextSkipWhiteSpace());
        Assert.AreEqual('3', (char)charSource.NextSkipWhiteSpace());
        Assert.AreEqual(']', (char)charSource.NextSkipWhiteSpace());
    }

    [Test]
    public void SkipWhiteSpace()
    {
        //............012345678901234567890123456789012345678
        string str = "         1               [ 1 , 3 ]";
        ICharSource source = Sources.StringSource(str);
        Assert.AreEqual('1', (char)source.NextSkipWhiteSpace());
        source.Next();
        source.SkipWhiteSpace();
        Assert.AreEqual(25, source.GetIndex());
        Assert.AreEqual('[', source.GetCurrentChar());
    }

    [Test]
    public void SkipWhiteSpace2()
    {
        //............ 01234567890123456789
        string json = "\t\n01";
        ICharSource source =
            Sources.StringSource(Json.NiceJson(json));

        source.Next();

        source.SkipWhiteSpace();

        char next = source.GetCurrentChar();
        Assert.AreEqual('0', next);
        Assert.AreEqual(2, source.GetIndex());

        next = (char)source.NextSkipWhiteSpace();
        Assert.AreEqual('1', next);
        Assert.AreEqual(3, source.GetIndex());
        Assert.AreEqual('1', source.GetCurrentChar());
        Assert.AreEqual('1', source.GetCurrentCharSafe());

        next = (char)source.NextSkipWhiteSpace();
        Assert.AreEqual(ParseConstants.Etx, next);
        Assert.AreEqual(4, source.GetIndex());

        // Does bound check so returns end of stream.
        Assert.AreEqual(ParseConstants.Etx, source.GetCurrentCharSafe());

        try
        {
            //Does no bounds check so gets the next char which is a space.
            Assert.AreEqual(' ', source.GetCurrentChar());
            Assert.Fail();
        }
        catch (Exception ex)
        {
            Console.WriteLine("" + ex);
        }
    }


    //Left off here TODO 
    
    
    [Test]
    public void GetCharAt() {

        //...................01234567890123456789
        const string json = "01";
        ICharSource source =
            Sources.StringSource(Json.NiceJson(json));

        Assert.AreEqual('0', source.GetChartAt(0));
        Assert.AreEqual('1', source.GetChartAt(1));

        try {
            /* No bounds check */
            Assert.AreEqual(' ', source.GetChartAt(2));
            Assert.Fail();
        } catch (Exception ex) {
            Console.WriteLine(ex);
        }

    }

    [Test]
    public void ToStringTest() {

        //...................01234567890123456789
        const string json = "01";
        ICharSource source =
            Sources.StringSource(Json.NiceJson(json));

        Assert.AreEqual("01", source.ToString());

    }


    [Test]
    public void FindEndOfNumberFast() {

        //...................01234567890123456789
        const string json = "01";
        ICharSource source =
            Sources.StringSource(Json.NiceJson(json));

        NumberParseResult result  = source.FindEndOfNumberFast();
        Assert.AreEqual(2, result.EndIndex());
        Assert.False(result.WasFloat());

    }
    
    [Test]
    public void FindEndOfNumberFastFloat() {

        //...................01234567890123456789
        const string json = "0.2";
        ICharSource source =
                Sources.StringSource(Json.NiceJson(json));

        NumberParseResult result  = source.FindEndOfNumberFast();
        Assert.AreEqual(3, result.EndIndex());
        Assert.True(result.WasFloat());

    }

    [Test]
    public void FindEndOfNumberFastFloatExponentInArray() {

        //...................01234567890123456789
        const string json = "0.2e12] ";
        ICharSource source =
                Sources.StringSource(Json.NiceJson(json));

        NumberParseResult result  = source.FindEndOfNumberFast();
        Assert.AreEqual(6, result.EndIndex());
        Assert.True(result.WasFloat());

    }



    [Test]
    public void FindEndOfNumberFastInArray() {

        //...................01234567890123456789
        const string json = "01]";
        ICharSource source =
                Sources.StringSource(Json.NiceJson(json));

        NumberParseResult result  = source.FindEndOfNumberFast();
        Assert.AreEqual(2, result.EndIndex());
        Assert.False(result.WasFloat());

    }

    [Test]
    public void FindEndOfNumberFastFloatInArray() {

        //...................01234567890123456789
        const string json = "0.2]";
        ICharSource source =
                Sources.StringSource(Json.NiceJson(json));

        NumberParseResult result  = source.FindEndOfNumberFast();
        Assert.AreEqual(3, result.EndIndex());
        Assert.True(result.WasFloat());

    }

    [Test]
    public void FindEndOfNumberFastFloatExponentInArray2() {

        //...................01234567890123456789
        const string json = "0.2e12] ";
        ICharSource source =
                Sources.StringSource(Json.NiceJson(json));

        NumberParseResult result  = source.FindEndOfNumberFast();
        Assert.AreEqual(6, result.EndIndex());
        Assert.True(result.WasFloat());

    }


    [Test]
    public void FindEndOfNumber() {

        //...................01234567890123456789
        const string json = "12";
        ICharSource source =
            Sources.StringSource(Json.NiceJson(json));
        source.Next();

        NumberParseResult result  = source.FindEndOfNumber();
        Assert.AreEqual(2, result.EndIndex());
        Assert.False(result.WasFloat());

    }

    [Test]
    public void FindEndOfNumberFloat() {

        //...................01234567890123456789
        const string json = "0.2";
        ICharSource source =
            Sources.StringSource(Json.NiceJson(json));

        source.Next();

        NumberParseResult result  = source.FindEndOfNumber();
        Assert.AreEqual(3, result.EndIndex());
        Assert.True(result.WasFloat());

    }

    [Test]
    public void FindEndOfNumberFloatExponentInArray() {

        //...................01234567890123456789
        const string json = "0.2e12] ";
        ICharSource source =
            Sources.StringSource(Json.NiceJson(json));

        source.Next();
        NumberParseResult result  = source.FindEndOfNumber();
        Assert.AreEqual(6, result.EndIndex());
        Assert.True(result.WasFloat());

    }



    [Test]
    public void FindEndOfNumberInArray() {

        //...................01234567890123456789
        const string json = "12]";
        ICharSource source =
            Sources.StringSource(Json.NiceJson(json));

        source.Next();

        NumberParseResult result  = source.FindEndOfNumber();
        Assert.AreEqual(2, result.EndIndex());
        Assert.False(result.WasFloat());

    }
    
    [Test]
    public void FindEndOfNumberFloatInArray()
    {
        //...................01234567890123456789
        string json = "0.2]";
        ICharSource source =
            Sources.StringSource(Json.NiceJson(json));

        source.Next();
        NumberParseResult result = source.FindEndOfNumber();
        Assert.AreEqual(3, result.EndIndex());
        Assert.True(result.WasFloat());
    }

    [Test]
    public void FindEndOfNumberFloatExponentInArray2()
    {
        //...................01234567890123456789
        string json = "0.2e12] ";
        ICharSource source =
            Sources.StringSource(Json.NiceJson(json));

        source.Next();
        NumberParseResult result = source.FindEndOfNumber();
        Assert.AreEqual(6, result.EndIndex());
        Assert.True(result.WasFloat());
    }

    [Test]
    public void FindTrueEnd()
    {
        //...................01234567890123456789
        string json = "true";
        ICharSource source =
            Sources.StringSource(Json.NiceJson(json));

        source.Next();
        Assert.AreEqual(4, source.FindTrueEnd());
    }

    [Test]
    public void FindFalseEnd()
    {
        //...................01234567890123456789
        string json = "false";
        ICharSource source =
            Sources.StringSource(Json.NiceJson(json));

        source.Next();
        Assert.AreEqual(5, source.FindFalseEnd());
    }

    [Test]
    public void FindNullEnd()
    {
        //...................01234567890123456789
        const string json = "null";
        ICharSource source =
            Sources.StringSource(Json.NiceJson(json));

        source.Next();
        Assert.AreEqual(4, source.FindNullEnd());
    }


    [Test]
    public void MatchChars()
    {
        //...................01234567890123456789
        const string json = "abcd";
        ICharSource source =
            Sources.StringSource(Json.NiceJson(json));

        source.Next();
        Assert.True(source.MatchChars(1, 3, new StringCharSequence(new string("bc"))));
        Assert.False(source.MatchChars(1, 3, new StringCharSequence( new string("ab"))));
    }

    [Test]
    public void ParseDoubleSimple()
    {
        //...................01234567890123456789
        string json = " 1.2 ";
        ICharSource source =
            Sources.StringSource(Json.NiceJson(json));

        Assert.AreEqual(1.2, source.GetDouble(1, 4));
    }

    [Test]
    public void ParseDoubleExp()
    {
        //...................01234567890123456789
        string json = " 1.2e12 ";
        ICharSource source =
            Sources.StringSource(Json.NiceJson(json));

        Assert.AreEqual(1.2e12, source.GetDouble(1, 7));
    }

    [Test]
    public void FindCommaOrEnd()
    {
        // .......................012345
        //...................01234567890123456789
        string json = "  ,2] ";
        ICharSource source =
            Sources.StringSource(Json.NiceJson(json));

        source.Next();

        Assert.False(source.FindCommaOrEndForArray());
        Assert.AreEqual(2, source.GetIndex());
    }

    [Test]
    public void FindCommaOrEndEndCase()
    {
        // .......................012345
        //...................01234567890123456789
        string json = "  ] ";
        ICharSource source =
            Sources.StringSource(Json.NiceJson(json));
        source.Next();

        Assert.True(source.FindCommaOrEndForArray());
        Assert.AreEqual(3, source.GetIndex());
    }


    [Test]
    public void ReadNumberWithNothingBeforeDecimal()
    {
        //.................0123456789
        const string json = "-.123";
        ICharSource source = Sources.StringSource(Json.NiceJson(json));
        source.Next();
        try
        {
            source.FindEndOfNumber();
            Assert.True(false);
        }
        catch (Exception ex)
        {
            Console.WriteLine("" + ex);
        }
    }

    [Test]
    public void ReadNumberNormalNegativeFloat()
    {
        //.................0123456789
        const string json = "-0.123";
        ICharSource source = Sources.StringSource(Json.NiceJson(json));
        source.Next();
        NumberParseResult result = source.FindEndOfNumber();
        Assert.True(result.WasFloat());
        Assert.AreEqual(6, result.EndIndex());
    }


    [Test]
    public void Encoding()
    {
        //.................0123456789
        string json = "'`u0003'";
        ICharSource source = Sources.StringSource(Json.NiceJson(json));
        source.Next();
        int end = source.FindEndOfEncodedString();
        Assert.AreEqual(7, end);
        Assert.AreEqual(8, source.GetIndex());
    }

    [Test]
    public void EncodingFast()
    {
        //.................0123456789
        string json = "'`u0003'";
        ICharSource source = Sources.StringSource(Json.NiceJson(json));
        source.Next();
        int end = source.FindEndOfEncodedStringFast();
        Assert.AreEqual(7, end);
        Assert.AreEqual(8, source.GetIndex());
    }

    [Test]
    public void Encoding2()
    {
        //.................0123456789012
        string json = "'abc`n`u0003'";
        ICharSource source = Sources.StringSource(Json.NiceJson(json));
        source.Next();
        int end = source.FindEndOfEncodedString();
        Assert.AreEqual(12, end);
        Assert.AreEqual(13, source.GetIndex());
    }

    [Test]
    public void Encoding2Fast()
    {
        //.................0123456789012
        string json = "'abc`n`u0003'";
        ICharSource source = Sources.StringSource(Json.NiceJson(json));
        source.Next();
        int end = source.FindEndOfEncodedStringFast();
        Assert.AreEqual(12, end);
        Assert.AreEqual(13, source.GetIndex());
    }

    [Test]
    public void Encoding3()
    {
        //.................0123456789
        string json = "'abc`n`b`r`u1234'";
        ICharSource source = Sources.StringSource(Json.NiceJson(json));
        source.Next();
        int end = source.FindEndOfEncodedString();
        Assert.AreEqual(16, end);
    }


    // string json =;


    [Test]
    public void NextSkipWhiteSpaceSampleTest()
    {
        //.....................01234
        string str = "[ 1 , 3 ]";
        ICharSource charSource = Sources.StringSource(str);


        Assert.AreEqual('[', (char)charSource.NextSkipWhiteSpace());
        Assert.AreEqual('1', (char)charSource.NextSkipWhiteSpace());
        Assert.AreEqual(',', (char)charSource.NextSkipWhiteSpace());
        Assert.AreEqual('3', (char)charSource.NextSkipWhiteSpace());
        Assert.AreEqual(']', (char)charSource.NextSkipWhiteSpace());
    }

    [Test]
    public void SkipWhiteSpaceTest()
    {
        //.....................012345678901234567890123456789012345678
        const string str = "         1               [ 1 , 3 ]";
        ICharSource source = Sources.StringSource(str);
        Assert.AreEqual('1', (char)source.NextSkipWhiteSpace());
        source.Next();
        source.SkipWhiteSpace();
        Assert.AreEqual(25, source.GetIndex());
        Assert.AreEqual('[', source.GetCurrentChar());
    }

    [Test]
    public void TestSimpleObjectTwoItemsWeirdSpacing()
    {
        //.................. 0123456789012345678901234567890123456789012345
        const string json = "   {'h':   'a',\n\t 'i':'b'\n\t } \n\t    \n";


        ICharSource charSource = Sources.StringSource(Json.NiceJson(json));
        Assert.AreEqual('{', (char)charSource.NextSkipWhiteSpace());
        Assert.AreEqual('"', (char)charSource.NextSkipWhiteSpace());
        Assert.AreEqual('h', (char)charSource.NextSkipWhiteSpace());
        Assert.AreEqual('"', (char)charSource.NextSkipWhiteSpace());
        Assert.AreEqual(':', (char)charSource.NextSkipWhiteSpace());
        Assert.AreEqual('"', (char)charSource.NextSkipWhiteSpace());
        Assert.AreEqual('a', (char)charSource.NextSkipWhiteSpace());
        Assert.AreEqual('"', (char)charSource.NextSkipWhiteSpace());
        Assert.AreEqual(',', (char)charSource.NextSkipWhiteSpace());
        Assert.AreEqual('"', (char)charSource.NextSkipWhiteSpace());
        Assert.AreEqual('i', (char)charSource.NextSkipWhiteSpace());
        Assert.AreEqual('"', (char)charSource.NextSkipWhiteSpace());
        Assert.AreEqual(':', (char)charSource.NextSkipWhiteSpace());
        Assert.AreEqual('"', (char)charSource.NextSkipWhiteSpace());
        Assert.AreEqual('b', (char)charSource.NextSkipWhiteSpace());
        Assert.AreEqual('"', (char)charSource.NextSkipWhiteSpace());
        Assert.AreEqual('}', (char)charSource.NextSkipWhiteSpace());
    }

    [Test]
    public void NextSkipWhiteSpace()
    {
        //..................01234
        string str = "   12";
        ICharSource charSource = Sources.StringSource(str);

        char ch = (char)charSource.NextSkipWhiteSpace();


        Assert.AreEqual('1', ch);
        Assert.AreEqual('1', charSource.GetCurrentChar());
        Assert.AreEqual(3, charSource.GetIndex());
        Assert.AreEqual('2', charSource.Next());
        Assert.AreEqual(4, charSource.GetIndex());
    }

    [Test]
    public void ParseDouble()
    {
        string s = "1.0e9";
        ICharSource charSource = Sources.StringSource(s);
        Assert.AreEqual(1000000000.0, charSource.GetDouble(0, s.Length));


        s = "1e9";
        charSource = Sources.StringSource(s);
        Assert.AreEqual(1000000000.0, charSource.GetDouble(0, s.Length));


        s = "1.1";
        charSource = Sources.StringSource(s);
        Assert.AreEqual(1.1, charSource.GetDouble(0, s.Length));


        s = "1.0e12";
        charSource = Sources.StringSource(s);
        Assert.AreEqual(1e12, charSource.GetDouble(0, s.Length));
    }

    [Test]
    public void IsInteger()
    {
        string s = "-1";
        ICharSource charSource = Sources.StringSource(s);
        Assert.True(charSource.IsInteger(0, s.Length));
        Assert.AreEqual(-1, charSource.GetInt(0, s.Length));
    }

    [Test]
    public void IsIntegerFalse()
    {
        //...................01234567890123456789
        string json = "" + long.MaxValue;
        ICharSource source = Sources.StringSource(json);

        source.Next();
        Assert.False(source.IsInteger(1, json.Length));
    }

    [Test]
    public void IsIntegerButLong()
    {
        string s = "" + long.MaxValue;
        ICharSource charSource = Sources.StringSource(s);
        Assert.False(charSource.IsInteger(0, s.Length));

        long value = int.MaxValue + 1L;
        s = "" + value;
        charSource = Sources.StringSource(s);
        Assert.False(charSource.IsInteger(0, s.Length));


        value = int.MaxValue;
        s = "" + value;
        charSource = Sources.StringSource(s);
        Assert.True(charSource.IsInteger(0, s.Length));


        value = int.MinValue;
        s = "" + value;
        charSource = Sources.StringSource(s);
        Assert.True(charSource.IsInteger(0, s.Length));


        value = int.MinValue - 1L;
        s = "" + value;
        charSource = Sources.StringSource(s);
        Assert.False(charSource.IsInteger(0, s.Length));
    }
    
    


    [Test]
    public void ParseFloatSimple() {
        // .......................012345
        //...................01234567890123456789
        const string json = "1.2";
        ICharSource charSource = Sources.StringSource(Json.NiceJson(json));

        Assert.AreEqual(1.2f, charSource.GetFloat(0, 3), 0.000001);

    }

    [Test]
    public void ParseFloatExp() {
        // .......................012345
        //...................01234567890123456789
        const string json = "1.2e12";
        ICharSource charSource = Sources.StringSource(Json.NiceJson(json));

        Assert.AreEqual(1.2e12f, charSource.GetFloat(0, 6), 0.0001e12f);

    }



    [Test]
    public void ParseIntSimple() {
        // .......................012345
        //...................01234567890123456789
        const string json = "100";
        ICharSource charSource = Sources.StringSource(Json.NiceJson(json));

        Assert.AreEqual("100", charSource.GetString(0, 3));
        Assert.AreEqual(100, charSource.GetInt(0, 3));

    }

    [Test]
    public void ParseLongSimple() {
        // .......................012345
        //...................01234567890123456789
        const string json = "100";
        ICharSource charSource = Sources.StringSource(Json.NiceJson(json));

        Assert.AreEqual("100", charSource.GetString(0, 3));
        Assert.AreEqual(100, charSource.GetLong(0, 3));

    }

    [Test]
    public void GetBigDecimal() {
        // .......................012345
        //...................01234567890123456789
        const string json = "100";
        ICharSource source = Sources.StringSource(Json.NiceJson(json));

        Assert.AreEqual("100", source.GetString(0, 3));
        Assert.AreEqual(100m, source.GetDecimal(0,3));

    }

    [Test]
    public void GetBigInt() {
        // .......................012345
        //...................01234567890123456789
        const string json = "100";
        ICharSource source = Sources.StringSource(Json.NiceJson(json));

        Assert.AreEqual("100", source.GetString(0, 3));
        Assert.AreEqual(new BigInteger(100), source.GetBigInteger(0, 3));

    }

    [Test]
    public void GetEncodedString() {
        // ....................................012345678901
        //................................01234567890123456789
        string json = Json.NiceJson("`b`n`r`t ");
        ICharSource source = Sources.StringSource(Json.NiceJson(json));

        Assert.AreEqual(Json.NiceJson("`b`n`r`t"), source.GetString(0, 8));
        Assert.AreEqual("\b\n\r\t", source.GetEncodedString(0, 8));

    }

    [Test]
    public void ToEncodedStringIfNeeded() {
        // ....................................012345678901
        //................................01234567890123456789
        string json = Json.NiceJson("`b`n`r`t ");
        ICharSource source = Sources.StringSource(Json.NiceJson(json));

        Assert.AreEqual(Json.NiceJson("`b`n`r`t"), source.GetString(0, 8));
        Assert.AreEqual("\b\n\r\t", source.ToEncodedStringIfNeeded(0, 8));

    }

    [Test]
    public void ToEncodedStringIfNeededNotNeeded() {
        // ....................................012345678901
        //.................................01234567890123456789
        string json = Json.NiceJson("himom");
        ICharSource source = Sources.StringSource(Json.NiceJson(json));

        Assert.AreEqual("himom", source.ToEncodedStringIfNeeded(0, 5));

    }
}
#pragma warning restore NUnit2005