async function loadLayout() {
  const header = await fetch('partials/header.html').then(r => r.text());
  const footer = await fetch('partials/footer.html').then(r => r.text());

  document.getElementById('header').innerHTML = header;
  document.getElementById('footer').innerHTML = footer;
}

loadLayout();
