using FoundationKit.Extensions;
using InventorySystem.Identity.Application.Services;
using InventorySystem.Identity.Domain.Models;
using InventorySystem.Identity.Domain.Options;
using InventorySystem.Identity.Infrastructure.EF.Persistence;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
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
                .AddFoundationKitIdentity<User, ApplicationDbContext>()
                .ConfigureOptions(configuration)
                .ConfigureServices();

            return services;
        }

        private IServiceCollection ConfigureServices()
        {
            services.AddScoped<IUserService, UserService>();

            return services;
        }

        private IServiceCollection ConfigureOptions(IConfiguration configuration)
        {
            services.Configure<SeedOptions>(configuration.GetSection(SeedOptions.Seed));

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
            var corsOptions = new CorsConfigOptions();
            configuration.GetSection(CorsConfigOptions.Cors).Bind(corsOptions);

            var originsAllowed = corsOptions.OriginsAllowed ?? new List<string> { "*" };
            var methodsAllowed = corsOptions.MethodsAllowed ?? new List<string> { "*" };

            var corsPolicy = new CorsPolicyBuilder()
                .WithOrigins(string.Join(",", originsAllowed))
                .AllowAnyHeader()
                .WithMethods(string.Join(",", methodsAllowed))
                .Build();

            services.AddCors(c => c.AddDefaultPolicy(corsPolicy));

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

                config.AddSecurityRequirement(doc => new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecuritySchemeReference("Bearer", doc),
                        new List<string>()
                    }
                });
            });

            return services;
        }

    }
    
    extension(WebApplication app)
    {
        public void UseInfrastructure()
        {
            app.ExecuteSeeds();
            app.UseHttpsRedirection();

            app.UseCors();

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }

        private void ExecuteSeeds()
        {
            try
            {
                using var scope = app.Services.CreateScope();
                scope.ServiceProvider.GetRequiredService<IUserService>().SeedAsync().GetAwaiter().GetResult();
            }
            catch (Exception e)
            {
                app.Logger.LogError(e, "Error while executing seeds");
            }
        }
    }

}
