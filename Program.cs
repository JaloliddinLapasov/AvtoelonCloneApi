using AvtoelonCloneApi.Data;
    using AvtoelonCloneApi.Models;
    using AvtoelonCloneApi.Services; // TokenService uchun
    using Microsoft.AspNetCore.Authentication.JwtBearer;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.IdentityModel.Tokens;
    using Microsoft.OpenApi.Models;
    using System.Text;
    using System.Text.Json.Serialization; // Json Enum konvertatsiyasi uchun

    var builder = WebApplication.CreateBuilder(args);

builder.WebHost.UseUrls("http://localhost:5000", "https://localhost:5001"); // HTTP va HTTPS portlarini belgilash

// --- Servislarni Konfiguratsiya Qilish ---

// 1. DbContext (MSSQL Server)
builder.Services.AddDbContext<AppDataContext>(options =>
        options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"),
            // SQL Server uchun qo'shimcha sozlamalar (masalan, retry logic)
            sqlServerOptionsAction: sqlOptions =>
            {
                sqlOptions.EnableRetryOnFailure(
                    maxRetryCount: 5,
                    maxRetryDelay: TimeSpan.FromSeconds(30),
                    errorNumbersToAdd: null);
            }));

    // 2. Identity
    builder.Services.AddIdentity<AppUser, IdentityRole>(options =>
    {
        // Parol sozlamalari (Production uchun kuchliroq qilish kerak)
        options.Password.RequireDigit = false;
        options.Password.RequireLowercase = false;
        options.Password.RequireNonAlphanumeric = false;
        options.Password.RequireUppercase = false;
        options.Password.RequiredLength = 6;

        // Foydalanuvchi sozlamalari
        options.User.RequireUniqueEmail = true;

        // Kirish sozlamalari (masalan, email tasdiqlash)
        // options.SignIn.RequireConfirmedAccount = true;
    })
    .AddEntityFrameworkStores<AppDataContext>()
    .AddDefaultTokenProviders(); // Parolni tiklash kabi funksiyalar uchun

    // 3. JWT Autentifikatsiyasi
    var jwtSettings = builder.Configuration.GetSection("Jwt");
    var jwtKey = jwtSettings["Key"];
    if (string.IsNullOrEmpty(jwtKey))
    {
        throw new InvalidOperationException("JWT Key konfiguratsiyada topilmadi yoki bo'sh.");
    }

    builder.Services.AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        options.SaveToken = true; // Tokenni HttpContext da saqlash
        options.RequireHttpsMetadata = false; // Development uchun false, Production uchun true
        options.TokenValidationParameters = new TokenValidationParameters()
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtSettings["Issuer"],
            ValidAudience = jwtSettings["Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey)),
            ClockSkew = TimeSpan.Zero // Token muddati tugashi bilan darhol yaroqsiz bo'lishi uchun
        };
    });

    // 4. Authorization
    builder.Services.AddAuthorization();
    // Agar rollar asosida avtorizatsiya kerak bo'lsa:
    // builder.Services.AddAuthorization(options => {
    //     options.AddPolicy("AdminOnly", policy => policy.RequireRole("Admin"));
    // });

    // 5. Controllerlar va API sozlamalari
    builder.Services.AddControllers()
        .AddJsonOptions(options =>
        {
            // Enum larni string sifatida ko'rsatish (agar ishlatilsa)
            options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
            // Siklik bog'lanishlarni e'tiborsiz qoldirish (agar kerak bo'lsa)
            // options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
        });


    // 6. Swagger / OpenAPI
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen(options =>
    {
        options.SwaggerDoc("v1", new OpenApiInfo { Title = "Avtoelon Clone API", Version = "v1" });

        // Swagger UI da JWT avtorizatsiyasini qo'shish
        options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
        {
            In = ParameterLocation.Header,
            Description = "Iltimos, 'Bearer' [space] va keyin tokeningizni kiriting.",
            Name = "Authorization",
            Type = SecuritySchemeType.Http, // Http ishlatish yaxshiroq
            BearerFormat = "JWT",
            Scheme = "Bearer"
        });
        options.AddSecurityRequirement(new OpenApiSecurityRequirement
        {
            {
                new OpenApiSecurityScheme
                {
                    Reference = new OpenApiReference
                    {
                        Type = ReferenceType.SecurityScheme,
                        Id = "Bearer"
                    }
                },
                new string[] {} // Bosh massiv
            }
        });
    });

    // 7. CORS (Cross-Origin Resource Sharing)
    builder.Services.AddCors(options =>
    {
        options.AddPolicy("AllowFrontend", policy =>
        {
            // Frontend manzilingizni shu yerga yozing
            policy.WithOrigins("http://localhost:4200")
                  .AllowAnyHeader()
                  .AllowAnyMethod()
                  .WithExposedHeaders("X-Pagination-TotalItems", "X-Pagination-PageSize", "X-Pagination-CurrentPage", "X-Pagination-TotalPages"); // Pagination headerlarini ochish
                  // .AllowCredentials(); // Agar cookie yoki authorization headerlari kerak bo'lsa
        });
    });

    // 8. Custom Servislar (masalan, TokenService)
    builder.Services.AddScoped<ITokenService, TokenService>();

    // 9. Logging
    builder.Logging.ClearProviders();
    builder.Logging.AddConsole();
    // Boshqa logging providerlar (masalan, Serilog) qo'shish mumkin


    // --- Ilovani Qurish ---
    var app = builder.Build();


    // --- HTTP Request Pipeline ni Konfiguratsiya Qilish ---

    // Development muhiti uchun sozlamalar
    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI(c =>
        {
            c.SwaggerEndpoint("/swagger/v1/swagger.json", "Avtoelon Clone API V1");
            // c.RoutePrefix = string.Empty; // Swagger UI ni bosh sahifada ochish uchun
        });
        app.UseDeveloperExceptionPage(); // Batafsil xatoliklarni ko'rsatish
    }
    else
    {
        // Production uchun xatoliklarni boshqarish
        // app.UseExceptionHandler("/Error"); // Maxsus xatolik sahifasi
        app.UseHsts(); // HTTP Strict Transport Security
    }

    app.UseHttpsRedirection(); // HTTP ni HTTPS ga yo'naltirish

    app.UseRouting(); // Marshrutlashni yoqish

    app.UseCors("AllowFrontend"); // CORS policy ni qo'llash

    app.UseAuthentication(); // Kimligini tekshirish
    app.UseAuthorization(); // Ruxsatini tekshirish

    app.MapControllers(); // Controller endpointlarini bog'lash

    // --- Ma'lumotlar Bazasini Tayyorlash (Migratsiya va Seed) ---
    using (var scope = app.Services.CreateScope())
    {
        var services = scope.ServiceProvider;
        var logger = services.GetRequiredService<ILogger<Program>>();
        try
        {
            logger.LogInformation("Ma'lumotlar bazasini migratsiya qilish boshlandi.");
            var context = services.GetRequiredService<AppDataContext>();
            await context.Database.MigrateAsync(); // Migratsiyalarni avtomatik qo'llash
            logger.LogInformation("Ma'lumotlar bazasini migratsiya qilish tugallandi.");

            // Boshlang'ich ma'lumotlarni (Seed Data) qo'shish
            // var userManager = services.GetRequiredService<UserManager<AppUser>>();
            // var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
            // await SeedData.Initialize(services, userManager, roleManager, context); // Maxsus SeedData klassi orqali
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Ilova ishga tushishida xatolik yuz berdi (migratsiya yoki seed).");
            // Ilovani to'xtatish yoki boshqa chora ko'rish
        }
    }

    // --- Ilovani Ishga Tushirish ---
    app.Run();
