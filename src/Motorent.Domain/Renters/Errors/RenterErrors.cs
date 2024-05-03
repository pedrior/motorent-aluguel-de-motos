using Motorent.Domain.Renters.ValueObjects;

namespace Motorent.Domain.Renters.Errors;

internal static class RenterErrors
{
    public static readonly Error CNHIsNotPendingValidation = Error.Failure(
        "The CNH is not pending validation.", 
        code: "renter.cnh_is_not_pending_validation");
    
    public static readonly Error CNHIsNotWaitingApproval = Error.Failure(
        "The CNH is not waiting approval.", 
        code: "renter.cnh_is_not_waiting_approval");
    
    public static Error CNPJNotUnique(CNPJ cnpj) => Error.Conflict(
        "There is already a renter with the same CNPJ in the system.",
        code: "renter.cnpj_not_unique",
        details: new() { ["cnpj"] = cnpj.ToString() });
    
    public static Error CNHNotUnique(CNH cnh) => Error.Conflict(
        "There is already a renter with the same CNH in the system.",
        code: "renter.cnh_not_unique",
        details: new() { ["cnh"] = cnh.ToString() });
}