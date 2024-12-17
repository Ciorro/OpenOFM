using Microsoft.Extensions.DependencyInjection;
using OpenOFM.Ui.Navigation;
using OpenOFM.Ui.Navigation.Attributes;
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
                if (type.IsAssignableTo(typeof(IPage)) && !type.IsInterface)
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
    }
}
