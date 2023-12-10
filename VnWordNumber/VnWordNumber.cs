using System.Numerics;
using System.Text;

namespace khiemnguyen.dev.utility
{
    public static class VnWordNumber
    {
        private static readonly string[] PrimaryNumbers = { "không", "một", "hai", "ba", "bốn", "năm", "sáu", "bảy", "tám", "chín" };
        public static StringBuilder ReadNumber(BigInteger number, StringBuilder? builder = default)
        {
            builder ??= new();

            if (number == 0)
            {
                AppendWord("không", builder);
                return builder;
            }

            if (number.Sign == -1)
            {
                AppendWord("âm", builder);
                number = BigInteger.Multiply(number, -1);
            }

            if(Implement1(ref number, builder) is false)
            {
                Implement2(ref number, builder);
            }

            return builder;
        }
        public static StringBuilder ReadNumber(decimal number, StringBuilder? builder = default)
        {
            decimal fractionalPart;
            {
                decimal integerPart = Math.Truncate(number);
                builder = ReadNumber(new BigInteger(integerPart), builder);

                fractionalPart = Math.Abs(number - integerPart);
            }

            if (fractionalPart > 0)
            {
                AppendWord("phẩy", builder);

                do
                {
                    fractionalPart *= 10;

                    byte integerPart = (byte)Math.Truncate(fractionalPart);
                    AppendWord(PrimaryNumbers[integerPart], builder);

                    fractionalPart -= integerPart;
                }
                while (fractionalPart > 0);
            }

            return builder;
        }
        private static bool Implement1(ref BigInteger number, StringBuilder builder)
        {
            BigInteger billions;
            {
                BigInteger count;
                {
                    double tempCount = Math.Round(BigInteger.Log10(number));

                    if (tempCount <= int.MaxValue)
                    {
                        BigInteger check = BigInteger.Pow(10, (int)tempCount);

                        if (check <= number)
                        {
                            ++tempCount;
                        }

                        count = new BigInteger(tempCount);
                    }
                    else
                    {
                        BigInteger temp = number;
                        count = 0;

                        do
                        {
                            ++count;
                            temp /= 10;
                        }
                        while (temp > 0);
                    }
                }

                billions = (count - 1) / 9;

                BigInteger exponent = billions * 9;

                if (exponent > int.MaxValue)
                {
                    return false;
                }

                BigInteger billionsValue = BigInteger.Pow(10, (int)exponent);

                uint nine = (uint)BigInteger.DivRem(number, billionsValue, out number);
                ReadNine(nine, true, builder);
            }

            while (billions > 0)
            {
                AppendWord("tỷ", builder);

                BigInteger billionsValue = BigInteger.Pow(10, (int)(--billions * 9));

                uint nine = (uint)BigInteger.DivRem(number, billionsValue, out number);
                ReadNine(nine, false, builder);
            }

            return true;
        }
        private static bool Implement2(ref BigInteger number, StringBuilder builder)
        {
            number = BigInteger.DivRem(number, 1000000000, out BigInteger remainder);

            bool isFirst = true;

            if (number > 0)
            {
                Implement2(ref number, builder);

                AppendWord("tỷ", builder);

                isFirst = false;
            }

            ReadNine((uint)remainder, isFirst, builder);

            return true;
        }
        private static bool ReadTree(ushort number, bool isFirst, StringBuilder builder)
        {
            if (number == 0)
            {
                return false;
            }

            byte hundreds;
            byte tens;
            byte units;
            {
                hundreds = (byte)Math.DivRem(number, 100, out int remain);
                tens = (byte)Math.DivRem(remain, 10, out remain);
                units = (byte)remain;
            }

            #region Hàng trăm
            if (!isFirst || hundreds != 0)
            {
                AppendWord(PrimaryNumbers[hundreds] + " trăm", builder);
            }
            #endregion

            #region Hàng chục
            switch (tens)
            {
                case 0:
                    if (units != 0 && (hundreds != 0 || !isFirst))
                    {
                        AppendWord("linh", builder);
                    }
                    break;
                case 1:
                    AppendWord("mười", builder);
                    break;
                default:
                    AppendWord(PrimaryNumbers[tens] + " mươi", builder);
                    break;
            }
            #endregion

            #region Hàng đơn vị
            switch (units)
            {
                case 0:
                    break;
                case 1:
                    if (tens == 0 || tens == 1)
                    {
                        AppendWord("một", builder);
                    }
                    else
                    {
                        AppendWord("mốt", builder);
                    }
                    break;
                case 5:
                    if (tens == 0)
                    {
                        AppendWord("năm", builder);
                    }
                    else
                    {
                        AppendWord("lăm", builder);
                    }
                    break;
                default:
                    AppendWord(PrimaryNumbers[units], builder);
                    break;
            }
            #endregion

            return true;
        }
        private static bool ReadNine(uint number, bool isFirst, StringBuilder builder)
        {
            if (number == 0)
            {
                return false;
            }

            ushort millions;
            ushort thousands;
            ushort units;
            {
                millions = (ushort)Math.DivRem(number, 1000000, out long remain);
                thousands = (ushort)Math.DivRem(remain, 1000, out remain);
                units = (ushort)remain;
            }

            if (ReadTree(millions, isFirst, builder))
            {
                isFirst = false;
                AppendWord("triệu", builder);
            }

            if (ReadTree(thousands, isFirst, builder))
            {
                isFirst = false;
                AppendWord("ngàn", builder);
            }

            ReadTree(units, isFirst, builder);

            return true;
        }
        private static void AppendWord(string text, StringBuilder builder)
        {
            if (string.IsNullOrEmpty(text))
            {
                return;
            }

            if (builder.Length != 0)
            {
                builder.Append(' ');
            }

            builder.Append(text);
        }
    }
}