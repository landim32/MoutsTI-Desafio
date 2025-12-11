using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using MoutsTI.Domain.Entities.Interfaces;
using MoutsTI.Domain.Services;
using MoutsTI.Domain.Services.Interfaces;
using MoutsTI.Infra.Context;
using MoutsTI.Infra.Interfaces.Repositories;
using MoutsTI.Infra.Mapping.Profiles;
using MoutsTI.Infra.Repositories;
using System.Text;

namespace MoutsTI.Application
{
    public class Initializer
    {
        private static void injectDependency(Type serviceType, Type implementationType, IServiceCollection services, bool scoped = true)
        {
            if (scoped)
                services.AddScoped(serviceType, implementationType);
            else
                services.AddTransient(serviceType, implementationType);
        }

        public static void Configure(IServiceCollection services, string? connection, IConfiguration configuration, bool scoped = true)
        {
            if (scoped)
            {
                services.AddDbContext<MoutsTIContext>(x =>
                {
                    x.UseLazyLoadingProxies()
                     .UseNpgsql(connection)
                     .EnableSensitiveDataLogging()
                     .EnableDetailedErrors();
                });
            }
            else
            {
                services.AddDbContextFactory<MoutsTIContext>(x =>
                {
                    x.UseLazyLoadingProxies()
                     .UseNpgsql(connection)
                     .EnableSensitiveDataLogging()
                     .EnableDetailedErrors();
                });
            }

            services.AddLogging();

            #region Infra
            injectDependency(typeof(MoutsTIContext), typeof(MoutsTIContext), services, scoped);
            #endregion

            #region Repository
            injectDependency(typeof(IEmployeeRepository<IEmployeeModel>), typeof(EmployeeRepository), services, scoped);
            injectDependency(typeof(IEmployeeRoleRepository<IEmployeeRoleModel>), typeof(EmployeeRoleRepository), services, scoped);
            #endregion

            #region AutoMapper
            services.AddAutoMapper(cfg => { }, typeof(EmployeeProfile).Assembly);
            services.AddAutoMapper(cfg => { }, typeof(EmployeeRoleProfile).Assembly);
            services.AddAutoMapper(cfg => { }, typeof(EmployeePhoneProfile).Assembly);
            #endregion

            #region Service
            injectDependency(typeof(IEmployeeService), typeof(EmployeeService), services, scoped);
            injectDependency(typeof(IEmployeeRoleService), typeof(EmployeeRoleService), services, scoped);
            injectDependency(typeof(IAuthService), typeof(AuthService), services, scoped);
            #endregion

            #region JWT Authentication
            ConfigureJwtAuthentication(services, configuration);
            #endregion
        }

        private static void ConfigureJwtAuthentication(IServiceCollection services, IConfiguration configuration)
        {
            var jwtSettings = configuration.GetSection("JwtSettings");
            var secretKey = jwtSettings["SecretKey"] ?? throw new InvalidOperationException("JWT SecretKey not configured");
            var issuer = jwtSettings["Issuer"] ?? "MoutsTI.API";
            var audience = jwtSettings["Audience"] ?? "MoutsTI.Client";

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = issuer,
                    ValidAudience = audience,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey)),
                    ClockSkew = TimeSpan.Zero
                };
            });

            services.AddAuthorization();
        }
    }
}
