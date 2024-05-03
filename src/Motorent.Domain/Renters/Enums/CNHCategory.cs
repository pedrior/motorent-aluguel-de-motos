using Motorent.Domain.Common.Enums;

namespace Motorent.Domain.Renters.Enums;

public sealed class CNHCategory : Enumeration<CNHCategory>
{
    public static readonly CNHCategory A = new("A", 1);
    public static readonly CNHCategory B = new("B", 2);
    public static readonly CNHCategory AB = new("AB", 3);
    
    private CNHCategory(string name, int value) : base(name, value)
    {
    }
}