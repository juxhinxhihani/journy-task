using System.Text;
using Journey.Application.Abstractions.DbContext;
using Journey.Domain.Configuration;
using Journey.Domain.Users;
using Journey.Infrastructure.Data.Interceptors;
using Journey.Infrastructure.Extensions;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Quartz;

namespace Journey.Infrastructure;

public static class DependencyInjection    
{
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("JournyDb") ??
                throw new ArgumentNullException(nameof(configuration));

            services.AddScoped<ISaveChangesInterceptor, AuditableEntityInterceptor>();

            services.AddDbContext<ApplicationDbContext>((sp, options) =>
            {
                options.UseNpgsql(connectionString);
                options.AddInterceptors(sp.GetServices<ISaveChangesInterceptor>());
            });
            services.AddCustomAuthorization(configuration);
            
            services.AddScoped<IApplicationDbContext>(provider => provider.GetRequiredService<ApplicationDbContext>());
            services.AddSingleton(TimeProvider.System);
            
            AppSettingsConfigHelper.SetConfiguration(configuration);
            
            return services;
        }
        private static void AddCustomAuthorization(this IServiceCollection services, IConfiguration configuration)
        {
            var jwtSecret = configuration
                .GetSection(nameof(JwtConfiguration))
                .GetValue<string>(nameof(JwtConfiguration.Secret));

            services.AddCors(options =>
            {
                options.AddPolicy("AllowOrigin", builder =>
                {
                    builder
                        .WithOrigins() 
                        .AllowAnyHeader()
                        .AllowAnyMethod()
                        .AllowCredentials();
                });
            });

            services
               .AddAuthentication(options =>
               {
                   options.DefaultSignInScheme = IdentityConstants.TwoFactorUserIdScheme;
                   options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                   options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
                   options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
               })        
               .AddCookie(IdentityConstants.TwoFactorUserIdScheme, o =>
               {
                   o.Cookie.HttpOnly = false;
                   o.ExpireTimeSpan = TimeSpan.FromDays(1);
                   o.Cookie.SameSite = SameSiteMode.None;
               }).AddCookie(IdentityConstants.ApplicationScheme)
               .AddJwtBearer(options =>
               {
                   options.SaveToken = true;
                   options.TokenValidationParameters = new TokenValidationParameters
                   {
                       ValidateIssuerSigningKey = true,
                       IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(jwtSecret)),
                       ValidateIssuer = false,
                       ValidateAudience = false,
                       RequireExpirationTime = true,
                       ValidateLifetime = true,
                       ClockSkew = TimeSpan.Zero
                   };
               });

            services
                .AddIdentity<User, IdentityRole<Guid>> (options =>
                {
                    options.SignIn.RequireConfirmedEmail = true;
                    options.Tokens.AuthenticatorTokenProvider = TokenOptions.DefaultAuthenticatorProvider;
                    options.Password.RequiredLength = 12;
                })
                .AddSignInManager()
                .AddRoles<IdentityRole>()
                .AddDefaultTokenProviders()          
                .AddEntityFrameworkStores<ApplicationDbContext>();
            services
                .Configure<JwtConfiguration>(configuration.GetSection(nameof(JwtConfiguration)));

            services.ConfigureApplicationCookie(o =>
            {
                o.Cookie.HttpOnly = false;
                o.ExpireTimeSpan = TimeSpan.FromDays(1);
                o.Cookie.SameSite = SameSiteMode.None;
            });

        }
}
