using System.Security.Claims;
using System.Text;
using CommerceFlow.Application.Authentication;
using CommerceFlow.Application.Products;
using CommerceFlow.Infrastructure.Customers;
using CommerceFlow.Infrastructure.Persistence;
using CommerceFlow.Infrastructure.Products;
using CommerceFlow.Infrastructure.Security;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using CommerceFlow.Application.Carts;
using CommerceFlow.Infrastructure.Carts;
using CommerceFlow.Application.Orders;
using CommerceFlow.Infrastructure.Orders;
using CommerceFlow.Application.Inventory;
using CommerceFlow.Infrastructure.Inventory;
using CommerceFlow.Application.Payments;
using CommerceFlow.Infrastructure.Payments;
using CommerceFlow.Application.Messaging;
using CommerceFlow.Infrastructure.Messaging;

namespace CommerceFlow.Api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

          

            builder.Services.AddControllers();
            builder.Services.AddOpenApi();

            builder.Services.AddDbContext<CommerceFlowDbContext>(
                options =>
                    options.UseSqlServer(
                        builder.Configuration.GetConnectionString(
                            "CommerceFlowDatabase")));

            builder.Services.AddStackExchangeRedisCache(options =>
            {
                options.Configuration =
                    builder.Configuration["Redis:Configuration"];

                options.InstanceName =
                    builder.Configuration["Redis:InstanceName"];
            });

            builder.Services.AddScoped<
    ICartRepository,
    RedisCartRepository>();

            builder.Services.AddScoped<
                ICartService,
                CartService>();
            builder.Services.Configure<JwtOptions>(
                builder.Configuration.GetSection(
                    JwtOptions.SectionName));

            var jwtOptions = builder.Configuration
                .GetSection(JwtOptions.SectionName)
                .Get<JwtOptions>()
                ?? throw new InvalidOperationException(
                    "JWT configuration is missing.");

            if (string.IsNullOrWhiteSpace(jwtOptions.Key))
                throw new InvalidOperationException(
                    "JWT signing key is missing.");

            var signingKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(jwtOptions.Key));

            builder.Services
                .AddAuthentication(
                    JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters =
                        new TokenValidationParameters
                        {
                            ValidateIssuer = true,
                            ValidIssuer = jwtOptions.Issuer,

                            ValidateAudience = true,
                            ValidAudience = jwtOptions.Audience,

                            ValidateIssuerSigningKey = true,
                            IssuerSigningKey = signingKey,

                            ValidateLifetime = true,

                            ClockSkew = TimeSpan.FromMinutes(1),

                            NameClaimType =
                                ClaimTypes.NameIdentifier,

                            RoleClaimType =
                                ClaimTypes.Role
                        };
                });

            builder.Services.AddAuthorization();

            builder.Services.AddScoped<
                IProductService,
                ProductService>();

            builder.Services.AddScoped<
                IProductRepository,
                ProductRepository>();

            builder.Services.AddScoped<
                ICustomerRepository,
                CustomerRepository>();

            builder.Services.AddScoped<
                IPasswordService,
                PasswordService>();

            builder.Services.AddScoped<
                ITokenService,
                JwtTokenService>();

            builder.Services.AddScoped<
                IAuthService,
                AuthService>();

            builder.Services.AddScoped<
                ICartRepository,
                RedisCartRepository>();

            builder.Services.AddScoped<
                ICartService,
                CartService>();

            builder.Services.AddScoped<
                IOrderRepository,
                OrderRepository>();

            builder.Services.AddScoped<
                IOrderService,
                OrderService>();

            builder.Services.AddScoped<
    IInventoryRepository,
    InventoryRepository>();

            builder.Services.AddScoped<
                IInventoryService,
                InventoryService>();

            builder.Services.AddScoped<
    IPaymentRepository,
    PaymentRepository>();

            builder.Services.AddScoped<
                IPaymentService,
                PaymentService>();

            builder.Services.AddScoped<
                IPaymentGateway,
                SimulatedPaymentGateway>();

            builder.Services.Configure<RabbitMqOptions>(
    builder.Configuration.GetSection(
        RabbitMqOptions.SectionName));

            builder.Services.AddSingleton<
    IMessagePublisher,
    RabbitMqMessagePublisher>();

           

            builder.Services.AddHostedService<
                PaymentAnalyticsConsumer>();

            var app = builder.Build();

            using (var scope = app.Services.CreateScope())
            {
                var dbContext =
                    scope.ServiceProvider
                        .GetRequiredService<CommerceFlowDbContext>();

                dbContext.Database.Migrate();
            }

            if (app.Environment.IsDevelopment())
            {
                app.MapOpenApi();
            }

            app.UseHttpsRedirection();

            app.UseAuthentication();

            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}