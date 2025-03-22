
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.StaticFiles;
using GigaPizza.Domain;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// �������� ����������� � ���� ������
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

//builder.WebHost.UseUrls("http://*:5000", "https://*:5001");
builder.WebHost.UseUrls("http://*:5000");

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var dbContext = services.GetRequiredService<ApplicationDbContext>();

    // Если в режиме разработки – удаляем существующую базу, чтобы миграции применились корректно
    if (app.Environment.IsDevelopment())
    {
        dbContext.Database.EnsureDeleted();
    }

    // Применяем миграции; если база отсутствует, она будет создана согласно модели
    dbContext.Database.Migrate();

    // Инициализация данных, если необходимо
    ApplicationDbContext.Seed(services);
}

var provider = new FileExtensionContentTypeProvider();
provider.Mappings[".avif"] = "image/avif";

app.UseStaticFiles(new StaticFileOptions
{
    ContentTypeProvider = provider
});


// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
