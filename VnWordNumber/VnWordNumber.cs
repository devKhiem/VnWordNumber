using System.Numerics;
using System.Text;

namespace khiemnguyen.dev.utility
{
    public class VnWordNumber
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

            BigInteger billions;
            {
                BigInteger count = 0;
                {
                    BigInteger temp = number;
                    do
                    {
                        ++count;
                        temp /= 10;
                    } while (temp > 0);
                }

                billions = (count - 1) / 9;
                BigInteger billionsValue = BigInteger.Pow(10, (int)(billions * 9));

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
            return builder;
        }
        public static StringBuilder ReadNumber(decimal number, StringBuilder? builder = default)
        {
            decimal decimalNumber;
            {
                decimal integerNumber = Math.Truncate(number);
                builder = ReadNumber(new BigInteger(integerNumber), builder);
                decimalNumber = Math.Abs(number) - Math.Abs(integerNumber);
            }
            if (decimalNumber > 0)
            {
                AppendWord("phẩy", builder);
                while (decimalNumber > 0)
                {
                    decimalNumber *= 10;
                    int integerNumber = (int)Math.Truncate(decimalNumber);
                    AppendWord(PrimaryNumbers[integerNumber], builder);
                    decimalNumber -= integerNumber;
                }
            }
            return builder;
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