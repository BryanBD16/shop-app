let currentPage = 1;
const pageSize = 12;
let currentSearch = "";
let currentCategoryId = "";

// DOM elements
const productsDiv = document.getElementById('products');
const searchInput = document.getElementById('searchInput');
const prevBtn = document.getElementById('prevBtn');
const nextBtn = document.getElementById('nextBtn');
const pageInfo = document.getElementById('pageInfo');
const categorySelect = document.getElementById('categorySelect');

function renderPriceHtml(originalPrice, discountedPrice) {
    if (discountedPrice !== null && discountedPrice !== undefined) {
        return `
            <div class="fw-bold text-danger">$${discountedPrice.toFixed(2)}</div>
            <div class="small text-muted text-decoration-line-through">$${originalPrice.toFixed(2)}</div>
        `;
    }

    return `$${originalPrice.toFixed(2)}`;
}

function fetchProducts() {
    let url = `http://localhost:5000/api/products?page=${currentPage}&search=${encodeURIComponent(currentSearch)}`;

    if (currentCategoryId) {
        url += `&categoryId=${currentCategoryId}`;
    }

    fetch(url)
        .then(res => res.json())
        .then(data => {
            productsDiv.innerHTML = '';

            data.items.forEach(p => {
                const col = document.createElement('div');
                col.className = 'col-12 col-sm-6 col-md-4 col-lg-3';

                col.innerHTML = `
                    <div class="card">
                        <a href="product-detail.html?id=${p.id}" class="text-decoration-none text-dark">
                            <img 
                                src="http://localhost:5000${p.imagePath}" 
                                class="card-img-top" 
                                alt="${p.name}" 
                            />
                        </a>
                        <div class="card-body d-flex flex-column">
                            <a href="product-detail.html?id=${p.id}" class="text-decoration-none text-dark">
                                <h5 class="card-title">${p.name}</h5>
                                <div class="card-text">${renderPriceHtml(p.originalPrice ?? p.price, p.discountedPrice)}</div>
                            </a>
                            <button class="btn btn-orange mt-auto">
                                Add to Cart
                            </button>
                        </div>
                    </div>
                `;

                productsDiv.appendChild(col);
            });

            pageInfo.textContent = `Page ${data.currentPage} of ${data.totalPages}`;
            prevBtn.disabled = data.currentPage === 1;
            nextBtn.disabled = data.currentPage >= data.totalPages;
        })
        .catch(err => console.error(err));
}

function fetchCategories() {
    fetch("http://localhost:5000/api/categories")
        .then(res => res.json())
        .then(data => {
            data.forEach(cat => {
                const option = document.createElement("option");
                option.value = cat.id;
                option.textContent = cat.name;
                categorySelect.appendChild(option);
            });
        })
        .catch(err => console.error(err));
}

categorySelect.addEventListener('change', () => {
    currentCategoryId = categorySelect.value;
    currentPage = 1;
    fetchProducts();
});

// Previous
prevBtn.addEventListener('click', () => {
    if (currentPage > 1) {
        currentPage--;
        fetchProducts();
    }
});

// Next
nextBtn.addEventListener('click', () => {
    currentPage++;
    fetchProducts();
});

// Search
searchInput.addEventListener('input', () => {
    currentSearch = searchInput.value.trim();
    currentPage = 1;
    fetchProducts();
});

// Initial load
fetchCategories();
fetchProducts();
