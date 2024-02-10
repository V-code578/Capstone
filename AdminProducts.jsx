import React, { useState, useEffect } from 'react';

function AdminProducts() {
    const [products, setProducts] = useState([]);
    const [productName, setProductName] = useState('');
    const [description, setDescription] = useState('');
    const [price, setPrice] = useState(0);
    const [categoryId, setCategoryId] = useState('');
    const [imageFile, setImageFile] = useState(null);
    const [categoryIdForFilter, setCategoryIdForFilter] = useState('');
    const [productIdForDetail, setProductIdForDetail] = useState('');
    const [filteredProducts, setFilteredProducts] = useState([]);
    const [productDetail, setProductDetail] = useState(null);

    useEffect(() => {
        fetchAllProducts();
    }, []);

    const fetchAllProducts = async () => {
        try {
            const response = await fetch('http://localhost:5183/api/Product/AllProducts');
            const data = await response.json();
            setProducts(data);
        } catch (error) {
            console.error('Error fetching products:', error);
        }
    };

    const handleAddProduct = async (e) => {
        e.preventDefault();
        const formData = new FormData();
        formData.append('productName', productName);
        formData.append('description', description);
        formData.append('price', price);
        formData.append('categoryId', categoryId);
        formData.append('imageFile', imageFile);

        try {
            await fetch('http://localhost:5183/api/Product', {
                method: 'POST',
                body: formData
            });
            // Reset form fields after successful submission
            setProductName('');
            setDescription('');
            setPrice(0);
            setCategoryId('');
            setImageFile(null);
            fetchAllProducts(); // Refresh product list
        } catch (error) {
            console.error('Error adding product:', error);
        }
    };

    const handleUpdateProduct = async (productId) => {
        // Fetch the product detail for the provided productId
        try {
            const response = await fetch(`http://localhost:5183/api/Product/ProductById/${productId}`);
            const data = await response.json();
            setProductDetail(data);
        } catch (error) {
            console.error('Error fetching product detail:', error);
        }
    };

    const handleDeleteProduct = async (productId) => {
        try {
            await fetch(`http://localhost:5183/api/Product/${productId}`, {
                method: 'DELETE'
            });
            fetchAllProducts(); // Refresh product list
        } catch (error) {
            console.error('Error deleting product:', error);
        }
    };

    const handleFilterByCategory = async () => {
        try {
            const response = await fetch(`http://localhost:5183/api/Product/ProductByCategory/${categoryIdForFilter}`);
            const data = await response.json();
            setFilteredProducts(data);
        } catch (error) {
            console.error('Error filtering products by category:', error);
        }
    };

    const handleGetProductDetail = async () => {
        try {
            const response = await fetch(`http://localhost:5183/api/Product/ProductById/${productIdForDetail}`);
            const data = await response.json();
            setProductDetail(data);
        } catch (error) {
            console.error('Error fetching product detail:', error);
        }
    };

    return (
        <div className="container">
            <h2>Admin Products Panel</h2>
            {/* Add Product Form */}
            <form onSubmit={handleAddProduct}>
                {/* Form fields for adding product */}
                <button type="submit">Add Product</button>
            </form>

            {/* Product List */}
            <h3>Product List</h3>
            <ul>
                {products.map(product => (
                    <li key={product.productId}>
                        {product.productName} - {product.description} - ${product.price}
                        <button onClick={() => handleUpdateProduct(product.productId)}>Edit</button>
                        <button onClick={() => handleDeleteProduct(product.productId)}>Delete</button>
                    </li>
                ))}
            </ul>

            {/* Filter Products by Category */}
            <div>
                <h3>Filter Products by Category</h3>
                <input
                    type="text"
                    placeholder="Enter Category ID"
                    value={categoryIdForFilter}
                    onChange={(e) => setCategoryIdForFilter(e.target.value)}
                />
                <button onClick={handleFilterByCategory}>Filter</button>
            </div>

            {/* Display Filtered Products */}
            <ul>
                {filteredProducts.map(product => (
                    <li key={product.productId}>{product.productName}</li>
                ))}
            </ul>

            {/* Get Product Detail by ID */}
            <div>
                <h3>Get Product Detail by ID</h3>
                <input
                    type="text"
                    placeholder="Enter Product ID"
                    value={productIdForDetail}
                    onChange={(e) => setProductIdForDetail(e.target.value)}
                />
                <button onClick={handleGetProductDetail}>Get Detail</button>
                {productDetail && (
                    <div>
                        <p>Name: {productDetail.productName}</p>
                        <p>Description: {productDetail.description}</p>
                        <p>Price: ${productDetail.price}</p>
                    </div>
                )}
            </div>
        </div>
    );
}

export default AdminProducts;
      
