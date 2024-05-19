using Motorent.Domain.Renters.ValueObjects;

namespace Motorent.Domain.Renters.Errors;

internal static class RenterErrors
{
    public static readonly Error CNHIsNotPendingValidation = Error.Failure(
        "A CNH não está pendente de validação.", 
        code: "renter.cnh_is_not_pending_validation");
    
    public static readonly Error CNHIsNotWaitingApproval = Error.Failure(
        "A CNH não está esperando aprovação.", 
        code: "renter.cnh_is_not_waiting_approval");
    
    public static Error DocumentNotUnique(Document document) => Error.Conflict(
        "Já existe locatário com o mesmo documento CNPJ no sistema",
        code: "renter.document_not_unique",
        details: new() { ["document"] = document.ToString() });
    
    public static Error CNHNotUnique(CNH cnh) => Error.Conflict(
        "Já existe locatário com a mesma CNH no sistema",
        code: "renter.cnh_not_unique",
        details: new() { ["cnh"] = cnh.ToString() });
}