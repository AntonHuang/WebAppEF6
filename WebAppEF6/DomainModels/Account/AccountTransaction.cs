using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApp.DomainModels.Account
{
    /**
      <<moment-interval>>
    */
    public class AccountTransaction
    {
        public string ID { get; set; }
        public string Type { get; set; }
        public string Status { get; set; }
        public ICollection<AccountTransactionDetail> DetailItems { get; set; }

        public void Transaction(Account fromAccount, Account toAccount, Amount amount) {

            AccountTransactionDetail fromDetial = new AccountTransactionDetail()
            {
                Transaction = this,
                Account = fromAccount,
                Quantity = -amount,
                Description = "Transaction From"
            };

            AccountTransactionDetail toDetial = new AccountTransactionDetail()
            {
                Transaction = this,
                Account = toAccount,
                Quantity = amount,
                Description = "Transaction To"
            };

            DetailItems.Add(fromDetial);
            DetailItems.Add(toDetial);

            fromDetial.ApplyTransaction();
            toDetial.ApplyTransaction();
        }
    }

    /**
      <<moment-interval>>
    */
    public class AccountTransactionDetail
    {
        public AccountTransaction Transaction { get; set; }
        public Account Account { get; set; }
        public Amount Quantity { get; set; }
        public AmountExchangeRate Rate { get; set; }
        public string Description { get; set; }
        

        public void ApplyTransaction() {
            Rate = RateService.GetExchangeRate(Quantity, Account.GetAmountForExchangeParse());
            Account.modifyAmount(Rate.GetAmount(Quantity));
        }
    }

    public class AmountExchangeRate {
        public Amount From { get; set; }
        public Amount To { get; set; }
        public decimal Rate { get; set; }

        public Amount GetAmount(Amount qty)
        {
            if (From.Type.Equals(qty.Type) == false) {
                throw new ArgumentException(
                    String.Format("Expect {0} Amount Type.But Argument Amount Type is {1}",
                         From.Type.ToString(), qty.Type.ToString() ));
            }
            return new Amount(To.Type, qty.ValueOfNumber * Rate );
        }
    }

    public class RateService
    {
        internal static AmountExchangeRate GetExchangeRate(Amount from, Amount to)
        {
            if (from.Type.Equals("MemberPiont") && to.Type.Equals("RMB"))
            {

                return new AmountExchangeRate()
                {
                    From = new Amount("MemberPiont"),
                    To = new Amount("RMB"),
                    Rate = 50
                };
            }
            else if (from.Type.Equals("RMB") && to.Type.Equals("MemberPiont"))
            {
                return new AmountExchangeRate()
                {
                    From = new Amount("RMB"),
                    To = new Amount("MemberPiont"),
                    Rate = 1/50
                };
            }
            else {

                return new AmountExchangeRate()
                {
                    From = new Amount( from.Type),
                    To = new Amount(to.Type),
                    Rate = 1
                };
            }
        }
    }

}
