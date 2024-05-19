using Motorent.Domain.Renters.Services;
using Motorent.Domain.Renters.ValueObjects;

namespace Motorent.TestUtils.Factories;

public static partial class Factories
{
    public static class Renter
    {
        public static Task<Result<Domain.Renters.Renter>> CreateAsync(
            RenterId? id = null,
            string? userId = null,
            Document? document = null,
            EmailAddress? email = null,
            FullName? fullName = null,
            Birthdate? birthdate = null,
            DriverLicense? driverLicense = null,
            IDocumentService? documentService = null,
            IDriverLicenseService? driverLicenseService = null)
        {
            document ??= Constants.Constants.Renter.Document;
            driverLicense ??= Constants.Constants.Renter.DriverLicense;

            if (documentService is null)
            {
                documentService = A.Fake<IDocumentService>();
                A.CallTo(() => documentService.IsUniqueAsync(document, A<CancellationToken>.Ignored))
                    .Returns(true);
            }

            if (driverLicenseService is null)
            {
                driverLicenseService = A.Fake<IDriverLicenseService>();
                A.CallTo(() => driverLicenseService.IsUniqueAsync(driverLicense, A<CancellationToken>.Ignored))
                    .Returns(true);
            }

            return Domain.Renters.Renter.CreateAsync(
                id: id ?? Constants.Constants.Renter.Id,
                userId: userId ?? Constants.Constants.Renter.UserId,
                document: document,
                email: email ?? Constants.Constants.Renter.Email,
                fullName: fullName ?? Constants.Constants.Renter.FullName,
                birthdate: birthdate ?? Constants.Constants.Renter.Birthdate,
                driverLicense: driverLicense,
                documentService: documentService,
                driverLicenseService: driverLicenseService);
        }
    }
}