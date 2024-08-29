namespace sharpJParse.JsonParser.source.support
{
    public static class ParseDoubleSupport
    {
        private static readonly double[] PowersOf10 = {
            1e0, 1e1, 1e2, 1e3, 1e4, 1e5, 1e6, 1e7,
            1e8, 1e9, 1e10, 1e11, 1e12, 1e13, 1e14, 1e15, 1e16, 1e17, 1e18, 1e19, 1e20, 1e21, 1e22
        };

        public static double ParseDouble(char[] chars, int startIndex, int endIndex)
        {
            bool negative = false;
            int i = startIndex;
            double result = 0;
            bool seenDot = false;

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
                        if (seenDot)
                            throw new UnexpectedCharacterException("parsing double", "Multiple decimal points", null, ch, i);
                        seenDot = true;
                        result = ParseFractionPart(i + 1, endIndex, chars, result);
                        return negative ? -result : result;
                    case 'E':
                    case 'e':
                        result = ParseExponent(i + 1, endIndex, chars, result);
                        return negative ? -result : result;
                    default:
                        throw new UnexpectedCharacterException("parsing double", "Illegal character", null, ch, i);
                }
            }

            return negative ? -result : result;
        }

        private static double ParseFractionPart(int i, int endIndex, char[] chars, double result)
        {
            double fraction = 0.1;
            bool seenDigit = false;

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
                        seenDigit = true;
                        break;
                    case 'E':
                    case 'e':
                        if (!seenDigit)
                            throw new UnexpectedCharacterException("double parsing fraction part", "No digits after decimal point", null, ch, i);
                        return ParseExponent(i + 1, endIndex, chars, result);
                    default:
                        throw new UnexpectedCharacterException("double parsing fraction part", "Illegal character", null, ch, i);
                }
            }
            return result;
        }

        private static double ParseExponent(int i, int endIndex, char[] chars, double result)
        {
            bool exponentNegative = false;
            int exponent = 0;
            bool seenDigit = false;

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
                        seenDigit = true;
                        break;
                    default:
                        throw new UnexpectedCharacterException("double parsing exponent", "Illegal character", null, ch, i);
                }
            }

            if (!seenDigit)
                throw new UnexpectedCharacterException("double parsing exponent", "No digits in exponent", null, 'e', i - 1);

            if (exponentNegative)
            {
                exponent = -exponent;
            }

            // Use Lookup table for powers of 10
            if (exponent >= 0)
            {
                while (exponent >= PowersOf10.Length)
                {
                    result *= 1e22;
                    exponent -= 22;
                }
                result *= PowersOf10[exponent];
            }
            else
            {
                while (-exponent >= PowersOf10.Length)
                {
                    result /= 1e22;
                    exponent += 22;
                }
                result /= PowersOf10[-exponent];
            }

            return result;
        }
    }
}