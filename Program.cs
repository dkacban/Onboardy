using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.SemanticKernel;
using Microsoft.Extensions.Configuration;
using Azure.Identity;
using Microsoft.Agents.Hosting.AspNetCore;
using Microsoft.Agents.Storage;
using Obnoarding.Agents;
using Obnoarding;
using Microsoft.AspNetCore.Http;
using System.IO;

var builder = WebApplication.CreateBuilder(args);

if (builder.Environment.IsDevelopment())
{
    builder.Configuration.AddUserSecrets<Program>();
}

builder.Services.AddControllers();
builder.Services.AddHttpClient();
builder.Services.AddKernel();

// Register the AI service of your choice. AzureOpenAI and OpenAI are demonstrated...
if (builder.Configuration.GetSection("AIServices").GetValue<bool>("UseAzureOpenAI"))
{
    string deploymentName = builder.Configuration.GetSection("AIServices:AzureOpenAI").GetValue<string>("DeploymentName");
    string endpoint = builder.Configuration.GetSection("AIServices:AzureOpenAI").GetValue<string>("Endpoint");
    string apiKey = builder.Configuration.GetSection("AIServices:AzureOpenAI").GetValue<string>("ApiKey");

    builder.Services.AddAzureOpenAIChatCompletion(deploymentName, endpoint, apiKey);
}
else
{
    builder.Services.AddOpenAIChatCompletion(
        modelId: builder.Configuration.GetSection("AIServices:OpenAI").GetValue<string>("ModelId"),
        apiKey: builder.Configuration.GetSection("AIServices:OpenAI").GetValue<string>("ApiKey"));
}

builder.Services.AddTransient<OnboardingPlanAgent>();

// Add AspNet token validation
//builder.Services.AddAgentAspNetAuthentication(builder.Configuration);

builder.AddAgentApplicationOptions();

builder.AddAgent<Onboarding>();

builder.Services.AddSingleton<IStorage, MemoryStorage>();

var app = builder.Build();

app.MapGet("/", async (HttpContext context) =>
{
    var htmlContent = await File.ReadAllTextAsync("chat.html");
    context.Response.ContentType = "text/html";
    await context.Response.WriteAsync(htmlContent);
});
//app.MapGet("/", () => "Onboardy - your onboarding buddy - MS Teams & Microsoft Agents SDK & Semantic Kernel");
// / should be mapped to a static html page


//<iframe src='https://webchat.botframework.com/embed/onboardy2?s=YOUR_SECRET_HERE'  style='min-width: 400px; width: 100%; min-height: 500px;'></iframe>
app.UseDeveloperExceptionPage();
app.MapControllers().AllowAnonymous();

app.Run();