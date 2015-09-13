using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace WebApp.Models
{
    
    public class SellToCustomerViewModel
    {
        public string MattressID { get; set; }
        public string MattressTypeID { get; set; }
        public string DeliveryAddress { get; set; }
        public string CustomerID { get; set; }
        public DateTime SaleDate { get; set; }
        public string Gifts { get; set; }
        public bool IsUseCashCoupon { get; set; }

    }

    public class SellMemberPointViewModel
    {
        public string MemberID { get; set; }
        public string MemberLevel { get; set; }
        public string MemberName { get; set; }
        public decimal PointCount { get; set; }
        public string Up1ID { get; set; }
        public string Up1Level { get; set; }
        public string Up1Name { get; set; }
        public decimal Up1PointCount { get; set; }
        public string Up2ID { get; set; }
        public string Up2Level { get; set; }
        public string Up2Name { get; set; }
        public decimal Up2PointCount { get; set; }
    }

    public class MemberPointInfoViewModel {

        [Required]
        public string MemberID { get; set; }
        public string MemberName { get; set; }
        public string IDCard { get; set; }

        public decimal PointTotal { get; set; }
        public decimal UsablePoint { get; set; }
        public decimal ExchAmount { get; set; }

    }


}
