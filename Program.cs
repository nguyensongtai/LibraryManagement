using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using LibraryManagement.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// Add Authentication and Session
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
  .AddCookie(options =>
  {
    options.LoginPath = "/Account/Login"; // Redirect to login page
    options.LogoutPath = "/Account/Logout";
  });

builder.Services.AddSession(options =>
{
  options.IdleTimeout = TimeSpan.FromMinutes(30); // Session timeout
  options.Cookie.HttpOnly = true;
  options.Cookie.IsEssential = true;
});

builder.Services.AddDbContext<AppDbContext>(options =>
  options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

var app = builder.Build();

// Middleware pipeline
if (!app.Environment.IsDevelopment())
{
  app.UseExceptionHandler("/Home/Error");
  app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseSession(); // Enable session

app.UseAuthentication(); // Enable authentication
app.UseAuthorization();

app.Use(async (context, next) =>
{
  context.Response.Headers["Cache-Control"] = "no-cache, no-store, must-revalidate";
  context.Response.Headers["Pragma"] = "no-cache";
  context.Response.Headers["Expires"] = "0";

  await next();
});

app.MapControllerRoute(
  name: "default",
  pattern: "{controller=Home}/{action=Index}/{id?}"
);

app.MapControllerRoute(
  name: "BookLoansRoute",
  pattern: "BookLoans/Delete/{bookTitleId}/{libraryId}/{readerCardId}",
  defaults: new { controller = "BookLoans", action = "Delete" }
);

app.MapControllerRoute(
  name: "BookLoansRoute",
  pattern: "BookLoans/Edit/{bookTitleId}/{libraryId}/{readerCardId}",
  defaults: new { controller = "BookLoans", action = "Edit" }
);

app.MapControllerRoute(
  name: "BookLoansRoute",
  pattern: "BookLoans/Details/{bookTitleId}/{libraryId}/{readerCardId}",
  defaults: new { controller = "BookLoans", action = "Details" }
);

app.Run();
