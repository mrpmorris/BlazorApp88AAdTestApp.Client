using BlazorApp88AAdTestApp.Server;
using Microsoft.Identity.Web;

#if DEBUG
Microsoft.IdentityModel.Logging.IdentityModelEventSource.ShowPII = true;
#endif

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(options =>
{
    options.AddPolicy(name: "eggs", builder => builder
        .WithOrigins("https://localhost:6510") // AllowAnyOrigin is not recommended
        .AllowAnyHeader()
        .AllowAnyMethod());
});

builder.Services.AddMicrosoftIdentityWebApiAuthentication(builder.Configuration);
builder.Services.AddAuthorization();
builder.Services.AddControllers();
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

app.UseCors("eggs");

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapRazorPages();
app.MapControllers();
app.MapFallbackToFile("index.html");

app.Run();