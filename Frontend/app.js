fetch('http://localhost:5000/api/products')
    .then(res => res.json())
    .then(data => {
        const ul = document.getElementById('products');

        data.forEach(p => {
            const li = document.createElement('li');
            li.textContent = `${p.name} - ${p.price}$`;
            ul.appendChild(li);
        });
    })
    .catch(err => console.error(err));
