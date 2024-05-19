using Motorent.Domain.Renters.ValueObjects;

namespace Motorent.Domain.Renters.Errors;

internal static class RenterErrors
{
    public static readonly Error DriverLicenseNotPendingValidation = Error.Failure(
        "A CNH não está pendente de validação.", 
        code: "renter.driver_license_not_pending_validation");
    
    public static readonly Error DriverLicenseNotWaitingApproval = Error.Failure(
        "A CNH não está esperando aprovação.", 
        code: "renter.driver_license_not_waiting_approval");
    
    public static Error DocumentNotUnique(Document document) => Error.Conflict(
        "Já existe locatário com o mesmo documento CNPJ no sistema",
        code: "renter.document_not_unique",
        details: new() { ["document"] = document.ToString() });
    
    public static Error DriverLicenseNotUnique(DriverLicense driverLicense) => Error.Conflict(
        "Já existe locatário com a mesma CNH no sistema",
        code: "renter.driver_license_not_unique",
        details: new() { ["driver_license"] = driverLicense.ToString() });
}