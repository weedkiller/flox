using System.Threading.Tasks;
using NUnit.Framework;
using S3.App.AspNetCore3_1.IntegrationTests.Sut.BlueFlow.Pages;
using S3.App.AspNetCore3_1.IntegrationTests.Sut.ContainersFlow.Pages;
using S3.App.AspNetCore3_1.IntegrationTests.Sut.ContainersFlow.Pages.Container1;
using S3.App.AspNetCore3_1.IntegrationTests.Sut.ContainersFlow.Pages.Container2;

namespace S3.App.AspNetCore3_1.IntegrationTests.Sut.ContainersFlow.Tests.BlueFlow.Step0
{
	[TestFixture]
	internal class WhenMultipleNestingFlowStep0Tests : WhenInContainerFlowStep0Tests<Container2Page0>
	{

		protected override async Task TestScenarioArrangement()
		{
			var container2FlowPage = await App.ToContainer2Flow();
			await container2FlowPage.SelectContainerFlow();
			container2FlowPage = App.CurrentPageAs<Container2Page0>();
			
			var containersPage = container2FlowPage.GetCurrentContained<Container1Page0>();
			await containersPage.SelectBlueFlow();
			Assert.IsNotNull(AsStep<BlueFlowStep0>());
		}

		protected override IContainerPage ResolveImmediateContainer()
		{
			var page = App.CurrentPageAs<Container2Page0>();
			return page.GetCurrentContained<Container1Page0>();
		}

	

	}
}