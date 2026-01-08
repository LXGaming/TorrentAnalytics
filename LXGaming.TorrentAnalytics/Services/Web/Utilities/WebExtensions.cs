using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;
using LXGaming.Common.Text.Json;
using Microsoft.Extensions.DependencyInjection;

namespace LXGaming.TorrentAnalytics.Services.Web.Utilities;

public static class WebExtensions {

    public static IServiceCollection AddWebService(this IServiceCollection services) {
        return services.AddWebService<WebService>();
    }

    public static IServiceCollection AddWebService<TService>(this IServiceCollection services)
        where TService : WebService {
        return services.AddWebService<TService, TService>();
    }

    public static IServiceCollection AddWebService<TService, TImplementation>(this IServiceCollection services)
        where TService : WebService where TImplementation : TService {
        return services
            .AddSingleton(new JsonSerializerOptions {
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
                TypeInfoResolver = new DefaultJsonTypeInfoResolver()
                    .WithOrderPropertiesModifier()
                    .WithRequiredPropertiesModifier()
            })
            .AddSingleton<TService, TImplementation>();
    }
}