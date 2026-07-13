namespace SkillBridge.Api.Repositories;

public sealed class DuplicateEmailException : Exception
{
    public DuplicateEmailException()
        : base("A user with this email address already exists.")
    {
    }

    public DuplicateEmailException(Exception innerException)
        : base("A user with this email address already exists.", innerException)
    {
    }
}
