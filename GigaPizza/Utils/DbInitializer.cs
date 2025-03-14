using GigaPizza.Models;
using Microsoft.EntityFrameworkCore;


namespace GigaPizza.Utils
{

    public static class DbInitializer
    {

        public static void Initialize(ApplicationDbContext context)
        {
            try
            {
                // Явно указываем путь для логирования
                Console.WriteLine($"Database path: {context.Database.GetDbConnection().DataSource}");

                context.Database.EnsureCreated();

                if (!context.Pizzas.Any())
                {
                    context.Pizzas.AddRange(GetInitialPizzas());
                    context.SaveChanges();
                    Console.WriteLine("Database initialized successfully");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Database initialization error: {ex}");
                throw;
            }
        }
        public static List<Pizza> GetInitialPizzas()
        {

            return new List<Pizza>
    {
        new Pizza
        {
            Name = "Пицца Маргарита",
            Ingredients = string.Join(", ", new List<string> { "Томатный соус", "Моцарелла", "Базилик" }),
            Price = 499.99m,
            Image = "images/pizza/pizza_margarita.avif",
            Types = string.Join(", ", new List<string> { "Классическая", "Вегетарианская" }),
            Description = "Традиционная итальянская пицца с томатным соусом, моцареллой и базиликом.",
            RecommendedDrinks = string.Join(", ", new List<string> { "Красное вино", "Минеральная вода" })
        },
        new Pizza
        {
            Name = "Пицца Пепперони",
            Ingredients = string.Join(", ", new List<string> { "Томатный соус", "Моцарелла", "Пепперони" }),
            Price = 599.99m,
            Image = "images/pizza/pizza_pepperoni.avif",
            Types = string.Join(", ", new List<string> { "Острая", "Классическая" }),
            Description = "Пицца с пикантными колбасками пепперони и моцареллой.",
            RecommendedDrinks = string.Join(", ", new List<string> { "Кола", "Пиво" })
        },
        new Pizza
        {
            Name = "Пицца Сырная",
            Ingredients = string.Join(", ", new List<string> { "Моцарелла", "Горгонзола", "Пармезан", "Чеддер" }),
            Price = 649.99m,
            Image = "images/pizza/pizza_cheese.avif",
            Types = string.Join(", ", new List<string> { "Сырная", "Вегетарианская" }),
            Description = "Сырное наслаждение с четырьмя видами сыра.",
            RecommendedDrinks = string.Join(", ", new List<string> { "Белое вино", "Грушевая газировка" })
        },
        new Pizza
        {
            Name = "Пицца Ветчина и Грибы",
            Ingredients = string.Join(", ", new List<string> { "Ветчина", "Шампиньоны", "Моцарелла", "Томатный соус" }),
            Price = 549.00m,
            Image = "images/pizza/pizza_hum_and_mushrooms.avif",
            Types = string.Join(", ", new List<string> { "Мясная" }),
            Description = "Классическая пицца с ветчиной и грибами.",
            RecommendedDrinks = string.Join(", ", new List<string> { "Кола", "Апельсиновый сок" })
        },
        new Pizza
        {
            Name = "Пицца Диабло",
            Ingredients = string.Join(", ", new List<string> { "Колбаски чоризо", "Халапеньо", "Соус барбекю", "Митболы из говядины", "Томаты", "Сладкий перец", "Красный лук", "Моцарелла" }),
            Price = 649.00m,
            Image = "images/pizza/pizza_diablo.avif",
            Types = string.Join(", ", new List<string> { "Мясная", "Острая" }),
            Description = "Очень острая пицца с колбасками чоризо и халапеньо.",
            RecommendedDrinks = string.Join(", ", new List<string> { "Минеральная вода", "Кола" })
        },
        new Pizza
        {
            Name = "Пицца Кола-барбекю",
            Ingredients = string.Join(", ", new List<string> { "Пряная говядина", "Пикантная пепперони", "Острые колбаски чоризо", "Соус кола-барбекю", "Моцарелла" }),
            Price = 609.00m,
            Image = "images/pizza/pizza_cola_barbecue.avif",
            Types = string.Join(", ", new List<string> { "Мясная", "Острая" }),
            Description = "Пицца с уникальным соусом кола-барбекю.",
            RecommendedDrinks = string.Join(", ", new List<string> { "Кола", "Черный чай" })
        },
        new Pizza
        {
            Name = "Пицца Бефстроганов",
            Ingredients = string.Join(", ", new List<string> { "Пряная говядина", "Шампиньоны", "Грибной соус", "Маринованные огурчики", "Моцарелла" }),
            Price = 569.00m,
            Image = "images/pizza/pizza_beef_stroganoff.avif",
            Types = string.Join(", ", new List<string> { "Мясная" }),
            Description = "Пицца с бефстрогановым соусом и маринованными огурчиками",
            RecommendedDrinks = string.Join(", ", new List<string> { "Гранатовый сок", "Минеральная вода" })
        },
        new Pizza
        {
            Name = "Пицца Мясная с аджикой",
            Ingredients = string.Join(", ", new List<string> { "Баварские колбаски", "Острый соус аджика", "Чоризо", "Цыпленок", "Пепперони", "Моцарелла" }),
            Price = 569.00m,
            Image = "images/pizza/pizza_with_adjika.avif",
            Types = string.Join(", ", new List<string> { "Мясная", "Острая" }),
            Description = "Мясная пицца с острым соусом аджика",
            RecommendedDrinks = string.Join(", ", new List<string> { "Кола", "Черный чай" })
        },
        new Pizza
        {
            Name = "Пицца Аррива!",
            Ingredients = string.Join(", ", new List<string> { "Цыпленок", "Чоризо", "Соус бургер", "Сладкий перец", "Красный лук", "Моцарелла", "Соус ранч" }),
            Price = 659.00m,
            Image = "images/pizza/pizza_arriva.avif",
            Types = string.Join(", ", new List<string> { "Мясная", "Острая" }),
            Description = "Пицца с цыпленком и острыми колбасками",
            RecommendedDrinks = string.Join(", ", new List<string> { "Кола", "Черный чай" })
        }
    };
        }
    }
}
