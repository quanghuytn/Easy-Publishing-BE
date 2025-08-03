using app.Interface;
using app.Middleswares;
using app.Repository;
using app.Service;
using app.Service.Caching;
using EP.Application.Settings;
using EP.Infrastructure;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.OData;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using OfficeOpenXml;
using StackExchange.Redis;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);
var config = builder.Configuration;

ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

builder.Services.AddHttpContextAccessor();
// db
builder.Services.AddDbContext<app.Models.EasyPublishingContext>
    (option => option.UseSqlServer(builder.Configuration.GetConnectionString("MyCnn")));
builder.Services.AddDbContext<EP.Infrastructure.Data.Context>
    (option => option.UseSqlServer(builder.Configuration.GetConnectionString("MyCnn")));
builder.Services.AddScoped<app.Models.EasyPublishingContext>();
builder.Services.AddScoped<EP.Infrastructure.Data.Context>();

// Add services to the container.

builder.Services.AddLogging(logging =>
{
    logging.AddConsole();
    logging.AddDebug();
});

builder.Services.AddControllers()
    .AddOData(option =>
    option.Select().Filter().Count().OrderBy().Expand().SetMaxTop(100));

builder.Services.AddControllers()
        .AddJsonOptions(options =>
        {
            options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
        });
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials()
              .SetIsOriginAllowed(origin => true);
    });
});

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidIssuer = config["JWTConfig:Issuer"],
        ValidAudience = config["JWTConfig:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["JWTConfig:Key"]!)),
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true
    };

    options.Events = new JwtBearerEvents
    {
        OnChallenge = async context =>
        {
            context.HandleResponse();
            if(context.Response.StatusCode != StatusCodes.Status401Unauthorized)
            {
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            }

            var result = new
            {
                EC = -1,
                EM = "Yêu cầu đăng nhập"
            };

            var jsonOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase // Áp dụng camelCase
            };

            await context.Response.WriteAsync(System.Text.Json.JsonSerializer.Serialize(result, jsonOptions));
        },
        OnAuthenticationFailed = async context =>
        {
            //var endpoint = context.HttpContext.GetEndpoint();
            //if (endpoint?.Metadata?.GetMetadata<IAllowAnonymous>() != null)
            //{
            //    // Bỏ qua lỗi nếu endpoint có [AllowAnonymous]
            //    await Task.CompletedTask;
            //}
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            context.Response.ContentType = "application/json";

            var result = new
            {
                EC = -1,
                EM = "Yêu cầu đăng nhập"
            };

            var jsonOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase // Áp dụng camelCase
            };

            await context.Response.WriteAsync(System.Text.Json.JsonSerializer.Serialize(result, jsonOptions));
        }
    };
});

builder.Services.AddControllersWithViews();

var redisConfig = new ConfigurationOptions
{
    EndPoints = { builder.Configuration["Redis:EndPoint"] },
    Password = builder.Configuration["Redis:Password"],
    ConnectTimeout = 5000,
    SyncTimeout = 5000,
    AsyncTimeout = 5000
};

builder.Services.AddStackExchangeRedisCache(options =>
{
    options.ConfigurationOptions = redisConfig;
    options.InstanceName = builder.Configuration["Redis:InstanceName"];
});

builder.Services.AddSingleton<IConnectionMultiplexer>(provider =>
    ConnectionMultiplexer.Connect(redisConfig));

builder.Services.AddScoped<IRedisCacheService, RedisCacheService>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IShelvesRepository, ShelvesRepository>();
builder.Services.AddScoped<IHashService, HashService>();
builder.Services.AddAuthorization();

builder.Services.Configure<JwtSetting>(builder.Configuration.GetSection("JWTConfig"));
builder.Services.Configure<MailSetting>(builder.Configuration.GetSection("MailConfig"));

builder.Services.AddInfrastructureService();


var app = builder.Build();

app.UseDefaultFiles();
app.UseStaticFiles();
app.UseCors("AllowAll");

// Configure the HTTP request pipeline.
//if (app.Environment.IsDevelopment())
//{
app.UseSwagger();
app.UseSwaggerUI();
//}

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.UseCustomExceptionHandler();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Admin}/{action=Dashboard}/{id?}");

app.MapControllers();

app.Run();
