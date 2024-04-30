using Motorent.Domain.Common.Enums;

namespace Motorent.Domain.Motorcycles.Enums;

public sealed class Brand : Enumeration<Brand>
{
    public static readonly Brand Honda = new("honda", 1);
    public static readonly Brand Yamaha = new("yamaha", 2);
    public static readonly Brand Suzuki = new("suzuki", 3);
    
    private Brand(string name, int value) : base(name, value)
    {
    }
}