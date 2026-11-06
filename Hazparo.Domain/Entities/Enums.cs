namespace Hazparo.Domain.Entities;

public enum UserRole
{
    Client,
    Professional,
    Admin
}

public enum AccountStatus
{
    Pending,
    Active,
    Suspended,
    Deleted
}

public enum VerificationStatus
{
    NotSubmitted,
    Pending,
    Verified,
    Rejected
}

public enum ServiceRequestStatus
{
    Draft,
    Open,
    Quoted,
    Assigned,
    Accepted,
    EnRoute,
    InProgress,
    Completed,
    Cancelled,
    Expired
}

public enum QuoteStatus
{
    Sent,
    Accepted,
    Declined,
    Withdrawn,
    Expired
}
