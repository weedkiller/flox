﻿
using System;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using EI.RP.CoreServices.Encryption;

using Ei.Rp.Mvc.Core.System;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Filters;


namespace Ei.Rp.Mvc.Core.Cryptography.AntiTampering
{
#if !FrameworkDeveloper
	[DebuggerStepThrough]
#endif

	[AttributeUsage(AttributeTargets.Class)]
	public sealed class DecryptHiddenInputsAttribute : ActionFilterAttribute
	{
		private static readonly HttpMethod[] HttpMethods = { HttpMethod.Post, HttpMethod.Put, HttpMethod.Patch };

		public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
		{
			var httpMethod = new HttpMethod(context.HttpContext.Request.Method);

			if (HttpMethods.Any(x => x.Equals(httpMethod)))
			{
				var encryptionService = context.HttpContext.Resolve<IEncryptionService>();

				await DecryptHiddenFormAsync(context, encryptionService);
			}
			await base.OnActionExecutionAsync(context, next);
		}

		

		private async Task DecryptHiddenFormAsync(ActionExecutingContext context, IEncryptionService encryptionService)
		{
			var formCollection = context.HttpContext.Request.Form.ToDictionary(x=>x.Key,x=>x.Value);

			
			var protectedProperties = formCollection.Keys
				.Where(x => !x.StartsWith(EncryptionConstants.TokenPrefixAntiTampering)).ToArray();

			foreach (var protectedProperty in protectedProperties)
			{

				try
				{
					var stringValues = formCollection[protectedProperty].FirstOrDefault();
					formCollection[protectedProperty] = await encryptionService.DecryptAsync(stringValues, true);
				}
				catch (Exception ex)
				{
					throw new HiddenInputTamperedException("A required security token was not supplied or was invalid.", ex);
				}

				
			}

			context.HttpContext.Request.Form = new FormCollection(formCollection);
		}
	}


}