const discountsDiv = document.getElementById('discounts');

function parseServerDate(value) {
    if (!value) return null;

    // If value contains timezone info (Z or +/-) let Date parse it.
    // If it looks like an ISO without timezone (e.g. "2026-04-29T12:00:00"),
    // treat it as UTC by appending 'Z'.
    try {
        const tzPattern = /[zZ]|[+-]\d{2}:?\d{2}$/;
        if (typeof value === 'string' && !tzPattern.test(value)) {
            return new Date(value + 'Z');
        }

        return new Date(value);
    } catch (err) {
        return null;
    }
}

function formatDate(value) {
    const dt = parseServerDate(value);
    if (!dt || Number.isNaN(dt.getTime())) return value ? String(value) : '';
    return dt.toLocaleString(undefined, { dateStyle: 'medium', timeStyle: 'short' });
}

function isFutureDiscount(discount) {
    return new Date(discount.startDate) > new Date();
}

function getTargetLabel(discount) {
    if (discount.productName) {
        return `Product: ${discount.productName}`;
    }

    if (discount.categoryName) {
        return `Category: ${discount.categoryName}`;
    }

    if (discount.productId) {
        return `Product #${discount.productId}`;
    }

    if (discount.categoryId) {
        return `Category #${discount.categoryId}`;
    }

    return 'No target';
}

function renderDiscountCard(discount) {
    const canDelete = isFutureDiscount(discount);

    return `
        <div class="col-12 col-md-6 col-lg-4">
            <div class="card h-100">
                <div class="card-body d-flex flex-column">
                    <h5 class="card-title">${discount.title}</h5>
                    <p class="card-text mb-1"><strong>Percentage:</strong> ${discount.percentage}%</p>
                    <p class="card-text mb-1"><strong>Target:</strong> ${getTargetLabel(discount)}</p>
                    <p class="card-text mb-1"><strong>Start:</strong> ${formatDate(discount.startDate)}</p>
                    <p class="card-text mb-3"><strong>End:</strong> ${discount.endDate ? formatDate(discount.endDate) : 'No end date'}</p>
                    <div class="d-flex gap-2 mt-auto flex-wrap">
                        <a class="btn btn-orange" href="admin-discount-edit.html?id=${discount.id}">Edit</a>
                        ${canDelete ? `<button class="btn btn-white" onclick="deleteDiscount(${discount.id})">Delete</button>` : ''}
                    </div>
                </div>
            </div>
        </div>
    `;
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

function fetchDiscounts() {
    fetch('http://localhost:5000/api/discounts')
        .then(res => res.json())
        .then(discounts => {
            discountsDiv.innerHTML = '';

            if (!discounts.length) {
                discountsDiv.innerHTML = '<p class="text-center">No discounts found.</p>';
                return;
            }

            discounts.forEach(discount => {
                const wrapper = document.createElement('div');
                wrapper.innerHTML = renderDiscountCard(discount);
                discountsDiv.appendChild(wrapper.firstElementChild);
            });
        })
        .catch(err => console.error(err));
}

function deleteDiscount(id) {
    if (!confirm('Are you sure you want to delete this discount?')) {
        return;
    }

    fetch(`http://localhost:5000/api/discounts/${id}`, {
        method: 'DELETE'
    })
        .then(async res => {
            if (!res.ok) {
                throw new Error(await getApiErrorMessage(res));
            }

            fetchDiscounts();
        })
        .catch(err => {
            console.error(err);
            alert(err.message);
        });
}

fetchDiscounts();