namespace Studdit.Application.Common.Attributes
{
    /// <summary>
    /// Attribute to mark commands/queries that require specific permissions
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class RequirePermissionAttribute : Attribute
    {
        public string Permission { get; }

        public RequirePermissionAttribute(string permission)
        {
            Permission = permission;
        }
    }
}
