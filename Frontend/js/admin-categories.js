// DOM elements
const categoriesDiv = document.getElementById('categories');

function fetchCategories() {
    let url = `http://localhost:5000/api/categories`;

    fetch(url)
        .then(res => res.json())
        .then(data => {
            categoriesDiv.innerHTML = '';

            data.forEach(p => {
                const col = document.createElement('div');
                col.className = 'col-12 col-sm-6 col-md-4 col-lg-3';

                col.innerHTML = `
                    <div class="card">
                            <div class="card-body d-flex flex-column">
                                <a href="admin-category-detail.html?id=${p.id}" class="text-decoration-none text-dark">
                                    <h5 class="card-title">${p.name}</h5>
                                </a>
                                <div class="d-flex gap-2 mt-auto">
                                    <button class="btn btn-orange"
                                        onclick="location.href='admin-category-edit.html?id=${p.id}'">
                                        Edit
                                    </button>

                                    <button class="btn btn-white"
                                        onclick="deleteCategory(${p.id})">
                                        Delete
                                    </button>
                                </div>
                            </div>
                    </div>
                    
                `;

                categoriesDiv.appendChild(col);
            });
        })
        .catch(err => console.error(err));
}

function deleteCategory(id) {
    if (!confirm('Are you sure you want to delete this category?')) return;

    let url = `http://localhost:5000/api/admin/categories/${id}`;

    fetch(url, {
        method: 'DELETE'
    })
    .then(async res => {
        if (res.ok) {
            alert('Category deleted successfully');
            fetchCategories();
        } else {
            const text = await res.text();
            throw new Error(text || 'Failed to delete category');
        }
    })
    .catch(err => {
        console.error(err);
        alert(err.message); // 👈 affiche le vrai message
    });
}


// Initial load
fetchCategories();