using Motorent.Domain.Common.Enums;

namespace Motorent.Domain.Renters.Enums;

public sealed class DriverLicenseCategory : Enum<DriverLicenseCategory>
{
    public static readonly DriverLicenseCategory A = new("A", 1);
    public static readonly DriverLicenseCategory B = new("B", 2);
    public static readonly DriverLicenseCategory AB = new("AB", 3);
    
    private DriverLicenseCategory(string name, int value) : base(name, value)
    {
    }
}