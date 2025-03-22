
function validatePizzaName() {
    const pizzaName = document.getElementById('pizza-name').value.trim();
    const errorSpan = document.getElementById('pizza-name-error');
    errorSpan.innerHTML = '';
    errorSpan.style.display = 'none';

    if (!pizzaName.startsWith('Пицца ') || pizzaName.length < 8) {
        errorSpan.innerHTML = 'Название пиццы должно начинаться с "Пицца" и содержать минимум 8 символов.';
        errorSpan.style.display = 'inline';
    }
}

function validateIngredients() {
    const ingredients = document.getElementById('ingredients').value.trim();
    const errorSpan = document.getElementById('ingredients-error');
    errorSpan.innerHTML = '';
    errorSpan.style.display = 'none';

    const ingredientsList = ingredients.split(';').map(item => item.trim()).filter(item => item);
    const minLength = Math.min(...ingredientsList.map(item => item.length));

    if (ingredientsList.length < 1 || ingredients.endsWith(';')) {
        errorSpan.innerHTML = 'Ингредиенты должны быть разделены точкой с запятой и не могут заканчиваться на ";"';
        errorSpan.style.display = 'inline';
    } else if (minLength < 3) {
        errorSpan.innerHTML = 'Каждый ингредиент должен содержать минимум 3 символа.';
        errorSpan.style.display = 'inline';
    }
}

function validatePrice() {
    const price = document.getElementById('price').value;
    const errorSpan = document.getElementById('price-error');
    errorSpan.innerHTML = '';
    errorSpan.style.display = 'none';

    if (price <= 200) {
        errorSpan.innerHTML = 'Цена должна быть положительной и больше либо равно 200.';
        errorSpan.style.display = 'inline';
    }
}

function validatePhoto() {
    const photo = document.getElementById('photo').files[0];
    const errorSpan = document.getElementById('photo-error');
    errorSpan.innerHTML = '';
    errorSpan.style.display = 'none';

    if (!photo) {
        errorSpan.innerHTML = 'Выберите фото пиццы.';
        errorSpan.style.display = 'inline';
    } else {
        const allowedExtensions = /(\.jpg|\.jpeg|\.png|\.gif)$/i;
        if (!allowedExtensions.exec(photo.name)) {
            errorSpan.innerHTML = 'Допустимые форматы изображения: .jpg, .jpeg, .png, .gif.';
            errorSpan.style.display = 'inline';
        } else if (photo.size > 2 * 1024 * 1024) {
            errorSpan.innerHTML = 'Размер фото не должен превышать 2MB.';
            errorSpan.style.display = 'inline';
        }
    }
}

function validateDescription() {
    const description = document.getElementById('description').value.trim();
    const errorSpan = document.getElementById('description-error');
    errorSpan.innerHTML = '';
    errorSpan.style.display = 'none';

    if (description.length < 20) {
        errorSpan.innerHTML = 'Описание должно содержать минимум 20 символов.';
        errorSpan.style.display = 'inline';
    }
}

function validateCategory() {
    const checkboxes = document.querySelectorAll('input[type="checkbox"]');
    const errorSpan = document.getElementById('category-error');
    errorSpan.innerHTML = '';
    errorSpan.style.display = 'none';

    const isChecked = Array.from(checkboxes).some(checkbox => checkbox.checked);

    if (!isChecked) {
        errorSpan.innerHTML = 'Пожалуйста, выберите хотя бы одну категорию.';
        errorSpan.style.display = 'inline';
    }
}

function validateRecommendedDrinks() {
    const recommendedDrinks = document.getElementById('recommended-drinks').value.trim();
    const errorSpan = document.getElementById('recommended-drinks-error');
    errorSpan.innerHTML = '';
    errorSpan.style.display = 'none';

    const drinksList = recommendedDrinks.split(';').map(item => item.trim()).filter(item => item);
    const minLength = Math.min(...drinksList.map(item => item.length));

    if (drinksList.length < 1 || recommendedDrinks.endsWith(';')) {
        errorSpan.innerHTML = 'Рекомендуемые напитки должны быть разделены точкой с запятой и не могут заканчиваться на ";"';
        errorSpan.style.display = 'inline';
    } else if (minLength < 3) {
        errorSpan.innerHTML = 'Каждое название напитка должно содержать минимум 3 символа.';
        errorSpan.style.display = 'inline';
    }
}

// Вешаем обработчики валидации на события
document.getElementById('recommended-drinks').addEventListener('input', validateRecommendedDrinks);
document.getElementById('pizza-name').addEventListener('input', validatePizzaName);
document.getElementById('ingredients').addEventListener('input', validateIngredients);
document.getElementById('price').addEventListener('input', validatePrice);
document.getElementById('photo').addEventListener('change', validatePhoto);
document.getElementById('description').addEventListener('input', validateDescription);

const checkboxes = document.querySelectorAll('input[type="checkbox"]');
checkboxes.forEach(checkbox => {
    checkbox.addEventListener('change', validateCategory);
});

// Обработка отправки формы через AJAX
document.getElementById('pizza-form').addEventListener('submit', function (event) {
    event.preventDefault();

    // Выполняем все проверки перед отправкой
    validatePizzaName();
    validateIngredients();
    validatePrice();
    validatePhoto();
    validateDescription();
    validateCategory();
    validateRecommendedDrinks();

    const hasError = document.querySelectorAll('.error-message[style*="display: inline"]').length > 0;
    if (hasError) {
        // Если есть ошибки, не отправляем форму
        return;
    }

    // Собираем данные формы
    const form = this;
    const formData = new FormData(form);

    // Опционально: можно вывести данные для отладки
    // for (let [key, value] of formData.entries()) {
    //     console.log(`${key}: ${value}`);
    // }

    // Отправка AJAX-запроса
    fetch(form.action, {
        method: form.method,
        body: formData
    })
        .then(response => {
            if (!response.ok) {
                throw new Error('Ошибка при отправке данных');
            }
            return response.json();
        })
        .then(data => {
            // Обработка ответа сервера
            if (data.success) {
                alert('Пицца успешно добавлена!');
                // При необходимости можно перенаправить пользователя или сбросить форму:
                form.reset();
            } else {
                // Если сервер вернул ошибки валидации, отобразите их
                // Например, data.errors может быть объектом с ключами, соответствующими именам полей
                for (const key in data.errors) {
                    const errorSpan = document.getElementById(`${key.toLowerCase()}-error`);
                    if (errorSpan) {
                        errorSpan.innerHTML = data.errors[key];
                        errorSpan.style.display = 'inline';
                    }
                }
            }
        })
        .catch(error => {
            console.error('Ошибка отправки формы:', error);
            alert('При отправке данных произошла ошибка.');
        });
});
