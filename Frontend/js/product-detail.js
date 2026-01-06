// Récupérer l'id du produit depuis l'URL
const params = new URLSearchParams(window.location.search);
const productId = params.get('id');

const productName = document.getElementById('productName');
const productPrice = document.getElementById('productPrice');
const productImage = document.getElementById('productImage');
const productDescription = document.getElementById('productDescription');
const addToCartBtn = document.getElementById('addToCart');
const productQuantity = document.getElementById('productQuantity');

if (!productId) {
    productName.textContent = "Product not found";
} else {
    fetch(`http://localhost:5000/api/products/${productId}`)
        .then(res => res.json())
        .then(p => {
            productName.textContent = p.name;
            productPrice.textContent = `$${p.price.toFixed(2)}`;
            productImage.src = `http://localhost:5000${p.imagePath}`;
            productImage.alt = p.name;
            productDescription.textContent = p.description;
        })
        .catch(err => {
            console.error(err);
            productName.textContent = "Error loading product";
        });
}

// Pour l'instant, le bouton et la quantité ne font rien
addToCartBtn.addEventListener('click', () => {
    const qty = parseInt(productQuantity.value);
    alert(`Add ${qty} of ${productName.textContent} to cart (not implemented yet)`);
});
