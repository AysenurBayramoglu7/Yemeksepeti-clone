using Microsoft.EntityFrameworkCore;
using YemekSepeti.BLL.Abstract;
using YemekSepeti.BLL.Concrete;
using YemekSepeti.DAL;
using YemekSepeti.DAL.Abstract;
using YemekSepeti.DAL.EntityFramework;
using static YemekSepeti.DAL.Abstract.IFavoriRestoranlarDal;

var builder = WebApplication.CreateBuilder(args);

//Authentication(Kimlik Doğrulama) Servisini Ekleme
builder.Services.AddAuthentication("Cookies") // "Cookies" şeması ile kimlik doğrulamayı etkinleştir
    .AddCookie("Cookies", options =>
    {
        options.LoginPath = "/Kullanici/GirisYap"; // Giriş yapılmamışsa yönlendirilecek adres
        options.AccessDeniedPath = "/Home/AccessDenied"; // Yetkisiz erişimde yönlendirilecek adres
        options.ExpireTimeSpan = TimeSpan.FromMinutes(60); // Oturum süresi
        options.SlidingExpiration = true; // Oturum süresini kaydır
    });
//Dependency Injection (Bağımlılık Enjeksiyonu) Ayarları
// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddDbContext<YemekSepetiDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// DAL katmanı için servis kayıtları
builder.Services.AddScoped(typeof(IGenericDal<>), typeof(EfEntityRepositoryBase<>));
builder.Services.AddScoped<IKullaniciDal, EfKullaniciDal>();
builder.Services.AddScoped<IRolDal, EfRolDal>();

// BLL katmanı için servis kayıtları
builder.Services.AddScoped<IKullaniciService, KullaniciManager>();
builder.Services.AddScoped<IRolService, RolManager>();

//Restoran
builder.Services.AddScoped<IRestoranDal, EfRestoranDal>();
builder.Services.AddScoped<IRestoranService, RestoranManager>();
//Kategori
builder.Services.AddScoped<IKategoriDal, EfKategoriDal>();
builder.Services.AddScoped<IKategoriService, KategoriManager>();

builder.Services.AddScoped<IUrunDal, EfUrunDal>();
builder.Services.AddScoped<IUrunService, UrunManager>();

builder.Services.AddScoped<IUrunKategoriService, UrunKategoriManager>();
builder.Services.AddScoped<IUrunKategoriDal, EfUrunKategoriDal>();

builder.Services.AddScoped<IFavoriRestoranlarDal, EfFavoriRestoranlarDal>();
builder.Services.AddScoped<IFavoriRestoranlarService, FavoriRestoranlarManager>();

// Siparis
builder.Services.AddScoped<ISiparisDal, EfSiparisDal>();
builder.Services.AddScoped<ISiparisService, SiparisManager>();



builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

var app = builder.Build();

// Veritabanı migrasyonlarını başlangıçta otomatik olarak uygula
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<YemekSepetiDbContext>();
    dbContext.Database.Migrate();
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}


app.UseAuthentication();
app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseSession(); // Session Middleware

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
