using IMS_Dashboard.Configurations;
using IMS_Dashboard.Services.CategoryServices.Interface;
using IMS_Dashboard.Services.CategoryServices.Service;
using IMS_Dashboard.Services.CustomerServices.Interface;
using IMS_Dashboard.Services.CustomerServices.Service;
using IMS_Dashboard.Services.InventoryServices.Interface;
using IMS_Dashboard.Services.InventoryServices.Service;
using IMS_Dashboard.Services.ManufacturerServices.Interface;
using IMS_Dashboard.Services.ManufacturerServices.Service;
using IMS_Dashboard.Services.OrderServices.Interface;
using IMS_Dashboard.Services.OrderServices.Service;
using IMS_Dashboard.Services.OrderStatusServices.Interface;
using IMS_Dashboard.Services.OrderStatusServices.Service;
using IMS_Dashboard.Services.PaymentMethodServices.Interface;
using IMS_Dashboard.Services.PaymentMethodServices.Service;
using IMS_Dashboard.Services.ProductServices.Interface;
using IMS_Dashboard.Services.ProductServices.Service;
using IMS_Dashboard.Services.SupplierServices.Interface;
using IMS_Dashboard.Services.SupplierServices.Service;
using IMS_Dashboard.Services.TransactionTypeServices.Interface;
using IMS_Dashboard.Services.TransactionTypeServices.Service;
using IMS_Dashboard.Services.UnitOfMeasureServices.Interface;
using IMS_Dashboard.Services.UnitOfMeasureServices.Service;
using System.Net.Http.Headers;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddAutoMapper(typeof(MappingProfile));

// Read Base Address from Configuration
var apiBaseAddress = builder.Configuration["ApiSettings:BaseAddress"];

// Register HttpClient with configuration
builder.Services.AddHttpClient("ApiClient", client =>
{
    client.BaseAddress = new Uri(apiBaseAddress);
    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
});

// Register HttpClient for OrderService with specific configuration
builder.Services.AddHttpClient("OrderClient", client =>
{
    client.BaseAddress = new Uri(apiBaseAddress);
    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
});

builder.Services.AddHttpClient("OrderStatusClient", client =>
{
    client.BaseAddress = new Uri(apiBaseAddress);
    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
});

builder.Services.AddHttpClient<ICustomerService, CustomerService>(client =>
{
    client.BaseAddress = new Uri(apiBaseAddress);
    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
});

builder.Services.AddHttpClient<IProductService, ProductService>(client =>
{
    client.BaseAddress = new Uri(apiBaseAddress);
    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
});

builder.Services.AddHttpClient<IInventoryService, InventoryService>(client =>
{
    client.BaseAddress = new Uri(apiBaseAddress);
    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
});

builder.Services.AddHttpClient<ISupplierService, SupplierService>(client =>
{
    client.BaseAddress = new Uri(apiBaseAddress);
    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
});

builder.Services.AddScoped<IOrderService, OrderService>();

builder.Services.AddScoped<IOrderStatusService, OrderStatusService>();

builder.Services.AddHttpClient<ICategoryService, CategoryService>(client =>
{
    client.BaseAddress = new Uri(apiBaseAddress);
    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
});

builder.Services.AddHttpClient<IManufacturerService, ManufacturerService>(client =>
{
    client.BaseAddress = new Uri(apiBaseAddress);
    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
});

builder.Services.AddHttpClient<IPaymentMethodService, PaymentMethodService>(client =>
{
    client.BaseAddress = new Uri(apiBaseAddress);
    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
});

builder.Services.AddHttpClient<ITransactionTypeService, TransactionTypeService>(client =>
{
    client.BaseAddress = new Uri(apiBaseAddress);
    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
});

builder.Services.AddHttpClient<IUnitOfMeasureService, UnitOfMeasureService>(client =>
{
    client.BaseAddress = new Uri(apiBaseAddress);
    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
});


// Add services to the container.
builder.Services.AddControllersWithViews();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
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
