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
            .map(checkbox => checkbox.value);
        const searchTerm = searchInput.value;
        const minPrice = parseFloat(minPriceInput.value) || 0;
        // Если maxPriceInput не заполнен, используем большое число
        const maxPrice = parseFloat(maxPriceInput.value) || Infinity;
        
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
                    <strong>${pizza.name}</strong>
                    <p>${pizza.ingredients}</p>
                    <p>Цена: ${pizza.price}</p>
                    <a href="#" class="select-pizza" data-id="${pizza.name}">Выбрать</a>
                </div>
            `).join('');
    
            pageInfo.textContent = `Страница ${data.currentPage} из ${data.totalPages}`;
            prevPageButton.disabled = data.currentPage === 1;
            nextPageButton.disabled = data.currentPage === data.totalPages;
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
});

document.addEventListener('DOMContentLoaded', () => {
    const pizzaContainer = document.querySelector('.pizza-container');

    // Обработчик кликов по кнопке "Выбрать"
    pizzaContainer.addEventListener('click', (event) => {
        if (event.target.classList.contains('select-pizza')) {
            event.preventDefault(); // Предотвращаем переход по ссылке
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
