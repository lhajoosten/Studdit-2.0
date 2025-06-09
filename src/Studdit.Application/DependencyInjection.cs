using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using Studdit.Application.Common;
using Studdit.Application.Common.Behaviours;
using System.Reflection;

namespace Studdit.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            // Register MediatR
            services.AddMediatR(cfg =>
            {
                cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());
                cfg.AddOpenBehavior(typeof(UnhandledExceptionBehaviour<,>));
                cfg.AddOpenBehavior(typeof(ValidationBehaviour<,>));
                cfg.AddOpenBehavior(typeof(PerformanceBehaviour<,>));
            });

            // Register AutoMapper using mapping profile
            services.AddAutoMapper(cfg => cfg.AddProfile<MappingProfile>(), Assembly.GetExecutingAssembly());

            // Register FluentValidation
            services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

            return services;
        }
    }
}
