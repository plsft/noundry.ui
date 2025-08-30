using Noundry.UI.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddNoundryUI(options =>
{
    options.IncludeAlpineJS = true;
    options.IncludeTailwindCSS = true;
    options.Theme = "default";
    options.Defaults.ButtonVariant = "primary";
    options.Defaults.ToastPosition = "top-right";
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();