using HiringTask.Contracts;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HiringTask.Business
{
    public class CurrencyConverter : ICurrencyConverter
    {
        private readonly ConcurrentDictionary<string, Dictionary<string, double>> _rates;
        private static readonly Lazy<ICurrencyConverter> _instance = new (()=> new CurrencyConverter());

        private CurrencyConverter()
        {
            _rates = new ConcurrentDictionary<string, Dictionary<string, double>>();
        }

        public static ICurrencyConverter Instance => _instance.Value;

        public void ClearConfiguration()
        {
            _rates.Clear();
        }

        public double Convert(string fromCurrency, string toCurrency, double amount)
        {
            if (fromCurrency == toCurrency)
            {
                return amount;
            }

            var path = FindConversionPath(fromCurrency, toCurrency);
            if (path == null)
            {
                throw new ArgumentException("No conversion path found between currencies");
            }

            double convertedAmount = amount;
            foreach (var currency in path)
            {
                convertedAmount = GetRate(currency.Previous, currency.Current) * convertedAmount;
            }

            return convertedAmount;
        }

        public void UpdateConfiguration(IEnumerable<Tuple<string, string, double>> conversionRates)
        {
            foreach (var rate in conversionRates)
            {
                var fromCurrency = rate.Item1;
                var rates = _rates.GetOrAdd(fromCurrency, _ => new Dictionary<string, double>());
                rates[rate.Item2] = rate.Item3;
            }
        }

        private List<CurrencyLink> FindConversionPath(string fromCurrency, string toCurrency)
        {
            var queue = new Queue<CurrencyLink>();
            queue.Enqueue(new CurrencyLink(fromCurrency));

            var visited = new HashSet<string>();
            visited.Add(fromCurrency);

            while (queue.Count > 0)
            {
                var current = queue.Dequeue();
                if (current.Current == toCurrency)
                {
                    return current.Path;
                }

                foreach (var nextCurrency in _rates.GetOrAdd(current.Current, new Dictionary<string, double>()))
                {
                    if (!visited.Contains(nextCurrency.Key))
                    {
                        visited.Add(nextCurrency.Key);
                        queue.Enqueue(new CurrencyLink(current, nextCurrency.Key));
                    }
                }
            }

            return null;
        }

        private double GetRate(string fromCurrency, string toCurrency)
        {
            if (!_rates.TryGetValue(fromCurrency, out var rates))
            {
                throw new ArgumentException($"Missing conversion rate for {fromCurrency}");
            }

            if (!rates.TryGetValue(toCurrency, out var rate))
            {
                throw new ArgumentException($"Missing conversion rate from {fromCurrency} to {toCurrency}");
            }

            return rate;
        }

    }
}
