using AutoMapper;
using FluentValidation;
using FluentValidation.AspNetCore;
using MicroElements.Swashbuckle.FluentValidation;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using $safeprojectname$.Domain.Contracts;
using $safeprojectname$.Domain.Infra.OracleObjects;
using $safeprojectname$.Domain.Managers;
using Swashbuckle.AspNetCore.Swagger;
using Swashbuckle.AspNetCore.SwaggerUI;
using System;
using System.IO;
using System.Linq;
using System.Reflection;

namespace $safeprojectname$
{
    /// <summary>
    /// 
    /// </summary>
    public class Startup
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="configuration"></param>
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        /// <summary>
        /// 
        /// </summary>
        public IConfiguration Configuration { get; }

        /// <summary>
        /// This method gets called by the runtime. Use this method to add services to the container. 
        /// </summary>
        /// <param name="services"></param>
        public void ConfigureServices(IServiceCollection services)
        {
            #region [ Automapper ]
            //Register Automapper
            services.AddAutoMapper(typeof(Helpers.MappingProfile));
            #endregion

            #region [ OracleConfiguration ] 
            var appSettingSection = Configuration.GetSection("OracleSettings");
            services.Configure<OracleSettings>(appSettingSection);
            #endregion

            #region [ FluentValidation ]
            services
                .AddMvc()
                .AddFluentValidation(c =>
                {
                    c.RegisterValidatorsFromAssemblyContaining<Startup>();
                    c.ValidatorFactoryType = typeof(HttpContextServiceProviderValidatorFactory);
                });

            // Validators
            //services.AddSingleton<IValidator<PersonDTO>, PersonDTOValidator>();

            // Register all validators as IValidator?
            var serviceDescriptors = services.Where(descriptor => descriptor.ServiceType.GetInterfaces().Contains(typeof(IValidator))).ToList();
            serviceDescriptors.ForEach(descriptor => services.Add(ServiceDescriptor.Transient(typeof(IValidator), descriptor.ImplementationType)));

            // override modelstate
            services.Configure<ApiBehaviorOptions>(options =>
            {
                options.InvalidModelStateResponseFactory = (context) =>
                {
                    var errors = context.ModelState.Values.SelectMany(x => x.Errors.Select(p => p.ErrorMessage)).ToList();
                    var result = new
                    {
                        Message = "Validation input errors",
                        Errors = errors
                    };
                    return new BadRequestObjectResult(result);
                };
            });
            #endregion

            #region [ Configurações do Swagger ]

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    //Version = "v1",
                    Title = "Nome de sua API",
                    Description = "Insira aqui a descrição sucinta de sua API. O Controller PERSON é um modelo, deve ser utilizado como exemplo básico em suas implementações, delete o controller e suas classes derivadas após sua implementação e antes de subir para ambiente de produção."
                });

                // Set the comments path for the Swagger JSON and UI.
                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                c.IncludeXmlComments(xmlPath);
                c.AddFluentValidationRules();
            });

            #endregion

            #region [ Register Services ]
            // Managers
            services.AddTransient<IPersonManager, PersonManager>();
            #endregion
        }

        /// <summary>
        /// This method gets called by the runtime. Use this method to configure the HTTP request pipeline. 
        /// </summary>
        /// <param name="app"></param>
        /// <param name="env"></param>
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            app.UseStaticFiles();
            app.UseRouting();
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "ASP.NET Core Template API V1");
                c.RoutePrefix = string.Empty;
                c.IndexStream = () => GetType().GetTypeInfo().Assembly.GetManifestResourceStream("$safeprojectname$.swagger.index.html");
                c.InjectStylesheet("swagger-ui/custom.css");
                c.DocumentTitle = "Your Project Name";
                c.DocExpansion(DocExpansion.List);
            });

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
