// ============================================================
// File: Program.cs
// Author: Victor Marrujo
// Course: CST-350
// Description: Application entry point. Configures the ASP.NET
//              Core MVC pipeline and registers all DAL services
//              via dependency injection.
// ============================================================

using BibleVerseApp.DAL;

// Create the web application builder
var builder = WebApplication.CreateBuilder(args);

// -------------------------------------------------------
// Register MVC services with the DI container
// -------------------------------------------------------
builder.Services.AddControllersWithViews();

// Read the connection string from appsettings.json
string connStr = builder.Configuration.GetConnectionString("BibleVerseDb")
    ?? throw new InvalidOperationException("Connection string 'BibleVerseDb' not found.");

// -------------------------------------------------------
// Register DAL implementations against their interfaces.
// Using a factory lambda so the connection string is passed
// at construction time. Scoped lifetime means one instance
// per HTTP request, which is appropriate for DB connections.
// -------------------------------------------------------
builder.Services.AddScoped<IBibleVerseDAO>(_ => new SqlBibleVerseDAO(connStr));
builder.Services.AddScoped<IBibleBookDAO>(_ => new SqlBibleBookDAO(connStr));
builder.Services.AddScoped<IVerseNoteDAO>(_ => new SqlVerseNoteDAO(connStr));

// Build the application
var app = builder.Build();

// -------------------------------------------------------
// Configure the HTTP request pipeline
// -------------------------------------------------------

// Use developer exception page in development; generic handler in production
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

// Redirect HTTP to HTTPS
app.UseHttpsRedirection();

// Serve static files from wwwroot (CSS, JS, images)
app.UseStaticFiles();

// Enable routing
app.UseRouting();

// Enable authorization middleware (required even if no auth rules are set)
app.UseAuthorization();

// Define the default MVC route pattern
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

// Start the application
app.Run();
