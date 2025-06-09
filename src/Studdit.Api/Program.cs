using FluentValidation;
using Microsoft.OpenApi.Models;
using Serilog;
using Serilog.Events;
using Studdit.Api.Filters;
using Studdit.Api.Services;
using Studdit.Application;
using Studdit.Application.Common.Interfaces;
using Studdit.Persistence;
using Studdit.Persistence.Extensions;
using System.Reflection;

namespace Studdit.Api
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            // Configure Serilog
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
                .MinimumLevel.Override("Microsoft.EntityFrameworkCore", LogEventLevel.Warning)
                .Enrich.FromLogContext()
                .WriteTo.Console()
                .WriteTo.File("logs/studdit-.txt", rollingInterval: RollingInterval.Day)
                .CreateLogger();

            try
            {
                Log.Information("Starting Studdit API");

                var builder = WebApplication.CreateBuilder(args);

                // Add Serilog
                builder.Host.UseSerilog();

                // Add services to the container
                ConfigureServices(builder.Services, builder.Configuration);

                var app = builder.Build();

                // Configure the HTTP request pipeline
                await Configure(app);

                // Initialize database
                await app.Services.InitializeDatabaseAsync();

                await app.RunAsync();
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "Application terminated unexpectedly");
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }

        private static void ConfigureServices(IServiceCollection services, IConfiguration configuration)
        {
            // Add controllers with filters
            services.AddControllers(options =>
            {
                options.Filters.Add<UnhandeledExceptionFilter>();
            });

            // Add API Explorer and Swagger
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "Studdit API",
                    Version = "v1",
                    Description = "A Stack Overflow-like Q&A platform API",
                    Contact = new OpenApiContact
                    {
                        Name = "Studdit Team",
                        Email = "support@studdit.com"
                    }
                });

                // Include XML comments
                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                if (File.Exists(xmlPath))
                {
                    c.IncludeXmlComments(xmlPath);
                }

                // Add JWT authentication
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer"
                });

                c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        Array.Empty<string>()
                    }
                });
            });

            // Add CORS
            services.AddCors(options =>
            {
                options.AddPolicy("AllowSpecificOrigins", policy =>
                {
                    policy.WithOrigins(configuration.GetSection("Cors:AllowedOrigins").Get<string[]>() ?? new[] { "http://localhost:4200" })
                          .AllowAnyHeader()
                          .AllowAnyMethod()
                          .AllowCredentials();
                });
            });

            // Add Application layer
            services.AddApplication();

            // Add Persistence layer
            services.AddPersistence(configuration);

            // Add Current User Service
            services.AddScoped<ICurrentUserService, HttpContextCurrentUserService>();

            // Add FluentValidation
            services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

            // Add Problem Details
            services.AddProblemDetails();

            // Add Health Checks
            services.AddHealthChecks()
                .AddSqlServer(configuration.GetConnectionString("DefaultConnection")!);

            // Add Response Compression
            services.AddResponseCompression();

            // Add Memory Cache
            services.AddMemoryCache();
        }

        private static async Task Configure(WebApplication app)
        {
            // Configure the HTTP request pipeline
            if (app.Environment.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c =>
                {
                    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Studdit API V1");
                    c.RoutePrefix = string.Empty; // Set Swagger UI at app's root
                });
            }
            else
            {
                app.UseExceptionHandler("/Error");
                app.UseHsts();
            }

            // Middleware pipeline
            app.UseHttpsRedirection();
            app.UseResponseCompression();

            // Security headers
            app.Use(async (context, next) =>
            {
                context.Response.Headers.Add("X-Content-Type-Options", "nosniff");
                context.Response.Headers.Add("X-Frame-Options", "DENY");
                context.Response.Headers.Add("X-XSS-Protection", "1; mode=block");
                context.Response.Headers.Add("Referrer-Policy", "strict-origin-when-cross-origin");
                await next();
            });

            app.UseCors("AllowSpecificOrigins");

            app.UseAuthentication();
            app.UseAuthorization();

            // Request logging
            app.UseSerilogRequestLogging();

            // Health checks
            app.MapHealthChecks("/health");

            app.MapControllers();

            // Add API versioning endpoint
            app.MapGet("/api/version", () => new { version = "1.0.0", timestamp = DateTime.UtcNow });
        }
    }
}
