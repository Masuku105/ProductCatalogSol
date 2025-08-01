using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using ProductCatalogRepo;
using AutoMapper;
using ProductCatalogService;

namespace ProductCatalogAPI
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
            services.AddControllers();
            services.AddAutoMapper(cfg =>
            {
                cfg.AddProfile(new ProductMappingProfile());
            });
            services.AddScoped<IProductCatalogService, ProductCatalogService.ProductCatalogService>();

            services.Configure<FakeStoreApiSettings>(Configuration.GetSection("ApiSettings"));

            var apiSettings = Configuration.GetSection("ApiSettings").Get<FakeStoreApiSettings>();

            services.AddHttpClient<IProductRepo, ProductRepo>(client =>
            {
                client.BaseAddress = new Uri(apiSettings.ProductApiBaseUrl);
            });
            services.AddStackExchangeRedisCache(options =>
            {
                options.Configuration = Configuration.GetConnectionString("Redis");
            });
            services.AddSwaggerGen();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
