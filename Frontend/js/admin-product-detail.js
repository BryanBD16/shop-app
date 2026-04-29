const params = new URLSearchParams(window.location.search);
const productId = params.get('id');

const productName = document.getElementById('productName');
const productPrice = document.getElementById('productPrice');
const productImage = document.getElementById('productImage');
const productDescription = document.getElementById('productDescription');
const editBtn = document.getElementById('edit');
const productStock = document.getElementById('productStock');
const productIsPublished = document.getElementById('productIsPublished');

function renderPriceHtml(originalPrice, discountedPrice) {
    if (discountedPrice !== null && discountedPrice !== undefined) {
        return `
            <div class="fw-bold text-danger">$${discountedPrice.toFixed(2)}</div>
            <div class="small text-muted text-decoration-line-through">$${originalPrice.toFixed(2)}</div>
        `;
    }

    return `$${originalPrice.toFixed(2)}`;
}

if (!productId) {
    productName.textContent = "Product not found";
} else {
    fetch(`http://localhost:5000/api/admin/products/${productId}`)
        .then(res => res.json())
        .then(p => {
            productName.textContent = p.name;
            productPrice.innerHTML = renderPriceHtml(p.originalPrice ?? p.price, p.discountedPrice);
            productImage.src = `http://localhost:5000${p.imagePath}`;
            productImage.alt = p.name;
            productDescription.textContent = p.description;
            productStock.textContent = `Stock: ${p.stockQuantity}`;
            productIsPublished.textContent = `Published: ${p.isPublished ? "Yes" : "No"}`;
        })
        .catch(err => {
            console.error(err);
            productName.textContent = "Error loading product";
        });
}

editBtn.addEventListener('click', () => {
    location.href=`admin-product-edit.html?id=${productId}`;
});

