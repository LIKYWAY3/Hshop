async function loadProducts() {
    try {
        const response = await fetch('/api/products');
        const products = await response.json();

        const productList = document.getElementById('product-list');
        productList.innerHTML = '';

        products.forEach(product => {
            const image = product.imageUrl || product.thumbnailUrl || 'images/product-1.png';
            const price = product.price.toLocaleString('vi-VN') + 'đ';

            productList.innerHTML += `
                <div class="col-12 col-md-4 col-lg-3 mb-5 mb-md-0">
                    <a class="product-item" href="/Product/Detail/${product.productId}">
                        <img src="${image}" class="img-fluid product-thumbnail" alt="${product.productName}">
                        <h3 class="product-title">${product.productName}</h3>
                        <strong class="product-price">${price}</strong>
                        <span class="icon-cross">
                            <img src="images/cross.svg" class="img-fluid" alt="">
                        </span>
                    </a>
                </div>
            `;
        });
    } catch (error) {
        console.error('Lỗi load products:', error);
    }
}

loadProducts();