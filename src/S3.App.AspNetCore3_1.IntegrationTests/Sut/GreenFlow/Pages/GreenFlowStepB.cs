using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AngleSharp.Html.Dom;
using S3.App.AspNetCore3_1.IntegrationTests.Infrastructure;
using S3.UI.TestServices.Sut;

namespace S3.App.AspNetCore3_1.IntegrationTests.Sut.GreenFlow.Pages
{
	class GreenFlowStepB : SutPage<PrototypeApp>
	{

		private IHtmlElement _node;


		protected override bool IsInPage()
		{
			_node = _node ?? (IHtmlElement)Document.Body.QuerySelector("body > div");
			return _node.QuerySelector("div:nth-child(1) > div > h2")?.TextContent == "Process GreenFlow::FillDataStep_StepBScreen";
		}
		public GreenFlowStepB(PrototypeApp app) : base(app)
		{
		}
		public GreenFlowStepB(PrototypeApp app, IHtmlElement root) : base(app)
		{
			_node = root;
			if (_node != null && !this.IsInPage())
			{
				throw new InvalidOperationException("The requested page is not correct");
			}
		}

		public IEnumerable<string> Errors()
		{
			return Document.QuerySelectorAll("#GreenFlowPage > div:nth-child(3) > div > div > ul > li").Select(x => x.TextContent).Where(x => x.Trim().Any());
		}
		public IHtmlInputElement Input => Document.QuerySelector("#StepBValue1") as IHtmlInputElement;

		public GreenFlowStepB InputValues(string input)
		{
			Input.Value = input;
			return this;

		}

		public async Task<ISutApp> Next()
		{
			return await ClickOnElementByText("Next");
		}

		public async Task<ISutApp> Previous()
		{
			return await ClickOnElementByText("Previous");
		}
	}
}