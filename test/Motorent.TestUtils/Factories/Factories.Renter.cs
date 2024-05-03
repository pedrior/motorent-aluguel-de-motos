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
            CNPJ? cnpj = null,
            EmailAddress? email = null,
            FullName? fullName = null,
            Birthdate? birthdate = null,
            CNH? cnh = null,
            ICNPJService? cnpjService = null,
            ICNHService? cnhService = null)
        {
            cnpj ??= Constants.Constants.Renter.CNPJ;
            cnh ??= Constants.Constants.Renter.CNH;

            if (cnpjService is null)
            {
                cnpjService = A.Fake<ICNPJService>();
                A.CallTo(() => cnpjService.IsUniqueAsync(cnpj, A<CancellationToken>.Ignored))
                    .Returns(true);
            }

            if (cnhService is null)
            {
                cnhService = A.Fake<ICNHService>();
                A.CallTo(() => cnhService.IsUniqueAsync(cnh, A<CancellationToken>.Ignored))
                    .Returns(true);
            }

            return Domain.Renters.Renter.CreateAsync(
                id: id ?? Constants.Constants.Renter.Id,
                userId: userId ?? Constants.Constants.Renter.UserId,
                cnpj: cnpj,
                email: email ?? Constants.Constants.Renter.Email,
                fullName: fullName ?? Constants.Constants.Renter.FullName,
                birthdate: birthdate ?? Constants.Constants.Renter.Birthdate,
                cnh: cnh,
                cnpjService: cnpjService,
                cnhService: cnhService);
        }
    }
}