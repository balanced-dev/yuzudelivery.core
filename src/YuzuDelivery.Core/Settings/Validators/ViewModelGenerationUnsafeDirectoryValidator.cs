using System.IO;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

namespace YuzuDelivery.Core.Settings.Validators;

public class ViewModelGenerationUnsafeDirectoryValidator : IValidateOptions<ViewModelGenerationSettings>
{
    private readonly IHostEnvironment _env;

    public ViewModelGenerationUnsafeDirectoryValidator(IHostEnvironment env)
    {
        _env = env;
    }

    public ValidateOptionsResult Validate(string name, ViewModelGenerationSettings options)
    {
        if (options.AcceptUnsafeDirectory)
        {
            return ValidateOptionsResult.Success;;
        }

        var directory = options.Directory;
        if (!Path.IsPathFullyQualified(directory))
        {
            directory = Path.Combine(_env.ContentRootPath, directory);
        }

        return Path.GetFullPath(directory).StartsWith(Path.GetFullPath(_env.ContentRootPath))
            ? ValidateOptionsResult.Success
            : ValidateOptionsResult.Fail("ViewModel output directory cannot be outside of content root unless AcceptUnsafeDirectory is enabled");
    }
}
