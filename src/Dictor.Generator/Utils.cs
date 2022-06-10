using System.Reflection;
using System.Runtime.CompilerServices;
using Microsoft.CodeAnalysis.Text;

[assembly: InternalsVisibleTo("Dictor.Unit.Tests")]

namespace Dictor.Generator;

internal static class Utils
{
    public static string GetAttributesFileContent()
    {
        using var resourceStream =
            Assembly.GetExecutingAssembly().GetManifestResourceStream("Dictor.Generator.Attributes.cs")!;

        return SourceText.From(resourceStream, canBeEmbedded: true).ToString();
    }
}
