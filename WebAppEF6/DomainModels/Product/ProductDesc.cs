using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApp.DomainModels.Product
{
    /**
      <<description>>
    */
    public class ProductDesc
    {
        public string ID { get; set; }
        public string Type { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public string DisplayName { get; set; }
        public string SizeSpec { get; set; }
        public string Unit { get; set; }
        public int StateFlag { get; set; }

        public ProductDesc() {
            StateFlag = 1;
        }
    }
}
