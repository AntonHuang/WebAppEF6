using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApp.DomainModels.Customer;

namespace WebApp.DomainModels.Product
{
    /**
      <<moment-interval>>
    */
    public class SaleToCustomer
    {
        public SaleToCustomer() {

            CreateDate = DateTime.Now;
            DetailItems = new HashSet<SaleToCustomerDetail>();
        }

        public string ID { get; set; }

        public virtual Member Customer {get;set;}
        public virtual Member SellingAgents { get; set; }

        public virtual ICollection<SaleToCustomerDetail> DetailItems { get; set; }

        public DateTime DealDate { get; set; }
        public string State { get; set; }

        public DateTime CreateDate { get; set; }
    }


    public class SaleToCustomerDetail
    {
        public int ID { get; set; }

        public virtual SaleToCustomer Sale { get; set; }
        public virtual Mattress Prodect { get; set; }
        
        public decimal Price { get; set; }
        public decimal Discount { get; set; }
        public decimal CashCoupon { get; set; }
        public string DeliveryAddress { get; set; }
        public string Gifts { get; set; }
        public string State { get; set; }

    }


}
