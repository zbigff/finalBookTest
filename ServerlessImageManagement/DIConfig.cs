using Autofac;
using AzureFunctions.Autofac.Configuration;
using Utilities;
using Utilities.IdentityProvider;

namespace ServerlessImageManagement
{
    public class DIConfig
    {
        public DIConfig(string functionName)
        {
            DependencyInjection.Initialize(builder =>
            {
                builder.RegisterType<RecOrderValidator>().As<IRecOrderValidator>();
                builder.RegisterType<IdentityProvider>().As<IIdentityProvider>();
            }, functionName);
        }
    }
}

