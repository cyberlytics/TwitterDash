using conductor.activities;
using conductor.background_services;
using conductor.factories;
using conductor.workflows;
using Twitterdash;

AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", true);


var trendServiceIP = Environment.GetEnvironmentVariable("TRENDSERVICE_IP") ?? "localhost";
var trendServicePort = Environment.GetEnvironmentVariable("TRENDSERVICE_PORT") ?? "50051";

var builder = WebApplication.CreateBuilder(args);

var elsaConfig = builder.Configuration.GetSection("Elsa");

builder.Services.AddElsa(elsa => elsa
                    .AddConsoleActivities()
                    .AddHttpActivities(elsaConfig.GetSection("Server").Bind)
                    .AddActivity<ValidateTrends>()
                    .AddActivity<PersistTrends>()
                    .AddWorkflow<TrendWorkflow>());

builder.Services.AddElsaApiEndpoints();
builder.Services.AddRazorPages();

builder.Services.AddSingleton<TrendProvider.TrendProviderClient>((serviceProvider) =>
grpcClientFactory.BuildClient<TrendProvider.TrendProviderClient>($"http://{trendServiceIP}:{trendServicePort}"));

builder.Services.AddHostedService((sp) => sp.GetRequiredService<TrendProviderService>());
builder.Services.AddSingleton<TrendProviderService>();
builder.Services.AddSingleton<WorkflowController>();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
    app.UseDeveloperExceptionPage();
}

app.UseStaticFiles() // For Dashboard.
    .UseHttpActivities()
    .UseRouting()
    .UseEndpoints(endpoints =>
    {
        // Elsa API Endpoints are implemented as regular ASP.NET Core API controllers.
        endpoints.MapControllers();

        // For Dashboard.
        endpoints.MapFallbackToPage("/_Host");
    });

app.Services.GetRequiredService<WorkflowController>();
//trendService.OnTrendsRecieved += (_, resonse) => { 
//    Console.WriteLine(resonse.ToString());
//};
//var task = Task.Run(async () =>
//{
//    while (true)
//    {
//        using (var scope = app.Services.CreateScope())
//        {
//            var workflowRunner = scope.ServiceProvider.GetRequiredService<IBuildsAndStartsWorkflow>();
//            await workflowRunner.BuildAndStartWorkflowAsync<TrendWorkflow>();
//            await Task.Delay(3000);
//        }
//    }
//});
app.Run();
