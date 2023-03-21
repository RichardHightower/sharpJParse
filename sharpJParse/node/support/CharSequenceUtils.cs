namespace sharpJParse.support;

public class CharSequenceUtils
{
    public static bool Equals(CharSequence cs1, CharSequence cs2)
    {
        if (cs1.Length() != cs2.Length()) return false;
        var len = cs1.Length();

        for (var i = 0; i < len; i++)
        {
            var a = cs1.CharAt(i);
            var b = cs2.CharAt(i);
            if (a != b) return false;
        }

        return true;
    }

    public static int GetHashCode(CharSequence cs)
    {
        var h = 0;
        for (var index = 0; index < cs.Length(); index++)
        {
            var v = cs.CharAt(index);
            h = 31 * h + v;
        }

        return h;
    }
}