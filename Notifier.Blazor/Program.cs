using Coravel;
using MudBlazor;
using MudBlazor.Services;
using Notifier.Blazor.Components;
using Notifier.Blazor.DI;
using Notifier.Blazor.Helpers;
using Notifier.DataAccess.DI;
using Notifier.Logic.DI;
using Notifier.Telegram.DI;
using Notifier.Vk.DI;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddMudServices(config =>
{
    config.SnackbarConfiguration.PositionClass = Defaults.Classes.Position.BottomLeft;

    config.SnackbarConfiguration.PreventDuplicates = true;
    config.SnackbarConfiguration.NewestOnTop = true;
    config.SnackbarConfiguration.ShowCloseIcon = true;
    config.SnackbarConfiguration.VisibleStateDuration = 5000;
    config.SnackbarConfiguration.HideTransitionDuration = 500;
    config.SnackbarConfiguration.ShowTransitionDuration = 500;
    config.SnackbarConfiguration.SnackbarVariant = Variant.Filled;
});

builder.Services.AddMudBlazorDialog();
builder.Services.AddMudBlazorSnackbar();

builder.Services.AddScheduler();
builder.Services.AddQueue();

builder.Services.AddServices(builder.Configuration);
builder.Services.ConfigureSettings(builder.Configuration);

builder.Services.AddDataAccess(builder.Configuration);
builder.Services.AddLogicServices(builder.Configuration);
builder.Services.AddVkApiServices(builder.Configuration);
builder.Services.AddTelegramApiServices(builder.Configuration);

//builder.Logging.AddCustomFormatter(configure =>
//{
//    configure.ColorBehavior = Microsoft.Extensions.Logging.Console.LoggerColorBehavior.Enabled;
//    configure.CustomPrefix = "Test";
//});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    //app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();

app.Services.ConfigureScheduler();

app.UseRouting();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.MapControllers();

app.MapControllerRoute(
    name: "default",
    pattern: "api/{controller}/{action}");

app.Run();
