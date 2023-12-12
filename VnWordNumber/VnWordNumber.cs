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
                builder.AppendWord("không");

                return builder;
            }

            if (number.Sign == -1)
            {
                builder.AppendWord("âm");

                number = BigInteger.Multiply(number, -1);
            }

            builder.Run(ref number);

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
                builder.AppendWord("phẩy");

                do
                {
                    fractionalPart *= 10;

                    byte integerPart = (byte)Math.Truncate(fractionalPart);
                    builder.AppendWord(PrimaryNumbers[integerPart]);

                    fractionalPart -= integerPart;
                }
                while (fractionalPart > 0);
            }

            return builder;
        }
        private static void Run(this StringBuilder builder, ref BigInteger number)
        {
            number = BigInteger.DivRem(number, 1000000000, out BigInteger remainder);

            bool isFirst = true;

            if (number > 0)
            {
                builder.Run(ref number);
                builder.AppendWord("tỷ");

                isFirst = false;
            }

            builder.ReadNine((uint)remainder, isFirst);
        }
        private static bool ReadTree(this StringBuilder builder, ushort number, bool isFirst)
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
                builder.AppendWord(PrimaryNumbers[hundreds] + " trăm");
            }
            #endregion

            #region Hàng chục
            switch (tens)
            {
                case 0:
                    if (units != 0 && (hundreds != 0 || !isFirst))
                    {
                        builder.AppendWord("linh");
                    }
                    break;
                case 1:
                    builder.AppendWord("mười");
                    break;
                default:
                    builder.AppendWord(PrimaryNumbers[tens] + " mươi");
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
                        builder.AppendWord("một");
                    }
                    else
                    {
                        builder.AppendWord("mốt");
                    }
                    break;
                case 5:
                    if (tens == 0)
                    {
                        builder.AppendWord("năm");
                    }
                    else
                    {
                        builder.AppendWord("lăm");
                    }
                    break;
                default:
                    builder.AppendWord(PrimaryNumbers[units]);
                    break;
            }
            #endregion

            return true;
        }
        private static bool ReadNine(this StringBuilder builder, uint number, bool isFirst)
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

            if (builder.ReadTree(millions, isFirst))
            {
                isFirst = false;
                builder.AppendWord("triệu");
            }

            if (builder.ReadTree(thousands, isFirst))
            {
                isFirst = false;
                builder.AppendWord("ngàn");
            }

            builder.ReadTree(units, isFirst);

            return true;
        }
        private static void AppendWord(this StringBuilder builder, string text)
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