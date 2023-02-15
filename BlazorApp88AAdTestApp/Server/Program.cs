using BlazorApp88AAdTestApp.Server;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Identity.Web;
using Microsoft.IdentityModel.Logging;

namespace BlazorApp88AAdTestApp
{
  public class Program
  {
    public static void Main(string[] args)
    {
      var builder = WebApplication.CreateBuilder(args);

      builder.Services.AddCors(options =>
      {
        options.AddPolicy(name: "eggs", builder => builder
        .AllowAnyOrigin()
        .AllowAnyHeader()
        .AllowAnyMethod());
      });

      builder.Services.Configure<JwtBearerOptions>(options =>
      {
        var prev = options.Events.OnTokenValidated;
        options.Events.OnTokenValidated = async context =>
        {
          await prev(context);
        };
      });
#if DEBUG
      IdentityModelEventSource.ShowPII = true;
#endif
      //builder.Services
      //	.AddMicrosoftIdentityWebApiAuthentication(builder.Configuration, "AzureAd")
      //	.EnableTokenAcquisitionToCallDownstreamApi()
      //	.AddInMemoryTokenCaches();
      builder.Services
        .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
        .AddMicrosoftIdentityWebApi(builder.Configuration.GetSection("AzureAd"));
      builder.Services.AddControllersWithViews();
      builder.Services.AddRazorPages();
      builder.Services.AddSingleton<ContentSecurityPolicyMiddleware>();


      var app = builder.Build();
      app.UseMiddleware<ContentSecurityPolicyMiddleware>();

      // Configure the HTTP request pipeline.
      if (app.Environment.IsDevelopment())
      {
        app.UseWebAssemblyDebugging();
      }
      else
      {
        app.UseExceptionHandler("/Error");
        // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
        app.UseHsts();
      }

      app.UseHttpsRedirection();

      app.UseBlazorFrameworkFiles();
      app.UseStaticFiles();

      app.UseRouting();
      //app.UseCors("eggs");
      app.UseAuthorization();

      app.MapRazorPages();
      app.MapControllers();
      app.MapFallbackToFile("index.html");

      app.Run();
    }
  }
}