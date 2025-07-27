using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using app.Models;
using Microsoft.AspNetCore.OData;
using Microsoft.EntityFrameworkCore;
using System.Text;
using System.Text.Json.Serialization;
using OfficeOpenXml;
using app.Service.Caching;
using StackExchange.Redis;
using Microsoft.AspNetCore.Mvc;
using app.Interface;
using app.Repository;
using app.Service;
using System.Text.Json;
using app.Middlewares;

var builder = WebApplication.CreateBuilder(args);
var config = builder.Configuration;

ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

builder.Services.AddHttpContextAccessor();
// db
builder.Services.AddDbContext<EasyPublishingContext>
    (option => option.UseSqlServer(builder.Configuration.GetConnectionString("MyCnn")));
builder.Services.AddScoped<EasyPublishingContext>();

// Add services to the container.

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
builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
builder.Services.AddScoped<IAuthorRepository, AuthorRepository>();
builder.Services.AddScoped<ICommentRepository, CommentRepository>();
builder.Services.AddScoped<IChapterRepository, ChapterRepository>();
builder.Services.AddScoped<IReportRepository, ReportRepository>();
builder.Services.AddScoped<IReviewRepository, ReviewRepository>();
builder.Services.AddScoped<IStoryRepository, StoryRepository>();
builder.Services.AddScoped<IShelvesRepository, ShelvesRepository>();
builder.Services.AddScoped<IInteractionRepository, InteractionRepository>();
builder.Services.AddScoped<IHashService, HashService>();
builder.Services.AddAuthorization();

var app = builder.Build();

// Configure the HTTP request pipeline.
//if (app.Environment.IsDevelopment())
//{
app.UseSwagger();
app.UseSwaggerUI();
//}

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Admin}/{action=Dashboard}/{id?}");

app.UseDefaultFiles();
app.UseStaticFiles();
app.UseCors("AllowAll");

app.UseAuthentication();
app.UseAuthorization();
app.UseCustomExceptionHandler();

app.MapControllers();

app.Run();
