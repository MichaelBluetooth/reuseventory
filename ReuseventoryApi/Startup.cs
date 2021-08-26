using System;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using ReuseventoryApi.Authentication;
using ReuseventoryApi.Models;
using ReuseventoryApi.Permissions;
using ReuseventoryApi.Services.CurrentUser;
using ReuseventoryApi.Services.Listings;
using ReuseventoryApi.Services.SeedData;

namespace ReuseventoryApi
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        private static bool IsDevelopment => Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Development";

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            if (IsDevelopment)
            {
                services.AddDbContext<ReuseventoryDbContext>(opt => opt.UseInMemoryDatabase("ReuseventoryDbContext"));
            }
            else
            {
                services.AddDbContext<ReuseventoryDbContext>(opt =>
                            {
                                string connectionUrl = Environment.GetEnvironmentVariable("DATABASE_URL");
                                Uri databaseUri = new Uri(connectionUrl);
                                string db = databaseUri.LocalPath.TrimStart('/');
                                string[] userInfo = databaseUri.UserInfo.Split(':', StringSplitOptions.RemoveEmptyEntries);
                                string connectionString = $"User ID={userInfo[0]};Password={userInfo[1]};Host={databaseUri.Host};Port={databaseUri.Port};Database={db};Pooling=true;SSL Mode=Require;Trust Server Certificate=True";
                                opt.UseNpgsql(connectionString);
                            });
            }

            services.AddControllers()
                .AddNewtonsoftJson(options =>
                    options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore
                );

            services.AddCors();

            JwtTokenConfig jwtTokenConfig;
            if (IsDevelopment)
            {
                jwtTokenConfig = Configuration.GetSection("jwtTokenConfig").Get<JwtTokenConfig>();
            }
            else
            {
                jwtTokenConfig = new JwtTokenConfig()
                {
                    Secret = Environment.GetEnvironmentVariable("SECRET"),
                    Issuer = Environment.GetEnvironmentVariable("ISSUER"),
                    Audience = Environment.GetEnvironmentVariable("AUDIENCE"),
                    AccessTokenExpiration = int.Parse(Environment.GetEnvironmentVariable("ACCESS_TOKEN_EXPIRATION")),
                    RefreshTokenExpiration = int.Parse(Environment.GetEnvironmentVariable("REFRESH_TOKEN_EXPIRATION")),
                };
            }
            services.AddSingleton(jwtTokenConfig);
            services.AddAuthentication(x =>
            {
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(x =>
            {
                x.RequireHttpsMetadata = true;
                x.SaveToken = true;
                x.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidIssuer = jwtTokenConfig.Issuer,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(jwtTokenConfig.Secret)),
                    ValidAudience = jwtTokenConfig.Audience,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.FromMinutes(1)
                };
            });
            services.AddAuthorization(o =>
            {
                o.AddPolicy("IsOwnerOrAdmin", policy =>
                {
                    policy.AddRequirements(new IsOwnerOrAdminRequirement());
                });
            });

            services.AddTransient<IStartupFilter, SeedDataStartupFilter>();

            services.AddAutoMapper(typeof(Startup));

            services.AddHttpContextAccessor();
            services.AddSingleton<IJwtAuthManager, JwtAuthManager>();
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            
            services.AddTransient<IAuthorizationHandler, IsOwnerOrAdminHandler>();

            services.AddHostedService<JwtRefreshTokenCache>();
            services.AddScoped<ICurrentUserService, CurrentUserService>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IListingsService, ListingsService>();

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "ReuseventoryApi", Version = "v1" });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ReuseventoryDbContext dataContext)
        {
            if (false == IsDevelopment)
            {
                dataContext.Database.Migrate();
            }

            if (env.IsDevelopment())
            {
                //app.UseDeveloperExceptionPage();
                app.UseExceptionHandler("/error-local-development");
            }
            else
            {
                app.UseExceptionHandler("/error");
            }

            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "ReuseventoryApi v1");
                c.SupportedSubmitMethods(new Swashbuckle.AspNetCore.SwaggerUI.SubmitMethod[] { });
            });

            app.UseCors(
                options => options
                    .AllowAnyHeader()
                    .WithMethods("OPTIONS", "GET", "POST")
                    .AllowAnyOrigin()
            );

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
