using BrainRush.Components;
using BrainRush.Services;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

// Supabase: one client for the whole app, registered once and used with @inject.
// Url and Key come from wwwroot/appsettings.json (the publishable key is safe in the browser;
// the row level security policies in database/schema.sql are what protect the data).
var url = builder.Configuration["Supabase:Url"];
var key = builder.Configuration["Supabase:Key"];
if (string.IsNullOrWhiteSpace(url) || string.IsNullOrWhiteSpace(key) || url.Contains("YOUR-REF"))
    throw new InvalidOperationException("Set Supabase:Url and Supabase:Key in wwwroot/appsettings.json.");

var supabase = new Supabase.Client(url, key,
    new Supabase.SupabaseOptions { AutoConnectRealtime = false });
await supabase.InitializeAsync();
builder.Services.AddSingleton(supabase);

// Holds the current player ("you") so the side menu and pages share one copy.
builder.Services.AddSingleton<PlayerSession>();

await builder.Build().RunAsync();
