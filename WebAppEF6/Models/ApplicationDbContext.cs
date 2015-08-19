using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.Core.Objects.DataClasses;
using System.Linq;
using System.Threading.Tasks;
using WebApp.DomainModels.Core;
using WebApp.DomainModels.Customer;
using WebApp.DomainModels.Product;
using WebAppEF6;
using WebAppEF6.Models;

namespace WebApp.Models
{
 
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {

        public ApplicationDbContext(IOwinContext context)
           : base("CRMDBConnection", throwIfV1Schema: false)
        {
            Database.SetInitializer<ApplicationDbContext>(new DBInitData(context) );
        }

        public static ApplicationDbContext Create(IdentityFactoryOptions<ApplicationDbContext> options, IOwinContext context)
        {
            return new ApplicationDbContext(context);
        }


        public virtual DbSet<Member> Members { get; set; }
        public virtual DbSet<Mattress> Mattress { get; set; }
        public virtual DbSet<ProductDesc> ProductDesc { get; set; }
        public virtual DbSet<SaleToCustomer> SaleToCustomer { get; set; }
        public virtual DbSet<SaleToCustomerDetail> SaleToCustomeDetails { get; set; }
        public virtual DbSet<MemberPoint> MemberPoint { get; set; }
        public virtual DbSet<Setting> Settings { get; set; }
        public virtual DbSet<SettingHistory> SettingHistorys { get; set; }


        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {

            base.OnModelCreating(modelBuilder);
            // Customize the ASP.NET Identity model and override the defaults if needed.
            // For example, you can rename the ASP.NET Identity table names and more.
            // Add your customizations after calling base.OnModelCreating(builder);

            modelBuilder.Entity<Member>().HasKey(e => e.MemberID)
                    .Property(e => e.MemberID).HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);
            //one to many relationship
            modelBuilder.Entity<Member>().HasOptional<Member>(m => m.Reference)
                                .WithMany(m => m.Candidates)
                                .HasForeignKey(m=>m.ReferenceMemberID);


            //one to one relationship between ApplicationUser and Member
            modelBuilder.Entity<ApplicationUser>().HasRequired(u => u.MemberInfo).WithRequiredDependent();


            //modelBuilder.Entity<ApplicationUser>()
            //    .Property(appUser => appUser.ChangedPassword).DefaultValue<bool>(0);



            modelBuilder.Entity<ProductDesc>().HasKey(pd => pd.ID);


            modelBuilder.Entity<Mattress>().HasKey(m => m.ID);
            //one to many relationship
            modelBuilder.Entity<Mattress>().HasRequired(m => m.TypeDesc).WithMany()
                .HasForeignKey(m => m.TypeDescID);


            modelBuilder.Entity<SaleToCustomerDetail>().HasKey(d => d.ID)
                .Property(d => d.ID).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
            //one to many relationship
            modelBuilder.Entity<SaleToCustomerDetail>().HasRequired(d => d.Sale).WithMany(s => s.DetailItems);


            modelBuilder.Entity<MemberPoint>().HasKey(d => d.ID);

            //one to many relationship
            modelBuilder.Entity<Member>().HasMany(m => m.MemberPointItems).WithOptional(mp => mp.Owner)
                    .HasForeignKey(m => m.OwnerMemberID);

            //one to many relationship
            modelBuilder.Entity<Member>().HasMany(m => m.MemberPointOperationItems).WithOptional(mp => mp.OperationBy)
                    .HasForeignKey(m => m.OperationByMemberID);

            //one to many relationship
            modelBuilder.Entity<MemberPoint>().HasOptional(mp => mp.Product).WithMany()
                    .HasForeignKey(m => m.ProductID);

            //one to many relationship
            modelBuilder.Entity<MemberPoint>().HasOptional(mp => mp.ProductBuyer).WithMany()
                    .HasForeignKey(m => m.ProductBuyerMemberID);

            modelBuilder.Entity<Setting>().HasKey(s => s.ID);

            modelBuilder.Entity<SettingHistory>().HasKey(h => h.ID)
                .Property(h => h.ID)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
        }
    }


    public static class ApplicationDbExtensions {

        public static bool Exists< TEntity>(this ApplicationDbContext context, TEntity entity, Func<TEntity, TEntity, bool> predicate) where TEntity : class
        {
            return context.Set<TEntity>().Local.Any( e => predicate(e, entity));
        }


        public static void TryToAttach<TEntity>(this ApplicationDbContext context, TEntity entity, Func<TEntity, TEntity, bool> predicate) where TEntity : class
        {
            if (context.Exists(entity, predicate) == false) {
                context.Set<TEntity>().Attach(entity);
                context.Entry(entity).State = EntityState.Detached;
            }
        }

        public static void TryToAttachMember(this ApplicationDbContext context, Member entity)
        {
            context.TryToAttach(entity, (l, r) =>
            {
                bool result = l.MemberID.Equals(r.MemberID);
                return result;
            });
        }

        public static Member FindOrAttachToLocal(this ApplicationDbContext context, string memberID)
        {
            var member = context.Members.Local.FirstOrDefault(m => m.MemberID.Equals(memberID));
            if (member == null) {
                member = new Member { MemberID = memberID };
                context.Members.Attach(member);
                context.Entry(member).State = EntityState.Unchanged;
            }
            return member;
        }

    }

}
