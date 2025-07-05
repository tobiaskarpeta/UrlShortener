using MediatR;
using UrlShortener.Api.Mediator.Behaviors;
using UrlShortener.Api.Repositories;

namespace UrlShortener.Api.Extensions;
public static class ServiceCollectionExtensions
{
    public static IServiceCollection ConfigureApplicationServices(this IServiceCollection services)
    {
        services.AddScoped<IUrlRepository, UrlRepository>();
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(CachingBehavior<,>));

        return services;
    }
}