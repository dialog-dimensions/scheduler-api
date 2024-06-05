using System.Globalization;
using System.Net.Http.Headers;
using System.Text;
using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using Azure.Storage.Blobs;
using Hangfire;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Localization;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;
using OpenAI.Extensions;
using SchedulerApi;
using SchedulerApi.DAL;
using SchedulerApi.DAL.Repositories;
using SchedulerApi.DAL.Repositories.Interfaces;
using SchedulerApi.Models.Entities.Factories;
using SchedulerApi.Services.ApiFlashClient;
using SchedulerApi.Services.ChatGptClient;
using SchedulerApi.Services.ChatGptClient.Interfaces;
using SchedulerApi.Services.ImageGenerationServices.ScheduleHtmlServices;
using SchedulerApi.Services.ImageGenerationServices.ScheduleImageServices;
using SchedulerApi.Services.JWT;
using SchedulerApi.Services.JWT.AuthorizationFilters;
using SchedulerApi.Services.ScheduleEngine;
using SchedulerApi.Services.ScheduleEngine.Comparers;
using SchedulerApi.Services.ScheduleEngine.Comparers.Interfaces;
using SchedulerApi.Services.ScheduleEngine.Interfaces;
using SchedulerApi.Services.Storage.BlobStorageServices;
using SchedulerApi.Services.Storage.HtmlStorageServices;
using SchedulerApi.Services.Storage.ImageStorageServices;
using SchedulerApi.Services.WhatsAppClient.Twilio;
using SchedulerApi.Services.Workflows.Procedures;
using SchedulerApi.Services.Workflows.Processes.Classes;
using SchedulerApi.Services.Workflows.Processes.Factories.Classes;
using SchedulerApi.Services.Workflows.Processes.Factories.Interfaces;
using SchedulerApi.Services.Workflows.Processes.Interfaces;
using SchedulerApi.Services.Workflows.Steps;
using SchedulerApi.Services.Workflows.Strategies.Classes;
using SchedulerApi.Services.Workflows.Strategies.Interfaces;

var builder = WebApplication.CreateBuilder(args);

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

Console.WriteLine("Adding CORS.");
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

Console.WriteLine("Adding Blob Storage Services.");
// Blob Storage Services
var blobServiceConnectionString = builder.Configuration["ConnectionStrings:AzureBlobStorage"]!;
Console.WriteLine($"blobServiceConnectionString: {blobServiceConnectionString}");

var blobServiceClient = new BlobServiceClient(blobServiceConnectionString);
builder.Services.AddSingleton(blobServiceClient);
builder.Services.AddTransient<IBlobStorageServices, BlobStorageServices>();
builder.Services.AddTransient<IHtmlStorageServices, HtmlStorageServices>();
builder.Services.AddTransient<IImageStorageServices, ImageStorageServices>();

Console.WriteLine("Adding Schedule Image Services.");
// Schedule Image Services
builder.Services.AddTransient<IApiFlashClient, ApiFlashClient>();
builder.Services.AddTransient<IScheduleHtmlGenerator, ScheduleHtmlGenerator>();
builder.Services.AddTransient<IScheduleImagePublisher, ScheduleImagePublisher>();

Console.WriteLine("Adding Secret Client.");
// secrets, auth tokens and sids.
var keyVaultUrl = builder.Configuration["KeyVault:Url"]!;
Console.WriteLine($"keyVaultUrl is {keyVaultUrl}");

var secretClient =
    new SecretClient(new Uri(keyVaultUrl), new DefaultAzureCredential());
builder.Services.AddSingleton(secretClient);

Console.WriteLine("Adding JWT Authentication.");
// JWT Authentication
var jwtKvSecret = secretClient.GetSecret(builder.Configuration["KeyVault:SecretNames:Jwt:Secret"]);
var jwtSecret = jwtKvSecret.Value.Value;
Console.WriteLine($"jwtSecret is {jwtSecret}");

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
builder.Services.AddTransient<IJwtGenerator, JwtGenerator>();

Console.WriteLine("Adding Twilio Client and Services.");
// Twilio
var twilioKvAccountAuthToken = secretClient.GetSecret(builder.Configuration["KeyVault:SecretNames:Twilio:AccountAuthToken"]);
var twilioAccountAuthToken = twilioKvAccountAuthToken.Value.Value;
Console.WriteLine($"twilioAccountAuthToken is {twilioAccountAuthToken}");

var twilioClient = new HttpClient
{
    BaseAddress = new Uri(builder.Configuration["Twilio:BaseUrl"]!),
};
twilioClient.DefaultRequestHeaders.Authorization =
    new AuthenticationHeaderValue("Bearer", twilioAccountAuthToken);
builder.Services.AddSingleton(twilioClient);
builder.Services.AddScoped<ITwilioServices, TwilioServices>();

Console.WriteLine("Adding ChatGPT Services.");
// ChatGPT
var chatGptKvApiKey = secretClient.GetSecret(builder.Configuration["KeyVault:SecretNames:ChatGPT:ApiKey"]);
var chatGptApiKey = chatGptKvApiKey.Value.Value;
Console.WriteLine($"chatGptApiKey is {chatGptApiKey}");

builder.Services.AddOpenAIService(settings =>
{
    settings.ApiKey = chatGptApiKey;
    settings.UseBeta = true;
});
builder.Services.AddSingleton<IAssistantServices, AssistantServices>();
builder.Services.AddScoped<ISchedulerGptServices, SchedulerGptServices>();
builder.Services.AddScoped<ISchedulerGptSessionRepository, SchedulerGptSessionRepository>();

Console.WriteLine("Adding Repositories.");
// Entity Model Services
builder.Services.AddScoped<IEmployeeRepository, EmployeeRepository>();
builder.Services.AddScoped<IShiftRepository, ShiftRepository>();
builder.Services.AddTransient<IScheduleFactory, ScheduleFactory>();
builder.Services.AddScoped<IScheduleRepository, ScheduleRepository>();
builder.Services.AddScoped<IShiftExceptionRepository, ShiftExceptionRepository>();
builder.Services.AddScoped<IDeskRepository, DeskRepository>();
builder.Services.AddScoped<IUnitRepository, UnitRepository>();

Console.WriteLine("Adding Schedule Engine Services.");
// Schedule Engine Services
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

Console.WriteLine("Adding Process Services.");
// Process Services
builder.Services.AddScoped<IAutoScheduleStrategy, AutoScheduleStrategy>();
builder.Services.AddScoped<IGptStrategy, GptStrategy>();
builder.Services.AddScoped<IAutoScheduleProcess, AutoScheduleProcess>();
builder.Services.AddScoped<IGptScheduleProcess, GptScheduleProcess>();
builder.Services.AddScoped<IRepository<Step>, StepRepository>();
builder.Services.AddScoped<IAutoScheduleProcessRepository, AutoScheduleProcessRepository>();
builder.Services.AddTransient<IAutoScheduleProcessFactory, AutoScheduleProcessFactory>();
builder.Services.AddTransient<IStep, Step>();

Console.WriteLine("Adding Job Services.");
// Job Services
builder.Services.AddScoped<IGptScheduleProcessProcedures, GptScheduleProcessProcedures>();

Console.WriteLine("Adding DB Context Services.");
// EF Core
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

Console.WriteLine("Configuring Localization and Globalization.");
// Localization and Globalization
builder.Services.Configure<RequestLocalizationOptions>(options =>
{
    var supportedCultures = new[] { new CultureInfo("he-IL") };
    options.DefaultRequestCulture = new RequestCulture("he-IL");
    options.SupportedCultures = supportedCultures;
    options.SupportedUICultures = supportedCultures;
});

Console.WriteLine("Adding Hangfire Services.");
// Hangfire
builder.Services.AddHangfire(configuration => configuration
    .UseSqlServerStorage(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddHangfireServer();


// 

Console.WriteLine("Adding Routing.");
builder.Services.AddRouting();

Console.WriteLine("Building Application.");
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

// Job Initializations
// const string scannerJobId = "scanner";
//
// string scannerCronExpression;
// string deskId;
// if (app.Environment.IsDevelopment())
// {
//     scannerCronExpression = "*/30 * * * *";
//     deskId = "4";
// }
// else
// {
//     scannerCronExpression = Cron.Daily(10);
//     deskId = "1";
// }

// var recurringJobManager = app.Services.GetRequiredService<IRecurringJobManager>();
// recurringJobManager.AddOrUpdate<GptProcessInitiationJob>(
//     scannerJobId,
//     job => job.Execute(deskId),
//     scannerCronExpression
//     );

app.UseAuthentication();
app.UseAuthorization();

app.UseHttpsRedirection();

app.UseHangfireDashboard("/hangfire", new DashboardOptions
{
    Authorization = new[] { new JwtAuthorizationFilter() } // TODO: implement bearer logic to the filter.
}); 

app.MapControllers();


app.Run();

namespace SchedulerApi
{
    public class ErrorDetails
    {
        public int StatusCode { get; set; }
        public string Message { get; set; }

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}
