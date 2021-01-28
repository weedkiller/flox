using System.Linq;
using System.Threading.Tasks;
using EI.RP.NavigationPrototype.Flows.AppFlows.BlueFlow.FlowDefinitions;
using EI.RP.NavigationPrototype.IntegrationTests.Sut.BlueFlow.Pages;
using EI.RP.NavigationPrototype.IntegrationTests.Sut.ContainersFlow.Pages.Container1;
using EI.RP.NavigationPrototype.IntegrationTests.Sut.GreenFlow.Pages;
using EI.RP.UI.TestServices.Sut;
using EI.RP.UiFlows.Core.Flows.Screens;
using EI.RP.UiFlows.Core.Flows.Screens.Metadata;
using NUnit.Framework;

namespace EI.RP.NavigationPrototype.IntegrationTests.Sut.ContainersFlow.Tests.BlueFlow.Step0
{
	[TestFixture]
	internal abstract class
		WhenInContainer_BlueFlowStep0Tests<TRootContainerPage> : ContainedBlueFlowTestsBase<TRootContainerPage>
		where TRootContainerPage : ISutPage
	{


		[Test]
		public async Task CanReloadPage()
		{
			Assert.IsNotNull(AsStep0());
			await App.ReloadCurrentPage();
			Assert.IsNotNull(AsStep0());
		}

		[Test]
		public async Task CanGoToNextContainerPage_AndKeepsStateWhenBack()
		{

			await App.ClickOnElementById("containerNext");
			await App.ClickOnElementById("containerPrevious");
			Assert.IsNotNull(AsStep0());
		}

		[Test]
		public async Task AndNoInput_Next_ShowsError()
		{
			var page = AsStep0();
			page.SampleInput.Value = "22";
			await page.Next();
			page = AsStep0();
			var errors = page.Errors();
			CollectionAssert.IsNotEmpty(errors);

			Assert.AreEqual("StepValue1 is required", errors.Single());
			Assert.AreEqual(errors.Single(), page.FieldValidatorValue);
		}

		[Test]
		public async Task AndNoComponentInput_Next_ShowsError()
		{
			var page = AsStep0();
			await page.ClickOnElementByText("Next");
			page = AsStep0();
			var errors = page.Errors().ToArray();
			Assert.AreEqual(2, errors.Length);

			Assert.AreEqual("StepValue1 is required", errors[0]);
			Assert.AreEqual("ERROR TEST VALUE", errors[1]);
			Assert.AreEqual(errors[0], page.FieldValidatorValue);
		}

		[Test]
		public async Task AndInputIsNumeric_Next_ShowsError()
		{
			var page = AsStep0();
			page.SampleInput.Value = "22";
			page.Input.Value = "22";
			await page.ClickOnElementByText("Next");
			page = AsStep0();
			var errors = page.Errors();
			CollectionAssert.IsNotEmpty(errors);
			Assert.AreEqual(string.Empty, page.FieldValidatorValue);
			Assert.AreEqual("Numeric only value  are not allowed", errors.Single());
		}

		[Test]
		public async Task AndInputIsA_Next_ShowsStepC_ThenPreviousKeptInput()
		{
			var page = AsStep0();
			page.SampleInput.Value = "22";
			page.Input.Value = "a";

			await page.Next();

			await AsStepC().Previous();

			page = ResolveImmediateContainer()
				.GetCurrentContained<BlueFlowStep0>();

			Assert.AreEqual("a", page.Input.Value);
			var errors = page.Errors();
			Assert.IsTrue(!errors.Any());
			Assert.AreEqual(string.Empty, page.FieldValidatorValue);
		}

		[Test]
		public async Task AndInputIsAnother_Next_ShowsStepA_ThenPreviousKeptInput()
		{
			var page = AsStep0();
			page.SampleInput.Value = "22";
			page.Input.Value = "sdfa";
			await page.Next();
			var blueFlowStepA = ResolveImmediateContainer()
				.GetCurrentContained<BlueFlowStepA>();
			await blueFlowStepA.Previous();
			page = ResolveImmediateContainer()
				.GetCurrentContained<BlueFlowStep0>();

			Assert.AreEqual("sdfa", page.Input.Value);
			var errors = page.Errors();
			Assert.IsTrue(!errors.Any());
			Assert.AreEqual(string.Empty, page.FieldValidatorValue);
		}

		[TestCase("")]
		[TestCase("22")]
		[TestCase("sadf")]
		public async Task Resets_RestartsStep0(string input)
		{
			var page = AsStep0();
			if (input != null)
			{
				page.Input.Value = input;
			}

			await page.Reset();
			page = ResolveImmediateContainer()
				.GetCurrentContained<BlueFlowStep0>();
			CollectionAssert.IsEmpty(page.Errors());
			Assert.IsEmpty(page.Input.Value);
			Assert.AreEqual(string.Empty, page.FieldValidatorValue);
		}

		[Test]
		public async Task OpenGreenFlow_ShowsTheFlow()
		{
			var page = AsStep0();
			await page.ClickOnOpenSibling();
			var greenFlowStep0 = ResolveImmediateContainer()
				.GetCurrentContained<GreenFlowStep0>();
			Assert.IsNotNull(greenFlowStep0);
		}
		[Test]
		public async Task FailInitialisationShowsErrorPage()
		{
			await AsStep0().ClickFailInitialisation();
			var errorPage = ResolveImmediateContainer().GetCurrentContained<BlueFlowErrorScreen>();
			Assert.IsNotNull(errorPage);
			Assert.AreEqual("Triggering failure on FlowInitializer", errorPage.Error);
			Assert.AreEqual(ScreenName.PreStart.ToString(), errorPage.Step);
			Assert.AreEqual(ScreenLifecycleStage.FlowInitialization.ToString(), errorPage.OnLifecycleEvent);
		}

		[Test]
		public async Task FailCreationShowsErrorPage()
		{
			await AsStep0().ClickFailCreatingScreen();
			var errorPage = ResolveImmediateContainer().GetCurrentContained<BlueFlowErrorScreen>();
			Assert.IsNotNull(errorPage);
			Assert.AreEqual($"Failing on {BlueFlowScreenName.Step0Screen}.{ScreenLifecycleStage.CreatingStepData}", errorPage.Error);
			Assert.AreEqual(BlueFlowScreenName.Step0Screen.ToString(), errorPage.Step);
			Assert.AreEqual(ScreenLifecycleStage.CreatingStepData.ToString(), errorPage.OnLifecycleEvent);
		}
	}
}