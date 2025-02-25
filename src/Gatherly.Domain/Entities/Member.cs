using Gatherly.Domain.Primitives;

namespace Gatherly.Domain.Entities;

public sealed class Member : Entity
{
    public Member(Guid id, string firstName, string lastName, string email)
        : base(id)
    {
        FirstName = firstName;
        LastName = lastName;
        Email = email;
    }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
}
