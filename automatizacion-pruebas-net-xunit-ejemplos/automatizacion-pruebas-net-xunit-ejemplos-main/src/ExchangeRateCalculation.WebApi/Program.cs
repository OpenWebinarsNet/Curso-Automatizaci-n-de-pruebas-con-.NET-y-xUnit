using ExchangeRateCalculation;

var builder = WebApplication.CreateBuilder(args);

// Registro de IDictionary con implementación concreta (hardcodeada)
builder
    .Services
    .AddSingleton<IDictionary<string, double>>(sp => new Dictionary<string, double>()
    {
        { "HBAR", 0.212 },
        { "BTC", 31200.12 },
        { "ETH", 2172.34 }
    });

// Registro de CryptoCalculator
builder
    .Services
    .AddTransient<CryptoCalculator>();

var app = builder.Build();

// Endpoint para http://localhost:4000/api/cryptoCurrencies/HBAR/calculations?cryptoAmount=300
app.MapGet(
    "api/cryptoCurrencies/{cryptoCode}/calculations",
    (string cryptoCode, double cryptoAmount, CryptoCalculator cryptoCalculator) =>
    {
        app.Services.GetRequiredService<CryptoCalculator>();
        var result = cryptoCalculator.ToEur(cryptoCode, cryptoAmount);
        return $"{result} EUR";
    });

app.Run();

public partial class Program { } // See https://docs.microsoft.com/en-us/aspnet/core/test/integration-tests?view=aspnetcore-6.0