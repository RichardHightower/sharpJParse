namespace sharpJParse.JsonParser.source.support
{
    public static class ParseFloatSupport
    {
        private static readonly float[] PowersOf10 = {
            1e0f, 1e1f, 1e2f, 1e3f, 1e4f, 1e5f, 1e6f, 1e7f,
            1e8f, 1e9f, 1e10f, 1e11f, 1e12f, 1e13f, 1e14f, 1e15f, 1e16f, 1e17f, 1e18f
        };

        public static float ParseFloat(char[] chars, int startIndex, int endIndex)
        {
            bool negative = false;
            int i = startIndex;
            float result = 0f;

            // Check for a negative sign
            if (i < endIndex && chars[i] == '-')
            {
                negative = true;
                i++;
            }

            while (i < endIndex)
            {
                char ch = chars[i];
                switch (ch)
                {
                    case '0':
                    case '1':
                    case '2':
                    case '3':
                    case '4':
                    case '5':
                    case '6':
                    case '7':
                    case '8':
                    case '9':
                        result = result * 10 + (ch - '0');
                        i++;
                        break;
                    case '.':
                        result = ParseFractionPart(i + 1, endIndex, chars, result);
                        return negative ? -result : result;
                    case 'E':
                    case 'e':
                        result = ParseExponent(i + 1, endIndex, chars, result);
                        return negative ? -result : result;
                    default:
                        throw new UnexpectedCharacterException("parsing float", "Illegal character", null, ch, i);
                }
            }

            return negative ? -result : result;
        }

        private static float ParseFractionPart(int i, int endIndex, char[] chars, float result)
        {
            float fraction = 0.1f;

            while (i < endIndex)
            {
                char ch = chars[i];
                switch (ch)
                {
                    case '0':
                    case '1':
                    case '2':
                    case '3':
                    case '4':
                    case '5':
                    case '6':
                    case '7':
                    case '8':
                    case '9':
                        result += (ch - '0') * fraction;
                        fraction /= 10;
                        i++;
                        break;
                    case 'E':
                    case 'e':
                        return ParseExponent(i + 1, endIndex, chars, result);
                    default:
                        throw new UnexpectedCharacterException("float parsing fraction part", "Illegal character", null, ch, i);
                }
            }

            return result;
        }

        private static float ParseExponent(int i, int endIndex, char[] chars, float result)
        {
            bool exponentNegative = false;
            int exponent = 0;

            if (i < endIndex)
            {
                char sign = chars[i];
                if (sign == '-')
                {
                    exponentNegative = true;
                    i++;
                }
                else if (sign == '+')
                {
                    i++;
                }
            }

            while (i < endIndex)
            {
                char ch = chars[i];
                switch (ch)
                {
                    case '0':
                    case '1':
                    case '2':
                    case '3':
                    case '4':
                    case '5':
                    case '6':
                    case '7':
                    case '8':
                    case '9':
                        exponent = exponent * 10 + (ch - '0');
                        i++;
                        break;
                    default:
                        throw new UnexpectedCharacterException("float parsing exponent", "Illegal character", null, ch, i);
                }
            }

            if (exponentNegative)
            {
                exponent = -exponent;
            }

            // Use Lookup table for powers of 10
            if (exponent >= 0)
            {
                while (exponent >= PowersOf10.Length)
                {
                    result *= 1e18f;
                    exponent -= 18;
                }
                result *= PowersOf10[exponent];
            }
            else
            {
                while (-exponent >= PowersOf10.Length)
                {
                    result /= 1e18f;
                    exponent += 18;
                }
                result /= PowersOf10[-exponent];
            }

            return result;
        }
    }
}
