using FluentValidation.AspNetCore;
using LabManager.DbModel;
using LabManager.DbModel.Infrastructure;
using LabManager.DbModel.Repositories;
using LabManager.Services.Commanders;
using LabManager.Services.Instance;
using LabManager.Services.Resources;
using LabManager.Services.Runtime;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Saturn72.Core.Caching;
using Saturn72.Core.Services;
using Saturn72.Core.Caching;
using Saturn72.Core.Services.Events;
using Saturn72.Core.Services.Logging;
using Swashbuckle.AspNetCore.Swagger;
using Saturn72.Core.Services.Caching;
using Saturn72.Core.Configuration;
using Saturn72.Core.Services.Configuration;

namespace LabManager.WebService
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
            services.AddMemoryCache();
            services.AddCors();

            services.AddMvc();
            services.AddSwaggerGen(c => c.SwaggerDoc("v1", new Info { Title = "My API", Version = "v1" }));

            RegisterDependencies(services);
        }

        private void RegisterDependencies(IServiceCollection services)
        {
            //Core components
            services.AddSingleton<IConfigManager, ConfigManager>();

            //Audity
            var auditHelper = new AuditHelper(new LabManagerWorkContext());
            services.AddSingleton<AuditHelper>(auditHelper);

            //register core components
            services.AddSingleton<ILogger, DbLogger>();
            services.AddSingleton<ICacheManager, MemoryCacheManager>();
            services.AddScoped<ILogRecordRepository, LogRecordRepository>();

            services.AddScoped<IEventPublisher, EventPublisher>();
            services.AddScoped<ISubscriptionService, SubscriptionService>();

            services.AddScoped<IResourceService, ResourceService>();
            services.AddScoped<IResourceRepository, ResourceRepository>();

            services.AddScoped<IInstanceService, InstanceService>();
            services.AddScoped<IResourceCommander, LvpResourceCommander>();
            services.AddScoped<IRuntimeManager, RuntimeManager>();

            services.AddSingleton<IDbAdapter>(sp =>
            {
                var configManager = sp.GetService<IConfigManager>();
                var liteDbConfig = configManager.GetConfig<LiteDbConfig>();
                var dbName = liteDbConfig.DbName;
                LiteDbMapper.Map(dbName);
                var dbAdapter = new LiteDbAdapter(dbName);
                return dbAdapter;
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseSwagger();
            app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Lab Manager Api"));
            app.UseMvc();
        }
    }
}
