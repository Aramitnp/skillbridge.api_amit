using System.Security.Cryptography.X509Certificates;

public class CreateUserRequestDto
{
    public string Name { get; set; }

    public string Email { get; set; }

    public string Password { get; set; }

    public string type { get; set; }
}