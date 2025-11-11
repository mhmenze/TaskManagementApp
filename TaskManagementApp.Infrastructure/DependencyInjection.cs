using Microsoft.Extensions.DependencyInjection;
using TaskManagementApp.Application.Interfaces;
using TaskManagementApp.Infrastructure.Repositories;

namespace TaskManagementApp.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, string connectionString)
        {
            services.AddSingleton(connectionString);

            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<ITaskRepository, TaskRepository>();

            return services;
        }
    }
}