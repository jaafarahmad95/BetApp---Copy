using System;
using System.Collections.Generic;
using System.Text;
using testtest.Models;
using AspNetCore.Identity.Mongo;
using MongoDB.Driver;
using testtest.helpers;
using testtest.Models.Deposit;

namespace testtest.DataAccess
{
    public class DbInitializer
    {
       

        public DbInitializer()
        {
           
           
            
        }
        public static void SeedData(IMongoDbContext context)
        {
           var _context = context;
            var _AppUser = _context.GetCollection<AppUser>(typeof(AppUser).Name);
          var  _AppRole = _context.GetCollection<AppUserRole>(typeof(AppUserRole).Name);
            var role = _AppRole.Find<AppUserRole>(AppUserRole => AppUserRole.Name == "Admin").FirstOrDefault();
            if (role == null)
            {
                SeedRoles( _AppRole);
                SeedUsers(_AppUser);
            }

            SeedTables(context);
        }

        public static void SeedTables(IMongoDbContext context)
        {
            var _context = context;
            var paymeth = _context.GetCollection<DepositMethod>(typeof(DepositMethod).Name);
            var curr = _context.GetCollection<Currency>(typeof(Currency).Name);
            if (paymeth.Find(x => true).CountDocuments() == 0)
            {
                List<DepositMethod> listofmethode = new List<DepositMethod>
                { new DepositMethod { MethodId=1, Name = "Cash" },
                    new DepositMethod {  MethodId=2,Name = "Credit Card" },
                    new DepositMethod { MethodId=3, Name = "Debit Card" },
                    new DepositMethod { MethodId=4, Name = "Bank Transfer" },
                    new DepositMethod { MethodId=5, Name = "Cheque" },
                    new DepositMethod { MethodId=6, Name = "Online Transaction" } };
                paymeth.InsertManyAsync(listofmethode);
            }
            if (curr.Find(x => true).CountDocuments() == 0)
            {
                List<Currency> listofcurrencies = new List<Currency>
                {
                    new Currency {CurrenceyId=1,Code = "USD", Name = "American Dollar", Value = 3.77m, Symbol = '$', LastUpdate = DateTime.Now, IsDefault = false },
                    new Currency {CurrenceyId=2, Code = "EURO", Name = "Euro", Value = 4.57m, Symbol = '€', LastUpdate = DateTime.Now, IsDefault = false },
                    new Currency {CurrenceyId=3,Code = "STG", Name = "Pound Sterling", Value = 5.13m, Symbol = '£', LastUpdate = DateTime.Now, IsDefault = false },
                    new Currency {CurrenceyId=4,Code = "TL", Name = "Turkish Lira", Value = 1, Symbol = '₺', LastUpdate = DateTime.Now, IsDefault = true }
                };
                curr.InsertManyAsync(listofcurrencies);
        
            }
        }
        public static void SeedRoles(IMongoCollection<AppUserRole> roleManager)
        {
           
                AppUserRole role = new AppUserRole();
               
                role.Name = "Admin";
                role.NormalizedName = "ADMIN";
                roleManager.InsertOne(role);
            var clientrole = roleManager.Find<AppUserRole>(AppUserRole => AppUserRole.Name == "Client").FirstOrDefault();
            if (clientrole == null)
            {
                AppUserRole Crole = new AppUserRole();

                Crole.Name = "Client";
                Crole.NormalizedName = "Client";

                roleManager.InsertOne(Crole);
            }
        }

        
        public static void SeedUsers(IMongoCollection<AppUser> Appuser)
        {
            var clients = Appuser.CountDocuments(x=>x.Roles[0]== "Admin");
           // var clients = Appuser.Find<AppUser>(AppUser => true).ToList();
           

            if (clients<1)
            {
                string pass = "Disc123!";
                var creater = new Passhash();
                AppUser admin = new AppUser();
                admin.UserName = "Administrator";
                admin.Email = "user1@localhost.com";
                admin.Roles.Add ("Admin") ;
                admin.PasswordHash = creater.gethash(pass);
                admin.Name = "Amir";
                admin.LastName = "Nasser";
                Appuser.InsertOne(admin);
               
            }
        }


    }
}
