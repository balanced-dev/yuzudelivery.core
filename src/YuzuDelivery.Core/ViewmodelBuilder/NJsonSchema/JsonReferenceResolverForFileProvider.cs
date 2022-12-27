using System.Collections.Generic;
using System.Linq;
using System.IO;
using NJsonSchema;
using Microsoft.Extensions.FileProviders;
using NJsonSchema.References;
using System.Threading.Tasks;
using System.Threading;

namespace YuzuDelivery.Core.ViewModelBuilder
{
    public class JsonReferenceResolverForFileProvider : JsonReferenceResolver
    {
        private readonly JsonSchemaAppender _schemaAppender;
        private readonly Dictionary<string, IJsonReference> _resolvedObjects = new Dictionary<string, IJsonReference>();
        private readonly IDictionary<string, IFileInfo> _blocks;
        private readonly ReferencesService _referencesService;

        public JsonReferenceResolverForFileProvider(JsonSchemaAppender appender, IDictionary<string, IFileInfo> blocks, ReferencesService referencesService)
            : base(appender)
        {
            _schemaAppender = appender;
            _blocks = blocks;
            _referencesService = referencesService;
        }

        public async override Task<IJsonReference> ResolveFileReferenceAsync(string filePath, CancellationToken cancellationToken = default)
        {
            var filename = Path.GetFileNameWithoutExtension(filePath);

            var json = string.Empty;
            var schemaFile = _blocks.FirstOrDefault(x => x.Key == filename);
            var fileStream = schemaFile.Value.CreateReadStream();
            using var reader = new StreamReader(fileStream);
            json = reader.ReadToEnd();
            json = _referencesService.Fix(json);

            return await JsonSchema.FromJsonAsync(json, filePath, schema => this, cancellationToken).ConfigureAwait(false);
        }

    }
}
