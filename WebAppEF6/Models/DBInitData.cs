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
                                ID = "PDESC001",
                                Name = "床垫",
                                DisplayName = "1.0*1.9*0.20",
                                SizeSpec = "1.0*1.9*0.20",
                                Type = "床垫",
                                Unit = "张",
                                Price = 17530
                            },
                        new ProductDesc {
                                ID = "PDESC002",
                                Name = "床垫",
                                DisplayName = "1.0*2.0*0.20",
                                SizeSpec = "1.0*2.0*0.20",
                                Type = "床垫",
                                Unit = "张",
                                Price = 18580
                            },
                        new ProductDesc {
                                ID = "PDESC003",
                                Name = "床垫",
                                DisplayName = "1.5*1.9*0.20",
                                SizeSpec = "1.5*1.9*0.20",
                                Type = "床垫",
                                Unit = "张",
                                Price = 20980
                            },
                        new ProductDesc {
                                ID = "PDESC004",
                                Name = "床垫",
                                DisplayName = "1.5*2.0*0.20",
                                SizeSpec = "1.5*2.0*0.20",
                                Type = "床垫",
                                Unit = "张",
                                Price = 22130
                            },
                        new ProductDesc {
                                ID = "PDESC005",
                                Name = "床垫",
                                DisplayName = "1.8*2.0*0.20",
                                SizeSpec = "1.8*2.0*0.20",
                                Type = "床垫",
                                Unit = "张",
                                Price = 26110
                            },
                         new ProductDesc {
                                ID = "PDESC006",
                                Name = "床垫",
                                DisplayName = "2.0*2.2*0.20",
                                SizeSpec = "2.0*2.2*0.20",
                                Type = "床垫",
                                Unit = "张",
                                Price = 32090
                            },
                          new ProductDesc {
                                ID = "PDESC007",
                                Name = "学生床",
                                DisplayName =  "0.9*1.9*0.07",
                                SizeSpec = "0.9*1.9*0.07",
                                Type = "学生床",
                                Unit = "张",
                                Price = 13330
                            },
                          new ProductDesc {
                                ID = "PDESC008",
                                Name = "体验床",
                                DisplayName =  "0.75*1.9*0.20",
                                SizeSpec = "0.75*1.9*0.20",
                                Type = "体验床",
                                Unit = "张",
                                Price = 11370
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
