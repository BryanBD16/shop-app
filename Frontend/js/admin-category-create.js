const form = document.getElementById('createCategoryForm');

const categoryName = document.getElementById('categoryName');
const errorDiv = document.getElementById('categoryNameError');

/* ============================
   Submit form (CREATE category)
============================ */
form.addEventListener('submit', (e) => {
    e.preventDefault();

    const newCategory = {
        name: categoryName.value.trim()
    };

    fetch('http://localhost:5000/api/admin/categories', {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json'
        },
        body: JSON.stringify(newCategory)
    })
    .then(async res => {
        errorDiv.textContent = ''; // reset

        if (!res.ok) {
            const text = await res.text();
            throw new Error(text);
        }

        return res.json();
    })
    .then(createdCategory => {
        // Redirect after successful creation
        window.location.href = `admin-categories.html`;
    })
    .catch(err => {
        console.error(err);
        errorDiv.textContent = err.message; 
    });
});
