
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using PawBuddy.Data;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddScoped<IEmailSender, EmailSender>();

// Configuração existente (mantida igual)
var connectionString = builder.Configuration.GetConnectionString("ConStringMySQL") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseMySql(
        builder.Configuration.GetConnectionString("ConStringMySQL"),
        new MySqlServerVersion(new Version(8, 0, 39))
    ));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();
// Registrar o serviço em segundo plano
builder.Services.AddHostedService<AdocaoBackgroundService>();
builder.Services.AddDefaultIdentity<IdentityUser>(options => {
    options.SignIn.RequireConfirmedAccount = true;
    options.Password.RequireDigit = true;
    options.Password.RequiredLength = 8;
})
.AddRoles<IdentityRole>()

.AddEntityFrameworkStores<ApplicationDbContext>();
builder.Services.AddControllersWithViews();
builder.Services.AddSession(options => {
    options.IdleTimeout = TimeSpan.FromMinutes(10);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});
builder.Services.AddDistributedMemoryCache();
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("NaoAdmin", policy =>
        policy.RequireAuthenticatedUser()
            .RequireAssertion(context => !context.User.IsInRole("Admin"))
    );
});

var app = builder.Build();

// Pipeline existente (mantido igual)
if (app.Environment.IsDevelopment()) {
    app.UseMigrationsEndPoint();
}
else {
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseSession();

app.UseAuthentication();
app.UseAuthorization();

// ===== Seed de Roles (Apenas Admin e Cliente) =====
using (var scope = app.Services.CreateScope()) {
    var services = scope.ServiceProvider;
    try {
        var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
        var userManager = services.GetRequiredService<UserManager<IdentityUser>>();

        // Criar apenas estes 2 roles
        string[] roles = { "Admin", "Cliente" };  // Removido "Veterinario"
        foreach (var role in roles) {
            if (!await roleManager.RoleExistsAsync(role)) {
                await roleManager.CreateAsync(new IdentityRole(role));
            }
        }

        // Criar admin padrão (apenas em desenvolvimento)
        if (app.Environment.IsDevelopment()) {
            var adminEmail = "admin@pawbuddy.com";
            var adminUser = await userManager.FindByEmailAsync(adminEmail);
            if (adminUser == null) {
                adminUser = new IdentityUser {
                    UserName = adminEmail,
                    Email = adminEmail,
                    EmailConfirmed = true
                };
                await userManager.CreateAsync(adminUser, "Admin123!");
                await userManager.AddToRoleAsync(adminUser, "Admin");  // Atribui role Admin
            }
        }
    }
    catch (Exception ex) {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "Erro ao inicializar roles");
    }
}
//Ativa a confirmação de email no Program.cs
/*builder.Services.AddDefaultIdentity<IdentityUser>(options =>
{
    options.SignIn.RequireConfirmedAccount = true; //obriga confirmação
    options.Password.RequireDigit = true;
    options.Password.RequiredLength = 8;
});*/

// Rotas
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapRazorPages();

app.Run();