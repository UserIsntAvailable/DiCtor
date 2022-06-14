// ReSharper disable once RedundantUsingDirective
using System;

namespace Dictor
{
    // TODO: Should I call it Dictor instead?
    [AttributeUsage(AttributeTargets.Class)]
    // ReSharper disable once ClassNeverInstantiated.Global
#pragma warning disable MA0048
    internal class DiCtorAttribute : Attribute
#pragma warning restore MA0048
    {
    }
}
