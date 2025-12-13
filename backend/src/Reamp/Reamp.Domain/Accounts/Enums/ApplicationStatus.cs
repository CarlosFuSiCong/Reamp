namespace Reamp.Domain.Accounts.Enums
{
    public enum ApplicationStatus : int
    {
        Pending = 0,
        UnderReview = 1,
        Approved = 2,
        Rejected = 3,
        Cancelled = 4
    }
}
