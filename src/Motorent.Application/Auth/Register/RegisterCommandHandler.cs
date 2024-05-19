using Motorent.Application.Common.Abstractions.Identity;
using Motorent.Application.Common.Abstractions.Requests;
using Motorent.Application.Common.Abstractions.Security;
using Motorent.Contracts.Auth.Responses;
using Motorent.Domain.Renters;
using Motorent.Domain.Renters.Enums;
using Motorent.Domain.Renters.Repository;
using Motorent.Domain.Renters.Services;
using Motorent.Domain.Renters.ValueObjects;

namespace Motorent.Application.Auth.Register;

internal sealed class RegisterCommandHandler(
    IUserService userService,
    ISecurityTokenProvider securityTokenProvider,
    IRenterRepository renterRepository,
    IDocumentService documentService,
    IDriverLicenseService driverLicenseService)
    : ICommandHandler<RegisterCommand, TokenResponse>
{
    public async Task<Result<TokenResponse>> Handle(RegisterCommand command, CancellationToken cancellationToken)
    {
        var document = Document.Create(command.Document);
        var email = EmailAddress.Create(command.Email);
        var fullName = new FullName(command.GivenName, command.FamilyName);
        var birthdate = Birthdate.Create(command.Birthdate);
        var driverLicense = DriverLicense.Create(
            command.DriverLicenseNumber,
            DriverLicenseCategory.FromName(command.DriverLicenseCategory, ignoreCase: true),
            command.DriverLicenseExpiry);

        var errors = ErrorCombiner.Combine(document, email, birthdate, driverLicense);
        if (errors.Any())
        {
            return errors;
        }

        var result = userService.CreateUserAsync(
            command.Email,
            command.Password,
            roles: [UserRoles.Renter],
            claims: new Dictionary<string, string>
            {
                [UserClaimTypes.GivenName] = command.GivenName,
                [UserClaimTypes.FamilyName] = command.FamilyName,
                [UserClaimTypes.Birthdate] = command.Birthdate.ToString("yyyy-MM-dd")
            },
            cancellationToken: cancellationToken);

        return await result
            .ThenAsync(userId => CreateRenterUserAsync(
                userId,
                document.Value,
                email.Value,
                fullName,
                birthdate.Value,
                driverLicense.Value,
                cancellationToken))
            .ThenAsync(renter => GenerateSecurityTokenAsync(renter.UserId, cancellationToken))
            .Then(securityToken => securityToken.Adapt<TokenResponse>());
    }

    private Task<Result<Renter>> CreateRenterUserAsync(
        string userId,
        Document document,
        EmailAddress email,
        FullName fullName,
        Birthdate birthdate,
        DriverLicense driverLicense,
        CancellationToken cancellationToken)
    {
        var result = Renter.CreateAsync(
            id: RenterId.New(),
            userId: userId,
            document: document,
            email: email,
            fullName: fullName,
            birthdate: birthdate,
            driverLicense: driverLicense,
            documentService: documentService,
            driverLicenseService: driverLicenseService,
            cancellationToken: cancellationToken);

        return result.ThenAsync(renter => renterRepository.AddAsync(renter, cancellationToken));
    }

    private Task<SecurityToken> GenerateSecurityTokenAsync(string userId, CancellationToken cancellationToken) =>
        securityTokenProvider.GenerateSecurityTokenAsync(userId, cancellationToken);
}