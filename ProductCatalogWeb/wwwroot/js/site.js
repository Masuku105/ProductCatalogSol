let currentPage = 1;
let totalPages = 1;

async function loadData(search = '', page = 1) {
    const category = document.getElementById('categoryFilter').value;
    const sort = document.getElementById('sortOrder').value;

    const query = new URLSearchParams({ search, page, category, sort });

    const resp = await fetch(`/Product/GetProductsJson?${query}`);
    const data = await resp.json();
    renderProducts(data.products);
    renderPagination(data.currentPage, data.totalPages);
}

function renderProducts(products) {
    const container = document.getElementById('productsContainer');
    container.innerHTML = '';

    if (!products?.length) {
        container.innerHTML = `
            <div class="col-12">
                <div class="alert alert-warning text-center" role="alert">
                    <i class="fas fa-exclamation-circle me-2"></i> No products found.
                </div>
            </div>`;
        return;
    }

    products.forEach(p => {
        const price = parseFloat(p.price).toFixed(2);
        const rating = p.rating?.rate ?? 0;
        const count = p.rating?.count ?? 0;

        let starsHtml = '';
        for (let i = 1; i <= 5; i++) {
            if (rating >= i) {
                starsHtml += '<i class="fas fa-star text-warning"></i>';
            } else if (rating >= i - 0.5) {
                starsHtml += '<i class="fas fa-star-half-alt text-warning"></i>';
            } else {
                starsHtml += '<i class="far fa-star text-muted"></i>';
            }
        }

        container.innerHTML += `
            <div class="col-md-4 mb-4 d-flex">
                <div class="card h-100 w-100">
                    <img src="${p.image}" class="card-img-top" alt="${p.title}" style="height:200px; object-fit:contain;" />
                    <div class="card-body d-flex flex-column">
                        <h5 class="card-title product-title">${p.title}</h5>
                        <p class="card-text product-description">${p.description}</p>                            
                        <div class="mt-auto mb">
                            <div class="d-flex justify-content-between align-items-center">
                                <strong>Price:</strong>
                                <strong>$${price}</strong>
                            </div>
                        </div>
                        <div class="mb-2 mt-2">
                            ${starsHtml} <span class="text-muted">(${count})</span>
                        </div>
                    </div>
                </div>
            </div>`;
    });
}

function renderPagination(page, total) {
    currentPage = page;
    totalPages = total;
    const nav = document.getElementById('paginationContainer');
    nav.innerHTML = '';
    for (let i = 1; i <= total; i++) {
        nav.innerHTML += `
            <li class="page-item ${i === page ? 'active' : ''}">
                <a class="page-link" href="#" onclick="goToPage(${i})">${i}</a>
            </li>`;
    }
}

function goToPage(page) {
    const search = document.getElementById('searchInput').value;
    loadData(search, page);
}

function refreshProducts() {
    document.getElementById('searchInput').value = '';
    document.getElementById('categoryFilter').value = '';
    document.getElementById('sortOrder').value = '';

    const container = document.getElementById('productsContainer');
    container.innerHTML = `
        <div class="text-center w-100">
            <div class="spinner-border text-primary" role="status"></div>
            <p class="mt-2">Refreshing products...</p>
        </div>`;

    loadData('', 1);
}

// Event bindings
document.addEventListener("DOMContentLoaded", () => {
    document.getElementById('searchInput').addEventListener('input', e => loadData(e.target.value, 1));
    document.getElementById('categoryFilter').addEventListener('change', () => {
        const search = document.getElementById('searchInput').value;
        loadData(search, 1);
    });
    document.getElementById('sortOrder').addEventListener('change', () => {
        const search = document.getElementById('searchInput').value;
        loadData(search, 1);
    });

    $('body').on('click', '.create-btn', function () {
        $.get('/Product/GetCreateModal', html => {
            $('#modalPlaceholder').html(html);
            $('#productModal').modal('show');
        });
    });

    $('body').on('click', '.edit-btn', function () {
        $.get(`/Product/GetEditModal?id=${this.dataset.id}`, html => {
            $('#modalPlaceholder').html(html);
            $('#productModal').modal('show');
        });
    });

    $('body').on('ajax:success', 'form[data-ajax="true"]', function () {
        $('#productModal').modal('hide');
        loadData($('#searchInput').val(), currentPage);
    });

    $('body').on('ajax:error', 'form[data-ajax="true"]', function (event) {
        const response = event.detail[2]?.response;
        $('#productModal .modal-content').html(response);
        $('#productModal').modal('show');
    });
});

