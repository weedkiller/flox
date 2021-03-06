using System.Linq;
using System.Threading.Tasks;
using NUnit.Framework;
using S3.App.AspNetCore3_1.IntegrationTests.Sut.BlueFlow.Pages;
using S3.UI.TestServices.Sut;

namespace S3.App.AspNetCore3_1.IntegrationTests.Sut.ContainersFlow.Tests.BlueFlow.StepB
{
	[TestFixture]
	internal abstract class
		WhenInContainerFlowStepBTests<TRootContainerPage> : ContainedFlowTestsBase<TRootContainerPage>
		where TRootContainerPage : ISutPage
	{
		protected BlueFlowStepB PageSut { get; set; }

		protected async Task<BlueFlowStepB> ResolveSut(BlueFlowStep0 step0)
		{
			await step0.InputValues("ggg", "just validate")
				.ClickOnElementByText("Next");
			await AsStep<BlueFlowStepA>().InputValues("AAA").ClickOnElementByText("Next");
			return AsStep<BlueFlowStepB>();
		}

		[Test]
		public async Task CanGoToNextContainerPage_AndKeepsStateWhenBack()
		{
			await App.ClickOnElementById("containerNext");
			await App.ClickOnElementById("containerPrevious");
			Assert.IsNotNull(AsStep<BlueFlowStepB>());

		}
		[Test]
		public async Task AndNoInput_Next_ShowsError()
		{
			await PageSut.ClickOnElementByText("Next");
			PageSut = AsStep<BlueFlowStepB>();

			var errors = PageSut.Errors();
			CollectionAssert.IsNotEmpty(errors);

			Assert.AreEqual("Value cannot be empty", errors.Single());
		}


		[Test]
		public async Task AndInputIsOk_Next_ShowsStepC_ThenPreviousKeptInput()
		{
			PageSut.Input.Value = "Ok";
			await PageSut.ClickOnElementByText("Next");

			await AsStep<BlueFlowStepC>().ClickOnElementByText("Previous");
			PageSut = AsStep<BlueFlowStepB>();

			Assert.AreEqual("Ok", PageSut.Input.Value);
			var errors = PageSut.Errors();
			Assert.IsTrue(!errors.Any());
		}


	
		[TestCase("22")]
		[TestCase("sadf")]
		public async Task Resets_RestartsStep0(string input)
		{
			if (input != null)
			{
				PageSut.Input.Value = input;
			}
			await PageSut.ClickOnElementByText("Reset");
			var step0 = AsStep<BlueFlowStep0>();
			CollectionAssert.IsEmpty(step0.Errors());
			Assert.IsEmpty(step0.Input.Value);
			Assert.AreEqual(string.Empty, step0.FieldValidatorValue);
		}
	}
}