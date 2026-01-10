const form = document.getElementById('createProductForm');

const productName = document.getElementById('productName');
const productPrice = document.getElementById('productPrice');
const productDescription = document.getElementById('productDescription');
const productStock = document.getElementById('productStock');
const productIsPublished = document.getElementById('productIsPublished');

const imageGallery = document.getElementById('imageGallery');
const imagePreview = document.getElementById('productImagePreview');
const imagePathInput = document.getElementById('productImagePath');

/* ============================
   Load available product images
============================ */
fetch('http://localhost:5000/api/admin/product-images')
    .then(res => res.json())
    .then(images => {
        images.forEach(path => {
            const img = document.createElement('img');
            img.src = `http://localhost:5000${path}`;
            img.className = 'img-thumbnail';
            img.style.width = '80px';
            img.style.cursor = 'pointer';

            img.addEventListener('click', () => {
                // Update image preview
                imagePreview.src = img.src;
                imagePathInput.value = path;

                // Remove visual error state if present
                imageGallery.classList.remove('image-error');
            });

            imageGallery.appendChild(img);
        });
    })
    .catch(err => console.error('Failed to load images', err));

/* ============================
   Submit form (CREATE product)
============================ */
form.addEventListener('submit', (e) => {
    e.preventDefault();

    // Image selection is required
    if (!imagePathInput.value) {
        imageGallery.classList.add('image-error');
        alert('Please select a product image.');
        return;
    } else {
        imageGallery.classList.remove('image-error');
    }

    const newProduct = {
        name: productName.value.trim(),
        price: parseFloat(productPrice.value),
        imagePath: imagePathInput.value,
        description: productDescription.value.trim(),
        stockQuantity: parseInt(productStock.value),
        isPublished: productIsPublished.value === "true"
    };

    fetch('http://localhost:5000/api/admin/products', {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json'
        },
        body: JSON.stringify(newProduct)
    })
    .then(res => {
        if (!res.ok) throw new Error('Failed to create product');
        return res.json();
    })
    .then(createdProduct => {
        // Redirect after successful creation
        window.location.href = `admin-product-detail.html?id=${createdProduct.id}`;
    })
    .catch(err => {
        console.error(err);
        alert('Error while creating product');
    });
});
