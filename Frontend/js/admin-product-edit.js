// ==========================================
// Admin Product Edit
// ==========================================

const params = new URLSearchParams(window.location.search);
const productId = params.get('id');

const form = document.getElementById('editProductForm');

const productName = document.getElementById('productName');
const productPrice = document.getElementById('productPrice');
const productImagePath = document.getElementById('productImagePath');
const productImagePreview = document.getElementById('productImagePreview');
const productDescription = document.getElementById('productDescription');
const productStock = document.getElementById('productStock');
const productIsPublished = document.getElementById('productIsPublished');
const imageGallery = document.getElementById('imageGallery');

let currentImagePath = null;

if (!productId) {
    alert("Product not found");
}

// ==========================================
// Load product data
// ==========================================
fetch(`http://localhost:5000/api/admin/products/${productId}`)
    .then(res => {
        if (!res.ok) throw new Error("Failed to load product");
        return res.json();
    })
    .then(product => {
        productName.value = product.name;
        productPrice.value = product.price;
        productDescription.value = product.description;
        productStock.value = product.stockQuantity;
        productIsPublished.value = product.isPublished.toString();

        currentImagePath = product.imagePath;
        productImagePath.value = currentImagePath;

        productImagePreview.src = `http://localhost:5000${currentImagePath}`;
        productImagePreview.alt = product.name;

        loadImageGallery(currentImagePath);
    })
    .catch(err => {
        console.error(err);
        alert("Error loading product");
    });

// ==========================================
// Load image gallery
// ==========================================
function loadImageGallery(selectedPath) {
    fetch("http://localhost:5000/api/admin/product-images")
        .then(res => res.json())
        .then(images => {
            imageGallery.innerHTML = "";

            images.forEach(path => {
                const img = document.createElement('img');
                img.src = `http://localhost:5000${path}`;
                img.className = "image-thumb";
                img.dataset.path = path;
                img.alt = "Product image";

                // Auto-select current image
                if (path === selectedPath) {
                    img.classList.add("selected");
                }

                img.addEventListener('click', () => {
                    document
                        .querySelectorAll('.image-thumb')
                        .forEach(i => i.classList.remove('selected'));

                    img.classList.add('selected');

                    productImagePath.value = path;
                    productImagePreview.src = `http://localhost:5000${path}`;
                });

                imageGallery.appendChild(img);
            });
        })
        .catch(err => {
            console.error(err);
            alert("Failed to load image gallery");
        });
}

// ==========================================
// Save changes
// ==========================================
form.addEventListener('submit', (e) => {
    e.preventDefault();

    const payload = {
        name: productName.value.trim(),
        price: parseFloat(productPrice.value),
        imagePath: productImagePath.value,
        description: productDescription.value.trim(),
        stockQuantity: parseInt(productStock.value),
        isPublished: productIsPublished.value === "true"
    };

    fetch(`http://localhost:5000/api/admin/products/${productId}`, {
        method: "PUT",
        headers: {
            "Content-Type": "application/json"
        },
        body: JSON.stringify(payload)
    })
    .then(res => {
        if (!res.ok) throw new Error("Update failed");
        alert("Product updated successfully");
        window.location.href = `admin-product-detail.html?id=${productId}`;
    })
    .catch(err => {
        console.error(err);
        alert("Error saving product");
    });
});
