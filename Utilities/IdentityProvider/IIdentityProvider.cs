using System.Security.Principal;

namespace Utilities.IdentityProvider
{
    public interface IIdentityProvider
    {
        string WhoAmI(IIdentity identity);
    }
}