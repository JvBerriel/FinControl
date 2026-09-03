using FinControl.Domain.Entities;
using FinControl.Domain.Interfaces;
using FinControl.Infrastructure.Data;
using FinControl.Infrastructure.Identity;
using FinControl.Infrastructure.Repositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace FinControl.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection");

        services.AddDbContext<FinControlDbContext>(options =>
            options.UseNpgsql(connectionString));

        services.AddIdentityCore<Usuario>(options =>
            {
                options.Password.RequiredLength = 8;
                options.Password.RequireNonAlphanumeric = false;
                options.User.RequireUniqueEmail = true;
            })
            .AddRoles<IdentityRole<int>>()
            .AddEntityFrameworkStores<FinControlDbContext>()
            .AddErrorDescriber<IdentityErrorDescriberPtBr>();

        services.AddScoped<IUnitOfWork>(provider => provider.GetRequiredService<FinControlDbContext>());
        services.AddScoped<IUsuarioRepository, UsuarioRepository>();
        services.AddScoped<ICategoriaRepository, CategoriaRepository>();
        services.AddScoped<ITransacaoRepository, TransacaoRepository>();
        services.AddScoped<IMetaMensalRepository, MetaMensalRepository>();

        return services;
    }
}
