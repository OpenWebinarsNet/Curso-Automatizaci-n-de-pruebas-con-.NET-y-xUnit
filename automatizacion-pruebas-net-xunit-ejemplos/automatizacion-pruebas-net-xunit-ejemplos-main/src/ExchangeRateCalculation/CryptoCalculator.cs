namespace ExchangeRateCalculation;

public class CryptoCalculator
{
    private readonly IDictionary<string, double> _ratesInEur;

    public CryptoCalculator(IDictionary<string, double> ratesInEur)
    {
        _ratesInEur = ratesInEur;
    }

    public double ToEur(string cryptoCode, double cryptoAmount)
    {
        var isCryptoSupported = _ratesInEur.TryGetValue(cryptoCode, out var rateInEuro);
        if (!isCryptoSupported)
        {
            throw new CryptoNotSupportedException(cryptoCode);
        }

        var result = cryptoAmount * rateInEuro;
        return result;
    }
}