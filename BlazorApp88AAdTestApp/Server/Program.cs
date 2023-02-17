using BlazorApp88AAdTestApp.Server;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Identity.Web;
using Microsoft.IdentityModel.Logging;
using Microsoft.IdentityModel.Tokens;
using System.Text;

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

#if DEBUG
      IdentityModelEventSource.ShowPII = true;
#endif
      //builder.Services
      //	.AddMicrosoftIdentityWebApiAuthentication(builder.Configuration, "AzureAd")
      //	.EnableTokenAcquisitionToCallDownstreamApi()
      //	.AddInMemoryTokenCaches();
      builder.Services
        .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
        .AddJwtBearer(options =>
        {
          var originalEvents = options.Events;
          options.Events = new JwtBearerEvents()
          {
            OnAuthenticationFailed = context =>
            {
              return originalEvents?.OnAuthenticationFailed(context) ?? Task.CompletedTask;
            },
            OnChallenge = context =>
            {
              return originalEvents?.OnChallenge(context) ?? Task.CompletedTask;
            },
            OnForbidden = context =>
            {
              return originalEvents?.OnForbidden(context) ?? Task.CompletedTask;
            },
            OnMessageReceived = context =>
            {
              return originalEvents?.OnMessageReceived(context) ?? Task.CompletedTask;
            },
            OnTokenValidated = context =>
            {
              return originalEvents?.OnTokenValidated(context) ?? Task.CompletedTask;
            }
          };
          options.Authority = "https://sts.windows.net/47f62749-2a26-422a-b1ba-f6c1d8d66eb3/";
          options.Audience = "api://8244fb6c-364c-4762-a078-70880972f7f0";
          options.TokenValidationParameters = new TokenValidationParameters
          {
            ValidateIssuerSigningKey = false,
            //IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("EKM8Q~PdGBdkPFy_cTKrGh2PdHK~Yp9beLOrNcsw")),
            ValidateIssuer = true,
            ValidIssuer = "https://sts.windows.net/47f62749-2a26-422a-b1ba-f6c1d8d66eb3/",
            ValidateAudience = true,
            ValidAudience = "api://8244fb6c-364c-4762-a078-70880972f7f0",
            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero
          };
        });
        //.AddMicrosoftIdentityWebApi(builder.Configuration.GetSection("AzureAd"));
      builder.Services.AddAuthorization();
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
      app.UseCors("eggs");
      app.UseAuthentication();
      app.UseAuthorization();

      app.MapRazorPages();
      app.MapControllers();
      app.MapFallbackToFile("index.html");

      app.Run();
    }
  }
}