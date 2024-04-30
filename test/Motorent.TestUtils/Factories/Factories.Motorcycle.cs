using Motorent.Domain.Common.ValueObjects;
using Motorent.Domain.Motorcycles.Enums;
using Motorent.Domain.Motorcycles.Services;
using Motorent.Domain.Motorcycles.ValueObjects;

namespace Motorent.TestUtils.Factories;

public static partial class Factories
{
    public static class Motorcycle
    {
        public static Task<Result<Domain.Motorcycles.Motorcycle>> CreateAsync(
            MotorcycleId? id = null,
            string? model = null,
            Brand? brand = null,
            Year? year = null,
            Money? dailyPrice = null,
            LicensePlate? licensePlate = null,
            ILicensePlateService? licensePlateService = null)
        {
            if (licensePlateService is null)
            {
                licensePlateService = A.Fake<ILicensePlateService>();
                A.CallTo(() => licensePlateService.IsUniqueAsync(A<LicensePlate>._, A<CancellationToken>._))
                    .Returns(true);
            }

            return Domain.Motorcycles.Motorcycle.CreateAsync(
                id ?? Constants.Constants.Motorcycle.Id,
                model ?? Constants.Constants.Motorcycle.Model,
                brand ?? Constants.Constants.Motorcycle.Brand,
                year ?? Constants.Constants.Motorcycle.Year,
                dailyPrice ?? Constants.Constants.Motorcycle.DailyPrice,
                licensePlate ?? Constants.Constants.Motorcycle.LicensePlate,
                licensePlateService);
        }
    }
}