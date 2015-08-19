using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApp.DomainModels.Account
{
    /**
      <<thing>>
    */
    public class Account
    {
        public string AccountID { get; set; }
        public Amount Number { get; set; }
        public string Status { get; set; }

        public virtual IAccountHolder Holder { get; set; }
        public virtual IOrgEntityResponsibleForAccount OrgEntity { get; set; }

        public Amount GetAmountForExchangeParse()
        {
            return new Amount(Number.Type);
        }

        public void modifyAmount(Amount qty)
        {
            Number = Number + qty;
        }
    }

    /**
      <<Role>>
    */
    public interface IOrgEntityResponsibleForAccount
    {


    }

    /**
     <<Role>>
    */
    public interface IAccountHolder
    {
       ICollection<Account> Account { get; set; }
    }
}
