namespace Studdit.Application.Common.Attributes
{
    /// <summary>
    /// Attribute to mark commands/queries that require authentication
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class RequireAuthenticationAttribute : Attribute
    {
    }
}
