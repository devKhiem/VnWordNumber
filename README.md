# VnWordNumber
Convert number to Vietnamese text.
## Usage
```
BigInteger number1 = new BigInteger(double.MaxValue);
BigInteger number2 = new BigInteger(23456);
BigInteger number3 = BigInteger.Parse("500,604,845 010,000,001".Replace(",", "").Replace(" ", ""));

Console.OutputEncoding = Encoding.UTF8;

string text1 = VnWordNumber.ReadNumber(number1).ToString();
Console.WriteLine(text1);
Console.ReadKey();
Console.Clear();

string text2 = VnWordNumber.ReadNumber(number2).ToString();
Console.WriteLine(text2);
Console.ReadKey();
Console.Clear();

string text3 = VnWordNumber.ReadNumber(number3).ToString();
Console.WriteLine(text3);
Console.ReadKey();
Console.Clear();
```