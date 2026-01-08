let currentPage = 1;
const pageSize = 12;
let currentSearch = "";

// DOM elements
const productsDiv = document.getElementById('products');
const searchInput = document.getElementById('searchInput');
const prevBtn = document.getElementById('prevBtn');
const nextBtn = document.getElementById('nextBtn');
const pageInfo = document.getElementById('pageInfo');

function fetchProducts() {
    fetch(`http://localhost:5000/api/admin/products?page=${currentPage}&search=${encodeURIComponent(currentSearch)}`)
        .then(res => res.json())
        .then(data => {
            productsDiv.innerHTML = '';

            data.items.forEach(p => {
                const col = document.createElement('div');
                col.className = 'col-12 col-sm-6 col-md-4 col-lg-3';

                col.innerHTML = `
                    <a href="admin-product-detail.html?id=${p.id}" class="text-decoration-none text-dark">
                        <div class="card">
                            <img 
                                src="http://localhost:5000${p.imagePath}" 
                                class="card-img-top" 
                                alt="${p.name}" 
                            />
                            <div class="card-body d-flex flex-column">
                                <h5 class="card-title">${p.name}</h5>
                                <p class="card-text">$${p.price.toFixed(2)}</p>
                                <p class="card-text">Stock: ${p.stockQuantity}</p>
                                <button class="btn btn-orange mt-auto">
                                    Edit
                                </button>
                            </div>
                        </div>
                    </a>
                `;

                productsDiv.appendChild(col);
            });


            // Pagination UI
            pageInfo.textContent = `Page ${data.currentPage} of ${data.totalPages}`;
            prevBtn.disabled = data.currentPage === 1;
            nextBtn.disabled = data.currentPage >= data.totalPages;
        })
        .catch(err => console.error(err));
}

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
fetchProducts();
