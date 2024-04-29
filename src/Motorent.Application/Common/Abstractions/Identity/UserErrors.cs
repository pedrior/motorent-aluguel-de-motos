namespace Motorent.Application.Common.Abstractions.Identity;

public static class UserErrors
{
    public static readonly Error DuplicateEmail = Error.Conflict("The email address is already in use.");
    
    public static readonly Error InvalidCredentials = Error.Unauthorized("The email address or password is incorrect.");
}