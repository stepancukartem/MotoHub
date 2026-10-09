using FluentValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using MotoHub.DAL;
using MotoHub.DAL.Repositories;
using MotoHub.DAL.Repositories.Interfaces;
using MotoHub.DTOs;
using MotoHub.Services;
using MotoHub.Services.Mapping;
using MotoHub.Validators;
using Scalar.AspNetCore;
using System.Text;
using Serilog;
using MotoHub.Middleware;
using MotoHub.Jobs;
using Quartz;

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .WriteTo.Console()
    .WriteTo.File(
        "Logs/motohub-.log",
        rollingInterval: RollingInterval.Day,
        retainedFileCountLimit: 7)
    .CreateLogger();

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddScoped<IMotorcycleRepository, MotorcycleRepository>();
builder.Services.AddControllers();
builder.Host.UseSerilog();
builder.Services.AddScoped<MotoServiceService>();
builder.Services.AddScoped<ReviewService>();
builder.Services.AddScoped<BrandService>();
builder.Services.AddScoped<CategoryService>();
builder.Services.AddScoped<OrderService>();
builder.Services.AddOpenApi();
builder.Services.AddValidatorsFromAssemblyContaining<CreateMotorcycleValidator>();
builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<MotorcycleService>();
builder.Services.AddDbContext<AppDbContext>(options =>
{
    var connectionString =
        builder.Configuration.GetConnectionString("DefaultConnection");

    options.UseNpgsql(connectionString);
});
var jwtKey = builder.Configuration["Jwt:Key"];
builder.Services.AddAutoMapper(
    typeof(MotoHubMappingProfile));

builder.Services.AddQuartz(options =>
{
    var jobKey = new JobKey("DailyLogJob");

    options.AddJob<DailyLogJob>(job =>
        job.WithIdentity(jobKey));

    options.AddTrigger(trigger =>
        trigger
            .ForJob(jobKey)
            .WithIdentity("DailyLogJob-trigger")
            .WithSimpleSchedule(schedule =>
                schedule
                    .WithIntervalInHours(24)
                    .RepeatForever()));
});

builder.Services.AddQuartzHostedService(options =>
{
    options.WaitForJobsToComplete = true;
});

builder.Services.AddAuthentication(
    JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,

            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(jwtKey!)),

            RoleClaimType = System.Security.Claims.ClaimTypes.Role
        };
    });
builder.Services.AddScoped<
    FluentValidation.IValidator<CreateBrandDto>,
    CreateBrandValidator>();

builder.Services.AddScoped<
    FluentValidation.IValidator<CreateCategoryDto>,
    CreateCategoryValidator>();
builder.Services.AddAuthorization();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
   var dbContext = scope.ServiceProvider
        .GetRequiredService<AppDbContext>();

    await DbSeeder.SeedAsync(dbContext);
}

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}
app.UseMiddleware<RequestTimingMiddleware>();
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();