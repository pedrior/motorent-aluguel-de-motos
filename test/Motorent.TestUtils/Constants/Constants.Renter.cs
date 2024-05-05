using Motorent.Domain.Renters.Enums;
using Motorent.Domain.Renters.ValueObjects;

namespace Motorent.TestUtils.Constants;

public static partial class Constants
{
    public static class Renter
    {
        public static readonly RenterId Id = RenterId.New();

        public static readonly string UserId = Ulid.NewUlid().ToString();

        public static readonly CNPJ CNPJ = CNPJ.Create("92.411.039/0001-10").Value;

        public static readonly EmailAddress Email = EmailAddress.Create("john@doe.com").Value;

        public static readonly FullName FullName = new("John", "Doe");

        public static readonly Birthdate Birthdate = Birthdate.Create(new DateOnly(2000, 09, 05)).Value;
        
        public static readonly CNH CNH = CNH.Create(
            "94171421375",
            CNHCategory.AB,
            new DateOnly(DateTime.Today.Year + 1, 01, 01)).Value;

        public static readonly CNHValidationImages CNHValidationImages = new(
            frontImageUrl: new Uri($"https://motorent-images.s3.amazonaws.com/renters/{Id}/cnh-front.jpg"),
            backImageUrl: new Uri($"https://motorent-images.s3.amazonaws.com/renters/{Id}/cnh-back.jpg"));
    }
}