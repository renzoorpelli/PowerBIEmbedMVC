using PowerBI.Infrastructure.Services;
using PowerBIReports.Domain.Entities;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();


//obtengo los valores del appsettings.json al modelo
builder.Services.Configure<AzureAD>(builder.Configuration.GetSection("AzureAD"));
builder.Services.Configure<PowerBIModel>(builder.Configuration.GetSection("PowerBI"));

//inyecto los servicios
builder.Services.AddScoped(typeof(AADService));
builder.Services.AddScoped(typeof(PBIEmbedService));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
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
