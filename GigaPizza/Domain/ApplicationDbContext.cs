using GigaPizza.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace GigaPizza.Domain
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
             : base(options) { }

        public DbSet<Pizza> Pizzas { get; set; }
        public DbSet<PizzaType> PizzaTypes { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Настройка связи Many-to-Many между Pizza и PizzaType с использованием промежуточной таблицы "PizzaPizzaType"
            modelBuilder.Entity<Pizza>()
                .HasMany(p => p.Types)
                .WithMany(t => t.Pizzas)
                .UsingEntity(j => j.ToTable("PizzaPizzaType"));
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlite("Data Source=/data/pizza.db");
            }
        }

        public static void Seed(IServiceProvider serviceProvider)
        {
            using var context = serviceProvider.GetRequiredService<ApplicationDbContext>();

            // Проверяем, есть ли данные в таблицах
            if (context.Pizzas.Any() || context.PizzaTypes.Any())
            {
                return; // Если данные уже есть, то пропускаем
            }

            // Добавляем начальные данные
            var pizzas = new List<Pizza>
        {
            new Pizza
{
    Name = "Пицца Маргарита".ToLower(),
    Ingredients = string.Join(", ", new List<string> { "томатный соус", "моцарелла", "базилик" }).ToLower(),
    Price = 499.99m,
    Image = "images/pizza/pizza_margarita.avif",
    Types = new List<PizzaType>
    {
        new PizzaType { Name = "классическая".ToLower() },
        new PizzaType { Name = "вегетарианская".ToLower() }
    },
    Description = "Традиционная итальянская пицца с томатным соусом, моцареллой и базиликом.".ToLower(),
    RecommendedDrinks = string.Join(", ", new List<string> { "Красное вино", "Минеральная вода" }).ToLower()
},
new Pizza
{
    Name = "Пицца Пепперони".ToLower(),
    Ingredients = string.Join(", ", new List<string> { "Томатный соус", "Моцарелла", "Пепперони" }).ToLower(),
    Price = 599.99m,
    Image = "images/pizza/pizza_pepperoni.avif",
    Types = new List<PizzaType>
    {
        new PizzaType { Name = "Острая".ToLower() },
        new PizzaType { Name = "Классическая".ToLower() }
    },
    Description = "Пицца с пикантными колбасками пепперони и моцареллой.".ToLower(),
    RecommendedDrinks = string.Join(", ", new List<string> { "Кола", "Пиво" }).ToLower()
},
new Pizza
{
    Name = "Пицца Сырная".ToLower(),
    Ingredients = string.Join(", ", new List<string> { "Моцарелла", "Горгонзола", "Пармезан", "Чеддер" }).ToLower(),
    Price = 649.99m,
    Image = "images/pizza/pizza_cheese.avif",
    Types = new List<PizzaType>
    {
        new PizzaType { Name = "Сырная".ToLower() },
        new PizzaType { Name = "Вегетарианская".ToLower() }
    },
    Description = "Сырное наслаждение с четырьмя видами сыра.".ToLower(),
    RecommendedDrinks = string.Join(", ", new List<string> { "Белое вино", "Грушевая газировка" }).ToLower()
},
new Pizza
{
    Name = "Пицца Ветчина и Грибы".ToLower(),
    Ingredients = string.Join(", ", new List<string> { "Ветчина", "Шампиньоны", "Моцарелла", "Томатный соус" }).ToLower(),
    Price = 549.00m,
    Image = "images/pizza/pizza_hum_and_mushrooms.avif",
    Types = new List<PizzaType>
    {
        new PizzaType { Name = "Мясная".ToLower() }
    },
    Description = "Классическая пицца с ветчиной и грибами.".ToLower(),
    RecommendedDrinks = string.Join(", ", new List<string> { "Кола", "Апельсиновый сок" }).ToLower()
},
new Pizza
{
    Name = "Пицца Диабло".ToLower(),
    Ingredients = string.Join(", ", new List<string> { "Колбаски чоризо", "Халапеньо", "Соус барбекю", "Митболы из говядины", "Томаты", "Сладкий перец", "Красный лук", "Моцарелла" }).ToLower(),
    Price = 649.00m,
    Image = "images/pizza/pizza_diablo.avif",
    Types = new List<PizzaType>
    {
        new PizzaType { Name = "Мясная".ToLower() },
        new PizzaType { Name = "Острая".ToLower() }
    },
    Description = "Очень острая пицца с колбасками чоризо и халапеньо.".ToLower(),
    RecommendedDrinks = string.Join(", ", new List<string> { "Минеральная вода", "Кола" }).ToLower()
},
new Pizza
{
    Name = "Пицца Кола-барбекю".ToLower(),
    Ingredients = string.Join(", ", new List<string> { "Пряная говядина", "Пикантная пепперони", "Острые колбаски чоризо", "Соус кола-барбекю", "Моцарелла" }).ToLower(),
    Price = 609.00m,
    Image = "images/pizza/pizza_cola_barbecue.avif",
    Types = new List<PizzaType>
    {
        new PizzaType { Name = "Мясная".ToLower() },
        new PizzaType { Name = "Острая".ToLower() }
    },
    Description = "Пицца с уникальным соусом кола-барбекю.".ToLower(),
    RecommendedDrinks = string.Join(", ", new List<string> { "Кола", "Черный чай" }).ToLower()
},
new Pizza
{
    Name = "Пицца Бефстроганов".ToLower(),
    Ingredients = string.Join(", ", new List<string> { "Пряная говядина", "Шампиньоны", "Грибной соус", "Маринованные огурчики", "Моцарелла" }).ToLower(),
    Price = 569.00m,
    Image = "images/pizza/pizza_beef_stroganoff.avif",
    Types = new List<PizzaType>
    {
        new PizzaType { Name = "Мясная".ToLower() }
    },
    Description = "Пицца с бефстрогановым соусом и маринованными огурчиками".ToLower(),
    RecommendedDrinks = string.Join(", ", new List<string> { "Гранатовый сок", "Минеральная вода" }).ToLower()
},
new Pizza
{
    Name = "Пицца Мясная с аджикой".ToLower(),
    Ingredients = string.Join(", ", new List<string> { "Баварские колбаски", "Острый соус аджика", "Чоризо", "Цыпленок", "Пепперони", "Моцарелла" }).ToLower(),
    Price = 569.00m,
    Image = "images/pizza/pizza_with_adjika.avif",
    Types = new List<PizzaType>
    {
        new PizzaType { Name = "Мясная".ToLower() },
        new PizzaType { Name = "Острая".ToLower() }
    },
    Description = "Мясная пицца с острым соусом аджика".ToLower(),
    RecommendedDrinks = string.Join(", ", new List<string> { "Кола", "Черный чай" }).ToLower()
},
new Pizza
{
    Name = "Пицца Аррива!".ToLower(),
    Ingredients = string.Join(", ", new List<string> { "Цыпленок", "Чоризо", "Соус бургер", "Сладкий перец", "Красный лук", "Моцарелла", "Соус ранч" }).ToLower(),
    Price = 659.00m,
    Image = "images/pizza/pizza_arriva.avif",
    Types = new List<PizzaType>
    {
        new PizzaType { Name = "Мясная".ToLower() },
        new PizzaType { Name = "Острая".ToLower() }
    },
    Description = "Пицца с цыпленком и острыми колбасками".ToLower(),
    RecommendedDrinks = string.Join(", ", new List<string> { "Кола", "Черный чай" }).ToLower()
}

        };

            context.Pizzas.AddRange(pizzas);

            // Сохраняем изменения в БД
            context.SaveChanges();
        }



        public static void EnsureDatabaseMigrated(IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            try
            {
                // Если нет миграций - создаем их
                if (!dbContext.Database.GetMigrations().Any())
                {
                    Console.WriteLine("Создаем первую миграцию...");
                    System.Diagnostics.Process.Start("dotnet", "ef migrations add InitialCreate")?.WaitForExit();
                    System.Diagnostics.Process.Start("dotnet", "ef database update")?.WaitForExit();
                }

                // Применяем миграции
                //dbContext.Database.Migrate();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при миграции БД: {ex.Message}");
            }
        }

    }

    public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
    {
        public ApplicationDbContext CreateDbContext(string[] args)
        {
            var basePath = Directory.GetCurrentDirectory();
            IConfigurationRoot configuration = new ConfigurationBuilder()
                .SetBasePath(basePath)
                .AddJsonFile("appsettings.json", optional: false)
                .Build();

            var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
            var connectionString = configuration.GetConnectionString("DefaultConnection");
            optionsBuilder.UseSqlite(connectionString);

            return new ApplicationDbContext(optionsBuilder.Options);
        }
    }

}
