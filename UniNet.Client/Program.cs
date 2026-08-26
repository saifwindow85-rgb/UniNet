using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using UniNet.Client;
using UniNet.Client.Services.Http;
using UniNet.Client.Services.Identity;
using UniNet.Client.State;
using UniNet.Client.Services.Academic;
using UniNet.Client.Services.Employee;
using UniNet.Client.Services.Study;
using UniNet.Client.Services.Students;

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

// خدمات الهوية (تستخدم العميل المُصادَق "Api").
builder.Services.AddScoped<RoleApiService>();
builder.Services.AddScoped<UserApiService>();
builder.Services.AddScoped<UserRoleApiService>();

// النطاق + خدمات الهيكل الأكاديمي.
builder.Services.AddScoped<ScopeContext>();
builder.Services.AddScoped<UniversityApiService>();
builder.Services.AddScoped<CollegeApiService>();
builder.Services.AddScoped<DepartmentApiService>();
builder.Services.AddScoped<BatchApiService>();
builder.Services.AddScoped<SectionApiService>();

// خدمة الموظفين (مسؤولو الجامعة/الكلية/القسم) — مُدركة للنطاق في الخادم.
builder.Services.AddScoped<EmployeeApiService>();

// وحدة الدراسة (المواد/الفصول/الربط/النتائج) + الطلاب وحالاتهم.
builder.Services.AddScoped<SubjectApiService>();
builder.Services.AddScoped<SemesterApiService>();
builder.Services.AddScoped<SectionSubjectApiService>();
builder.Services.AddScoped<StudentResultApiService>();
builder.Services.AddScoped<StudentApiService>();
builder.Services.AddScoped<StudentStatusApiService>();

// مساعد قوائم الاختيار المُدركة للنطاق (يعالج قيد endpoint الأقسام).
builder.Services.AddScoped<UniNet.Client.Services.Lookups.ScopeLookups>();

builder.Services.AddScoped(sp => new AuthApiService(
    sp.GetRequiredService<IHttpClientFactory>().CreateClient("PublicApi"),
    sp.GetRequiredService<TokenStore>(),
    sp.GetRequiredService<JwtAuthStateProvider>()));

await builder.Build().RunAsync();
