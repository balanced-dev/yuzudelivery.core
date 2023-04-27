using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using System.IO;
using System.Reflection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using YuzuDelivery.Core.Settings;
using YuzuDelivery.Core.Settings.Validators;

namespace YuzuDelivery.Core
{
    public static class ServiceCollectionExtensions
    {

        public static IServiceCollection AddYuzuCore(this IServiceCollection services)
        {
            services.AddTransient<ISchemaMetaService, SchemaMetaService>();
            services.AddTransient<ISchemaMetaPropertyService, SchemaMetaPropertyService>();

            services.AddOptions<CoreSettings>()
                    .Configure<IConfiguration, IHostEnvironment>((s, cfg, host) =>
                    {
                        if (s.SchemaFileProvider != null)
                        {
                            return;
                        }

                        cfg.GetSection("Yuzu:Core").Bind(s);

                        if (!Path.IsPathFullyQualified(s.SchemaPath))
                        {
                            s.SchemaPath = Path.Combine(host.ContentRootPath, s.SchemaPath);
                        }

                        if (Directory.Exists(s.SchemaPath))
                        {
                            s.SchemaFileProvider = new PhysicalFileProvider(s.SchemaPath);
                        }
                    });

            services.AddSingleton<IValidateOptions<ViewModelGenerationSettings>, ViewModelGenerationUnsafeDirectoryValidator>();
            services.RegisterYuzuManualMapping(Assembly.GetEntryAssembly());

            return services;
        }
    }
}
