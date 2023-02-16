using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

namespace BlazorApp88AAdTestApp.Client
{
	public class Program
	{
		public static async Task Main(string[] args)
		{
			var builder = WebAssemblyHostBuilder.CreateDefault(args);
			builder.RootComponents.Add<App>("#app");
			builder.RootComponents.Add<HeadOutlet>("head::after");

			builder.Services.AddHttpClient("BlazorApp88AAdTestApp.ServerAPI", client => client.BaseAddress = new Uri(builder.HostEnvironment.BaseAddress))
					.AddHttpMessageHandler<BaseAddressAuthorizationMessageHandler>();

			// Supply HttpClient instances that include access tokens when making requests to the server project
			builder.Services.AddScoped(sp => sp.GetRequiredService<IHttpClientFactory>().CreateClient("BlazorApp88AAdTestApp.ServerAPI"));

			builder.Services.AddMsalAuthentication(options =>
			{
				//options.ProviderOptions.LoginMode = "redirect";
				builder.Configuration.Bind("AzureAd", options.ProviderOptions.Authentication);
				string? scopes =
					builder.Configuration!.GetSection("ServerApi")["Scopes"]
					?? throw new InvalidOperationException("ServerApi::Scopes is missing from appsettings.json");
				options.ProviderOptions.DefaultAccessTokenScopes.Add(scopes);
				options.ProviderOptions.LoginMode = "redirect";
			});
			await builder.Build().RunAsync();
		}
	}
}