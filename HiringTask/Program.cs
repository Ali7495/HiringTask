// See https://aka.ms/new-console-template for more information
// Define some sample conversion rates
using HiringTask.Business;
using HiringTask.Contracts;

var conversionRates = new List<Tuple<string, string, double>>
{
    new Tuple<string, string, double>("USD","CAD",1.34),
    new Tuple<string, string, double>("CAD","GBP",0.58),
    new Tuple<string, string, double>("USD","EUR",0.86)
};


// Update the currency converter with the rates
ICurrencyConverter converter = CurrencyConverter.Instance;
converter.UpdateConfiguration(conversionRates);

// Get user input for conversion
Console.WriteLine("Enter the source currency (e.g., USD, CAD, EUR): ");
var fromCurrency = Console.ReadLine();

Console.WriteLine("Enter the target currency (e.g., USD, CAD, EUR): ");
var toCurrency = Console.ReadLine();

Console.WriteLine("Enter the amount to convert: ");
double amount = double.Parse(Console.ReadLine());

// Perform the conversion
try
{
    double convertedAmount = converter.Convert(fromCurrency, toCurrency, amount);
    Console.WriteLine($"{amount} {fromCurrency} is equivalent to {convertedAmount} {toCurrency}.");
}
catch (ArgumentException ex)
{
    Console.WriteLine("Error: " + ex.Message);
}