/*
    By default application server kestrel is running which handles the http request, store it as http context object, pass it
    to application code, which returns response to the kestrel application server and it then return it to client

    In production, reverse proxy servers are there
    client <--> reverse proxies (IIS/NGINX/ Apache) <--> [ Kestral + Application Code]
    Benefits of reverse proxies:
    -> load balancing
    -> authentication
    -> caching
    -> URL rewriting
    -> Decrypting of SSSL certificates
    -> Decompressing the requests

    When run the app, in background kestrel server will be running
 */
using EcommerceBackend.Data;
using EcommerceBackend.Middlewares;
using EcommerceBackend.Repositories;
using EcommerceBackend.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// ==========================================
// 1. CONFIGURATIONS & CORE INFRAS (SETTINGS)
// ==========================================

// JWT Configurations ko appsettings.json se read kiya
var jwtKey = builder.Configuration["Jwt:Key"]!;
var jwtIssuer = builder.Configuration["Jwt:Issuer"];
var jwtAudience = builder.Configuration["Jwt:Audience"];

// Connection String ko appsettings.json se read kiya
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");


// ==========================================
// 2. DEPENDENCY INJECTION (DI) CONTAINER
// ==========================================

// Framework ko bataya ki hum API Controllers use karenge
builder.Services.AddControllers();

// JWT Authentication Services ko register kiya
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtIssuer,
        ValidAudience = jwtAudience,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
    };

    options.Events = new JwtBearerEvents
    {
        OnMessageReceived = context =>
        {
            if(context.Request.Cookies.ContainsKey("X-Auth-Token"))
            {
                context.Token = context.Request.Cookies["X-Auth-Token"];
            }
            return Task.CompletedTask;
        },

        OnTokenValidated = async context =>
        {
            var userIdClaim = context.Principal?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var tokenStampClaim = context.Principal?.FindFirst("SecurityStamp")?.Value;

            if(string.IsNullOrEmpty(userIdClaim) || string.IsNullOrEmpty(tokenStampClaim))
            {
                context.Fail("Invalid token structure: Missing claims");
                return;
            }

            var dbContext = context.HttpContext.RequestServices.GetRequiredService<AppDbContext>();
            var user = await dbContext.Users.FindAsync(int.Parse(userIdClaim));
            
            if(user == null || user.SecurityStamp != tokenStampClaim)
            {
                context.Fail("Token is no longer valid. Session revoked due to security updates/password reset.");
            }
        }
    };
});

// DbContext ko register kiya (SQL Server Connection ke sath)
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(connectionString));

// Custom Data Layers (Repository & Service) ko register kiya as Scoped
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IUserProfileRepository, UserProfileRepository>();
builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<ICartRepository, CartRepository>();
builder.Services.AddScoped<IOrderRepository, OrderRepository>();
builder.Services.AddScoped<IInventoryService, InventoryService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IUserProfileService, UserProfileService>();
builder.Services.AddScoped<ICartService, CartService>();
builder.Services.AddScoped<IOrderService, OrderService>();
builder.Services.AddScoped<IEmailService, EmailService>();


// ==========================================
// 3. APPLICATION BUILD
// ==========================================
var app = builder.Build(); // Kestrel application object built here


// ==========================================
// 4. MIDDLEWARE PIPELINE (EXECUTION ORDER)
// ==========================================

// A. Sabse Pehle: Error Guard (Taki pure pipeline ke crash ko pakad sake)
app.UseMiddleware<ExceptionHandlingMiddleware>();

// B. Performance Guard: Request aur Response ka execution time log karne ke liye
app.UseMiddleware<PerformanceTrackerMiddleware>();

// C. Security Guard 1: User ki pehchan check karna (Token validation)
app.UseAuthentication();

// D. Security Guard 2: User ki permission check karna (Role/Access check)
app.UseAuthorization();

// E. Initial Temporary Route (Optional - testing ke liye rakh sakte ho)
app.MapGet("/", () => "Hello World!");

// F. Routing Engine: Request ko sahi Controller aur Method tak pahunchana
app.MapControllers();


// ==========================================
// 5. APPLICATION RUN (THE INFINITE LOOP)
// ==========================================

app.Run(); // Keeps the Kestrel server up and listening for HTTP requests