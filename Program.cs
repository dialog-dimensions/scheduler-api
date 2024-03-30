using System.Globalization;
using System.Net.Http.Headers;
using System.Text;
using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Localization;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;
using SchedulerApi.DAL;
using SchedulerApi.DAL.Repositories;
using SchedulerApi.DAL.Repositories.Interfaces;
using SchedulerApi.Models.Entities.Factories;
using SchedulerApi.Services.JWT;
using SchedulerApi.Services.ScheduleEngine;
using SchedulerApi.Services.ScheduleEngine.Comparers;
using SchedulerApi.Services.ScheduleEngine.Comparers.Interfaces;
using SchedulerApi.Services.ScheduleEngine.Interfaces;
using SchedulerApi.Services.WhatsAppClient.Twilio;
using SchedulerApi.Services.Workflows.Processes.Classes;
using SchedulerApi.Services.Workflows.Processes.Factories.Classes;
using SchedulerApi.Services.Workflows.Processes.Factories.Interfaces;
using SchedulerApi.Services.Workflows.Processes.Interfaces;
using SchedulerApi.Services.Workflows.Scanners;
using SchedulerApi.Services.Workflows.Steps;
using SchedulerApi.Services.Workflows.Strategies.Classes;
using SchedulerApi.Services.Workflows.Strategies.Interfaces;

var builder = WebApplication.CreateBuilder(args);
Console.WriteLine("builder created");

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo { Title = "My API", Version = "v1" });

    // Define the OAuth2.0 scheme that's being used (e.g., implicit, password, application, accessCode)
    var securityScheme = new OpenApiSecurityScheme
    {
        Name = "JWT Authentication",
        Description = "Enter JWT Bearer token **_only_**",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = "bearer", // must be lower case
        BearerFormat = "JWT",
        Reference = new OpenApiReference
        {
            Type = ReferenceType.SecurityScheme,
            Id = "Bearer"
        }
    };

    options.AddSecurityDefinition("Bearer", securityScheme);

    var securityRequirement = new OpenApiSecurityRequirement
    {
        { securityScheme, new[] { "Bearer" } }
    };

    options.AddSecurityRequirement(securityRequirement);
});

// builder.Services.AddCors(options =>
// {
//     options.AddPolicy("WebAppPolicy", corsBuilder =>
//     {
//         corsBuilder
//             .WithOrigins(
//                 builder.Configuration["WebApp:BaseUrl"]!, 
//                 builder.Configuration["WebApp:BaseUrlHttps"]!)
//             .AllowAnyHeader()
//             .AllowAnyMethod();
//     });
// });

builder.Services.AddCors(options =>
{
    options.AddPolicy("BroadDevPolicy", corsBuilder =>
    {
        corsBuilder
            .AllowAnyOrigin()
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

// secrets, auth tokens and sids.
var keyVaultParams = builder.Configuration.GetSection("KeyVault");
var keyVaultSecretNames = keyVaultParams.GetSection("SecretNames");
var keyVaultJwtSecretNames = keyVaultSecretNames.GetSection("Jwt");
var keyVaultTwilioSecretNames = keyVaultSecretNames.GetSection("Twilio");

var keyVaultUrl = keyVaultParams["Url"]!;

var secretClient =
    new SecretClient(new Uri(keyVaultUrl), new DefaultAzureCredential());

builder.Services.AddSingleton(secretClient);

var jwtKvSecret = secretClient.GetSecret(keyVaultJwtSecretNames["Secret"]);
var twilioKvAccountAuthToken = secretClient.GetSecret(keyVaultTwilioSecretNames["AccountAuthToken"]);

var jwtSecret = jwtKvSecret.Value.Value;
var twilioAccountAuthToken = twilioKvAccountAuthToken.Value.Value;

builder.Services.AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecret))
        };
    });

var twilioClient = new HttpClient
{
    BaseAddress = new Uri(builder.Configuration["Twilio:BaseUrl"]!),
};
twilioClient.DefaultRequestHeaders.Authorization =
    new AuthenticationHeaderValue("Bearer", twilioAccountAuthToken);
builder.Services.AddSingleton(twilioClient);

builder.Services.AddScoped<ITwilioServices, TwilioServices>();

builder.Services.AddTransient<IJwtGenerator, JwtGenerator>();

builder.Services.AddTransient<IScheduleFactory, ScheduleFactory>();

builder.Services.AddScoped<IShiftRepository, ShiftRepository>();
builder.Services.AddScoped<IScheduleRepository, ScheduleRepository>();
builder.Services.AddScoped<IEmployeeRepository, EmployeeRepository>();
builder.Services.AddScoped<IShiftExceptionRepository, ShiftExceptionRepository>();


builder.Services.AddTransient<IQuotaCalculator, QuotaCalculator>();
builder.Services.AddTransient<IAssignmentScorer, AssignmentScorer>();
builder.Services.AddScoped<IBalancer, Balancer>();
builder.Services.AddScoped<IEmployeeComparer, EmployeeComparer>();
builder.Services.AddScoped<IShiftAssigner, ShiftAssigner>();
builder.Services.AddTransient<IShiftComparer, ShiftComparer>();
builder.Services.AddTransient<IDataGatherer, DataGatherer>();
builder.Services.AddTransient<IDataGatherer, DataGathererNoSchedule>();
builder.Services.AddTransient<IScheduleScorer, ScheduleScorer>();
builder.Services.AddTransient<IScheduleReportBuilder, ScheduleReportBuilder>();
builder.Services.AddTransient<IScheduler, Scheduler>();

builder.Services.AddScoped<IAutoScheduleStrategy, AutoScheduleStrategy>();
builder.Services.AddScoped<IAutoScheduleProcess, AutoScheduleProcess>();
builder.Services.AddSingleton<IAutoScheduleScanner, AutoScheduleScanner>();
builder.Services.AddTransient<IAutoScheduleProcessFactory, AutoScheduleProcessFactory>();
builder.Services.AddScoped<IAutoScheduleProcessRepository, AutoScheduleProcessRepository>();
builder.Services.AddScoped<IRepository<Step>, StepRepository>();
builder.Services.AddTransient<IStep, Step>();

builder.Services.AddDbContext<ApiDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services
    .AddDefaultIdentity<IdentityUser>(options =>
    {
        options.SignIn.RequireConfirmedAccount = false;
        options.Password.RequireDigit = true;
        options.Password.RequiredLength = 6;
        options.Password.RequireLowercase = true;
        options.Password.RequireNonAlphanumeric = false;
        options.Password.RequireUppercase = true;
        options.Password.RequiredUniqueChars = 0;
    })
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<ApiDbContext>();

builder.Services.AddRouting();

builder.Services.Configure<RequestLocalizationOptions>(options =>
{
    var supportedCultures = new[] { new CultureInfo("he-IL") };
    options.DefaultRequestCulture = new RequestCulture("he-IL");
    options.SupportedCultures = supportedCultures;
    options.SupportedUICultures = supportedCultures;
});

var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseSwagger();
app.UseSwaggerUI();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler(appBuilder =>
    {
        appBuilder.Run(async context =>
        {
            context.Response.StatusCode = StatusCodes.Status500InternalServerError;
            context.Response.ContentType = "application/json";

            var exceptionHandlerFeature = context.Features.Get<IExceptionHandlerFeature>();
            if (exceptionHandlerFeature != null)
            {
                var logger = app.Services.GetRequiredService<ILogger<Program>>();
                logger.LogError($"Something went wrong: {exceptionHandlerFeature.Error}");

                await context.Response.WriteAsJsonAsync(new ErrorDetails
                {
                    StatusCode = context.Response.StatusCode,
                    Message = "An unexpected error occurred."
                });
            }
        });
    });
}
else
{
    app.UseDeveloperExceptionPage();
}

app.UseRequestLocalization();

app.UseRouting();

// app.UseCors("WebAppPolicy");
app.UseCors("BroadDevPolicy");


app.UseAuthentication();
app.UseAuthorization();

app.UseHttpsRedirection();
app.MapControllers();


app.Run();

public class ErrorDetails
{
    public int StatusCode { get; set; }
    public string Message { get; set; }

    public override string ToString()
    {
        return JsonConvert.SerializeObject(this);
    }
}
