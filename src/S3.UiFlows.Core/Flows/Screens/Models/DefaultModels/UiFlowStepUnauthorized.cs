namespace S3.UiFlows.Core.Flows.Screens.Models.DefaultModels
{
	public class UiFlowStepUnauthorized : UiFlowScreenModel
	{
		public override bool IsValidFor(ScreenName screenStep)
		{
			return true;
		}
	}
}