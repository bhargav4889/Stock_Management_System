using Stock_Management_System.BAL;
using Stock_Management_System.Email_Services;
using WebOptimizer;
using NUglify;
using NUglify.JavaScript;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();



builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(10);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});


builder.Services.AddDistributedMemoryCache();

builder.Services.AddHttpContextAccessor();
builder.Services.AddSingleton<CV>();

// Register EmailSender with SMTP settings
builder.Services.AddTransient<IEmailSender>(i => new EmailSender(
    builder.Configuration["EmailSettings:Host"],
    int.Parse(builder.Configuration["EmailSettings:Port"]),
    bool.Parse(builder.Configuration["EmailSettings:EnableSSL"]),
    builder.Configuration["EmailSettings:UserName"],
    builder.Configuration["EmailSettings:Password"]
));



builder.Services.AddWebOptimizer(pipeline =>
{
    // stock

    pipeline.AddJavaScriptBundle("/js/bundle/stock/add-bundle.js", "/js/Section/stock/add.js");
    pipeline.AddJavaScriptBundle("/js/bundle/stock/edit-bundle.js", "/js/Section/stock/edit.js");
    pipeline.AddJavaScriptBundle("/js/bundle/stock/stocks-bundle.js", "/js/Section/stock/stocks.js");
    pipeline.AddJavaScriptBundle("/js/bundle/stock/details-bundle.js", "/js/Section/stock/details.js");

    // invoice --> s --> 

    pipeline.AddJavaScriptBundle("/js/bundle/invoice/s/add-bundle.js", "/js/Section/invoice/s/add.js");
    pipeline.AddJavaScriptBundle("/js/bundle/invoice/s/edit-bundle.js", "/js/Section/invoice/s/edit.js");
    pipeline.AddJavaScriptBundle("/js/bundle/invoice/s/sales-bundle.js", "/js/Section/invoice/s/sales.js");
    pipeline.AddJavaScriptBundle("/js/bundle/invoice/s/preview-bundle.js", "/js/Section/invoice/s/preview.js");
    pipeline.AddJavaScriptBundle("/js/bundle/invoice/s/details-bundle.js", "/js/Section/invoice/s/details.js");
});


var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Manage/Error_Page");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.UseSession();

app.UseWebOptimizer();

System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);



app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllerRoute(
      name: "areas",
      pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}"
    );
});