using System.Net.Http;
using System.Reflection;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Xunit;

namespace ExchangeRateCalculation.FunctionalTests.UseCases;

public class CalculateCryptoInEurTests
{
    private readonly HttpClient _httpClient;

    public CalculateCryptoInEurTests()
    {
        var application =
            new WebApplicationFactory<Program>()
                .WithWebHostBuilder(builder =>
                {
                    builder.ConfigureAppConfiguration((hostingContext, config) =>
                    {
                        var env = hostingContext.HostingEnvironment;

                        config
                            .AddJsonFile("appsettings.json", true, false)
                            .AddJsonFile($"appsettings.{env.EnvironmentName}.json", true, false)
                            .AddEnvironmentVariables();

                        if (env.IsDevelopment())
                        {
                            var appAssembly = Assembly.Load(new AssemblyName(env.ApplicationName));
                            config.AddUserSecrets(appAssembly, true);
                        }
                    });
                });

        var client = application.CreateClient();

        _httpClient = client;
    }

    [Fact]
    public async Task Given_A_Calculation_Endpoint_And_An_Amount_Of_Crypto_When_Getting_Calculation_Then_It_Should_Return_Expected_Http_Response()
    {
        // Given
        var cryptoCode = "HBAR";
        var cryptoAmount = 300;
        var url = $"/api/cryptoCurrencies/{cryptoCode}/calculations?cryptoAmount={cryptoAmount}";

        // When
        var response = await _httpClient.GetStringAsync(url);

        // Then
        response.Should().Be("63.6 EUR");
    }
}
