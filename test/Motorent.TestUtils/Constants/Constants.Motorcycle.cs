using Motorent.Domain.Motorcycles.ValueObjects;

namespace Motorent.TestUtils.Constants;

public static partial class Constants
{
    public static class Motorcycle
    {
        public static readonly MotorcycleId Id = MotorcycleId.New();
        
        public static readonly string Model = "Titan 160cc ABS";
        
        public static readonly Year Year = Year.Create(DateTime.Today.Year - 2).Value;

        public static readonly LicensePlate LicensePlate = LicensePlate.Create("PIA2A91").Value;
    }
}