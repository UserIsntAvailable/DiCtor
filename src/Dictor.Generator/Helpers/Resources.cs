using System.Reflection;
using System.Runtime.CompilerServices;
using Microsoft.CodeAnalysis.Text;

[assembly: InternalsVisibleTo("Dictor.Unit.Tests")]

namespace Dictor.Generator.Helpers;

internal static class Resources
{
    public static SourceText GetAttributesFileContent()
    {
        using var resourceStream =
            Assembly.GetExecutingAssembly().GetManifestResourceStream("Dictor.Generator.Attributes.cs")!;

        return SourceText.From(resourceStream, canBeEmbedded: true);
    }
}
