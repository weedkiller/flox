using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;

namespace S3.Mvc.Core.ViewModels
{
	public class HttpResponseMessageResult : IActionResult
	{
		private readonly HttpResponseMessage _responseMessage;

		public HttpResponseMessageResult(HttpResponseMessage responseMessage)
		{
			_responseMessage = responseMessage ?? throw new ArgumentNullException(nameof(responseMessage)); 
		}

		public async Task ExecuteResultAsync(ActionContext context)
		{
			context.HttpContext.Response.StatusCode = (int)_responseMessage.StatusCode;

			foreach (var header in _responseMessage.Headers)
			{
				context.HttpContext.Response.Headers.TryAdd(header.Key, new StringValues(header.Value.ToArray()));
			}

			using (var stream = await _responseMessage.Content.ReadAsStreamAsync())
			{
				await stream.CopyToAsync(context.HttpContext.Response.Body);
				await context.HttpContext.Response.Body.FlushAsync();
			}
		}
	}
}