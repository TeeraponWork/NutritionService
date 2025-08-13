using Application.Abstractions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;

namespace Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration config)
        {
            // EF Core + PostgreSQL + snake_case
            services.AddDbContext<NutritionDbContext>(opt =>
            {
                opt.UseNpgsql(config.GetConnectionString("NutritionDb"));
                opt.UseSnakeCaseNamingConvention();
            });
            services.AddScoped<INutritionDbContext>(sp => sp.GetRequiredService<NutritionDbContext>());

            // Redis publisher
            services.AddSingleton<IConnectionMultiplexer>(_ =>
                ConnectionMultiplexer.Connect(config.GetConnectionString("Redis")));
            services.AddSingleton<IEventPublisher, RedisEventPublisher>();

            return services;
        }
    }
}
