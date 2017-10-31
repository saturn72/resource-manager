using FluentValidation.AspNetCore;
using LabManager.DbModel;
using LabManager.DbModel.Infrastructure;
using LabManager.DbModel.Repositories;
using LabManager.Services.Resources;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using QAutomation.Core.Services.Events;
using QAutomation.Core.Services.Logging;
using Swashbuckle.AspNetCore.Swagger;
using System.Reflection;

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
            services.AddMvc()
                .AddFluentValidation(fv=>fv.RegisterValidatorsFromAssembly(Assembly.GetExecutingAssembly()));
            services.AddSwaggerGen(c => c.SwaggerDoc("v1", new Info { Title = "My API", Version = "v1" }));

            RegisterDependencies(services);
        }

        private void RegisterDependencies(IServiceCollection services)
        {
            //register core components
            services.AddSingleton<ILogger, DbLogger>();
            services.AddScoped<ILogRecordRepository, LogRecordRepository>();

            services.AddScoped<IEventPublisher, EventPublisher>();
            services.AddScoped<ISubscriptionService, SubscriptionService>();

            services.AddScoped<IResourceService, ResourceService>();
            services.AddScoped<IResourceRepository, ResourceRepository>();

            const string dbName = "lab-manager.db";
            LiteDbMapper.Map(dbName);
            var liteDbAdapter = new LiteDbAdapter(dbName);
            services.AddSingleton<IDbAdapter>(liteDbAdapter);
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
