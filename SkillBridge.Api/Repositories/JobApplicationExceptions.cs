namespace SkillBridge.Api.Repositories;

public sealed class JobNotFoundException : Exception
{
    public JobNotFoundException() : base("Job was not found.")
    {
    }
}

public sealed class JobUnavailableException : Exception
{
    public JobUnavailableException()
        : base("The job is inactive or its deadline has passed.")
    {
    }
}

public sealed class DuplicateJobApplicationException : Exception
{
    public DuplicateJobApplicationException()
        : base("You have already applied for this job.")
    {
    }

    public DuplicateJobApplicationException(Exception innerException)
        : base("You have already applied for this job.", innerException)
    {
    }
}
