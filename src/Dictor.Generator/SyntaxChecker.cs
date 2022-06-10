namespace Dictor.Generator;

internal static class SyntaxChecker
{
    public static bool IsDecoratedClass(SyntaxNode node, CancellationToken cancellationToken)
    {
        return node is ClassDeclarationSyntax classDeclaration && HasKnownAttributes(classDeclaration);
    }

    private static bool HasKnownAttributes(ClassDeclarationSyntax classDeclaration)
    {
        foreach(var attributeList in classDeclaration.AttributeLists)
        {
            foreach(var attribute in attributeList.Attributes)
            {
                return IsKnownAttribute(attribute);
            }
        }

        return false;
    }

    private static bool IsKnownAttribute(AttributeSyntax attribute)
    {
        // TODO: Why not just use attribute.Name.ToString()? ( They seem exactly the same )
        var attributeName = (attribute.Name as SimpleNameSyntax)!.Identifier.Text;

        // TODO: Use variable constant instead of magic string
        return attributeName switch
        {
            "DiCtor" => true,
            _        => false,
        };
    }
}
