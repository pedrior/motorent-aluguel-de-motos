using Motorent.Domain.Common.Enums;

namespace Motorent.Domain.Renters.Enums;

public sealed class CNHStatus : Enumeration<CNHStatus>
{
    public static readonly CNHStatus PendingValidation = new("pending_validation", 1);
    public static readonly CNHStatus WaitingApproval = new("waiting_approval", 2);
    public static readonly CNHStatus Approved = new("approved", 3);
    public static readonly CNHStatus Rejected = new("rejected", 4);
    
    private CNHStatus(string name, int value) : base(name, value)
    {
    }
}