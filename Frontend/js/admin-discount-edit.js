const params = new URLSearchParams(window.location.search);
const discountId = params.get('id');

const editDiscountForm = document.getElementById('editDiscountForm');
const discountTitle = document.getElementById('discountTitle');
const discountPercentage = document.getElementById('discountPercentage');
const discountStartDate = document.getElementById('discountStartDate');
const discountEndDate = document.getElementById('discountEndDate');
const discountTargetType = document.getElementById('discountTargetType');
const discountProductId = document.getElementById('discountProductId');
const discountCategoryId = document.getElementById('discountCategoryId');
const productField = document.getElementById('productField');
const categoryField = document.getElementById('categoryField');
const discountError = document.getElementById('discountError');

let currentProductId = null;
let currentCategoryId = null;
let originalStartIso = null;

function toDateTimeLocalValue(value) {
    const date = new Date(value);
    const offset = date.getTimezoneOffset() * 60000;
    return new Date(date.getTime() - offset).toISOString().slice(0, 16);
}

function toggleTargetFields() {
    const isProduct = discountTargetType.value === 'product';

    productField.classList.toggle('d-none', !isProduct);
    categoryField.classList.toggle('d-none', isProduct);

    discountProductId.disabled = !isProduct;
    discountCategoryId.disabled = isProduct;

    if (isProduct) {
        discountCategoryId.value = '';
    } else {
        discountProductId.value = '';
    }
}

async function getApiErrorMessage(response) {
    const contentType = response.headers.get('content-type') || '';

    if (contentType.includes('application/json')) {
        try {
            const data = await response.json();

            if (typeof data === 'string') {
                return data;
            }

            if (data?.detail) {
                return data.detail;
            }

            if (data?.title && data?.errors) {
                const messages = Object.values(data.errors).flat().filter(Boolean).join(' ');
                return messages ? `${data.title}: ${messages}` : data.title;
            }

            if (data?.message) {
                return data.message;
            }

            return JSON.stringify(data);
        } catch (error) {
            return 'Failed to read API error response';
        }
    }

    const text = await response.text();
    return text || 'Request failed';
}

async function loadProducts() {
    const firstResponse = await fetch('http://localhost:5000/api/admin/products?page=1&search=');
    const firstPage = await firstResponse.json();
    const products = [...(firstPage.items || [])];

    const totalPages = firstPage.totalPages || 0;
    const requests = [];

    for (let page = 2; page <= totalPages; page++) {
        requests.push(fetch(`http://localhost:5000/api/admin/products?page=${page}&search=`).then(res => res.json()));
    }

    const remainingPages = await Promise.all(requests);
    remainingPages.forEach(page => products.push(...(page.items || [])));

    discountProductId.innerHTML = '<option value="">Select a product</option>';

    products.forEach(product => {
        const option = document.createElement('option');
        option.value = product.id;
        option.textContent = product.name;
        discountProductId.appendChild(option);
    });
}

async function loadCategories() {
    const categories = await fetch('http://localhost:5000/api/categories').then(res => res.json());

    discountCategoryId.innerHTML = '<option value="">Select a category</option>';

    categories.forEach(category => {
        const option = document.createElement('option');
        option.value = category.id;
        option.textContent = category.name;
        discountCategoryId.appendChild(option);
    });
}

async function loadDiscount() {
    if (!discountId) {
        alert('Discount not found');
        return;
    }

    const response = await fetch(`http://localhost:5000/api/discounts/${discountId}`);

    if (!response.ok) {
        throw new Error(await getApiErrorMessage(response));
    }

    const discount = await response.json();

    discountTitle.value = discount.title;
    discountPercentage.value = discount.percentage;
    originalStartIso = discount.startDate;
    discountStartDate.value = toDateTimeLocalValue(discount.startDate);
    discountEndDate.value = discount.endDate ? toDateTimeLocalValue(discount.endDate) : '';
    currentProductId = discount.productId;
    currentCategoryId = discount.categoryId;

    if (currentProductId) {
        discountTargetType.value = 'product';
    } else {
        discountTargetType.value = 'category';
    }

    toggleTargetFields();
}

discountTargetType.addEventListener('change', toggleTargetFields);

Promise.all([loadProducts(), loadCategories(), loadDiscount()])
    .then(() => {
        if (currentProductId) {
            discountProductId.value = String(currentProductId);
        }

        if (currentCategoryId) {
            discountCategoryId.value = String(currentCategoryId);
        }
    })
    .catch(err => {
        console.error(err);
        discountError.textContent = err.message;
    });

editDiscountForm.addEventListener('submit', async (e) => {
    e.preventDefault();
    discountError.textContent = '';

    const isProduct = discountTargetType.value === 'product';
    const targetId = isProduct ? discountProductId.value : discountCategoryId.value;

    if (!targetId) {
        discountError.textContent = isProduct ? 'Please select a product.' : 'Please select a category.';
        return;
    }

    // Preserve original start ISO when user did not change the start date input
    let startIsoToSend;
    try {
        const displayedLocal = discountStartDate.value;
        if (originalStartIso && displayedLocal === toDateTimeLocalValue(originalStartIso)) {
            startIsoToSend = originalStartIso;
        } else {
            startIsoToSend = new Date(discountStartDate.value).toISOString();
        }
    } catch (err) {
        startIsoToSend = new Date(discountStartDate.value).toISOString();
    }

    const payload = {
        title: discountTitle.value.trim(),
        percentage: parseInt(discountPercentage.value, 10),
        startDate: startIsoToSend,
        endDate: discountEndDate.value ? new Date(discountEndDate.value).toISOString() : null,
        productId: isProduct ? parseInt(targetId, 10) : null,
        categoryId: isProduct ? null : parseInt(targetId, 10)
    };

    fetch(`http://localhost:5000/api/discounts/${discountId}`, {
        method: 'PUT',
        headers: {
            'Content-Type': 'application/json'
        },
        body: JSON.stringify(payload)
    })
    .then(async res => {
        if (!res.ok) {
            throw new Error(await getApiErrorMessage(res));
        }

        window.location.href = 'admin-discounts.html';
    })
    .catch(err => {
        console.error(err);
        discountError.textContent = err.message;
    });
});