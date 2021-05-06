using AutoMapper;
using Hangfire;
using Hangfire.Mongo;
using Hangfire.Mongo.Migration.Strategies;
using Hangfire.Mongo.Migration.Strategies.Backup;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using NLog;
using Quartz;
using Quartz.Impl;
using testtest.Jobs;
using Swashbuckle.AspNetCore.SwaggerUI;
using System;
using System.Collections.Specialized;
using System.IO;
using testtest.Contract;
using testtest.DataAccess;
using testtest.Extensions;
using testtest.helpers;
using testtest.Installer;
using testtest.jobs;
using testtest.MappingProfile;
using testtest.Models.DataBaseSetting;
using testtest.Service;
using testtest.SignalRHub;
using Microsoft.AspNetCore.SignalR;
using testtestdbcontext.testtest.SignalRHub;

namespace testtest
{
    public class Startup
    {
        private IScheduler _quartzScheduler;
        private IScheduler _quartzScheduler2;
        public Startup(IConfiguration configuration)
        {
            LogManager.LoadConfiguration(string.Concat(Directory.GetCurrentDirectory(), "/nlog.config"));
            Configuration = configuration;
            _quartzScheduler = ConfigureQuartz();
            _quartzScheduler2 = ConfigureQuartz2();
        }
        public IScheduler ConfigureQuartz()
        {

            NameValueCollection props = new NameValueCollection
             {   
            { "quartz.serializer.type", "json" },
            {"quartz.jobStore.collectionPrefix" , "quartz"},
            {"quartz.jobStore.type","Quartz.Spi.MongoDbJobStore.MongoDbJobStore, Quartz.Spi.MongoDbJobStore"},
            {"quartz.jobStore.connectionString" , "mongodb://localhost/Quartzjobs"}
             
             };
            StdSchedulerFactory factory = new StdSchedulerFactory(props);
            var scheduler = factory.GetScheduler().Result;

            /*scheduler.ListenerManager.AddTriggerListener(new TriggerListener());
            scheduler.ListenerManager.AddJobListener(new JobListener());
            scheduler.ListenerManager.AddSchedulerListener(new SchedulerListener());*/
            return scheduler;

        }
         public IScheduler ConfigureQuartz2()
        {

            NameValueCollection props = new NameValueCollection
             {   
              { "quartz.serializer.type", "json" },
            {"quartz.jobStore.collectionPrefix" , "quartz"},
            {"quartz.jobStore.type","Quartz.Spi.MongoDbJobStore.MongoDbJobStore, Quartz.Spi.MongoDbJobStore"},
            {"quartz.jobStore.connectionString" , "mongodb://localhost/Quartzjobs"}
             };
            StdSchedulerFactory factory2 = new StdSchedulerFactory(props);
            var scheduler2 = factory2.GetScheduler().Result;

            /*scheduler.ListenerManager.AddTriggerListener(new TriggerListener());
            scheduler.ListenerManager.AddJobListener(new JobListener());
            scheduler.ListenerManager.AddSchedulerListener(new SchedulerListener());*/
            return scheduler2;

        }


        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services )
        {
         
            services.Configure<DatabaseSettings>(options =>
           {
               options.ConnectionString = Configuration.GetSection("DatabaseSettings:ConnectionString").Value;
               options.DatabaseName = Configuration.GetSection("DatabaseSettings:DatabaseName").Value;
           }
                );
             services.InstallServicesAssembly(Configuration);

            var mongoUrlBuilder = new MongoUrlBuilder("mongodb://localhost/jobs");
            var mongoClient = new MongoClient(mongoUrlBuilder.ToMongoUrl());
            services.AddScoped<UpdateOddsJob>();
            services.AddScoped(provider => _quartzScheduler);
            services.AddScoped<UpdateScoreBoard>();
            services.AddScoped(provider => _quartzScheduler2);
            services.AddQuartz();
            services.AddQuartzServer();
            services.AddHangfire(configuration => configuration
                .SetDataCompatibilityLevel(CompatibilityLevel.Version_170)
                .UseSimpleAssemblyNameTypeSerializer()
                .UseRecommendedSerializerSettings()
                .UseMongoStorage(mongoClient, mongoUrlBuilder.DatabaseName, new MongoStorageOptions
                {
                    MigrationOptions = new MongoMigrationOptions
                    {
                        MigrationStrategy = new MigrateMongoMigrationStrategy(),
                        BackupStrategy = new CollectionMongoBackupStrategy()
                    },
                    Prefix = "hangfire.mongo",
                    CheckConnection = true
                })
            );
           
            services.AddHangfireServer(serverOptions =>
            {
                serverOptions.ServerName = "Hangfire.Mongo server 1";
            });
            services.AddSignalR(hubOptions =>
            {
                hubOptions.EnableDetailedErrors = true;
                //hubOptions.KeepAliveInterval = TimeSpan.FromMinutes(1);
                

            });
           
            services.AddControllers();
        }

        private void OnShutdown()
        {
            //shutdown quartz is not shutdown already
            if (!_quartzScheduler.IsShutdown) _quartzScheduler.Shutdown();
           if (!_quartzScheduler2.IsShutdown) _quartzScheduler2.Shutdown();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env 
            , ILoggerManager logger, IMongoDbContext context, IHostApplicationLifetime appLifetime)
        {
            app.UseCors("DiscPolicy");

            _quartzScheduler.JobFactory = new AspnetCoreJobFactory(app.ApplicationServices);
            _quartzScheduler.Start().Wait();
            _quartzScheduler2.JobFactory = new AspnetCoreJobFactory(app.ApplicationServices);
            _quartzScheduler2.Start().Wait();
             appLifetime.ApplicationStopped.Register(() =>
            {
                // Drop hangfire database on server shutdown
                var mongoUrlBuilder = new MongoUrlBuilder("mongodb://localhost/jobs");
                var quartzmong = new MongoUrlBuilder("mongodb://localhost/Quartzjobs");
                var dbName = mongoUrlBuilder.DatabaseName;
                var quartzdb = quartzmong.DatabaseName;
                context.Dropdatabase(dbName);
                 context.Dropdatabase(quartzdb);
            });
        
            app.UseHangfireServer();
            app.UseHangfireDashboard();
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }
           // app.ConfigureExceptionHandler(logger);
            var swaggerSettings = new SwaggerSettings();
            Configuration.GetSection(nameof(swaggerSettings)).Bind(swaggerSettings);
            app.UseSwagger(options =>
            {
                options.RouteTemplate = swaggerSettings.JsonRoute;
            });
            app.UseSwaggerUI(setupAction =>
            {
                setupAction.SwaggerEndpoint(swaggerSettings.UiEndPoint, swaggerSettings.Description);
                setupAction.DocExpansion(DocExpansion.None);
            });
            DbInitializer.SeedData(context);
            app.UseHttpsRedirection();
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseEndpoints(endpoints =>
            {  
                endpoints.MapHub<LiveGameHub>("/LiveGameHub");
                endpoints.MapHub<BetSystemHub>("/Betsystemhub");
                 endpoints.MapHub<ScoreBoardHub>("/ScoreBHub");
                endpoints.MapControllers();
                
            });

            RecurringJob.AddOrUpdate<BetsSetJob>(
             job => job.SetBets(), " */5 * * * *");

             RecurringJob.AddOrUpdate<LiveGamesUpdateJob>(
             job => job.Updatelivegames(), "*/15 * * * * *");
        }
    }
}
