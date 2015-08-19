using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Owin;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using WebApp.DomainModels.Customer;
using WebApp.DomainModels.Product;
using WebAppEF6;
using WebAppEF6.Models;
using Microsoft.AspNet.Identity.Owin;

namespace WebApp.Models
{
    public class DBInitData : CreateDatabaseIfNotExists<ApplicationDbContext>
    {
        private ApplicationDbContext ctx;
        private ApplicationRoleManager roleManager;
        private ApplicationUserManager userManager;
        private IOwinContext owinContext;


        public DBInitData(IOwinContext context)
        {
           this.owinContext = context;
        }

        private void InitializeData()
        {
           
                    CreateRoles();
                    CreateUsers();
                    CreateProductDesc();

            
        }

        private  void CreateRoles()
        {
            if (this.roleManager.Roles.Count() == 0)
            {
                 this.roleManager.Create(new IdentityRole("Administrator"));
                 this.roleManager.Create(new IdentityRole("ShopManager"));
                ctx.SaveChanges();
            }
        }

        private void CreateUsers()
        {
            if (userManager.Users.Count() == 0)
            {
                 CreateUsers("BE0101001", "超级管理员" , "Administrator");
                 CreateUsers("BE0201001", "店长1", "ShopManager");
                 CreateUsers("BE0201002", "店长2", "ShopManager");
                 CreateUsers("BE0201003", "店长3", "ShopManager");
                 CreateUsers("BE0201004", "店长4", "ShopManager");
                 CreateUsers("BE0201005", "店长5", "ShopManager");
                 CreateUsers("BE0201006", "店长6", "ShopManager");
                 CreateUsers("BE0201007", "店长7", "ShopManager");
                 CreateUsers("BE0201008", "店长8", "ShopManager");
                 CreateUsers("BE0201009", "店长9", "ShopManager");
                 CreateUsers("BE0201010", "店长10", "ShopManager");
                 CreateUsers("BE0201011", "店长11", "ShopManager");
                 CreateUsers("BE0201012", "店长12", "ShopManager");
                 CreateUsers("BE0201013", "店长13", "ShopManager");
                 CreateUsers("BE0201014", "店长14", "ShopManager");
                 CreateUsers("BE0201015", "店长15", "ShopManager");
                 CreateUsers("BE0201016", "店长16", "ShopManager");
                 CreateUsers("BE0201017", "店长17", "ShopManager");
                 CreateUsers("BE0201018", "店长18", "ShopManager");
                 CreateUsers("BE0201019", "店长19", "ShopManager");
                 CreateUsers("BE0201020", "店长20", "ShopManager");

                ctx.SaveChanges();
            }

        }

        private  void CreateUsers(string id, string name, string role)
        {
            ApplicationUser user = new ApplicationUser
            {
                Id = id,
                UserName = id
            };
            user.MemberInfo = new Member { MemberID = id, Name = name };
            ctx.Members.Add(user.MemberInfo);
            var rulst =   userManager.Create(user, ApplicationUser.IINT_PASSWORD);
             userManager.AddToRole(user.Id, role);
            //await userManager.AddClaimAsync(user, new Claim("ManageStore", "Allowed"));
        }

        private void CreateProductDesc()
        {
            if (this.ctx.ProductDesc.Count() == 0)
            {
                ProductDesc[] items = {
                        new ProductDesc{
                                ID = "ID_1",
                                Name = "床垫类型_1",
                                Type = "TT1",
                                Price = 888
                            },
                        new ProductDesc {
                                ID = "ID_2",
                                Name = "床垫类型_2",
                                Type = "TT2",
                                Price = 999
                            },
                        new ProductDesc {
                                ID = "ID_3",
                                Name = "床垫类型_3",
                                Type = "TT1",
                                Price = 2499
                            },
                        new ProductDesc {
                                ID = "ID_4",
                                Name = "床垫类型_4",
                                Type = "TT2",
                                Price = 5000
                            },
                        new ProductDesc {
                                ID = "ID_5",
                                Name = "床垫类型_5",
                                Type = "TT6",
                                Price = 1788
                            }

                };

                this.ctx.ProductDesc.AddRange(items);
                this.ctx.SaveChanges();
            }
        }

        protected override void Seed(ApplicationDbContext context)
        {
            
            base.Seed(context);

            this.ctx = context;
            this.roleManager = owinContext.Get<ApplicationRoleManager>();
            this.userManager = owinContext.Get<ApplicationUserManager>();
            this.InitializeData();

        }
    }
}
