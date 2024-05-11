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

    public CNPJ CNPJ { get; init; } = null!;

    public EmailAddress Email { get; init; } = null!;

    public FullName FullName { get; private set; } = null!;

    public Birthdate Birthdate { get; private set; } = null!;

    public CNH CNH { get; private set; } = null!;

    public CNHStatus CNHStatus { get; private set; } = null!;
    
    public CNHValidationImages? CNHValidationImages { get; private set; }

    public static async Task<Result<Renter>> CreateAsync(
        RenterId id,
        string userId,
        CNPJ cnpj,
        EmailAddress email,
        FullName fullName,
        Birthdate birthdate,
        CNH cnh,
        ICNPJService cnpjService,
        ICNHService cnhService,
        CancellationToken cancellationToken = default)
    {
        if (!await cnpjService.IsUniqueAsync(cnpj, cancellationToken))
        {
            return RenterErrors.CNPJNotUnique(cnpj);
        }

        if (!await cnhService.IsUniqueAsync(cnh, cancellationToken))
        {
            return RenterErrors.CNHNotUnique(cnh);
        }

        return new Renter(id)
        {
            UserId = userId,
            CNPJ = cnpj,
            Email = email,
            FullName = fullName,
            Birthdate = birthdate,
            CNH = cnh,
            CNHStatus = CNHStatus.PendingValidation
        };
    }

    public void ChangePersonalInfo(FullName fullName, Birthdate birthdate)
    {
        FullName = fullName;
        Birthdate = birthdate;
    }

    public async Task<Result<Success>> ChangeCNHAsync(
        CNH cnh,
        ICNHService cnhService,
        CancellationToken cancellationToken = default)
    {
        if (!IsCNHPendingValidation())
        {
            return RenterErrors.CNHIsNotPendingValidation;
        }
        
        if (!await cnhService.IsUniqueAsync(cnh, cancellationToken))
        {
            return RenterErrors.CNHNotUnique(cnh);
        }

        CNH = cnh;

        return Success.Value;
    }

    public Result<Success> SendCNHImagesForValidation(CNHValidationImages cnhValidationImages)
    {
        if (!IsCNHPendingValidation())
        {
            return RenterErrors.CNHIsNotPendingValidation;
        }

        CNHStatus = CNHStatus.WaitingApproval;
        CNHValidationImages = cnhValidationImages;

        RaiseEvent(new CNHImagesSentForValidationEvent(Id, CNHValidationImages));
        
        return Success.Value;
    }

    public Result<Success> SetCNHApprovedStatus()
    {
        if (CNHStatus != CNHStatus.WaitingApproval)
        {
            return RenterErrors.CNHIsNotWaitingApproval;
        }
        
        CNHStatus = CNHStatus.Approved;
        
        RaiseEvent(new CNHStatusChangedToApprovedEvent(Id));
        
        return Success.Value;
    }
    
    public Result<Success> SetCNHRejectedStatus()
    {
        if (CNHStatus != CNHStatus.WaitingApproval)
        {
            return RenterErrors.CNHIsNotWaitingApproval;
        }
        
        CNHStatus = CNHStatus.Rejected;
        CNHValidationImages = null;
        
        RaiseEvent(new CNHStatusChangedToRejectedEvent(Id));
        
        return Success.Value;
    }
    
    private bool IsCNHPendingValidation() => CNHStatus == CNHStatus.PendingValidation 
                                             || CNHStatus == CNHStatus.Rejected;
}