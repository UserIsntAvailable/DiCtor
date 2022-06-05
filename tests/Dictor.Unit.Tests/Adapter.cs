namespace Dictor.Unit.Tests;

public class Adapter<TIncrementalGenerator> : ISourceGenerator, IIncrementalGenerator
    where TIncrementalGenerator : IIncrementalGenerator, new()
{
    private TIncrementalGenerator _generator;

    public Adapter()
    {
        _generator = new TIncrementalGenerator();
    }

    public void Initialize(GeneratorInitializationContext context)
    {
        throw new NotImplementedException();
    }

    public void Execute(GeneratorExecutionContext context)
    {
        throw new NotImplementedException();
    }

    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        _generator.Initialize(context);
    }
}
