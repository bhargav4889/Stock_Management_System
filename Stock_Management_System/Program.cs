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


    // sale

    pipeline.AddJavaScriptBundle("/js/bundle/sale/add-bundle.js", "/js/Section/sale/add.js");
    pipeline.AddJavaScriptBundle("/js/bundle/sale/edit-bundle.js", "/js/Section/sale/edit.js");
    pipeline.AddJavaScriptBundle("/js/bundle/sale/sales-bundle.js", "/js/Section/sale/sales.js");
    pipeline.AddJavaScriptBundle("/js/bundle/sale/details-bundle.js", "/js/Section/sale/details.js");

    // invoice --> s --> 

    pipeline.AddJavaScriptBundle("/js/bundle/invoice/s/add-bundle.js", "/js/Section/invoice/s/add.js");
    pipeline.AddJavaScriptBundle("/js/bundle/invoice/s/edit-bundle.js", "/js/Section/invoice/s/edit.js");
    pipeline.AddJavaScriptBundle("/js/bundle/invoice/s/sales-bundle.js", "/js/Section/invoice/s/sales.js");
    pipeline.AddJavaScriptBundle("/js/bundle/invoice/s/preview-bundle.js", "/js/Section/invoice/s/preview.js");
    pipeline.AddJavaScriptBundle("/js/bundle/invoice/s/details-bundle.js", "/js/Section/invoice/s/details.js");



    // invoice --> p --> 

    pipeline.AddJavaScriptBundle("/js/bundle/invoice/p/add-bundle.js", "/js/Section/invoice/p/add.js");
    pipeline.AddJavaScriptBundle("/js/bundle/invoice/p/edit-bundle.js", "/js/Section/invoice/p/edit.js");
    pipeline.AddJavaScriptBundle("/js/bundle/invoice/p/sales-bundle.js", "/js/Section/invoice/p/purchases.js");
    pipeline.AddJavaScriptBundle("/js/bundle/invoice/p/preview-bundle.js", "/js/Section/invoice/p/preview.js");
    pipeline.AddJavaScriptBundle("/js/bundle/invoice/p/details-bundle.js", "/js/Section/invoice/p/details.js");


    // customer 

    pipeline.AddJavaScriptBundle("/js/bundle/customer/add-bundle.js", "/js/Section/customer/add.js");
    pipeline.AddJavaScriptBundle("/js/bundle/customer/edit-bundle.js", "/js/Section/customer/edit.js");
    pipeline.AddJavaScriptBundle("/js/bundle/customer/ac-bundle.js", "/js/Section/customer/ac.js");
    pipeline.AddJavaScriptBundle("/js/bundle/customer/customers-bundle.js", "/js/Section/customer/customers.js");


    // payment

    pipeline.AddJavaScriptBundle("/js/bundle/payment/paid-bundle.js", "/js/Section/payment/paid.js");
    pipeline.AddJavaScriptBundle("/js/bundle/payment/pending-bundle.js", "/js/Section/payment/pending.js");
    pipeline.AddJavaScriptBundle("/js/bundle/payment/remain-bundle.js", "/js/Section/payment/remain.js");


    // information

    pipeline.AddJavaScriptBundle("/js/bundle/information/add-bundle.js", "/js/Section/information/add.js");
    pipeline.AddJavaScriptBundle("/js/bundle/information/edit-bundle.js", "/js/Section/information/edit.js");
    pipeline.AddJavaScriptBundle("/js/bundle/information/saveinformations-bundle.js", "/js/Section/information/saveinformations.js");


    // reminder

    pipeline.AddJavaScriptBundle("/js/bundle/reminder/add-bundle.js", "/js/Section/reminder/add.js");
    pipeline.AddJavaScriptBundle("/js/bundle/reminder/edit-bundle.js", "/js/Section/reminder/edit.js");
    pipeline.AddJavaScriptBundle("/js/bundle/reminder/reminders-bundle.js", "/js/Section/reminder/reminders.js");




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