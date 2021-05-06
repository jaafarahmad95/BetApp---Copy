using AspNetCore.Identity.Mongo;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using testtest.Contract;
using testtest.DataAccess;
using testtest.LoggingServices;
using testtest.MappingProfile;
using testtest.Models;
using testtest.Service;
using testtest.Service.General;
using testtest.Service.Site;
using testtestdbcontext.AppUserService;
using testtestdbcontext.AppUserService.Site;

namespace testtest.Installer
{
    public class ServicesInstaller : IInstaller
    {
        public void InstallServicesAssembly(IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<IAppUserService, AppUserService>();
            services.AddScoped<IAuthenticationService, AuthenticationService>();
            services.AddScoped<ILoggerManager, LoggerManager>();
            services.AddScoped<IMongoDbContext, MongoDbContext>();
            services.AddScoped<IAccountService, AccountService>();
            services.AddScoped<IOfferService, OfferService>();
            services.AddScoped<IEventServices, EventServices>();
            services.AddSingleton<IActionContextAccessor, ActionContextAccessor>();
            services.AddScoped<IBetCounterService,BetCounterService>();
            services.AddScoped<IConnectionService,ConnectionService>();
            services.AddScoped<IBetServices,BetServices>();
            services.AddScoped<IWithdrowService,WithdrowService>();
            services.AddScoped<IUrlHelper, UrlHelper>(implementationFactory =>
            {
                var actionContext = implementationFactory.GetService<IActionContextAccessor>().ActionContext;
                return new UrlHelper(actionContext);
            });
            services.AddTransient<IpropertyMappingService, PropertyMappingService>();
            services.AddTransient<ITypeHelperService, TypeHelperService>();
            services.AddScoped<IDepositService, DepositService>();
            services.AddScoped<ICurrencyService, CurrencyService>();


            
           


            var mapperConfig = new MapperConfiguration(mc =>
            {
                mc.AddProfile(new DomainToResponseProfile());
            });

            IMapper mapper = mapperConfig.CreateMapper();
            services.AddSingleton(mapper);
            services.AddCors(options => options.AddPolicy("DiscPolicy",
              builder =>
              {
                  
                  builder.WithOrigins("https://lacalhost:4200").AllowAnyHeader().AllowAnyMethod().AllowCredentials().SetIsOriginAllowed((host)=>true);
              }));



        }
    }
}
