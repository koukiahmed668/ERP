
using ERP.microservices.inventory.interfaces;
using ERP.microservices.inventory.repositories;
using ERP.microservices.inventory.services;
using ERP.infrastructure.data;
using System;
using Microsoft.EntityFrameworkCore;
using ERP.api.inventory.dtos;
using MySql.EntityFrameworkCore;
using ERP.infrastructure.cache;
using StackExchange.Redis;
using System.Text.Json.Serialization;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using ERP.microservices.user.interfaces;
using ERP.microservices.user.services;
using ERP.infrastructure.RateLimit;
using ERP.infrastructure.Mail;

namespace ERP
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
            var key = Encoding.ASCII.GetBytes(builder.Configuration["Jwt:Secret"]);

            // Add AutoMapper
            builder.Services.AddAutoMapper(typeof(MappingProfile));

            // Add DbContext and repository
            builder.Services.AddDbContext<AppDbContext>(options =>
                options.UseMySQL(connectionString)); // No need for MySqlServerVersion


            // Register the repository and service
            builder.Services.AddScoped<IInventoryRepository, InventoryRepository>();
            builder.Services.AddScoped<IInventoryService, InventoryService>();

            builder.Services.AddScoped<IUserService, UserService>();
            builder.Services.AddScoped<IUserRepository, UserRepository>();

            builder.Services.AddScoped<IRateLimitService, RateLimitService>();

            builder.Services.AddSingleton<IEmailService, EmailService>();


            // Add Redis configuration
            var redisConnectionString = builder.Configuration.GetSection("Redis:ConnectionString").Value;
            builder.Services.AddSingleton<IConnectionMultiplexer>(ConnectionMultiplexer.Connect(redisConnectionString));
            builder.Services.AddScoped<ICacheService, RedisCacheService>();

            // Add other services (JWT, Redis, Kafka, etc.)

            builder.Services.AddAuthentication(options =>
            {
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
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateAudience = false
                };
            });

            // Add services to the container.
            builder.Services.AddControllers()
                .AddJsonOptions(options =>
                {
                    options.JsonSerializerOptions.Converters.Add(new System.Text.Json.Serialization.JsonStringEnumConverter());
                });


            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();
            app.UseAuthentication();

            app.MapControllers();

            app.Run();
        }
    }
}
