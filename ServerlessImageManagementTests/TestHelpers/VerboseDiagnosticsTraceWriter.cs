using System.Diagnostics;
using Microsoft.Azure.WebJobs.Host;

namespace ServerlessImageManagementTests.TestHelpers
{
    public class VerboseDiagnosticsTraceWriter : TraceWriter
    {

        public VerboseDiagnosticsTraceWriter() : base(TraceLevel.Verbose)
        {

        }
        public override void Trace(TraceEvent traceEvent)
        {
            Debug.WriteLine(traceEvent.Message);
        }
    }
}

