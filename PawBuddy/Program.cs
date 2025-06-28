using System.Reflection;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using PawBuddy.Data;
using PawBuddy.Data.Seed;

var builder = WebApplication.CreateBuilder(args);

// ================== DEPENDENCY INJECTION ==================

// Conexão com MySQL
var connectionString = builder.Configuration.GetConnectionString("ConStringMySQL")
    ?? throw new InvalidOperationException("Connection string 'ConStringMySQL' not found.");

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseMySql(connectionString, new MySqlServerVersion(new Version(8, 0, 39))));

builder.Services.AddDatabaseDeveloperPageExceptionFilter();

// Identity (com confirmação de conta e roles)
builder.Services.AddDefaultIdentity<IdentityUser>(options =>
{
    options.SignIn.RequireConfirmedAccount = true;
    options.Password.RequireDigit = true;
    options.Password.RequiredLength = 8;
})
.AddRoles<IdentityRole>()
.AddEntityFrameworkStores<ApplicationDbContext>();

// Email sender
builder.Services.AddScoped<IEmailSender, EmailSender>();

// Background service
builder.Services.AddHostedService<AdocaoBackgroundService>();

// Sessão
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(10);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});
builder.Services.AddDistributedMemoryCache();

// Controllers e Serialização JSON (ignora ciclos)
builder.Services.AddControllersWithViews()
    .AddJsonOptions(options => 
        options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles);

// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Minha API de Adoções e Doações para Animais",
        Version = "v1",
        Description = "API para gestão de Utilizadores, Animais, Doações e Adoções"
    });

    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    options.IncludeXmlComments(xmlPath);
});

// Políticas de Autorização
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("NaoAdmin", policy =>
        policy.RequireAuthenticatedUser()
              .RequireAssertion(context => !context.User.IsInRole("Admin")));
});

// ================== BUILD APP ==================

var app = builder.Build();

// ================== MIDDLEWARE ==================

if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
    app.UseItToSeedSqlServer(); // Método de seed personalizado
    app.UseSwagger();
    app.UseSwaggerUI();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseSession();
app.UseAuthentication();
app.UseAuthorization();

// ================== SEED DE ROLES E ADMIN ==================

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
        var userManager = services.GetRequiredService<UserManager<IdentityUser>>();

        // Criar apenas estes dois roles
        string[] roles = { "Admin", "Cliente" };
        foreach (var role in roles)
        {
            if (!await roleManager.RoleExistsAsync(role))
            {
                await roleManager.CreateAsync(new IdentityRole(role));
            }
        }

        // Criar utilizador admin padrão (apenas em desenvolvimento)
        if (app.Environment.IsDevelopment())
        {
            var adminEmail = "admin@pawbuddy.com";
            var adminUser = await userManager.FindByEmailAsync(adminEmail);
            if (adminUser == null)
            {
                adminUser = new IdentityUser
                {
                    UserName = adminEmail,
                    Email = adminEmail,
                    EmailConfirmed = true
                };
                await userManager.CreateAsync(adminUser, "Admin123!");
                await userManager.AddToRoleAsync(adminUser, "Admin");
            }
        }
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "Erro ao inicializar roles");
    }
}

// ================== ROTAS ==================

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapRazorPages();

app.Run();
