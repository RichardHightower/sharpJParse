using System.Diagnostics.CodeAnalysis;
using sharpJParse.parser;

namespace sharpJParse.source;

#pragma warning disable NUnit2005
[SuppressMessage("ReSharper", "SuggestVarOrType_BuiltInTypes")]
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
            Assert.IsTrue(true);
        }
    }


    //Left off here TODO 
    
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
            NumberParseResult result = source.FindEndOfNumber();
            Assert.True(false);
        }
        catch (Exception ex)
        {
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
}
#pragma warning restore NUnit2005