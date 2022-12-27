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
                    .Configure<IConfiguration, IHostingEnvironment>((s, cfg, host) =>
                    {
                        cfg.GetSection("Yuzu:Core").Bind(s);

                        if (!Path.IsPathFullyQualified(s.Schema))
                        {
                            s.Schema = Path.Combine(host.ContentRootPath, s.Schema);
                        }

                        s.SchemaFileProvider = new PhysicalFileProvider(s.Schema);
                    });

            return services;
        }
    }
}
