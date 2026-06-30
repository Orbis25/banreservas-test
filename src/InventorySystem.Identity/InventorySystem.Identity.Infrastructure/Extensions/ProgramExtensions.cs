using FoundationKit.Extensions;
using InventorySystem.Identity.Domain.Models;
using InventorySystem.Identity.Infrastructure.EF.Presistence;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi;

namespace InventorySystem.Identity.Infrastructure.Extensions;

public static class ProgramExtensions
{
    extension(IServiceCollection services)
    {
        public IServiceCollection AddInfrastructure(IConfiguration configuration, string xmlDoc)
        {
            services
                .AddSwaggerConfig(xmlDoc)
                .AddPersistence(configuration)
                .AddIdentityConfig()
                .AddCorsWithConfiguration(configuration)
                .AddFoundationKitIdentity<User, ApplicationDbContext>();

            return services;
        }

        private IServiceCollection AddPersistence(IConfiguration configuration)
        {
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseNpgsql(configuration.GetConnectionString("DefaultConnection")));

            return services;
        }
        
        private IServiceCollection AddIdentityConfig()
        {
            services.Configure<IdentityOptions>(opt =>
            {
                opt.Password.RequireDigit = false;
                opt.Password.RequireLowercase = false;
                opt.Password.RequireUppercase = false;
                opt.Password.RequireNonAlphanumeric = false;
                opt.Password.RequiredLength = 6;
                opt.User.RequireUniqueEmail = true;
                opt.Password.RequiredUniqueChars = 0;
            });

            return services;
        }

        private IServiceCollection AddCorsWithConfiguration(IConfiguration configuration)
        {
            // var corsOptions = new CorsConfigOptions();
            // configuration.GetSection(CorsConfigOptions.Cors).Bind(corsOptions);
            //
            // var originsAllowed = corsOptions.OriginsAllowed ?? new List<string> { "*" };
            // var methodsAllowed = corsOptions.MethodsAllowed ?? new List<string> { "*" };
            //
            // var corsPolicy = new CorsPolicyBuilder()
            //     .WithOrigins(string.Join(",", originsAllowed))
            //     .AllowAnyHeader()
            //     .WithMethods(string.Join(",", methodsAllowed))
            //     .Build();
            //
            // services.AddCors(c => c.AddDefaultPolicy(corsPolicy));

            return services;
        }
        
        
        private IServiceCollection AddSwaggerConfig(string xml)
        {
            services.AddEndpointsApiExplorer();


            services.AddSwaggerGen(config =>
            {
                // Set the comments path for the Swagger JSON and UI.
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xml);
                config.IncludeXmlComments(xmlPath);

                config.SwaggerDoc("v1", new OpenApiInfo
                {
                    Version = "v1.0",
                    Title = "IDENTITY API",
                });

                config.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.ApiKey,
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Description = "JWT token"
                });

                // config.AddSecurityRequirement(new Func<OpenApiDocument, OpenApiSecurityRequirement>
                // {
                //     {
                //         new OpenApiSecurityScheme
                //         {
                //             Reference = new OpenApiReference
                //             {
                //                 Type = ReferenceType.SecurityScheme,
                //                 Id = "Bearer"
                //             }
                //         },
                //         Array.Empty<string>()
                //     }
                // });
            });

            return services;
        }

    }
}
