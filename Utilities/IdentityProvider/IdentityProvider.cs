using System.Security.Principal;

namespace Utilities.IdentityProvider
{
    public class IdentityProvider :  IIdentityProvider
    {
        public string WhoAmI(IIdentity identity)
        {
#if DEBUG
            return "dummyuser";
#endif
            return identity.Name.Replace("@", string.Empty).Replace(".", string.Empty);
        }
    }
}

