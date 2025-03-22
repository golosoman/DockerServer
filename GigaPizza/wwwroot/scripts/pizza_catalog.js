function sendAjaxRequest(url, method, data, callback) {
    const xhr = new XMLHttpRequest();
    xhr.open(method, url, true);
    xhr.setRequestHeader('Content-Type', 'application/json');

    xhr.onreadystatechange = function () {
        if (xhr.readyState === 4) { // Запрос завершён
            if (xhr.status >= 200 && xhr.status < 300) {
                callback(null, JSON.parse(xhr.responseText));
            } else {
                callback(new Error(`Ошибка HTTP: ${xhr.status}`));
            }
        }
    };
    
    xhr.send(JSON.stringify(data));
}

document.addEventListener('DOMContentLoaded', () => {
    const filterCheckboxes = document.querySelectorAll('input[name="type"]');
    const searchInput = document.getElementById('search');
    const minPriceInput = document.getElementById('min-price');
    const maxPriceInput = document.getElementById('max-price');
    const pizzaContainer = document.querySelector('.pizza-container');
    const prevPageButton = document.getElementById('prevPage');
    const nextPageButton = document.getElementById('nextPage');
    const pageInfo = document.getElementById('pageInfo');

    let currentPage = 1;
    const itemsPerPage = 4;

    function updatePizzas(page = 1) {
        const selectedTypes = Array.from(document.querySelectorAll('input[name="type"]:checked'))
            .map(checkbox => checkbox.value.toLowerCase());
        const searchTerm = searchInput.value.toLowerCase();
        const minPrice = parseFloat(minPriceInput.value) || 0;
        const maxPrice = parseFloat(maxPriceInput.value) || Infinity;

        console.log({
            Types: selectedTypes,
            SearchTerm: searchTerm,
            MinPrice: minPrice,
            MaxPrice: maxPrice,
            Page: page,
            ItemsPerPage: itemsPerPage
        });

        sendAjaxRequest('/api/pizza/filter', 'POST', {
            Types: selectedTypes,
            SearchTerm: searchTerm,
            MinPrice: minPrice,
            MaxPrice: maxPrice,
            Page: page,
            ItemsPerPage: itemsPerPage
        }, (error, data) => {
            if (error) {
                console.error("Ошибка при загрузке данных:", error);
                return;
            }

            console.log("Полученные данные:", data);

            pizzaContainer.innerHTML = data.pizzas.map((pizza) => `
            <div class="pizza-item">
                <div class="img-container">
                    <img src="${pizza.image}" alt="${pizza.name}">
                    <img src="${pizza.image}" alt="${pizza.name}" class="zoomed-img">
                </div>
                <strong>${pizza.name.charAt(0).toUpperCase() + pizza.name.slice(1)}</strong>

                <p>${pizza.ingredients}</p>
                <p>Цена: ${pizza.price}</p>
                <a href="#" class="select-pizza" data-id="${pizza.name}">Выбрать</a>
            </div>
        `).join('');

            pageInfo.textContent = `Страница ${data.totalPages == 0 ? 0 : data.currentPage} из ${data.totalPages}`;
            prevPageButton.disabled = data.currentPage === 1;
            nextPageButton.disabled = data.currentPage === data.totalPages;

            document.querySelectorAll('.img-container').forEach(imgContainer => {
                const zoomedImg = imgContainer.querySelector('.zoomed-img');
                // При движении мыши по контейнеру
                console.log("Сделано!")
                imgContainer.addEventListener('mousemove', (e) => {
                    const { left, top } = imgContainer.getBoundingClientRect();
                    const x = e.clientX - left;
                    const y = e.clientY - top;
                    zoomedImg.style.transform = `translate(-${x}px, -${y}px) scale(1)`;
                });
                // При уходе мыши с контейнера
                imgContainer.addEventListener('mouseleave', () => {
                    zoomedImg.style.transform = 'scale(0)';
                });
            });

        });
    }


    
    // Обработчики событий для фильтров
    filterCheckboxes.forEach(checkbox => checkbox.addEventListener('change', () => {
        currentPage = 1;
        updatePizzas(currentPage);
    }));
    searchInput.addEventListener('input', () => {
        currentPage = 1;
        updatePizzas(currentPage);
    });
    minPriceInput.addEventListener('input', () => {
        currentPage = 1;
        updatePizzas(currentPage);
    });
    maxPriceInput.addEventListener('input', () => {
        currentPage = 1;
        updatePizzas(currentPage);
    });

    // Обработчики событий для кнопок пагинации
    prevPageButton.addEventListener('click', () => {
        if (currentPage > 1) {
            currentPage--;
            updatePizzas(currentPage);
        }
    });
    nextPageButton.addEventListener('click', () => {
        currentPage++;
        updatePizzas(currentPage);
    });

    // Первоначальная загрузка данных
    updatePizzas(currentPage);

    //const searchInput = document.getElementById('search'); 
    const autocompleteResults = document.getElementById('autocomplete-results');

    // При вводе текста в поле поиска вызывается автодополнение
    searchInput.addEventListener('input', () => {
        const query = searchInput.value.trim();
        if (query.length < 2) {
            autocompleteResults.innerHTML = '';
            autocompleteResults.style.display = 'none';
            return;
        }

        // Отправляем запрос на автодополнение
        sendAjaxRequest(`/api/pizza/autocomplete?term=${encodeURIComponent(query)}`, 'GET', null, (error, data) => {
            if (error) {
                console.error("Ошибка при автозаполнении:", error);
                return;
            }

            // Очищаем предыдущие подсказки
            autocompleteResults.innerHTML = '';

            if (data.length === 0) {
                autocompleteResults.style.display = 'none';
                return;
            }

            // Ограничиваем количество подсказок, например, до 3-х
            const suggestions = data.slice(0, 3);

            function capitalize(str) {
                return str.charAt(0).toUpperCase() + str.slice(1);
            }

            // Формируем список подсказок
            suggestions.forEach(name => {
                const item = document.createElement('div');
                item.classList.add('autocomplete-item');
                item.textContent = capitalize(name);

                // При клике по подсказке заполняем поле ввода и скрываем список
                item.addEventListener('click', () => {
                    searchInput.value = capitalize(name);
                    autocompleteResults.innerHTML = '';
                    autocompleteResults.style.display = 'none';

                    updatePizzas(1);
                });

                autocompleteResults.appendChild(item);
            });

            autocompleteResults.style.display = 'block';
        });
    });

    // Закрываем подсказки при клике вне поля ввода и контейнера подсказок
    document.addEventListener('click', (event) => {
        if (!searchInput.contains(event.target) && !autocompleteResults.contains(event.target)) {
            autocompleteResults.innerHTML = '';
            autocompleteResults.style.display = 'none';
        }
    });
});

document.addEventListener("DOMContentLoaded", function () {
    document.querySelectorAll(".img-container").forEach(container => {
        const img = container.querySelector("img");
        const zoomedImg = container.querySelector(".zoomed-img");

        container.addEventListener("mousemove", (e) => {
            const rect = container.getBoundingClientRect();
            const x = e.clientX - rect.left;
            const y = e.clientY - rect.top;
            zoomedImg.style.transform = `translate(-${x}px, -${y}px)`;
        });

        container.addEventListener("mouseleave", () => {
            zoomedImg.style.transform = "scale(0)";
        });
    });
});

document.addEventListener('DOMContentLoaded', () => {
    const pizzaContainer = document.querySelector('.pizza-container');
    if (!pizzaContainer) return; // If pizzaContainer is not found, stop execution

    // Your event listeners here
    pizzaContainer.addEventListener('click', (event) => {
        if (event.target.classList.contains('select-pizza')) {
            event.preventDefault();
            const pizzaName = event.target.getAttribute('data-id');
            openPizzaModal(pizzaName);
        }

    });

    function openPizzaModal(pizzaName) {

        sendAjaxRequest(`/api/pizza/details?name=${encodeURIComponent(pizzaName)}`, 'GET', null, (error, pizza) => {
            if (error) {
                console.error("Ошибка при загрузке данных:", error);
                return;
            }
            showPizzaModal(pizza);
        });
    }

    function showPizzaModal(pizza) {
        // Создаём разметку модального окна
        const modal = document.createElement('div');
        modal.classList.add('modal');
        modal.innerHTML = `
            <div class="modal-content">
                <span class="close">&times;</span>
                <div class="container-for-image">
                    <img src="${pizza.image}" alt="${pizza.name}" class="pizza-image">
                </div>
                <div class="description">
                    <h1>${pizza.name}</h1>
                    <p>${pizza.description}</p>
                    <h2>Ингредиенты:</h2>
                    <ul>
                        ${pizza.ingredients.split(',').map(ingredient => `<li>${ingredient.trim()}</li>`).join('')}
                    </ul>
                    <h2>Рекомендации по напиткам:</h2>
                    <ul>
                        ${pizza.recommendedDrinks.split(',').map(drink => `<li>${drink.trim()}</li>`).join('')}
                    </ul>
                    <p><strong>Цена: ${pizza.price}</strong></p>
                    <button class="add-to-cart">Добавить в корзину</button>
                </div>
            </div>
        `;

        document.body.appendChild(modal);

        // Закрытие модального окна при клике по кнопке "Закрыть"
        modal.querySelector('.close').addEventListener('click', () => {
            document.body.removeChild(modal);
        });

        // Закрытие при клике вне окна
        modal.addEventListener('click', (e) => {
            if (e.target === modal) {
                document.body.removeChild(modal);
            }
        });
    }
});


