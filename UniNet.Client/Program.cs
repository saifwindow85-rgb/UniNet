using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using UniNet.Client;
using UniNet.Client.Services.Http;
using UniNet.Client.Services.Identity;
using UniNet.Client.State;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

// عنوان الـ API من wwwroot/appsettings.json (بديله عنوان الاستضافة نفسه إن غاب).
var apiBaseUrl = builder.Configuration["ApiBaseUrl"] ?? builder.HostEnvironment.BaseAddress;
builder.Services.AddSingleton(new ApiBaseAddress(apiBaseUrl));

// المصادقة والنطاق.
builder.Services.AddAuthorizationCore();
builder.Services.AddScoped<TokenStore>();
builder.Services.AddScoped<JwtAuthStateProvider>();
builder.Services.AddScoped<AuthenticationStateProvider>(sp => sp.GetRequiredService<JwtAuthStateProvider>());
builder.Services.AddTransient<AuthTokenHandler>();

// عميل عامّ (بلا رمز) لعمليات المصادقة، وعميل مُصادَق (مع تدوير الرمز) لبقية الـ API.
builder.Services.AddHttpClient("PublicApi", c => c.BaseAddress = new Uri(apiBaseUrl));
builder.Services.AddHttpClient("Api", c => c.BaseAddress = new Uri(apiBaseUrl))
    .AddHttpMessageHandler<AuthTokenHandler>();

builder.Services.AddScoped(sp => new AuthApiService(
    sp.GetRequiredService<IHttpClientFactory>().CreateClient("PublicApi"),
    sp.GetRequiredService<TokenStore>(),
    sp.GetRequiredService<JwtAuthStateProvider>()));

await builder.Build().RunAsync();
