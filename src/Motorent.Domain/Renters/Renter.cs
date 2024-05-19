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

    public CNH CNH { get; private set; } = null!;

    public CNHStatus CNHStatus { get; private set; } = null!;
    
    public Uri? CNHImageUrl { get; private set; }

    public static async Task<Result<Renter>> CreateAsync(
        RenterId id,
        string userId,
        Document document,
        EmailAddress email,
        FullName fullName,
        Birthdate birthdate,
        CNH cnh,
        IDocumentService documentService,
        ICNHService cnhService,
        CancellationToken cancellationToken = default)
    {
        if (!await documentService.IsUniqueAsync(document, cancellationToken))
        {
            return RenterErrors.DocumentNotUnique(document);
        }

        if (!await cnhService.IsUniqueAsync(cnh, cancellationToken))
        {
            return RenterErrors.CNHNotUnique(cnh);
        }

        return new Renter(id)
        {
            UserId = userId,
            Document = document,
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

    public Result<Success> SendCNHImage(Uri imageUrl)
    {
        if (!IsCNHPendingValidation())
        {
            return RenterErrors.CNHIsNotPendingValidation;
        }

        CNHStatus = CNHStatus.WaitingApproval;
        CNHImageUrl = imageUrl;

        RaiseEvent(new CNHImageSent(Id, CNHImageUrl));
        
        return Success.Value;
    }

    public Result<Success> ApproveCNH()
    {
        if (CNHStatus != CNHStatus.WaitingApproval)
        {
            return RenterErrors.CNHIsNotWaitingApproval;
        }
        
        CNHStatus = CNHStatus.Approved;
        
        return Success.Value;
    }
    
    public Result<Success> RejectCNH()
    {
        if (CNHStatus != CNHStatus.WaitingApproval)
        {
            return RenterErrors.CNHIsNotWaitingApproval;
        }
        
        CNHStatus = CNHStatus.Rejected;
        CNHImageUrl = null;

        return Success.Value;
    }
    
    private bool IsCNHPendingValidation() => CNHStatus == CNHStatus.PendingValidation 
                                             || CNHStatus == CNHStatus.Rejected;
}