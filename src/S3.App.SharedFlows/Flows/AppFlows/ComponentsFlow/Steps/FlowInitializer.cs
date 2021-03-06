using S3.App.Flows.AppFlows.MetadataTestFlow.FlowDefinitions;
using S3.UiFlows.Core.Configuration;
using S3.UiFlows.Core.DataSources;
using S3.UiFlows.Core.Flows.Initialization;
using S3.UiFlows.Core.Flows.Screens;

namespace S3.App.Flows.AppFlows.ComponentsFlow.Steps
{
	public class FlowInitializer : UiFlowStarter
	{

		public override IScreenFlowConfigurator OnDefiningAdditionalInitialStateTransitions(IScreenFlowConfigurator preStartCfg,
			UiFlowContextData contextData)
		{
			return preStartCfg.OnEventNavigatesTo(ScreenEvent.Start, MetadataTestFlowScreenScreenName.Step0Screen);
		}

		public override bool Authorize()
		{
			return true;
		}

		
	}
}