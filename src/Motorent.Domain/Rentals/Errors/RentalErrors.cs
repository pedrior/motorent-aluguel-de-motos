namespace Motorent.Domain.Rentals.Errors;

internal static class RentalErrors
{
    public static readonly Error RenterMustHaveCategoryADrivingLicense = Error.Failure(
        "O alugador deve ter carteira de habilitação na categoria A.",
        code: "rental.renter.must_have_category_a_driving_license");
}