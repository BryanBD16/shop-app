const params = new URLSearchParams(window.location.search);
const categoryId = params.get('id');

const form = document.getElementById('editCategoryForm');

const categoryName = document.getElementById('categoryName');
const errorDiv = document.getElementById('categoryNameError');

if (!categoryId) {
    alert("Category not found");
}


// ==========================================
// Load category data
// ==========================================
fetch(`http://localhost:5000/api/categories/${categoryId}`)
    .then(res => {
        if (!res.ok) throw new Error("Failed to load category");
        return res.json();
    })
    .then(category => {
        categoryName.value = category.name;
        errorDiv.textContent = "";
    })
    .catch(err => {
        console.error(err);
        alert("Failed to load category");
    });

// ==========================================
// Save changes
// ==========================================
form.addEventListener('submit', (e) => {
    e.preventDefault();

    errorDiv.textContent = '';

    const editCategory = {
        name: categoryName.value.trim()
    };

    fetch(`http://localhost:5000/api/admin/categories/${categoryId}`, {
        method: "PUT",
        headers: {
            "Content-Type": "application/json"
        },
        body: JSON.stringify(editCategory)
    })
    .then(async res => {
        if (!res.ok) {
            const text = await res.text();
            throw new Error(text);
        }
        return;
    })
    .then(() => {
        window.location.href = `admin-categories.html`;
    })
    .catch(err => {
        console.error(err);
        errorDiv.textContent = err.message; 
    });
});
