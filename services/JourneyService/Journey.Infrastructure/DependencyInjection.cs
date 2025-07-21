using System.Security.Claims;
using System.Text;
using System.Threading.RateLimiting;
using Journey.Application.Abstractions.DbContext;
using Journey.Domain.Configuration;
using Journey.Domain.Users;
using Journey.Infrastructure.Data.Interceptors;
using Journey.Infrastructure.Extensions;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Journey.Domain.Abstractions;
using Journey.Domain.Abstractions.Interface;
using Journey.Domain.Email;
using Journey.Domain.Identity.Interface;
using Journey.Domain.Journeys.Interface;
using Journey.Domain.OutboxMessages.Interface;
using Journey.Infrastructure.Data.Repositories;
using Journey.Infrastructure.Email;
using Journey.Infrastructure.Identity;
using Journey.Infrastructure.Messaging;
using Microsoft.AspNetCore.Identity.UI.Services;
using Quartz;

namespace Journey.Infrastructure;

public static class DependencyInjection    
{
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("JournyDb") ??
                throw new ArgumentNullException(nameof(configuration));

            services.AddScoped<ISaveChangesInterceptor, AuditableEntityInterceptor>();
            services.AddScoped<ISaveChangesInterceptor, DispatchDomainEventsInterceptor>();

            services.AddDbContext<ApplicationDbContext>((sp, options) =>
            {
                options.UseNpgsql(connectionString);
                options.AddInterceptors(sp.GetServices<ISaveChangesInterceptor>());
            });
            services.AddCustomAuthorization(configuration);
            
            services.Configure<RabbitMQOptions>(configuration.GetSection("RabbitMQ"));
            
            services.AddScoped<IApplicationDbContext>(provider => provider.GetRequiredService<ApplicationDbContext>());
            services.AddSingleton(TimeProvider.System);
            
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IJourneyRepository, JourneyRepository>();
            services.AddScoped<IOutboxMessageRepository, OutboxMessageRepository>();
            services.AddScoped<IEmailService, EmailService>();
            services.AddScoped<IEmailSender, EmailSender>();
            services.AddScoped<IIdentityService, IdentityService>();
            
            services.AddRateLimiter(options =>
            {
                options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(context =>
                {
                    var ip = context.Connection.RemoteIpAddress?.ToString() ?? "unknown";

                    return RateLimitPartition.Get(ip, key =>
                        new TokenBucketRateLimiter(
                            new TokenBucketRateLimiterOptions
                            {
                                TokenLimit = 5,
                                TokensPerPeriod = 5,
                                ReplenishmentPeriod = TimeSpan.FromMinutes(1),
                                QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
                                QueueLimit = 0
                            }));
                });

                options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
            });

            services.AddHealthChecksUI(setup =>
            {
                setup.AddHealthCheckEndpoint("Basic Health", "/readyz");
            }).AddInMemoryStorage();
            
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
                    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                })
                .AddJwtBearer(options =>
                {
                    options.RequireHttpsMetadata = false; 
                    options.SaveToken = true;
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(jwtSecret)),
                        ValidateIssuer = false,
                        ValidateAudience = false,
                        RequireExpirationTime = true,
                        ValidateLifetime = true,
                        ClockSkew = TimeSpan.Zero,

                        NameClaimType = ClaimTypes.NameIdentifier,      
                        RoleClaimType = ClaimTypes.Role 
                    };
                });

            services
                .AddIdentity<User, Role> (options =>
                {
                    options.SignIn.RequireConfirmedEmail = true;
                    options.Tokens.AuthenticatorTokenProvider = TokenOptions.DefaultAuthenticatorProvider;
                    options.Password.RequiredLength = 8;
                })
                .AddDefaultTokenProviders()          
                .AddEntityFrameworkStores<ApplicationDbContext>();
            
            services
                .Configure<JwtConfiguration>(configuration.GetSection(nameof(JwtConfiguration)));

            
            services
                .Configure<JwtConfiguration>(configuration.GetSection(nameof(JwtConfiguration)));
            
        }
}
