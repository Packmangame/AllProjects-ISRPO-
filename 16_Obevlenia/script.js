// Инициализация данных
let users = [
    {
        id: 1,
        name: "Администратор",
        email: "admin@example.com",
        password: "admin123",
        role: "admin"
    },
    {
        id: 2,
        name: "Пользователь",
        email: "user@example.com",
        password: "user123",
        role: "user"
    }
];

let ads = [
    {
        id: 1,
        title: "Ноутбук HP",
        description: "Отличный ноутбук в идеальном состоянии",
        category: "electronics",
        price: 25000,
        images: ["https://images.unsplash.com/photo-1593642632823-8f785ba67e45?ixlib=rb-1.2.1&auto=format&fit=crop&w=500&q=80"],
        userId: 2,
        date: "2023-05-10"
    },
    {
        id: 2,
        title: "Диван",
        description: "Мягкий диван, почти новый",
        category: "furniture",
        price: 15000,
        images: ["https://images.unsplash.com/photo-1555041469-a586c61ea9bc?ixlib=rb-1.2.1&auto=format&fit=crop&w=500&q=80"],
        userId: 2,
        date: "2023-05-15"
    },
    {
        id: 3,
        title: "Куртка",
        description: "Зимняя куртка, размер M",
        category: "clothes",
        price: 3000,
        images: ["https://images.unsplash.com/photo-1551232864-3f0890e580d9?ixlib=rb-1.2.1&auto=format&fit=crop&w=500&q=80"],
        userId: 1,
        date: "2023-05-18"
    }
];

let currentUser = null;

// DOM элементы
const loginModal = document.getElementById('login-modal');
const registerModal = document.getElementById('register-modal');
const adModal = document.getElementById('ad-modal');
const loginForm = document.getElementById('login-form');
const registerForm = document.getElementById('register-form');
const adForm = document.getElementById('ad-form');
const authButtons = document.querySelector('.auth-buttons');
const userProfile = document.querySelector('.user-profile');
const usernameSpan = document.querySelector('.username');
const addAdBtn = document.querySelector('.add-ad-btn');
const adsGrid = document.querySelector('.ads-grid');
const categoryFilter = document.getElementById('category-filter');
const priceFilter = document.getElementById('price-filter');
const sortBy = document.getElementById('sort-by');
const searchInput = document.querySelector('.search-box input');
const searchBtn = document.querySelector('.search-box button');
const categoryCards = document.querySelectorAll('.category-card');

// Инициализация при загрузке страницы
document.addEventListener('DOMContentLoaded', () => {
    renderAds();
    setupEventListeners();
    checkAuthStatus();
});

// Рендер объявлений
function renderAds(filteredAds = ads) {
    adsGrid.innerHTML = '';
    
    filteredAds.forEach(ad => {
        const adCard = document.createElement('div');
        adCard.className = 'ad-card';
        adCard.innerHTML = `
            <div class="ad-image" style="background-image: url('${ad.images[0]}')"></div>
            <div class="ad-info">
                <h3 class="ad-title">${ad.title}</h3>
                <div class="ad-price">${ad.price.toLocaleString()} ₽</div>
                <div class="ad-meta">
                    <span>${ad.category}</span>
                    <span>${ad.date}</span>
                </div>
            </div>
        `;
        adsGrid.appendChild(adCard);
    });
}

// Настройка обработчиков событий
function setupEventListeners() {
    // Кнопки входа/регистрации
    document.querySelector('.login-btn').addEventListener('click', () => {
        loginModal.classList.add('active');
    });

    document.querySelector('.register-btn').addEventListener('click', () => {
        registerModal.classList.add('active');
    });

    // Кнопка добавления объявления
    addAdBtn?.addEventListener('click', () => {
        adModal.classList.add('active');
    });

    // Закрытие модальных окон
    document.querySelectorAll('.close-modal').forEach(button => {
        button.addEventListener('click', () => {
            document.querySelectorAll('.modal').forEach(modal => {
                modal.classList.remove('active');
            });
        });
    });

    // Клик вне модального окна
    document.querySelectorAll('.modal').forEach(modal => {
        modal.addEventListener('click', (e) => {
            if (e.target === modal) {
                modal.classList.remove('active');
            }
        });
    });

    // Форма входа
    loginForm?.addEventListener('submit', (e) => {
        e.preventDefault();
        const email = e.target[0].value;
        const password = e.target[1].value;
        loginUser(email, password);
    });

    // Форма регистрации
    registerForm?.addEventListener('submit', (e) => {
        e.preventDefault();
        const name = e.target[0].value;
        const email = e.target[1].value;
        const password = e.target[2].value;
        const confirmPassword = e.target[3].value;
        
        if (password !== confirmPassword) {
            alert('Пароли не совпадают!');
            return;
        }
        
        registerUser(name, email, password);
    });

    // Форма добавления объявления
    adForm?.addEventListener('submit', (e) => {
        e.preventDefault();
        const title = e.target[0].value;
        const description = e.target[1].value;
        const category = e.target[2].value;
        const price = parseInt(e.target[3].value);
        
        // В реальном приложении здесь была бы загрузка изображений
        const newAd = {
            id: ads.length + 1,
            title,
            description,
            category,
            price,
            images: ['https://via.placeholder.com/500'],
            userId: currentUser.id,
            date: new Date().toISOString().split('T')[0]
        };
        
        ads.push(newAd);
        renderAds();
        adModal.classList.remove('active');
        adForm.reset();
        alert('Объявление успешно добавлено!');
    });

    // Фильтрация объявлений
    categoryFilter.addEventListener('change', filterAds);
    priceFilter.addEventListener('change', filterAds);
    sortBy.addEventListener('change', filterAds);
    searchBtn.addEventListener('click', filterAds);
    searchInput.addEventListener('keyup', (e) => {
        if (e.key === 'Enter') filterAds();
    });

    // Выбор категории
    categoryCards.forEach(card => {
        card.addEventListener('click', () => {
            const category = card.getAttribute('data-category');
            categoryFilter.value = category;
            filterAds();
        });
    });
}

// Фильтрация объявлений
function filterAds() {
    const category = categoryFilter.value;
    const priceRange = priceFilter.value;
    const sort = sortBy.value;
    const searchQuery = searchInput.value.toLowerCase();
    
    let filtered = [...ads];
    
    // Фильтр по категории
    if (category !== 'all') {
        filtered = filtered.filter(ad => ad.category === category);
    }
    
    // Фильтр по цене
    if (priceRange !== 'all') {
        const [min, max] = priceRange.split('-').map(Number);
        if (priceRange.endsWith('+')) {
            filtered = filtered.filter(ad => ad.price >= min);
        } else {
            filtered = filtered.filter(ad => ad.price >= min && ad.price <= max);
        }
    }
    
    // Поиск
    if (searchQuery) {
        filtered = filtered.filter(ad => 
            ad.title.toLowerCase().includes(searchQuery) || 
            ad.description.toLowerCase().includes(searchQuery)
        );
    }
    
    // Сортировка
    switch (sort) {
        case 'newest':
            filtered.sort((a, b) => new Date(b.date) - new Date(a.date));
            break;
        case 'oldest':
            filtered.sort((a, b) => new Date(a.date) - new Date(b.date));
            break;
        case 'price-asc':
            filtered.sort((a, b) => a.price - b.price);
            break;
        case 'price-desc':
            filtered.sort((a, b) => b.price - a.price);
            break;
    }
    
    renderAds(filtered);
}

// Работа с пользователями
function loginUser(email, password) {
    const user = users.find(u => u.email === email && u.password === password);
    
    if (user) {
        currentUser = user;
        localStorage.setItem('currentUser', JSON.stringify(user));
        checkAuthStatus();
        loginModal.classList.remove('active');
        loginForm.reset();
        alert(`Добро пожаловать, ${user.name}!`);
    } else {
        alert('Неверный email или пароль');
    }
}

function registerUser(name, email, password) {
    if (users.some(u => u.email === email)) {
        alert('Пользователь с таким email уже существует');
        return;
    }
    
    const newUser = {
        id: users.length + 1,
        name,
        email,
        password,
        role: 'user'
    };
    
    users.push(newUser);
    currentUser = newUser;
    localStorage.setItem('currentUser', JSON.stringify(newUser));
    checkAuthStatus();
    registerModal.classList.remove('active');
    registerForm.reset();
    alert('Регистрация прошла успешно!');
}

function logoutUser() {
    currentUser = null;
    localStorage.removeItem('currentUser');
    checkAuthStatus();
}

function checkAuthStatus() {
    const savedUser = localStorage.getItem('currentUser');
    
    if (savedUser) {
        currentUser = JSON.parse(savedUser);
        authButtons.classList.add('hidden');
        userProfile.classList.remove('hidden');
        usernameSpan.textContent = currentUser.name;
        addAdBtn.classList.remove('hidden');
        
        if (currentUser.role === 'admin') {
            // Здесь можно добавить функционал для администратора
        }
    } else {
        authButtons.classList.remove('hidden');
        userProfile.classList.add('hidden');
        addAdBtn.classList.add('hidden');
    }
    
    // Обработчик кнопки выхода
    document.querySelector('.logout-btn')?.addEventListener('click', logoutUser);
}