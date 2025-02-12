using Microsoft.Extensions.DependencyInjection;
using OpenOFM.Core.Api;
using OpenOFM.Core.Services.Stations;
using OpenOFM.Core.Settings;
using OpenOFM.Ui.Factories;
using OpenOFM.Ui.Navigation;
using OpenOFM.Ui.Navigation.Attributes;
using System.IO;
using System.Reflection;

namespace OpenOFM.Ui.Extensions
{
    internal static class ServiceCollectionExtensions
    {
        public static void AddPages(this IServiceCollection services, Assembly? assembly = null)
        {
            assembly = assembly ?? Assembly.GetExecutingAssembly();

            foreach (var type in assembly.GetTypes())
            {
                if (type.IsAssignableTo(typeof(IPage)) && !type.IsAbstract)
                {
                    object pageKey = type.Name;

                    if (type.TryGetCustomAttribute<PageKeyAttribute>(out var attr))
                    {
                        pageKey = attr.PageKey;
                    }

                    services.AddKeyedSingleton(typeof(IPage), pageKey, type);
                }
            }
        }

        public static void AddLocalSettings<T>(this IServiceCollection services, string filename)
            where T : new()
        {
            services.AddSingleton<ISettingsProvider<T>, JsonSettingsProvider<T>>(s =>
            {
                string appDataDir = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
                string fullPath = Path.Combine(appDataDir, "OpenOFM", filename);

                var settings = new JsonSettingsProvider<T>(fullPath);
                settings.Load();
                return settings;
            });
        }

        public static void AddApi(this IServiceCollection services)
        {
            services.AddSingleton<ApiClient>();
            services.AddSingleton<StationsApiClient>();
            services.AddSingleton<PlaylistApiClient>();
            services.AddSingleton<TokenApiClient>();
        }

        public static void AddFactory<TService>(this IServiceCollection services)
            where TService : class
        {
            services.AddTransient<TService>();
            services.AddSingleton<Func<TService>>(s => () => s.GetRequiredService<TService>());
            services.AddSingleton<IAbstractFactory<TService>, AbstractFactory<TService>>();
        }

        public static void AddFactory<TService, TImplementation>(this IServiceCollection services)
            where TService : class
            where TImplementation : class, TService
        {
            services.AddTransient<TService, TImplementation>();
            services.AddSingleton<Func<TService>>(s => () => s.GetRequiredService<TService>());
            services.AddSingleton<IAbstractFactory<TService>, AbstractFactory<TService>>();
        }

        public static void AddStationsProviderChain(this IServiceCollection services, StationProviderKey key, params Type[] implementationTypes)
        {
            if (!implementationTypes.All(x => x.IsAssignableTo(typeof(IStationsProvider))))
            {
                throw new ArgumentException($"All implementation types must be assignable to {nameof(IStationsProvider)}");
            }


        }
    }
}
