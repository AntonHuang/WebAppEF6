using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApp.DomainModels.Product
{
    /**
      <<moment-interval>>
    */
    public class ProductPrice
    {
        public string PriceQty { get; set; }
        public string PriceUOM { get; set; }
        public string Status { get; set; }

        public ProductDesc Product { get; set; }
        public IPricer Pricer { get; set; }

    }

    /**
      <<Role>>
    */
    public interface IPricer
    {
        
    }
}
