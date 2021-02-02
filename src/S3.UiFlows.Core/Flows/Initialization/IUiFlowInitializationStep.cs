using System.Collections.Generic;
using System.Threading.Tasks;
using S3.UiFlows.Core.Configuration;
using S3.UiFlows.Core.DataSources;
using S3.UiFlows.Core.Facade.Metadata;
using S3.UiFlows.Core.Flows.Screens;

namespace S3.UiFlows.Core.Flows.Initialization
{
	public interface IUiFlowInitializationStep
	{
		Task<ScreenEvent> InitializeContext(UiFlowContextData newContext, IDictionary<string, object> flowInputData,
			ScreenEvent defaultEventToTrigger);

		IScreenFlowConfigurator OnDefiningAdditionalInitialStateTransitions(
			IScreenFlowConfigurator preStartCfg, UiFlowContextData contextData);

		bool Authorize();
		
	}

	public interface IUiFlowInitializationStep<out TFlowType> : IUiFlowInitializationStep
	{
		TFlowType InitializerOfFlowType { get; }
	}
}