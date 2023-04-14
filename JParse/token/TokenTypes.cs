namespace JsonParser.token
{
    public class TokenTypes
    {
        public const int OBJECT_TOKEN = 0;
        public const int ATTRIBUTE_KEY_TOKEN = 1;
        public const int ATTRIBUTE_VALUE_TOKEN = 2;
        public const int ARRAY_TOKEN = 3;
        public const int ARRAY_ITEM_TOKEN = 4;


        public const int INT_TOKEN = 5;
        public const int FLOAT_TOKEN = 6;
        public const int STRING_TOKEN = 7;
        public const int BOOLEAN_TOKEN = 8;
        public const int NULL_TOKEN = 9;
        public const int PATH_KEY_TOKEN = 10;
        public const int PATH_INDEX_TOKEN = 11;

        public static string GetTypeName(int tokenType)
        {
            switch (tokenType)
            {
                case OBJECT_TOKEN:
                    return "Object";
                case ATTRIBUTE_KEY_TOKEN:
                    return "Key";
                case ATTRIBUTE_VALUE_TOKEN:
                    return "Attribute Value";
                case ARRAY_TOKEN:
                    return "Array";
                case ARRAY_ITEM_TOKEN:
                    return "Array Item";
                case INT_TOKEN:
                    return "Integer";
                case FLOAT_TOKEN:
                    return "Float";
                case STRING_TOKEN:
                    return "String";
                case BOOLEAN_TOKEN:
                    return "Boolean";
                case NULL_TOKEN:
                    return "Null";
                default:
                    return "" + tokenType;
            }
        }
    }
}