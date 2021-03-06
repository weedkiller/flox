using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using S3.UI.TestServices.Flows.Shared;
using S3.UiFlows.Core.Configuration;
using S3.UiFlows.Core.Flows.Screens;

namespace S3.UI.TestServices.Flows.FlowScreenUnitTest
{

	internal class TestFlowScreenConfigurator : TestFlowNavigationHelper, IInternalScreenFlowConfigurator
	{
		private readonly IUiFlowScreen _step;

		public TestFlowScreenConfigurator(IUiFlowScreen step) : base(step.ScreenNameId)
		{
			_step = step;
		}

		

		public void OnEntry(Func<Task> action, string entryActionDescription = null)
		{
			throw new NotSupportedException();
		}

		public void AddErrorTransitionIfUndefined()
		{
			if (!NavigationResolvers.ContainsKey(ScreenEvent.ErrorOccurred))
				//Add default reentry on error
				OnEventReentriesCurrent(ScreenEvent.ErrorOccurred);
		}

		public string ScreenName => _step.ToString();

		public IEnumerable<ScreenTransition> Transitions =>
			NavigationResolvers.Select(x => new ScreenTransition(x.Key, x.Value.Resolve()));

		protected override ScreenName OnExecute(NavigationResolver navigationResolver)
		{
			return navigationResolver.Resolve();
		}
	}
}