using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YuzuDelivery.Core;
using YuzuDelivery.Core.Settings;

namespace YuzuDelivery.Core
{
    public static class ServiceCollectionExtensions
    {

        public static IServiceCollection AddYuzuCore(this IServiceCollection services)
        {

            services.AddTransient<ISchemaMetaService, SchemaMetaService>();
            services.AddTransient<ISchemaMetaPropertyService, SchemaMetaPropertyService>();

            services.AddOptions<CoreSettings>()
                    .Configure<IConfiguration, IHostingEnvironment, IEnumerable<IBaseSiteConfig>>((s, cfg, host, baseConfigs) =>
                    {
                        cfg.GetSection("Yuzu:Core").Bind(s);

                        if (!Path.IsPathFullyQualified(s.SchemaPath))
                        {
                            s.SchemaPath = Path.Combine(host.ContentRootPath, s.SchemaPath);
                        }

                        var fileProviders = baseConfigs.Select(c => c.SchemaFileProvider).ToList();
                        if (s.IsPluginDev) fileProviders.Clear();
                        if(Directory.Exists(s.SchemaPath)) fileProviders.Insert(0, new PhysicalFileProvider(s.SchemaPath));

                        s.SchemaFileProvider = new CompositeFileProvider(fileProviders);
                    });

            return services;
        }
    }
}
