
using System.Reflection;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using PawBuddy.Data;
using PawBuddy.Data.Seed;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddScoped<IEmailSender, EmailSender>();

// Configuração existente (mantida igual)
var connectionString = builder.Configuration.GetConnectionString("ConStringMySQL") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseMySql(  //sqlserver
        builder.Configuration.GetConnectionString("ConStringMySQL"),
        new MySqlServerVersion(new Version(8, 0, 39))
    ));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();
// Registrar o serviço em segundo plano
builder.Services.AddHostedService<AdocaoBackgroundService>();
builder.Services.AddDefaultIdentity<IdentityUser>(options => {
    options.SignIn.RequireConfirmedAccount = false;
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
builder.Services.AddControllers()
    .AddJsonOptions(options => options.JsonSerializerOptions.ReferenceHandler=ReferenceHandler.IgnoreCycles);

// add swagger
// https://learn.microsoft.com/en-us/aspnet/core/tutorials/getting-started-with-swashbuckle?view=aspnetcore-8.0&tabs=visual-studio
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(
    options =>
    {
        options.SwaggerDoc("v1",new OpenApiInfo {
            Title="Minha API de Adoçoes e Doações para animais",
            Version="v1",
            Description="API para gestão de Utilizadores, Animais, Doações e Adoções",
        });

        // Caminho para o XML gerado
        var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
        var xmlPath = Path.Combine(AppContext.BaseDirectory,xmlFile);
        options.IncludeXmlComments(xmlPath);

    }
    
    );


var app = builder.Build();

// Pipeline existente (mantido igual)
if (app.Environment.IsDevelopment()) {
    app.UseMigrationsEndPoint();
    app.UseItToSeedSqlServer(); 
    // cria o swagger
    app.UseSwagger();
    app.UseSwaggerUI();
    
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

// Rotas
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapRazorPages();

app.Run();

