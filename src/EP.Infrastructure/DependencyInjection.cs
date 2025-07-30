using EP.Application.Common.Interfaces;
using EP.Application.Common.Interfaces.Repositories;
using EP.Application.Common.Interfaces.Services;
using EP.Infrastructure.Data;
using EP.Infrastructure.Repositories;
using EP.Infrastructure.Services;
using EP.Infrastructure.UnitOfWorks;
using Microsoft.Extensions.DependencyInjection;

namespace EP.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructureService(this IServiceCollection services)
        {
            services.AddMediatR(cfg =>
                cfg.RegisterServicesFromAssembly(typeof(Application.DependencyInjection).Assembly));

            services.AddScoped<ICategoryRepository, CategoryRepository>();
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IShelvesRepository, ShelvesRepository>();
            services.AddScoped<IStoryRepository, StoryRepository>();
            services.AddScoped<IAuthorRepository, AuthorRepository>();
            services.AddScoped<IVolumeRepository, VolumeRepository>();

            services.AddScoped<IFileStorageService, FileStorageService>();
            services.AddScoped<IHashService, HashService>();
            services.AddScoped<ITokenService, TokenService>();
            services.AddScoped<IMailService, MailService>();

            services.AddScoped<IUnitOfWork, UnitOfWork>();

            return services;
        }
    }
}
