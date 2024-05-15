namespace Motorent.Application.Common.Abstractions.Identity;

public static class UserErrors
{
    public static readonly Error DuplicateEmail = Error.Conflict("O endereço de e-mail já está em uso.");
    
    public static readonly Error InvalidCredentials = Error.Unauthorized(
        "O endereço de e-mail ou a senha estão incorretos.");
}