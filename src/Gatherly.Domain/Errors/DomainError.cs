using Gatherly.Domain.Shared;

namespace Gatherly.Domain.Errors;

public static class DomainError
{
    public static class Gathering
    {
        public static readonly Error InvitingCreator = new Error("Gathering.InvitingCreator", "Can't send invitation to the gathering creator.");

        public static readonly Error AlreadyPassed = new Error("Gathering.AlreadyPassed", "Can't send invitation for gathering in the past.");

        public static readonly Error Expired = new Error("Gathering.Expired", "Can't accept invitation for expired gathering.");
    }
}
