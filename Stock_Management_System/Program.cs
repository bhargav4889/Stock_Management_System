using Stock_Management_System.Email_Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();



builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(3);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});


builder.Services.AddDistributedMemoryCache();

builder.Services.AddHttpContextAccessor();

// Register EmailSender with SMTP settings
builder.Services.AddTransient<IEmailSender>(i => new EmailSender(
    builder.Configuration["EmailSettings:Host"],
    int.Parse(builder.Configuration["EmailSettings:Port"]),
    bool.Parse(builder.Configuration["EmailSettings:EnableSSL"]),
    builder.Configuration["EmailSettings:UserName"],
    builder.Configuration["EmailSettings:Password"]
));


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