using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApp.DomainModels.Product
{
    public class Mattress
    {
        public string ID { get; set; }
        public DateTime RegisterDate { get; set; }
        public DateTime SaleDate { get; set; }

        public string  TypeDescID { get; set; }
        public virtual ProductDesc TypeDesc { get; set; }
    }
}
