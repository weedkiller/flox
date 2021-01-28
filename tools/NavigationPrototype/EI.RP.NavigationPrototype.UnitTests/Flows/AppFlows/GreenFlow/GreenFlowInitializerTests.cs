using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Threading.Tasks;
using AutoFixture;
using EI.RP.CoreServices.System;
using EI.RP.NavigationPrototype.Flows.AppFlows;
using EI.RP.NavigationPrototype.Flows.AppFlows.GreenFlow.FlowDefinitions;
using EI.RP.NavigationPrototype.Flows.AppFlows.GreenFlow.Steps;
using EI.RP.UI.TestServices.Flows;
using EI.RP.UI.TestServices.Flows.FlowInitializerUnitTest;
using EI.RP.UiFlows.Core.Flows;
using EI.RP.UiFlows.Core.Flows.Screens;
using NUnit.Framework;

namespace EI.RP.NavigationPrototype.UnitTests.Flows.AppFlows.GreenFlow
{
	[TestFixture]
	public class GreenFlowInitializerTests
	{
		private readonly IFixture _fixture = new Fixture();
		
		[Test, Theory]
		public void InitializationIsCorrect(bool withInputParameters)
		{
			var configurator = new FlowInitializerTestConfigurator<FlowInitializer, SampleAppFlowType>(_fixture);
			ExpandoObject p = null;
			if (withInputParameters)
			{
				p = new
				{
					sampleParameter = _fixture.Create<string>(),
					Another = _fixture.Create<string>()
				}.ToExpandoObject();
			}
			configurator.NewInitializationRunner().WithInput(withInputParameters ? p : null)
				.Run()
				.AssertStepData<FlowInitializer.StartScreenModel>(stepData =>
				{
					Assert.AreEqual(withInputParameters ? p.ToDynamic().sampleParameter : null,
						stepData.SampleParameter);
				})
				.AssertTriggeredEventIs((actual)=>Assert.AreEqual(ScreenEvent.Start,actual));
		}

		[Test]
		public  void FlowTypeIsCorrect()
		{
			var configurator = new FlowInitializerTestConfigurator<FlowInitializer, SampleAppFlowType>(_fixture);
			var adapter = configurator.Adapter;
			Assert.AreEqual(SampleAppFlowType.GreenFlow,adapter.GetFlowType() );

		}

		[Test]
		public  async Task CanAuthorize()
		{
			var configurator = new FlowInitializerTestConfigurator<FlowInitializer, SampleAppFlowType>(_fixture);
			var adapter = configurator.Adapter;
			await adapter.Initialize();
			Assert.IsTrue(adapter.Authorize());
		}

		public static IEnumerable<TestCaseData> HandlingIsCorrect_OnStartCases()
		{
			yield return new TestCaseData(new FlowInitializer.StartScreenModel(), GreenFlowScreenName.Step0Screen).SetName($"{nameof(HandlingIsCorrect_OnStartCases)}_1");
			yield return new TestCaseData(new FlowInitializer.StartScreenModel
			{
				SampleParameter = "asdf"
			}, GreenFlowScreenName.Step0Screen).SetName($"{nameof(HandlingIsCorrect_OnStartCases)}_2");

			yield return new TestCaseData(new FlowInitializer.StartScreenModel
			{
				SampleParameter = "Finish"
			}, GreenFlowScreenName.FlowCompletedScreen).SetName($"{nameof(HandlingIsCorrect_OnStartCases)}_3");
		}



		[TestCaseSource(nameof(HandlingIsCorrect_OnStartCases))]
		public void HandlingIsCorrect_OnStart(FlowInitializer.StartScreenModel whenDataIs, ScreenName expectedStep)
		{
			var configurator = new FlowInitializerTestConfigurator<FlowInitializer, SampleAppFlowType>(_fixture);
			configurator.NewEventTestRunner()
				.GivenTheStepDataIs(whenDataIs)
				.WhenEvent(ScreenEvent.Start)
				.TheResultStepIs((actualStep) => Assert.AreEqual(expectedStep, actualStep));

		}
	}
}