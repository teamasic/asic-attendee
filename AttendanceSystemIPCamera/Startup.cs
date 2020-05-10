using AttendanceSystemIPCamera.BackgroundServices;
using AttendanceSystemIPCamera.Framework.AutoMapperProfiles;
using AttendanceSystemIPCamera.Framework.Database;
using AttendanceSystemIPCamera.Framework.GlobalStates;
using AttendanceSystemIPCamera.Framework.MyConfiguration;
using AttendanceSystemIPCamera.Repositories.UnitOfWork;
using AttendanceSystemIPCamera.Services.AttendanceService;
using AttendanceSystemIPCamera.Services.AttendeeService;
using AttendanceSystemIPCamera.Services.ChangeRequestService;
using AttendanceSystemIPCamera.Services.GroupService;
using AttendanceSystemIPCamera.Services.NetworkService;
using AttendanceSystemIPCamera.Services.RecordService;
using AttendanceSystemIPCamera.Services.SessionService;
using AttendanceSystemIPCamera.Services.UnitService;
using AutoMapper;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.SpaServices.ReactDevelopmentServer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using static AttendanceSystemIPCamera.Framework.Constants;

namespace AttendanceSystemIPCamera
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllersWithViews().AddNewtonsoftJson();

            // In production, the React files will be served from this directory
            services.AddSpaStaticFiles(configuration =>
            {
                configuration.RootPath = "ClientApp/build";
            });

            SetupDatabaseContext(services);
            SetupAutoMapper(services);
            SetupDependencyInjection(services);
            SetupBackgroundService(services);
            setupSwagger(services);
            SetupUnitConfig(services);
            SetupGlobalStateManager(services);
            SetupAppSettings(services);
        }


        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILoggerFactory loggerFactory)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseCors(options =>
            {
                options.AllowAnyMethod();
                options.AllowAnyOrigin();
                options.AllowAnyHeader();
            });

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseSpaStaticFiles();

            app.UseSwagger();
            app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "ASIC API"));

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller}/{action=Index}/{id?}");
            });

            loggerFactory.AddFile(string.Format($"{Constant.LOG_TEMPLATE}", "{Date}"));

            app.UseSpa(spa =>
            {
                spa.Options.SourcePath = "ClientApp";

                if (env.IsDevelopment())
                {
                    spa.UseReactDevelopmentServer(npmScript: "start");
                }
            });
        }

        private void setupSwagger(IServiceCollection services)
        {
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "ASIC API", Version = "v1" });
            });
        }

        private void SetupDatabaseContext(IServiceCollection services)
        {
            services.AddDbContext<MainDbContext>(options =>
            {
                options.UseSqlite(Configuration.GetConnectionString("SqliteDB"));
            });

        }
        private void SetupAutoMapper(IServiceCollection services)
        {
            Mapper.Initialize(cfg =>
            {
                cfg.AddProfile(new MapperProfile());
            });
        }

        private void SetupDependencyInjection(IServiceCollection services)
        {
            services.AddHttpContextAccessor();

            services.AddSingleton(Configuration);
            services.AddSingleton<IMapper>(Mapper.Instance);

            services.AddScoped<DbContext, MainDbContext>();
            services.AddScoped<IGroupService, GroupService>();
            services.AddScoped<IAttendeeService, AttendeeService>();
            services.AddScoped<IAttendanceService, AttendanceService>();
            services.AddScoped<IRecordService, RecordService>();
            services.AddScoped<ISessionService, SessionService>();
            services.AddScoped<IChangeRequestService, ChangeRequestService>();
            services.AddScoped<IAttendeeNetworkService, AttendeeNetworkService>();
            services.AddScoped< UnitService>();
            services.AddScoped<MyUnitOfWork>();
        }

        private void SetupBackgroundService(IServiceCollection services)
        {
            services.AddSingleton<IHostedService, WindowAppRunnerService>();
        }

        private void SetupUnitConfig(IServiceCollection services)
        {
            UnitService unitServiceInstance = UnitServiceFactory.Create(Configuration.GetValue<string>("UnitConfigFile"));
            services.AddSingleton(unitServiceInstance);
        }

        private void SetupGlobalStateManager(IServiceCollection services)
        {
            var globalState = new GlobalState();
            services.AddSingleton(globalState);
        }

        private void SetupAppSettings(IServiceCollection services)
        {
            var appSettings = Configuration.GetSection("AppSettings").Get<AppSettings>();
            services.AddSingleton(appSettings);
        }

    }
}
