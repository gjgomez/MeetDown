using AutoMapper;
using MeetDown.Events.Core.DataAccess;
using MeetDown.Events.Core.DataAccess.Repositories;
using MeetDown.Events.Infrastructure.DataAccess.EntityFramework;
using MeetDown.Events.Infrastructure.DataAccess.EntityFramework.Repositories;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace MeetDown.Events.API
{
    public class Startup
    {
        public static IConfiguration Configuration { get; private set; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();

            services.AddSingleton<IDbMigrator, DbMigrator>();
            services.AddScoped<IMeetRepository, MeetRepository>();
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler();
            }

            app.UseAppServiceRegionMiddleware();

            // update database schema and seed data if needed
            using (var serviceScope = app.ApplicationServices.GetRequiredService<IServiceScopeFactory>().CreateScope())
            {
                var dbMigrator = serviceScope.ServiceProvider.GetService<IDbMigrator>();

                if (!dbMigrator.AllMigrationsApplied())
                {
                    dbMigrator.ApplyPendingMigrations();
                }

                dbMigrator.SeedDatabase();
            }

            // setup auto-mapper configuration
            Mapper.Initialize(config => {
                config.CreateMap<Core.Entities.Group, ViewModels.Groups.ReadViewModel>();
                config.CreateMap<Core.Entities.Group, ViewModels.Groups.UpdateViewModel>();
                config.CreateMap<Core.Entities.Meet, ViewModels.Meets.ReadViewModel>();
                config.CreateMap<Core.Entities.Meet, ViewModels.Meets.UpdateViewModel>();
                config.CreateMap<ViewModels.Groups.CreateViewModel, Core.Entities.Group>();
                config.CreateMap<ViewModels.Groups.UpdateViewModel, Core.Entities.Group>();
                config.CreateMap<ViewModels.Meets.CreateViewModel, Core.Entities.Meet>();
                config.CreateMap<ViewModels.Meets.UpdateViewModel, Core.Entities.Meet>();
            });

            app.UseMvc();
        }
    }
}
