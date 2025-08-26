using EP.Application;
using EP.Application.Common.Interfaces;
using EP.Application.Common.Interfaces.Repositories;
using EP.Application.Common.Interfaces.Services.Cache;
using EP.Application.Common.Interfaces.Services.Common;
using EP.Application.Common.Interfaces.Services.Payment;
using EP.Infrastructure.Repositories;
using EP.Infrastructure.Services.Caching;
using EP.Infrastructure.Services.Common;
using EP.Infrastructure.Services.Payment;
using EP.Infrastructure.UnitOfWorks;
using Microsoft.Extensions.DependencyInjection;

namespace EP.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructureService(this IServiceCollection services)
        {
            services.AddApplicationService();

            services.AddScoped<ICategoryRepository, CategoryRepository>();
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IShelvesRepository, ShelvesRepository>();
            services.AddScoped<IStoryRepository, StoryRepository>();
            services.AddScoped<IAuthorRepository, AuthorRepository>();
            services.AddScoped<IVolumeRepository, VolumeRepository>();
            services.AddScoped<IChapterRepository, ChapterRepository>();
            services.AddScoped<IReportRepository, ReportRepository>();
            services.AddScoped<ITicketRepository, TicketRepository>();
            services.AddScoped<IRefundRequestsRepository, RefundRequestsRepository>();
            services.AddScoped<IWalletRepository, WalletRepository>();
            services.AddScoped<ITransactionRepository, TransactionRepository>();
            services.AddScoped(typeof(IRepository<>), typeof(Repository<>));

            services.AddScoped<IFileStorageService, FileStorageService>();
            services.AddScoped<IHashService, HashService>();
            services.AddScoped<ITokenService, TokenService>();
            services.AddScoped<IMailService, MailService>();
            services.AddScoped<IMomoService, MomoService>();
            services.AddScoped<IVNPayService, VNPayService>();

            services.AddScoped<IRedisCacheService, RedisCacheService>();


            services.AddScoped<IUnitOfWork, UnitOfWork>();

            return services;
        }
    }
}
