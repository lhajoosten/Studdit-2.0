using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Studdit.Application.Common.Interfaces;
using Studdit.Application.Common.Services;
using Studdit.Persistence.Context;
using Studdit.Persistence.Repositories;

namespace Studdit.Persistence
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddPersistence(this IServiceCollection services, IConfiguration configuration)
        {
            // Add DbContext
            services.AddDbContext<ApplicationDbContext>(options =>
            {
                var connectionString = configuration.GetConnectionString("DefaultConnection");
                options.UseSqlServer(connectionString, b =>
                {
                    b.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName);
                    b.EnableRetryOnFailure(
                        maxRetryCount: 3,
                        maxRetryDelay: TimeSpan.FromSeconds(5),
                        errorNumbersToAdd: null);
                });

                // Enable sensitive data logging in development
                if (configuration.GetSection("Logging:EnableSensitiveDataLogging").Value == "true")
                {
                    options.EnableSensitiveDataLogging();
                }

                // Enable detailed errors in development
                if (configuration.GetSection("Logging:EnableDetailedErrors").Value == "true")
                {
                    options.EnableDetailedErrors();
                }
            });

            // Register Application DbContext interface
            services.AddScoped<IApplicationDbContext>(provider => provider.GetRequiredService<ApplicationDbContext>());

            // Register repositories
            services.AddScoped(typeof(IQueryRepository<>), typeof(QueryRepository<>));
            services.AddScoped(typeof(IQueryRepository<,>), typeof(QueryRepository<,>));
            services.AddScoped(typeof(ICommandRepository<>), typeof(CommandRepository<>));
            services.AddScoped(typeof(ICommandRepository<,>), typeof(CommandRepository<,>));

            // Register Unit of Work
            services.AddScoped<IUnitOfWork, UnitOfWork>();

            // Register services
            services.AddScoped<IDateTimeService, DateTimeService>();

            // Add HttpContextAccessor for production CurrentUserService
            services.AddHttpContextAccessor();

            return services;
        }
    }
}
