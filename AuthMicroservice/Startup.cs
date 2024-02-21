using AuthMicroservice.Authentication;
using AuthMicroservice.Controllers;
using AuthMicroservice.Database;
using AuthMicroservice.MessageBroker.RabbitMq;
using AuthMicroservice.MessageBroker.RabbitMq.Interfaces;
using AuthMicroservice.Repositories;
using AuthMicroservice.Repositories.Interfaces;
using AuthMicroservice.Services;
using AuthMicroservice.Services.IServices;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Ocelot.Configuration;
using Ocelot.DependencyInjection;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
namespace AuthMicroservice
{
    public static class Startup
    {

        public static void ConfigureServices(this IServiceCollection services, IConfiguration Configuration)
        {
          
            services.AddScoped<IJsonWebTokenService <JwtSecurityToken, IdentityResult>, JsonWebTokenService> ();
            services.AddControllers();

            // For Entity Framework
            services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(Configuration.GetConnectionString("ConnStr")));

            // For Identity
            services.AddIdentity<ApplicationUser, IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();

            // Adding Authentication
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            })

            // Adding Jwt Bearer
            .AddJwtBearer(options =>
            {
                options.SaveToken = true;
                options.RequireHttpsMetadata = false;
                options.TokenValidationParameters = new TokenValidationParameters()
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidAudience = Configuration["JWT:ValidAudience"],
                    ValidIssuer = Configuration["JWT:ValidIssuer"],
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["JWT:Secret"]))
                };
            });

            // Add Swagger
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Your API", Version = "v1" });
                
                // Add JWT Authentication in Swagger
                var securityScheme = new OpenApiSecurityScheme
                {
                    Name = "JWT Authentication",
                    Description = "Enter your JWT token",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.Http,
                    Scheme = "bearer", // must be lower case
                    BearerFormat = "JWT",
                    Reference = new OpenApiReference
                    {
                        Id = JwtBearerDefaults.AuthenticationScheme,
                        Type = ReferenceType.SecurityScheme
                    }
                };
                c.AddSecurityDefinition(JwtBearerDefaults.AuthenticationScheme, securityScheme);
                c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    { securityScheme, Array.Empty<string>() }
                });
            });

           services.AddOcelot(Configuration);
            services.AddSingleton<IBus>(sp =>
            {
                var logger = sp.GetRequiredService<ILogger<IBus>>();
                return RabbitHutch.CreateBus("localhost", logger);
            });
            services.AddHostedService<Worker>();
            services.AddScoped<IUserRespository, UserRepository>();
            services.AddScoped<UserService>();
          
        }
    }
}
