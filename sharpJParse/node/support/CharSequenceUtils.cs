namespace sharpJParse.support;


public class CharSequenceUtils {

    public static bool Equals(CharSequence cs1, CharSequence cs2) {

        if (cs1.Length() != cs2.Length()) return false;
        int len = cs1.Length();

        for (int i = 0; i < len; i++) {
            char a = cs1.CharAt(i);
            char b = cs2.CharAt(i);
            if (a != b) {
                return false;
            }
        }
        return true;
    }

    public static int GetHashCode( CharSequence cs) {
        int h = 0;
        for (int index = 0; index < cs.Length(); index++) {
            char v = cs.CharAt(index);
            h = 31 * h + v;
        }
        return h;
    }
}
