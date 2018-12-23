using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using NoteManager.DataAccess.Modules;
using NoteManager.Models.Config;

namespace NoteManager.Web.Modules
{
    public static class WebModule
    {
        public static IServiceCollection AddWebModule(this IServiceCollection services)
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }
            services.AddSingleton(c => c.GetService<IOptions<MainConfig>>().Value);
            services.AddServiceModule();

            return services;
        }
    }
}
