using Aspire.Web;
using Aspire.Web.Entities;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using MudBlazor.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddMudServices();

builder.Services.Configure<HubCfg>(o => o.Url = builder.Configuration["hub_url"] ?? throw new Exception("hub_url not found"));

builder.Services.AddTransient(sp => new HttpClient {
    BaseAddress = new Uri(builder.Configuration["csgo_url"] ??
                          throw new Exception("csgo_url not found"))
});


await builder.Build().RunAsync();