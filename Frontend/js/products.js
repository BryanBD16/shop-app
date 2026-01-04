let currentPage = 1;
const pageSize = 12;
let currentSearch = "";

// DOM elements
const ul = document.getElementById('products');
const searchInput = document.getElementById('searchInput');
const prevBtn = document.getElementById('prevBtn');
const nextBtn = document.getElementById('nextBtn');
const pageInfo = document.getElementById('pageInfo');

function fetchProducts() {
    fetch(`http://localhost:5000/api/products?page=${currentPage}&search=${encodeURIComponent(currentSearch)}`)
        .then(res => res.json())
        .then(data => {
            ul.innerHTML = ''; // vider la liste avant d'ajouter
            data.forEach(p => {
                const li = document.createElement('li');
                li.textContent = `${p.name} - ${p.price}$`;
                ul.appendChild(li);
            });

            pageInfo.textContent = `Page ${currentPage}`;
            prevBtn.disabled = currentPage === 1;
            nextBtn.disabled = data.length < pageSize; // s'il y a moins que pageSize, fin
        })
        .catch(err => console.error(err));
}

// Gestion des boutons
prevBtn.addEventListener('click', () => {
    if (currentPage > 1) {
        currentPage--;
        fetchProducts();
    }
});

nextBtn.addEventListener('click', () => {
    currentPage++;
    fetchProducts();
});

// Recherche
searchInput.addEventListener('input', () => {
    currentSearch = searchInput.value.trim();
    currentPage = 1; // reset page
    fetchProducts();
});

// Initial load
fetchProducts();
