using Motorent.Domain.Common.Entities;
using Motorent.Domain.Renters.Enums;
using Motorent.Domain.Renters.Errors;
using Motorent.Domain.Renters.Events;
using Motorent.Domain.Renters.Services;
using Motorent.Domain.Renters.ValueObjects;

namespace Motorent.Domain.Renters;

public sealed class Renter : Entity<RenterId>, IAggregateRoot
{
    private Renter(RenterId id) : base(id)
    {
    }

    public string UserId { get; init; } = null!;

    public Document Document { get; init; } = null!;

    public EmailAddress Email { get; init; } = null!;

    public FullName FullName { get; private set; } = null!;

    public Birthdate Birthdate { get; private set; } = null!;

    public DriverLicense DriverLicense { get; private set; } = null!;

    public DriverLicenseStatus DriverLicenseStatus { get; private set; } = null!;

    public Uri? DriverLicenseImageUrl { get; private set; }

    public static async Task<Result<Renter>> CreateAsync(
        RenterId id,
        string userId,
        Document document,
        EmailAddress email,
        FullName fullName,
        Birthdate birthdate,
        DriverLicense driverLicense,
        IDocumentService documentService,
        IDriverLicenseService driverLicenseService,
        CancellationToken cancellationToken = default)
    {
        if (!await documentService.IsUniqueAsync(document, cancellationToken))
        {
            return RenterErrors.DocumentNotUnique(document);
        }

        if (!await driverLicenseService.IsUniqueAsync(driverLicense, cancellationToken))
        {
            return RenterErrors.DriverLicenseNotUnique(driverLicense);
        }

        return new Renter(id)
        {
            UserId = userId,
            Document = document,
            Email = email,
            FullName = fullName,
            Birthdate = birthdate,
            DriverLicense = driverLicense,
            DriverLicenseStatus = DriverLicenseStatus.PendingValidation
        };
    }

    public void ChangePersonalInfo(FullName fullName, Birthdate birthdate)
    {
        FullName = fullName;
        Birthdate = birthdate;
    }

    public async Task<Result<Success>> ChangeDriverLicenseAsync(
        DriverLicense driverLicense,
        IDriverLicenseService driverLicenseService,
        CancellationToken cancellationToken = default)
    {
        if (!IsDriverLicensePendingValidation())
        {
            return RenterErrors.DriverLicenseNotPendingValidation;
        }

        if (!await driverLicenseService.IsUniqueAsync(driverLicense, cancellationToken))
        {
            return RenterErrors.DriverLicenseNotUnique(driverLicense);
        }

        DriverLicense = driverLicense;

        return Success.Value;
    }

    public Result<Success> SendDriverLicenseImage(Uri driverLicenseImageUrl)
    {
        if (!IsDriverLicensePendingValidation())
        {
            return RenterErrors.DriverLicenseNotPendingValidation;
        }

        DriverLicenseStatus = DriverLicenseStatus.WaitingApproval;
        DriverLicenseImageUrl = driverLicenseImageUrl;

        RaiseEvent(new DriverLicenseImageSent(Id, DriverLicenseImageUrl));

        return Success.Value;
    }

    public Result<Success> ApproveDriverLicense()
    {
        if (DriverLicenseStatus != DriverLicenseStatus.WaitingApproval)
        {
            return RenterErrors.DriverLicenseNotWaitingApproval;
        }

        DriverLicenseStatus = DriverLicenseStatus.Approved;

        return Success.Value;
    }

    public Result<Success> RejectDriverLicense()
    {
        if (DriverLicenseStatus != DriverLicenseStatus.WaitingApproval)
        {
            return RenterErrors.DriverLicenseNotWaitingApproval;
        }

        DriverLicenseStatus = DriverLicenseStatus.Rejected;
        DriverLicenseImageUrl = null;

        return Success.Value;
    }

    private bool IsDriverLicensePendingValidation() => DriverLicenseStatus == DriverLicenseStatus.PendingValidation
                                                       || DriverLicenseStatus == DriverLicenseStatus.Rejected;
}