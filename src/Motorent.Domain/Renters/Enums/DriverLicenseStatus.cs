using Motorent.Domain.Common.Enums;

namespace Motorent.Domain.Renters.Enums;

public sealed class DriverLicenseStatus : Enum<DriverLicenseStatus>
{
    public static readonly DriverLicenseStatus PendingValidation = new("pending_validation", 1);
    public static readonly DriverLicenseStatus WaitingApproval = new("waiting_approval", 2);
    public static readonly DriverLicenseStatus Approved = new("approved", 3);
    public static readonly DriverLicenseStatus Rejected = new("rejected", 4);
    
    private DriverLicenseStatus(string name, int value) : base(name, value)
    {
    }
}