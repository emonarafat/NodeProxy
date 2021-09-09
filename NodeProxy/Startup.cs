using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;

using System;
using FluentValidation.AspNetCore;
using MediatR;
using Newtonsoft.Json.Serialization;
using FluentValidation;
using Microsoft.AspNetCore.Identity;
using NodeProxy.Features.Validator;
using NodeProxy.Extensions;
using Swashbuckle.AspNetCore.SwaggerUI;

namespace NodeProxy
{
    public class Startup
    {
        private const string CorsPolicy = "CorsPolicy";

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {

            services.AddControllers()
               // .AddFluentValidation(s => s.RegisterValidatorsFromAssemblyContaining<Startup>().DisableDataAnnotationsValidation = false)
                .AddNewtonsoftJson(opt =>
            {
                opt.AllowInputFormatterExceptionMessages = true;
                opt.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
                opt.SerializerSettings.Formatting = Newtonsoft.Json.Formatting.Indented;
                opt.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
            });
            services.AddValidatorsFromAssemblyContaining<SoapCommandValidator>(ServiceLifetime.Transient);
            services.AddMediatR(typeof(Startup));
            services.AddCors(opt => opt
            .AddPolicy(CorsPolicy, builder => builder
            .AllowCredentials().AllowAnyMethod().AllowAnyHeader().SetIsOriginAllowed(_ => true).SetPreflightMaxAge(TimeSpan.FromSeconds(2520))));
            services.AddSwaggerGen(c =>
            {
              
                // c.DocumentFilter<SwaggerExcludeFilter>();
                c.OperationFilter<AuthHeaderFilter>();
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "NodeProxy", Version = "v1" });
               // c.CustomSchemaIds(f => f.AssemblyQualifiedName);
            });
            services.AddSwaggerGenNewtonsoftSupport();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseExceptionHandler(env.IsDevelopment() ? "/error-local-development" : "/error");
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.RoutePrefix = "";
                
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "NodeProxy v1");
                c.DefaultModelExpandDepth(2);
                c.DefaultModelRendering(ModelRendering.Model);
               // c.DefaultModelsExpandDepth(-1);
                c.DisplayOperationId();
                c.DisplayRequestDuration();
                c.DocExpansion(DocExpansion.List);
                c.EnableDeepLinking();
                c.EnableFilter();
                c.MaxDisplayedTags(5);
                c.ShowExtensions();
                c.ShowCommonExtensions();
                c.EnableValidator();
                c.DocumentTitle = "API Docs";
            });
            app.UseCors(CorsPolicy);
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
