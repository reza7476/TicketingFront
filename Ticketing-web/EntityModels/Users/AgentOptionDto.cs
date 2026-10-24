namespace Ticketing_web.EntityModels.Users;

public class AgentOptionDto
{
    public string Id { get; set; } = string.Empty;

    public string Name { get; set; } = string.Empty;

    public string LastName { get; set; } = string.Empty;

    public string FullName => $"{Name} {LastName}";
}
