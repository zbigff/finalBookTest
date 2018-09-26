using Microsoft.Azure.WebJobs.Host;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ServerlessImageManagement
{
    public class AuthorizeFunctionAttribute : FunctionInvocationFilterAttribute
    {
        public override Task OnExecutingAsync(FunctionExecutingContext executingContext, CancellationToken cancellationToken)
        {
            if (!Thread.CurrentPrincipal.Identity.IsAuthenticated)
                throw new UnauthorizedException("Request is not authenticated");
            return base.OnExecutingAsync(executingContext, cancellationToken);
        }
    }

    public class UnauthorizedException : Exception
    {
        public UnauthorizedException(string message) : base(message)
        {
        }
    }
}
