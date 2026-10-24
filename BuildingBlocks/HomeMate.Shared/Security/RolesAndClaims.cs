namespace HomeMate.Shared.Security;

public static class Roles
{
    public const string Customer = "Customer";
    public const string Provider = "Provider";
    public const string Admin = "Admin";

    public static readonly IReadOnlyList<string> All = [Customer, Provider, Admin];
}

public static class ClaimTypesConstants
{
    public const string UserId = "uid";
    public const string Sub = "sub";
    public const string Email = "email";
    public const string Role = "role";
    public const string FirstName = "given_name";
    public const string LastName = "family_name";
}
