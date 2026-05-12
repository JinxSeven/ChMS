using System.Text;
using api.Extensions;
using ChMS.Modules.Auth;
using ChMS.Modules.Auth.Application.Settings;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.IdentityModel.Tokens;
using OpenTelemetry.Logs;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

var resourceBuilder = ResourceBuilder.CreateDefault().AddService(serviceName: "chms-api", serviceVersion: "1.0.0");

// LOGGING
var otelEndpoint =
    builder.Configuration["Otel:Endpoint"]
    ?? throw new InvalidOperationException("Otel:Endpoint is missing in appsettings.json");

builder.Logging.AddOpenTelemetry(options =>
{
    options.SetResourceBuilder(resourceBuilder);

    options.IncludeScopes = true;
    options.ParseStateValues = true;
    options.IncludeFormattedMessage = true;

    options.AddOtlpExporter(otlp =>
    {
        otlp.Endpoint = new Uri(otelEndpoint);
    });
});

// TRACES & METRICS
if (!builder.Environment.IsDevelopment())
{
    builder
        .Services.AddOpenTelemetry()
        .ConfigureResource(resource => resource.AddService("chms-api"))
        .WithTracing(tracing =>
        {
            tracing
                .SetResourceBuilder(resourceBuilder)
                .AddAspNetCoreInstrumentation()
                .AddHttpClientInstrumentation()
                .AddOtlpExporter(otlp =>
                {
                    otlp.Endpoint = new Uri(otelEndpoint);
                });
        })
        .WithMetrics(metrics =>
        {
            metrics
                .SetResourceBuilder(resourceBuilder)
                .AddAspNetCoreInstrumentation()
                .AddRuntimeInstrumentation()
                .AddOtlpExporter(otlp =>
                {
                    otlp.Endpoint = new Uri(otelEndpoint);
                });
        });
}

// CONTROLLERS

/*
 * This is the standard, recommended approach for adding module controllers in modular monoliths
 
 * AuthModule is a class inside the Auth module (AuthModule.cs)
 * typeof(AuthModule) gets the System.Type object representing that class
 * .Assembly gets the System.Reflection.Assembly containing that AuthModule that is Evently.Modules.Auth.dll
  
 * To put it plainly, .AddControllers() alone finds controllers in the main assembly
 * .AddApplicationPart(typeof(AuthModule).Assembly) tells it to also search in Evently.Modules.Auth.dll
 */

builder
    .Services.AddControllers()
    .ConfigureApplicationPartManager(manager =>
    {
        manager.ApplicationParts.Add(new AssemblyPart(typeof(AuthModule).Assembly));
    });
builder.Services.AddHttpContextAccessor();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddOpenApi(
    "v1",
    options =>
    {
        options.AddDocumentTransformer<BearerSecuritySchemeTransformer>();
    }
);

// ERROR HANDLING
builder.Services.AddProblemDetails();
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();

// AUTH
builder.Services.AddAuthModule(builder.Configuration);

var jwtSettings =
    builder.Configuration.GetSection("Jwt").Get<JwtSettings>()
    ?? throw new InvalidOperationException("JWT configuration section is missing.");

// This is the JWT middleware
// It runs on every incoming http requests to endpoints with [Authorize] attribute
// Once the token is decoded and checks all validations given below the request gets inside controller
builder
    .Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            ValidateAudience = true,
            ValidateIssuer = true,
            ValidateLifetime = true,

            ValidIssuer = jwtSettings.Issuer,
            ValidAudience = jwtSettings.Audience,

            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.Secret)),

            // JWT by default adds 5 more mins on top of given expiration
            // This line prevents that from happening
            ClockSkew = TimeSpan.Zero,
            RoleClaimType = "Role",
        };
    });

// CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy(
        "Dev",
        policy =>
        {
            policy.AllowAnyHeader().AllowAnyMethod().AllowCredentials().WithOrigins("http://localhost:5173");
        }
    );
});

var app = builder.Build();

app.UseExceptionHandler();

app.UseCors("Dev");

app.UseAuthentication();
app.UseAuthorization();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();

    app.MapScalarApiReference(options =>
    {
        options
            .WithTitle("ChMS Server")
            .WithTheme(ScalarTheme.Moon)
            .WithDefaultHttpClient(ScalarTarget.CSharp, ScalarClient.HttpClient);

        options
            .AddPreferredSecuritySchemes("Bearer")
            .AddHttpAuthentication(
                "Bearer",
                auth =>
                {
                    auth.Token = "";
                }
            )
            .EnablePersistentAuthentication();
    });

    app.RunMigrations();
}
else
{
    app.UseHttpsRedirection();
}

app.MapControllers();

app.Run();
