# VnWordNumber
Convert number to Vietnamese text.
## Usage
```
BigInteger number = new BigInteger(double.MaxValue);
string text = VnWordNumber.ReadNumber(number).ToString();
Console.WriteLine(text);
```