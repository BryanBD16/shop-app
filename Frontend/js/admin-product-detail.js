const params = new URLSearchParams(window.location.search);
const productId = params.get('id');

const productName = document.getElementById('productName');
const productPrice = document.getElementById('productPrice');
const productImage = document.getElementById('productImage');
const productDescription = document.getElementById('productDescription');
const editBtn = document.getElementById('edit');
const deleteBtn = document.getElementById('delete');
const productStock = document.getElementById('productStock');
const productIsPublished = document.getElementById('productIsPublished');

if (!productId) {
    productName.textContent = "Product not found";
} else {
    fetch(`http://localhost:5000/api/admin/products/${productId}`)
        .then(res => res.json())
        .then(p => {
            productName.textContent = p.name;
            productPrice.textContent = `$${p.price.toFixed(2)}`;
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

deleteBtn.addEventListener('click', () => {
    if (!productId) return;

    const confirmed = confirm(
        "Are you sure you want to delete this product?\nThis action cannot be undone."
    );

    if (!confirmed) return;

    fetch(`http://localhost:5000/api/admin/products/${productId}`, {
        method: "DELETE"
    })
    .then(res => {
        if (res.status === 204) {
            alert("Product deleted successfully.");
            window.location.href = "admin-products.html";
        } 
        else if (res.status === 404) {
            alert("Product not found.");
        } 
        else {
            throw new Error("Delete failed");
        }
    })
    .catch(err => {
        console.error(err);
        alert("Error deleting product");
    });
});
