using System.Numerics;
using System.Text;

namespace khiemnguyen.dev.utility
{
    public class VnWordNumber
    {
        private static readonly string[] PrimaryNumbers = { "không", "một", "hai", "ba", "bốn", "năm", "sáu", "bảy", "tám", "chín" };
        private static StringBuilder result = default!;
        public static string ReadNumber(BigInteger number)
        {
            if (number == 0)
            {
                return "không";
            }

            result = new();

            if (number.Sign == -1)
            {
                AppendWord("âm");
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
                ReadNine(nine, true);
            }

            while (billions > 0)
            {
                AppendWord("tỷ");

                BigInteger billionsValue = BigInteger.Pow(10, (int)(--billions * 9));

                uint nine = (uint)BigInteger.DivRem(number, billionsValue, out number);
                ReadNine(nine, false);
            }

            try
            {
                return result.ToString();
            }
            finally
            {
                result.Clear();
            }
        }
        private static bool ReadTree(ushort number, bool isFirst)
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
                AppendWord(PrimaryNumbers[hundreds] + " trăm");
            }
            #endregion

            #region Hàng chục
            switch (tens)
            {
                case 0:
                    if (units != 0 && (hundreds != 0 || !isFirst))
                    {
                        AppendWord("linh");
                    }
                    break;
                case 1:
                    AppendWord("mười");
                    break;
                default:
                    AppendWord(PrimaryNumbers[tens] + " mươi");
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
                        AppendWord("một");
                    }
                    else
                    {
                        AppendWord("mốt");
                    }
                    break;
                case 5:
                    if (tens == 0)
                    {
                        AppendWord("năm");
                    }
                    else
                    {
                        AppendWord("lăm");
                    }
                    break;
                default:
                    AppendWord(PrimaryNumbers[units]);
                    break;
            }
            #endregion

            return true;
        }
        private static bool ReadNine(uint number, bool isFirst)
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

            if (ReadTree(millions, isFirst))
            {
                isFirst = false;
                AppendWord("triệu");
            }

            if (ReadTree(thousands, isFirst))
            {
                isFirst = false;
                AppendWord("ngàn");
            }

            ReadTree(units, isFirst);

            return true;
        }
        private static void AppendWord(string text)
        {
            if (string.IsNullOrEmpty(text))
            {
                return;
            }
            if (result.Length != 0)
            {
                result.Append(' ');
            }
            result.Append(text);
        }
    }
}