using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace S3.Web.DebugTools.Infrastructure.FlowDiagnostics
{
	public interface IFlowChangesRecorder
	{
		 Task<IDisposable> NewScopeAsync(string scopeId);
		IEnumerable<FlowRecorderState> Requests();
		IEnumerable<FlowContextTrace> GetFlowStateDifferences(string flowHandler);
	}
}