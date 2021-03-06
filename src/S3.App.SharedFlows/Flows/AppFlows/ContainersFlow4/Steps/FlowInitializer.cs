using S3.App.Flows.AppFlows.ContainersFlow4.FlowDefinitions;
using S3.UiFlows.Core.Configuration;
using S3.UiFlows.Core.DataSources;
using S3.UiFlows.Core.Flows.Initialization;
using S3.UiFlows.Core.Flows.Screens;

namespace S3.App.Flows.AppFlows.ContainersFlow4.Steps
{
	public class FlowInitializer : UiFlowStarter
	{


		public override bool Authorize()
		{
			return true;
		}


		public override IScreenFlowConfigurator OnDefiningAdditionalInitialStateTransitions(IScreenFlowConfigurator preStartCfg,
			UiFlowContextData contextData)
		{
			return preStartCfg.OnEventNavigatesTo(ScreenEvent.Start, ContainersFlow4ScreenName.Number1ContainerScreen);
		}
	}
}