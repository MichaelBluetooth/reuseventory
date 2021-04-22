using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OData.Edm;
using Microsoft.OData.ModelBuilder;
using Microsoft.OpenApi.Models;
using ReuseventoryApi.Authentication;
using ReuseventoryApi.Models;
using ReuseventoryApi.Services.CurrentUser;

namespace ReuseventoryApi
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
            services.AddDbContext<ReuseventoryDbContext>(opt => opt.UseInMemoryDatabase("ReuseventoryDbContext"));

            services.AddControllers();
            services.AddOData(opt =>
            {
                opt.AddModel("api", GetEdmModel()).Select().SetMaxTop(100).Expand().Filter().SkipToken();
            });
            // services.AddMvc();

            services.AddCors();
            services.AddAuthorization();
            services.AddAuthentication();

            var jwtTokenConfig = Configuration.GetSection("jwtTokenConfig").Get<JwtTokenConfig>();
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

            services.AddAutoMapper(typeof(Startup));

            services.AddHttpContextAccessor();
            services.AddSingleton<IJwtAuthManager, JwtAuthManager>();
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddHostedService<JwtRefreshTokenCache>();
            services.AddScoped<ICurrentUserService, CurrentUserService>();
            services.AddScoped<IUserService, UserService>();

            // services.AddSwaggerGen(c =>
            // {
            //     c.SwaggerDoc("v1", new OpenApiInfo { Title = "ReuseventoryApi", Version = "v1" });
            // });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                // app.UseSwagger();
                // app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "ReuseventoryApi v1"));
            }

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

        private IEdmModel GetEdmModel()
        {
            ODataConventionModelBuilder builder = new ODataConventionModelBuilder();
            builder.EntitySet<User>("Users");
            builder.EntitySet<Listing>("Listings");

            var registerAction = builder.EntityType<User>().Collection.Action("Register");
            registerAction.Parameter<string>("username");
            registerAction.Parameter<string>("password");
            registerAction.ReturnsFromEntitySet<User>("Users");

            var loginAction = builder.EntityType<User>().Collection.Action("Login");
            loginAction.Parameter<string>("username");
            loginAction.Parameter<string>("password");
            loginAction.Returns<LoginResult>();

            builder.EntityType<User>().Collection.Action("Logout");

            var refreshTokenAction = builder.EntityType<User>().Collection.Action("RefreshToken");
            refreshTokenAction.Parameter<string>("refreshToken");
            refreshTokenAction.Returns<LoginResult>();

            // var containerReceiveAction = builder.EntityType<Container>().Collection.Action("Receive");
            // containerReceiveAction.CollectionParameter<ContainerReceipt>("containers");
            // containerReceiveAction.ReturnsCollectionFromEntitySet<Container>("containers");

            return builder.GetEdmModel();
        }
    }
}