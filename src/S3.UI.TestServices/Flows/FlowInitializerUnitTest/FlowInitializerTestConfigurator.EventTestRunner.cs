using System;
using NUnit.Framework;
using S3.UiFlows.Core.Flows.Screens;
using S3.UiFlows.Core.Flows.Screens.Models;

namespace S3.UI.TestServices.Flows.FlowInitializerUnitTest
{
	public partial class FlowInitializerTestConfigurator<TInitializer>
	{


		/// <summary>
		/// Retrieves a test runner specialized in testing events
		/// </summary>
		/// <returns></returns>
		public EventTestRunner NewEventTestRunner() => new EventTestRunner(Adapter);
		public class EventTestRunner
		{
			private readonly FlowInitializerWithLifecycleAdapter<TInitializer> _adapter;

			internal EventTestRunner(FlowInitializerWithLifecycleAdapter<TInitializer> adapter)
			{
				_adapter = adapter;
			}


			public EventTestAssert WhenEvent(ScreenEvent eventToTrigger)
			{
				return new EventTestAssert(_adapter,_adapter.RunNavigation(eventToTrigger));
			}

			public class EventTestAssert
			{
				private readonly FlowInitializerWithLifecycleAdapter<TInitializer> _adapter;
				private readonly ScreenName _result;

				public EventTestAssert(FlowInitializerWithLifecycleAdapter<TInitializer> adapter,ScreenName result)
				{
					_adapter = adapter;
					_result = result;
				}

				public EventTestAssert TheResultStepIs(Action<ScreenName> asserter)
				{
					asserter(_result);
					return this;
				}
				public EventTestAssert TheResultStepIs(ScreenName expected)
				{
					return TheResultStepIs((actual) => Assert.AreEqual(expected, actual));
				}
			}

			public EventTestRunner GivenTheStepDataIs<TStepData>(TStepData stepData) where TStepData:UiFlowScreenModel
			{
				_adapter.SetStepData(stepData);
				return this;
			}
		}
	}
}