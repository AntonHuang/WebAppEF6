using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApp.Common;
using WebApp.Models;
using WebApp.DomainModels.Product;
using WebApp.DomainModels.Customer;
using Microsoft.AspNet.Identity;
using System.Security.Claims;
using WebApp.DomainModels.Core;
using System.Data.Common;
using System.Web.Mvc;
using WebAppEF6;
using System.Web;
using Microsoft.AspNet.Identity.Owin;
using WebAppEF6.Models;

using System.Data.Entity;

// For more information on enabling MVC for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace WebApp.Controllers
{
    [Authorize()]
    public class MattressController : AsyncController
    {


        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;
        private ApplicationRoleManager _roleManager;
        private ApplicationDbContext _applicationDbContext;

        public MattressController()
        {
        }

        public MattressController(ApplicationUserManager userManager, ApplicationSignInManager signInManager, ApplicationRoleManager roleManager,
            ApplicationDbContext dbContext)
        {
            UserManager = userManager;
            SignInManager = signInManager;
            RoleManager = roleManager;
            AppDbContext = dbContext;
        }

        public ApplicationSignInManager SignInManager
        {
            get
            {
                return _signInManager ?? HttpContext.GetOwinContext().Get<ApplicationSignInManager>();
            }
            private set
            {
                _signInManager = value;
            }
        }

        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }

        public ApplicationRoleManager RoleManager
        {
            get
            {
                return _roleManager ?? HttpContext.GetOwinContext().Get<ApplicationRoleManager>();
            }
            private set
            {
                _roleManager = value;
            }
        }


        public ApplicationDbContext AppDbContext
        {
            get
            {
                return _applicationDbContext ?? HttpContext.GetOwinContext().Get<ApplicationDbContext>();
            }
            private set
            {
                _applicationDbContext = value;
            }
        }

     


        [HttpGet]
        public async Task<ActionResult> ListMattressType()
        {
            if (ModelState.IsValid)
            {
                var items = await  this.AppDbContext.ProductDesc.Select(
                                      pd => new { ID = pd.ID, Name = pd.Name, Type = pd.Type }
                                    ).ToListAsync();

                //if (items.Count() > 0)
                //{
                    return Json(items, JsonRequestBehavior.AllowGet);
                //}
                //else
                //{
                //    return Json(new { ID = "", Name = "", Type = "" });
                //}
            }
            return JsonMessage.BadRequestJsonResult(ModelState.Values.ToJson());
        }

        [HttpPost]
        public async Task<ActionResult> Sell(SellToCustomerViewModel model)
        {
            if (ModelState.IsValid)
            {

                var existMattresID = await AppDbContext.Mattress
                                                .Where(m => m.ID.Equals(model.MattressID, StringComparison.InvariantCultureIgnoreCase))
                                                .Take(1)
                                                .Select(m => m.ID)
                                                .FirstOrDefaultAsync();

                if (existMattresID != null)
                {
                    return JsonMessage.BadRequestJsonResult("MattressID is Exist.");
                }

                var existMattressTypeID = await AppDbContext.ProductDesc
                                       .Where(m => m.ID.Equals(model.MattressTypeID, StringComparison.InvariantCultureIgnoreCase))
                                       .Take(1)
                                       .Select(m => m.ID)
                                       .FirstOrDefaultAsync();
                if (existMattressTypeID == null)
                {
                    return JsonMessage.BadRequestJsonResult("MattressTypeID is not Exist.");
                }

                var existCustomerID = await AppDbContext.Members
                                       .Where(m => m.MemberID.Equals(model.CustomerID, StringComparison.InvariantCultureIgnoreCase))
                                       .Take(1)
                                       .Select(m => m.MemberID)
                                       .FirstOrDefaultAsync();
                if (existCustomerID == null)
                {
                    return JsonMessage.BadRequestJsonResult("CustomerID is not Exist.");
                }

                return await DoSell(model);
            }
            return JsonMessage.BadRequestJsonResult(ModelState.Values.SelectMany(x => x.Errors));
        }

        private async Task<ActionResult> DoSell(SellToCustomerViewModel model)
        {
            var loginUser = await GetCurrentUserAsync();

            ProductDesc productDesc = await AppDbContext.ProductDesc
                                        .Where(m => m.ID.Equals(model.MattressTypeID,
                                                StringComparison.InvariantCultureIgnoreCase))
                                        .FirstOrDefaultAsync();
            Mattress mattress = new Mattress
            {
                ID = model.MattressID,
                TypeDesc = productDesc,
                RegisterDate = model.SaleDate,
                SaleDate = model.SaleDate,
            };

            SaleToCustomerDetail saleToCustomerDetail = new SaleToCustomerDetail
            {
                Gifts = model.Gifts,
                DeliveryAddress = model.DeliveryAddress,
                Prodect = mattress,
                Price = productDesc.Price

            };
            SaleToCustomer saleToCustomer = new SaleToCustomer
            {
                ID = IDGenerator.GetSaleToCustomerIDGenerator(AppDbContext).GetNext(),
                Customer = this.AppDbContext.FindOrAttachToLocal(model.CustomerID ),
                SellingAgents = this.AppDbContext.FindOrAttachToLocal(loginUser.UserName),
                DealDate = model.SaleDate,
            };

            saleToCustomerDetail.Sale = saleToCustomer;
            saleToCustomer.DetailItems.Add(saleToCustomerDetail);

           // this.AppDbContext.TryToAttach(saleToCustomer.Customer, (l, r) => r.MemberID.Equals(l.MemberID));
          //  this.AppDbContext.TryToAttach(saleToCustomer.SellingAgents, (l, r) => r.MemberID.Equals(l.MemberID));
            //AppDbContext.Members.Attach(saleToCustomer.Customer);
           // AppDbContext.Members.Attach(saleToCustomer.SellingAgents);

            AppDbContext.Mattress.Add(mattress);
            AppDbContext.SaleToCustomer.Add(saleToCustomer);
            AppDbContext.SaleToCustomeDetails.Add(saleToCustomerDetail);

            var pointItems = await AddMemberPoint(saleToCustomerDetail);

            AppDbContext.SaveChanges();

            return Json(new {
                    saleToCustomerID = saleToCustomer.ID,
                    memberPointItems = pointItems,
                    sellMattressData = new {
                        MattressID= mattress.ID,
                        MattressTypeName= mattress.TypeDesc.Name,
                        DeliveryAddress= saleToCustomerDetail.DeliveryAddress,
                        CustomerID= saleToCustomer.Customer.MemberID,
                        SaleDate= saleToCustomer.DealDate.Date.ToString("yyyy'-'MM'-'dd"),
                        Gifts= saleToCustomerDetail.Gifts
                    },
                JsonRequestBehavior.AllowGet
            });
        }

        private async Task<SellMemberPointViewModel> AddMemberPoint(SaleToCustomerDetail saleToCustomerDetail)
        {
            MemberPointRule pointRule = await this.GetPointRule();

            // Left JOIN
            var customers = await (from m1 in this.AppDbContext.Members
                                   join m2 in this.AppDbContext.Members
                                       on m1.ReferenceMemberID equals m2.MemberID
                                    select new {
                                        MemberID = m1.MemberID,
                                        MemberName = m1.Name,
                                        MemberLevel = m1.Level,

                                        Up1ID = m2.MemberID,
                                        Up1Name = m2.Name,
                                        Up1Level = m2.Level,
                                        Up1ReferenceMemberID = m2.ReferenceMemberID
                                    } into M12
                                   join m3 in this.AppDbContext.Members 
                                       on M12.Up1ReferenceMemberID equals m3.MemberID into M23
                                   from m5 in M23.DefaultIfEmpty()
                                   where M12.MemberID.Equals(saleToCustomerDetail.Sale.Customer.MemberID, 
                                       StringComparison.InvariantCultureIgnoreCase)
                                   select new SellMemberPointViewModel
                                   {
                                       MemberID = M12.MemberID,
                                       MemberName = M12.MemberName,
                                       MemberLevel = M12.MemberLevel,

                                       Up1ID = M12.Up1ID,
                                       Up1Name = M12.Up1Name,
                                       Up1Level = M12.Up1Level,

                                       Up2ID = m5.MemberID,
                                       Up2Name = m5.Name,
                                       Up2Level = m5.Level,
                                   }).FirstOrDefaultAsync();

            
            //var customers = GetCustomersFromSqlCommand(AppDbContext.Database.Connection, 
           //      saleToCustomerDetail.Sale.Customer.MemberID);


            if (customers == null)
            {
                return new SellMemberPointViewModel
                {
                    MemberID = "",
                    MemberName = "",
                    MemberLevel = "",

                    Up1ID = "",
                    Up1Name = "",
                    Up1Level = "",

                    Up2ID = "",
                    Up2Name = "",
                    Up2Level = ""
                };
            }

            if (string.IsNullOrWhiteSpace(customers.MemberID) == false)
            {
                MemberPoint menberPoint = new MemberPoint(saleToCustomerDetail);
                menberPoint.Owner = this.AppDbContext.FindOrAttachToLocal(customers.MemberID );
                menberPoint.UseableDate = pointRule.CalcAvailableDate(menberPoint.DealDate);
                menberPoint.Quantity = pointRule.Calc(customers.MemberLevel, LevelRelation.Self, saleToCustomerDetail.Price);
                menberPoint.ID = IDGenerator.GetMemberPointIDGenerator(this.AppDbContext).GetNext();

                var pointInfo = await GetMemberPointInfo(menberPoint.Owner.MemberID);
                if(pointInfo != null)
                {
                    menberPoint.CurrentTotalQuantity = pointInfo.PointTotal;
                }

                this.AppDbContext.MemberPoint.Add(menberPoint);
                customers.PointCount = menberPoint.Quantity;
            }

            if (string.IsNullOrWhiteSpace(customers.Up1ID) == false)
            {
                MemberPoint menberPoint = new MemberPoint(saleToCustomerDetail);
                menberPoint.Owner = this.AppDbContext.FindOrAttachToLocal(customers.Up1ID );
                menberPoint.UseableDate = pointRule.CalcAvailableDate(menberPoint.DealDate);
                menberPoint.Quantity = pointRule.Calc(customers.Up1Level,
                    LevelRelation.Son, saleToCustomerDetail.Price);
                menberPoint.ID = IDGenerator.GetMemberPointIDGenerator(this.AppDbContext).GetNext();

                var pointInfo = await GetMemberPointInfo(menberPoint.Owner.MemberID);
                if (pointInfo != null)
                {
                    menberPoint.CurrentTotalQuantity = pointInfo.PointTotal;
                }

                this.AppDbContext.MemberPoint.Add(menberPoint);
                customers.Up1PointCount = menberPoint.Quantity;
            }

            if (string.IsNullOrWhiteSpace(customers.Up2ID) == false)
            {
                MemberPoint menberPoint = new MemberPoint(saleToCustomerDetail);
                menberPoint.Owner = this.AppDbContext.FindOrAttachToLocal(customers.Up2ID );
                menberPoint.UseableDate = pointRule.CalcAvailableDate(menberPoint.DealDate);
                menberPoint.Quantity = pointRule.Calc(customers.Up2Level,
                    LevelRelation.Grandson, saleToCustomerDetail.Price);
                menberPoint.ID = IDGenerator.GetMemberPointIDGenerator(this.AppDbContext).GetNext();

                var pointInfo = await GetMemberPointInfo(menberPoint.Owner.MemberID);
                if (pointInfo != null)
                {
                    menberPoint.CurrentTotalQuantity = pointInfo.PointTotal;
                }

                this.AppDbContext.MemberPoint.Add(menberPoint);
                customers.Up2PointCount = menberPoint.Quantity;
            }

            return customers;
        }

        public static SellMemberPointViewModel GetCustomersFromSqlCommand(DbConnection connection, string customerID)
        {
            bool isOpen = connection.State == System.Data.ConnectionState.Open;
            try {
                using (var command = connection.CreateCommand()) {
                    command.CommandText = @"SELECT 
                [m1].[MemberID] AS MemberID,
                [m1].[Name] AS MemberName,
                [m1].[Level]AS MemberLevel,
                [m2].[MemberID] AS Up1ID,
                [m2].[Name] AS Up1Name,
                [m2].[Level] AS Up1Level,
                [m3].[MemberID] AS Up2ID,
                [m3].[Name] AS Up2Name,
                [m3].[Level] AS Up2Level
                FROM [Member] AS[m1]
                    LEFT JOIN [Member] AS[m2] ON[m1].[ReferenceMemberID] = [m2].[MemberID]
                    LEFT JOIN [Member] AS[m3] ON[m2].[ReferenceMemberID] = [m3].[MemberID]
                WHERE [m1].[MemberID] = @CustomerID";

                    try
                    {
                        DbParameter p = command.CreateParameter();
                        p.DbType = System.Data.DbType.String;
                        p.ParameterName = "CustomerID";
                        p.Value = customerID == null ? "" : customerID;

                        command.Parameters.Add(p);
                        if (!isOpen)
                        {
                            connection.Open();
                        }
                        var r = command.ExecuteReader();
                        if (r.Read())
                        {
                            var result = new SellMemberPointViewModel
                            {
                                MemberID = r.IsDBNull(0) ? "" : r.GetString(0),
                                MemberName = r.IsDBNull(1) ? "" : r.GetString(1),
                                MemberLevel = r.IsDBNull(2) ? "" : r.GetString(2),

                                Up1ID = r.IsDBNull(3) ? "" : r.GetString(3),
                                Up1Name = r.IsDBNull(4) ? "" : r.GetString(4),
                                Up1Level = r.IsDBNull(5) ? "" : r.GetString(5),

                                Up2ID = r.IsDBNull(6) ? "" : r.GetString(6),
                                Up2Name = r.IsDBNull(7) ? "" : r.GetString(7),
                                Up2Level = r.IsDBNull(8) ? "" : r.GetString(8),
                            };
                            return result;
                        }
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                    }
                }
            }finally{
                if (isOpen) {
                    connection.Close();
                }
            }
            return null;
      }

        private async Task<MemberPointRule> GetPointRule() {
            var ruleSetting =  await(from s in this.AppDbContext.Settings
                         where s.ID.Equals(SettingName.PointRule.ToString(), StringComparison.InvariantCultureIgnoreCase)
                         select s
                          ).FirstOrDefaultAsync();
            if (ruleSetting == null)
            {
                return MemberPointRule.Default;
            }
            else {
                return MemberPointRule.fromJson(ruleSetting.SettingValue);
            }
        }


        private async Task<ApplicationUser> GetCurrentUserAsync()
        {
            return await this.UserManager.FindByIdAsync(GetCurrentUserID());
        }

        private string GetCurrentUserID()
        {

            var userID = SignInManager.GetVerifiedUserId();
            if (userID == null)
            {
                userID = User.Identity.GetUserId();
            }
            return userID;
        }




        [HttpGet]
        public async Task<ActionResult> PointExch(string id)
        {
            if (ModelState.IsValid)
            {
                var memberInfo = await (from m in AppDbContext.Members
                                        where m.MemberID.Equals(id, StringComparison.InvariantCultureIgnoreCase)
                                        select m)
                                        .FirstOrDefaultAsync();

                if (memberInfo == null)
                {
                    return HttpNotFound("MemberID is not Exist.");
                }

                MemberPointInfoViewModel pointInfo = await GetMemberPointInfo(memberInfo.MemberID);

                if (pointInfo == null)
                {
                    pointInfo = new MemberPointInfoViewModel
                    {
                        MemberID = memberInfo.MemberID,
                        PointTotal = 0,
                        UsablePoint = 0,
                    };
                }

                pointInfo.IDCard = memberInfo.IDCard;
                pointInfo.MemberName = memberInfo.Name;

                return Json(pointInfo, JsonRequestBehavior.AllowGet);
            }
            return JsonMessage.BadRequestJsonResult(ModelState.Values.SelectMany(x => x.Errors));
        }

        private async Task<MemberPointInfoViewModel> GetMemberPointInfo(string memberID)
        {

            return await (from m in AppDbContext.MemberPoint
                          where m.OwnerMemberID.Equals(memberID, StringComparison.InvariantCultureIgnoreCase)
                          group m by m.OwnerMemberID into memberGroup
                          select new MemberPointInfoViewModel
                          {
                              MemberID = memberGroup.Key,
                              PointTotal = memberGroup.Sum(item => item.Quantity),
                              UsablePoint = memberGroup.Where(item => item.UseableDate <= DateTime.Today.Date).Count() > 0 ?
                                                memberGroup.Where(item => item.UseableDate <= DateTime.Today.Date).Sum(item => item.Quantity) : 0
                          })
                          .FirstOrDefaultAsync();
        }

        [HttpPost]
        public async Task<ActionResult> PointExch(MemberPointInfoViewModel model)
        {
            if (ModelState.IsValid)
            {
                if (model.ExchAmount < 0) {
                    return JsonMessage.BadRequestJsonResult("ExchAmount must be greater than 0.");
                }

                MemberPointInfoViewModel pointInfo = await GetMemberPointInfo(model.MemberID);

                if (pointInfo == null || pointInfo.UsablePoint < model.ExchAmount) {
                    return JsonMessage.BadRequestJsonResult("ExchAmount is greater than UsablePoint.");
                }

                MemberPoint exchPoint = new MemberPoint();
                exchPoint.ID = IDGenerator.GetMemberPointIDGenerator(AppDbContext).GetNext();
                exchPoint.OwnerMemberID = model.MemberID ;
                exchPoint.Type = "PointExch";
                exchPoint.OperationBy = this.AppDbContext.FindOrAttachToLocal((await this.GetCurrentUserAsync()).UserName);
                exchPoint.Quantity = -model.ExchAmount;
                exchPoint.CurrentTotalQuantity = pointInfo.PointTotal;

                this.AppDbContext.MemberPoint.Add(exchPoint);
                this.AppDbContext.SaveChanges();


                MemberPointInfoViewModel newPointInfo = await GetMemberPointInfo(model.MemberID);
                newPointInfo.IDCard = model.IDCard;
                newPointInfo.MemberName = model.MemberName;
                return Json(newPointInfo, JsonRequestBehavior.AllowGet);
            }
            return JsonMessage.BadRequestJsonResult(ModelState.Values.SelectMany(x => x.Errors));
        }

        [HttpGet]
        public async Task<ActionResult> PointDetail(string id, int page, int pagesize)
        {
            if (ModelState.IsValid)
            {
                int totalSize = await (from mp in AppDbContext.MemberPoint
                                       where  mp.OwnerMemberID.Equals(id, StringComparison.InvariantCultureIgnoreCase)
                                                    && mp.Type.Equals("FromSale")
                                       select mp.ID
                                       ).CountAsync();
                                      
                if (totalSize == 0)
                {
                    return HttpNotFound("PointDetail is not Exist.");
                }

                var items = (from mp in AppDbContext.MemberPoint
                             join c in AppDbContext.Members on mp.ProductBuyerMemberID equals c.MemberID
                             join p in AppDbContext.Mattress on mp.ProductID equals p.ID
                             join pDesc in AppDbContext.ProductDesc on p.TypeDescID equals pDesc.ID

                             where mp.OwnerMemberID.Equals(id, StringComparison.InvariantCultureIgnoreCase)
                             orderby mp.CreateDate descending
                             select new
                             {
                                 ProductBuyerID = c.MemberID,
                                 ProductBuyerReferenceID = c.ReferenceMemberID,
                                 ProductBuyerName = c.Name,
                                 ProductType = pDesc.Name,
                                 DealDate = mp.DealDate,
                                 CreateDate = mp.DealDate,
                                 Point = mp.Quantity,
                                 CurrentTotalPoint = mp.CurrentTotalQuantity
                             });
                var result = await items.Skip(pagesize * page).Take(pagesize).ToListAsync();

                var newResult = (from item in result
                                 select new {
                                     ProductBuyerName = item.ProductBuyerName,
                                     BuyerRelation = item.ProductBuyerID.Equals(id) ? "自己"
                                                         : item.ProductBuyerReferenceID.Equals(id) ? "二级" : "三级",
                                     ProductTypeName = item.ProductType,
                                     DealDate = item.DealDate.ToString("yyyy'-'MM'-'dd"),
                                     Point = item.Point,
                                     CurrentTotalPoint = item.CurrentTotalPoint
                                 }).ToList();

                return Json(new
                {
                    TotalSize = totalSize,
                    MemberPointItems = newResult
                }, JsonRequestBehavior.AllowGet);
            }
            return JsonMessage.BadRequestJsonResult(ModelState.Values.SelectMany(x => x.Errors));
        }

        [HttpGet]
        public ActionResult MemberRelationDetail(string id)
        {
            if (ModelState.IsValid)
            {
                
                var children = ( from m1 in this.AppDbContext.Members
                                       join m2 in this.AppDbContext.Members
                                                on m1.MemberID equals m2.ReferenceMemberID
                                       join m3 in this.AppDbContext.Members
                                                on m2.MemberID equals m3.ReferenceMemberID into lefJ
                                       from m4 in lefJ.DefaultIfEmpty()
                                       where m1.MemberID.Equals(id, StringComparison.InvariantCultureIgnoreCase)
                                       select new
                                       {
                                           SonID = m2.MemberID == null ? "" : m2.MemberID,
                                           SonName = m2.Name == null ? "" : m2.Name,
                                           GrandSonID = m4.MemberID == null ? "" : m4.MemberID,
                                           GrandSonName = m4.Name == null ? "" : m4.Name
                                       }).ToList();
                /*
                var children = new List<MemberRelationViewModel>();
                using (var connection = this.AppDbContext.Database.Connection) {
                    using (var command = connection.CreateCommand()) {
                        command.CommandText = @"SELECT  m2.[MemberID] as sonID,  m2.[Name] as sonName,  m3.[MemberID] as grandsonID , m3.[Name] as grandsonName
                  from [Members] as m1
                  join [Members] as m2 on m1.MemberID = m2.ReferenceMemberID
                  left join [Members] as m3 on m2.MemberID = m3.ReferenceMemberID 
                  where m1.MemberID = @CustomerID";

                        try
                        {
                            DbParameter p = command.CreateParameter();
                            p.DbType = System.Data.DbType.String;
                            p.ParameterName = "CustomerID";
                            p.Value = id == null ? "" : id;

                            command.Parameters.Add(p);
                            connection.Open();
                            var r = command.ExecuteReader();
                            while (r.Read())
                            {
                                children.Add(new MemberRelationViewModel
                                {
                                    SonID = r.IsDBNull(0) ? "" : r.GetString(0),
                                    SonName = r.IsDBNull(1) ? "" : r.GetString(1),
                                    GrandSonID = r.IsDBNull(2) ? "" : r.GetString(2),
                                    GrandSonName = r.IsDBNull(3) ? "" : r.GetString(3)
                                });
                            }
                            r.Close();
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(e);
                        }

                    }

                }*/
                   
                var result = children.GroupBy(i => i.SonID).Select((g) => new
                {
                    ChildID = g.Key,
                    ChildName = g.Select(a => a.SonName).FirstOrDefault(),
                    children = g.Select(a => new {
                        ChildID = a.GrandSonID,
                        ChildName = a.GrandSonName
                    }).Where(item => string.IsNullOrWhiteSpace(item.ChildID) == false ).ToList()
                }).ToList();
                
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            return JsonMessage.BadRequestJsonResult(ModelState.Values.SelectMany(x => x.Errors));
        }

    

        private class MemberRelationViewModel
        {
            public string GrandSonID { get; set; }
            public string GrandSonName { get; set; }
            public string SonID { get; set; }
            public string SonName { get; set; }
        }
    }
}
