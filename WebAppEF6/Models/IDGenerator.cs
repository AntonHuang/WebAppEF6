using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApp.Models
{
    public class IDGenerator
    {
        private int nextIdx;
        private string prefix;
        private DateTime lastDatePrefix;

        private static IDGenerator _memberIDGenerator;
        private static IDGenerator _saleToCustomerIDGenerator;
        private static IDGenerator _memberPointIDGenerator;

        public IDGenerator(string prefix, int nextIdx) {
            this.prefix = prefix;
            this.nextIdx = nextIdx < 0 ? 0 : nextIdx;
            this.lastDatePrefix = DateTime.Now;
        }

        public void SetNextIdx(int nextIdx) {
            this.nextIdx = nextIdx;
        }

        private string GetPrefix() {
            return string.Format("{0}{1:yyMM}", prefix, DateTime.Now);
        }
        
        public string GetNext() {
            string result = string.Empty;
            lock (this)
            {
                if (this.lastDatePrefix.Year != DateTime.Now.Year
                    || this.lastDatePrefix.Month != DateTime.Now.Month)
                {
                    nextIdx = 1;
                }

                if (nextIdx < 10000)
                {
                    result = string.Format("{0}{1:D3}", GetPrefix(), nextIdx);
                }
                else
                {
                    result = string.Format("{0}{1:G}", GetPrefix(), nextIdx);
                }

                this.lastDatePrefix = DateTime.Now;
                nextIdx = nextIdx + 1;
            }
            
            return result;
        }

        public static IDGenerator GetMemberIDGenerator(ApplicationDbContext dbContext) {
            if (_memberIDGenerator == null) {
                lock (typeof(IDGenerator)) {
                    if(_memberIDGenerator == null)
                    {
                        var lastMemberID = dbContext.Members
                                           .OrderByDescending(m => m.RegisterDate)
                                           .Take(1)
                                           .Select(m => m.MemberID)
                                           .FirstOrDefault();
                        _memberIDGenerator = IDGenerator.CreateFromID(lastMemberID, "BE");
                    }
                   
                }
            }
            return _memberIDGenerator;
        }

        private static IDGenerator CreateFromID(string lastID, string prefix)
        {
            IDGenerator g = new IDGenerator(prefix, 1);
            if (string.IsNullOrWhiteSpace(lastID) == false) {
                string newPrefix = g.GetPrefix();
                if (lastID.StartsWith(newPrefix)) {
                    int currIdx;
                    int.TryParse(lastID.Substring(newPrefix.Length), out currIdx);
                    if(currIdx > 0)
                    {
                      g.SetNextIdx(currIdx + 1);
                    }
                }    
            }
            return g;
        }

        public static IDGenerator GetSaleToCustomerIDGenerator(ApplicationDbContext dbContext)
        {
            if (_saleToCustomerIDGenerator == null)
            {
                lock (typeof(IDGenerator))
                {
                    if (_saleToCustomerIDGenerator == null)
                    {
                        var lastMemberID = dbContext.SaleToCustomer
                                           .OrderByDescending(m => m.CreateDate)
                                           .Take(1)
                                           .Select(m => m.ID)
                                           .FirstOrDefault();
                        _saleToCustomerIDGenerator = IDGenerator.CreateFromID(lastMemberID, "STC");
                    }

                }
            }
            return _saleToCustomerIDGenerator;
        }

        public static IDGenerator GetMemberPointIDGenerator(ApplicationDbContext dbContext)
        {
            if (_memberPointIDGenerator == null)
            {
                lock (typeof(IDGenerator))
                {
                    if (_memberPointIDGenerator == null)
                    {
                        var lastMemberID = dbContext.MemberPoint
                                           .OrderByDescending(m => m.CreateDate )
                                           .Take(1)
                                           .Select(m => m.ID)
                                           .FirstOrDefault();
                        _memberPointIDGenerator = IDGenerator.CreateFromID(lastMemberID, "BEMP");
                    }

                }
            }
            return _memberPointIDGenerator;
        }

    }
}
