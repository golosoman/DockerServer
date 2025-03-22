using GigaPizza.Models;

namespace GigaPizza.Services
{
    public class PizzaService
    {
        public Pizza ProcessPizzaInput(AddPizzaViewModel model)
        {
            // Приводим название пиццы к нижнему регистру
            string pizzaName = model.PizzaName.ToLower();

            // Преобразуем ингредиенты:
            // 1. Приводим к нижнему регистру
            // 2. Разбиваем по символу ';'
            // 3. Удаляем лишние пробелы
            // 4. Собираем обратно через ", "
            string ingredients = string.Join(", ",
                model.Ingredients
                    .ToLower()
                    .Split(';', StringSplitOptions.RemoveEmptyEntries)
                    .Select(x => x.Trim()));

            // Цена: если значение меньше 200, устанавливаем 200
            decimal price = model.Price;

            // Генерация уникального имени для файла фото:
            // Получаем расширение файла и генерируем имя с GUID
            string extension = Path.GetExtension(model.Photo.FileName);
            string uniquePhotoName = $"{Guid.NewGuid()}{extension}";

            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/pizza/", uniquePhotoName);
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                model.Photo.CopyTo(stream);
            }

            // Приводим описание к нижнему регистру
            string description = model.Description.ToLower();

            // Преобразуем рекомендуемые напитки аналогичным образом:
            string recommendedDrinks = string.Join(", ",
                model.RecommendedDrinks
                    .ToLower()
                    .Split(';', StringSplitOptions.RemoveEmptyEntries)
                    .Select(x => x.Trim()));

            // Преобразуем категории:
            // Разбиваем строку, приводим к нижнему регистру, удаляем лишние пробелы.
            // Дополнительно можно выполнить поиск существующих категорий в БД.
            // Преобразуем категории:
            // Если model.Categories теперь массив, преобразуем каждое значение:
            var categoryNames = model.Categories
                .Select(x => x.ToLower().Trim())
                .ToList();

            // Создаем объекты PizzaType для каждой категории:
            List<PizzaType> pizzaTypes = categoryNames
                .Select(cat => new PizzaType { Name = cat })
                .ToList();


            Pizza pizza = new Pizza
            {
                Name = pizzaName,
                Ingredients = ingredients,
                Price = price,
                Image = "/images/pizza/" + uniquePhotoName,
                Description = description,
                RecommendedDrinks = recommendedDrinks,
                Types = pizzaTypes
            };

            Console.WriteLine("----- Обработанные данные модели -----");
            Console.WriteLine($"PizzaName: {pizza.Name}");
            Console.WriteLine($"Ingredients: {pizza.Ingredients}");
            Console.WriteLine($"Price: {pizza.Price}");
            Console.WriteLine($"Image (уникальное имя): {pizza.Image}");
            Console.WriteLine($"Categories: {string.Join(", ", categoryNames)}");
            Console.WriteLine($"Description: {pizza.Description}");
            Console.WriteLine($"RecommendedDrinks: {pizza.RecommendedDrinks}");
            Console.WriteLine("---------------------------------------");

            return pizza;
        }
    }

}
