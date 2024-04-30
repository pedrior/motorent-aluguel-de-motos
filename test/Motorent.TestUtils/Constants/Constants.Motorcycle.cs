using Motorent.Domain.Common.ValueObjects;
using Motorent.Domain.Motorcycles.Enums;
using Motorent.Domain.Motorcycles.ValueObjects;

namespace Motorent.TestUtils.Constants;

public static partial class Constants
{
    public static class Motorcycle
    {
        public static readonly MotorcycleId Id = MotorcycleId.New();
        
        public static readonly string Model = "Titan 160cc ABS";
        
        public static readonly Brand Brand = Brand.Honda;

        public static readonly Year Year = Year.Create(DateTime.Today.Year - 2).Value;
        
        public static readonly Money DailyPrice = Money.Create(38.99m).Value;

        public static readonly LicensePlate LicensePlate = LicensePlate.Create("PIA-2A91").Value;
    }
}