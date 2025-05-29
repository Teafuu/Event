using Azure.Identity;
using Domain.Caches;
using Domain.EventClient;
using Domain.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Identity.Web;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);


var config = builder.Configuration.AddUserSecrets<Program>().AddEnvironmentVariables().Build();

builder.Configuration.AddAzureAppConfiguration(options =>
{
    var tenantId = config["AZURE_TENANT_ID"];
    var clientId = config["AZURE_CLIENT_ID"];
    var clientSecret = config["AZURE_CLIENT_SECRET"] ;

    var shouldUseDefaultCredentials = tenantId is null || clientId is null || clientSecret is null;

    options.Connect(new Uri(config["AppConfig:Endpoint"]!), shouldUseDefaultCredentials 
        ? new DefaultAzureCredential() 
        : new ClientSecretCredential(tenantId, clientId, clientSecret));
});

builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddMicrosoftIdentityWebApi(builder.Configuration.GetSection("AzureAd"))
    .EnableTokenAcquisitionToCallDownstreamApi()
    .AddInMemoryTokenCaches();

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("cache.read", policy =>
    {
        policy.RequireRole("cache.read");
    });
});

builder.Services.AddSwaggerGen(options =>
{
    var jwtScheme = new OpenApiSecurityScheme
    {
        Reference = new OpenApiReference
        {
            Type = ReferenceType.SecurityScheme,
            Id = JwtBearerDefaults.AuthenticationScheme
        },
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = JwtBearerDefaults.AuthenticationScheme,
        BearerFormat = "JWT",
        Description = "Paste **only** the JWT (no “Bearer ” prefix)."
    };
    options.AddSecurityDefinition(jwtScheme.Reference.Id, jwtScheme);

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        { jwtScheme, [] }
    });
});

builder.Logging.ClearProviders().AddConsole();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(
        policy =>
        {
            policy.WithOrigins("http://localhost:3000");
        });
});

builder.Services.AddHttpClient("BiApi", (sp, client) =>
{
    var cfg = sp.GetRequiredService<IConfiguration>();
    client.BaseAddress = new Uri(cfg["bi-api-uri"]!);
}); 

builder.Services.AddScoped<IEventService, EventService>();
builder.Services.AddScoped<IBIEventClient, BIEventClient>();
builder.Services.AddSingleton<ICache, MemoryCache>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
