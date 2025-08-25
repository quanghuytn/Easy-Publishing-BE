using EP.Application.Behaviors;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace EP.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplicationService(this IServiceCollection services)
        {
            // Đăng ký MediatR
            services.AddMediatR(cfg =>
                cfg.RegisterServicesFromAssembly(typeof(Application.DependencyInjection).Assembly));

            // Đăng ký FluentValidation
            services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

            // Đăng ký Validation Behavior
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));


            return services;
        }
    }
}
