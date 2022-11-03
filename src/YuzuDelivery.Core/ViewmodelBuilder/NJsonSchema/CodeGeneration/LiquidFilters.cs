using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Fluid;
using Fluid.Values;
using NJsonSchema;
using YuzuDelivery.Core.ViewModelBuilder;

namespace YuzuDelivery.Core.ViewmodelBuilder.NJsonSchema.CodeGeneration;

internal static class LiquidFilters
{
    public static ValueTask<FluidValue> CSharpDocs(FluidValue input, FilterArguments arguments, TemplateContext context)
    {
        var tabCount = (int) arguments.At(0).ToNumberValue();
        var converted = ConversionUtilities.ConvertCSharpDocs(input.ToStringValue(), tabCount);
        return new ValueTask<FluidValue>(new StringValue(converted));
    }

    public static ValueTask<FluidValue> CamelCase(FluidValue input, FilterArguments arguments,
        TemplateContext context)
    {
        var firstCharacterMustBeAlpha = arguments["firstCharacterMustBeAlpha"].ToBooleanValue();
        var converted = ConversionUtilities.ConvertToLowerCamelCase(input.ToStringValue(), firstCharacterMustBeAlpha);
        return new ValueTask<FluidValue>(new StringValue(converted));
    }

    public static ValueTask<FluidValue> PascalCase(FluidValue input, FilterArguments arguments,
        TemplateContext context)
    {
        var firstCharacterMustBeAlpha = arguments["firstCharacterMustBeAlpha"].ToBooleanValue();
        var converted = ConversionUtilities.ConvertToUpperCamelCase(input.ToStringValue(), firstCharacterMustBeAlpha);
        return new ValueTask<FluidValue>(new StringValue(converted));
    }

    public static ValueTask<FluidValue> Literal(FluidValue input, FilterArguments arguments, TemplateContext context)
    {
        var converted = "\"" + ConversionUtilities.ConvertToStringLiteral(input.ToStringValue()) + "\"";
        return new ValueTask<FluidValue>(new StringValue(converted, encode: false));
    }

    public static ValueTask<FluidValue> StripVmTypePrefix(FluidValue input, FilterArguments arguments,
        TemplateContext context)
    {
        var result = input.ToStringValue().RemoveVmTypePrefix();
        return new ValueTask<FluidValue>(new StringValue(result, encode: false));
    }

    public static ValueTask<FluidValue> StripUsingDirective(FluidValue input, FilterArguments arguments, TemplateContext context)
    {
        var match = Regex.Match(input.ToStringValue(), @"using\s+(.+);");
        return match.Success
            ? new ValueTask<FluidValue>(new StringValue(match.Groups[1].Value, encode: false))
            : input;
    }
}
