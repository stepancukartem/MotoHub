
using AutoMapper;
using FluentValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using MotoHub.DAL;
using MotoHub.DAL.Repositories;
using MotoHub.DAL.Repositories.Interfaces;
using MotoHub.DTOs;
using MotoHub.Jobs;
using MotoHub.Middleware;
using MotoHub.Services;
using MotoHub.Services.Mapping;
using MotoHub.Validators;
using Quartz;
using Scalar.AspNetCore;
using Serilog;
using System.Security.Claims;
using System.Text;

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .WriteTo.Console()
    .WriteTo.File(
        "Logs/motohub-.log",
        rollingInterval: RollingInterval.Day,
        retainedFileCountLimit: 7)
    .CreateLogger();

try
{
    var builder = WebApplication.CreateBuilder(args);

    builder.Host.UseSerilog();

    builder.Services.AddControllers();
    builder.Services.AddOpenApi();

    // PostgreSQL
    builder.Services.AddDbContext<AppDbContext>(options =>
    {
        var connectionString =
            builder.Configuration.GetConnectionString("DefaultConnection");

        options.UseNpgsql(connectionString);
    });

    // Services
    builder.Services.AddScoped<AuthService>();
    builder.Services.AddScoped<MotorcycleService>();
    builder.Services.AddScoped<BrandService>();
    builder.Services.AddScoped<CategoryService>();
    builder.Services.AddScoped<OrderService>();
    builder.Services.AddScoped<ReviewService>();
    builder.Services.AddScoped<MotoServiceService>();

    // Repository
    builder.Services.AddScoped<
        IMotorcycleRepository,
        MotorcycleRepository>();

    // AutoMapper
    builder.Services.AddAutoMapper(
    cfg => { },
    typeof(MotoHubMappingProfile).Assembly);

    // FluentValidation
    builder.Services.AddScoped<
        IValidator<CreateMotorcycleDto>,
        CreateMotorcycleValidator>();

    builder.Services.AddScoped<
        IValidator<CreateBrandDto>,
        CreateBrandValidator>();

    builder.Services.AddScoped<
        IValidator<CreateCategoryDto>,
        CreateCategoryValidator>();

    // JWT
    var jwtKey = builder.Configuration["Jwt:Key"];

    if (string.IsNullOrWhiteSpace(jwtKey))
    {
        throw new InvalidOperationException(
            "У конфігурації відсутній Jwt:Key.");
    }

    builder.Services
        .AddAuthentication(
            JwtBearerDefaults.AuthenticationScheme)
        .AddJwtBearer(options =>
        {
            options.TokenValidationParameters =
                new TokenValidationParameters
                {
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,

                    IssuerSigningKey = new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(jwtKey)),

                    RoleClaimType = ClaimTypes.Role
                };
        });

    builder.Services.AddAuthorization();


    // Quartz: фонове завдання
    builder.Services.AddQuartz(options =>
    {
        var jobKey = new JobKey("DailyLogJob");

        options.AddJob<DailyLogJob>(
            job => job.WithIdentity(jobKey));

        options.AddTrigger(trigger => trigger
            .ForJob(jobKey)
            .WithIdentity("DailyLogJob-trigger")
            .WithSimpleSchedule(schedule => schedule
                .WithInterval(TimeSpan.FromHours(24))
                .RepeatForever()));
    });

    builder.Services.AddQuartzHostedService(options =>
    {
        options.WaitForJobsToComplete = true;
    });


    var app = builder.Build();

    // Початкові дані
    using (var scope = app.Services.CreateScope())
    {
        var dbContext =
            scope.ServiceProvider.GetRequiredService<AppDbContext>();

        await DbSeeder.SeedAsync(dbContext);
    }

    if (app.Environment.IsDevelopment())
    {
        app.MapOpenApi();
        app.MapScalarApiReference();
    }

    app.UseMiddleware<RequestTimingMiddleware>();

    app.UseSerilogRequestLogging();

    app.UseHttpsRedirection();

    app.UseAuthentication();
    app.UseAuthorization();

    app.MapControllers();

    await app.RunAsync();
}
catch (Exception ex)
{
    Log.Fatal(ex, "MotoHub завершив роботу через помилку");
}
finally
{
    await Log.CloseAndFlushAsync();
}
