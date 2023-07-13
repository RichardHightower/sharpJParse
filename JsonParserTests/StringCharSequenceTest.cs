using sharpJParse.JsonParser.source.support;
using sharpJParse.JsonParser.support;

namespace sharpJParse;

public class StringCharSequenceTest
{

    [Test]
    public void TestSimple()
    {

        const string test = "abcdefg";
        const string test2 = "abcdef2";

        ICharSequence charSequence = new StringCharSequence(test);
        ICharSequence charSequence2 = new StringCharSequence(test);
        ICharSequence charSequence3 = new StringCharSequence("bcdef");
        
        ICharSequence charSequence4 = new StringCharSequence(test2);
        
        
        Assert.That(charSequence.ToString(), Is.EqualTo(test));
        Assert.That(charSequence[0], Is.EqualTo(test[0]));
        Assert.That(charSequence.Length, Is.EqualTo(test.Length));
        Assert.AreEqual(charSequence, charSequence);
        Assert.True(charSequence.Equals(test));
        Assert.True(charSequence.Equals(charSequence));
        Assert.False(charSequence.Equals(test2));
        Assert.False(charSequence.Equals(charSequence4));
        Assert.AreNotEqual(charSequence, null);
        Assert.That(charSequence, Is.Not.EqualTo(charSequence3));
        Assert.That(charSequence, Is.Not.EqualTo(null));
        Assert.That(charSequence.SubSequence(1,3).ToString(), Is.EqualTo("bc"));
        Assert.That(charSequence.GetHashCode(), Is.EqualTo(test.GetHashCode()));
        Assert.That(charSequence.Equals(charSequence2), Is.EqualTo(true));
        Assert.That(charSequence.Equals("abcdefg"), Is.EqualTo(true));
        Assert.That(charSequence.GetHashCode(), Is.EqualTo("abcdefg".GetHashCode()));
    }
    
    
    [Test]
    public void TestSubSequence()
    {

        const string test = "_abcdefg_";
        const string test2 = "abcdef2";

        ICharSequence charSequence = new StringCharSequence(test).SubSequence(1, test.Length-1);
        ICharSequence charSequence2 = new StringCharSequence(test).SubSequence(1, test.Length-1);
        ICharSequence charSequence4 = new StringCharSequence(test2).SubSequence(1, test2.Length-1);

        
        Assert.That(charSequence.ToString(), Is.EqualTo("abcdefg"));
        
        Assert.That(charSequence.Equals(new StringCharSequence("abcdefg")), Is.EqualTo(true));
        
        Assert.That(charSequence.Equals(charSequence2), Is.EqualTo(true));
        
        Assert.That(charSequence.Equals("abcdefg"), Is.EqualTo(true));
        
        Assert.That(charSequence.GetHashCode(), Is.EqualTo("abcdefg".GetHashCode()));
        Assert.That(charSequence.GetHashCode(), Is.EqualTo("abcdefg".GetHashCode()));
        
        Assert.That(charSequence.Length, Is.EqualTo(test.Length-2));
        
        Assert.That(charSequence[0], Is.EqualTo(test[1]));
        
        Assert.That(charSequence.SubSequence(1,3).ToString(), Is.EqualTo("bc"));
        
        
        
        Assert.True(charSequence.Equals("abcdefg"));
        Assert.True(charSequence.Equals(charSequence));
        Assert.False(charSequence.Equals(test2));
        Assert.False(charSequence.Equals("123"));
        Assert.False(charSequence.Equals(new StringCharSequence("123")));
        Assert.False(charSequence.Equals(charSequence4));
        Assert.AreNotEqual(charSequence, null);

        

    }
}