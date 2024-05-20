using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HiringTask.Business
{
    public class CurrencyLink
    {
        public string Current { get; }
        public string Previous { get; }
        public List<CurrencyLink> Path { get; }

        public CurrencyLink(string currency)
        {
            Current = currency;
            Path = new List<CurrencyLink>();
        }

        public CurrencyLink(CurrencyLink previous, string currency) : this(currency)
        {
            Previous = previous?.Current;
            Path.AddRange(previous?.Path ?? Enumerable.Empty<CurrencyLink>());
            Path.Add(this);
        }
    }
}
