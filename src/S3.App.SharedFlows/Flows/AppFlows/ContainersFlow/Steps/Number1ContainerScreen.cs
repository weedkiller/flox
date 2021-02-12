using System.Threading.Tasks;
using S3.App.Flows.AppFlows.ContainersFlow.FlowDefinitions;
using S3.UiFlows.Core.Configuration;
using S3.UiFlows.Core.DataSources;
using S3.UiFlows.Core.Flows.Screens;
using S3.UiFlows.Core.Flows.Screens.Models;

namespace S3.App.Flows.AppFlows.ContainersFlow.Steps
{
	public class Number1ContainerScreen : ContainersFlowScreen
	{
		public override ScreenName ScreenStep =>  ContainersFlowScreenName.Number1ContainerScreen;

		protected override IScreenFlowConfigurator OnRegisterUserActions(
			IScreenFlowConfigurator screenConfiguration, IUiFlowContextData contextData)
		{
			return screenConfiguration.OnEventReentriesCurrent(ScreenEvent.ErrorOccurred)
				.OnEventNavigatesTo(StepEvent.Step2, ContainersFlowScreenName.Number2ContainerScreen);
		}

		protected override async Task<UiFlowScreenModel> OnCreateModelAsync(IUiFlowContextData contextData)
		{
			var result = await base.OnCreateModelAsync(contextData);
			result.SetContainedFlow("GreenFlow");
			return result;
		}

		public static class StepEvent
		{
			public static readonly ScreenEvent Step2 = new ScreenEvent(nameof(Number1ContainerScreen), nameof(Step2));
		}
	}
}