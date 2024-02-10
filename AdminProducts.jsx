import React, { useState, useEffect } from 'react';

function AdminProducts() {
    const [products, setProducts] = useState([]);
    const [product, setProduct] = useState({ productId: 0, productName: "", description: "", price: 0, categoryId: 0, productImage: null });

    useEffect(() => {
        fetchAllProducts();
    }, []);

    const fetchAllProducts = () => {
        fetch("http://localhost:5183/api/Product/AllProducts")
            .then(result => result.json())
            .then(result => {
                setProducts(result);
            });
    };

    const deleteProduct = (productId) => {
        fetch('http://localhost:5183/api/Product/' + productId, {
            method: 'DELETE',
        }).then(result => result.text()).then(result => {
            alert("Product deleted");
            setProducts(products.filter(p => p.productId !== productId));
        });
    };

    const updateProduct = (productId) => {
        fetch('http://localhost:5183/api/Product/' + productId, {
            method: 'PUT',
            headers: {
                'Content-Type': 'application/json',
            },
            body: JSON.stringify(product),
        })
            .then((result) => result.json())
            .then((updatedProduct) => {
                alert('Product updated');
                setProducts((prevProducts) =>
                    prevProducts.map((p) =>
                        p.productId === productId ? updatedProduct : p
                    )
                );
                setProduct({ productId: 0, productName: '', description: '', price: 0, categoryId: 0, productImage: null });
            });
    };

    const addProduct = () => {
        fetch("http://localhost:5183/api/Product", {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json'
            },
            body: JSON.stringify(product)
        }).then(result => result.json()).then(result => {
            alert("Product added");
            fetchAllProducts();
            setProduct({ productId: 0, productName: "", description: "", price: 0, categoryId: 0, productImage: null });
        });
    };

    return (
        <div className="container mt-4">
            <h2 className="main-heading">Product Form  </h2>
            <div className="underline"></div>
            <form>
                <div className="mb-3">
                    <label htmlFor="productId" className="form-label">Product Id:</label>
                    <input type="text" className="form-control" id="productId" value={product.productId}
                        onChange={(e) => setProduct(prev => ({ ...prev, productId: e.target.value }))} />
                </div>
                <div className="mb-3">
                    <label htmlFor="productName" className="form-label">Product Name:</label>
                    <input type="text" className="form-control" id="productName" value={product.productName}
                        onChange={(e) => setProduct(prev => ({ ...prev, productName: e.target.value }))} />
                </div>
                <div className="mb-3">
                    <label htmlFor="description" className="form-label">Description:</label>
                    <input type="text" className="form-control" id="description" value={product.description}
                        onChange={(e) => setProduct(prev => ({ ...prev, description: e.target.value }))} />
                </div>
                <div className="mb-3">
                    <label htmlFor="price" className="form-label">Price:</label>
                    <input type="number" className="form-control" id="price" value={product.price}
                        onChange={(e) => setProduct(prev => ({ ...prev, price: e.target.value }))} />
                </div>
                <div className="mb-3">
                    <label htmlFor="categoryId" className="form-label">Category ID:</label>
                    <input type="number" className="form-control" id="categoryId" value={product.categoryId}
                        onChange={(e) => setProduct(prev => ({ ...prev, categoryId: e.target.value }))} />
                </div>
                <div className="mb-3">
                    <label htmlFor="productImage" className="form-label">Product Image:</label>
                    <input type="file" className="form-control" id="productImage"
                        onChange={(e) => setProduct(prev => ({ ...prev, productImage: e.target.files[0] }))} />
                </div>
                <button type="button" className="btn btn-primary" onClick={fetchAllProducts}>Show All Products</button>
                <button type="button" className="btn btn-success ms-2" onClick={addProduct}>Add Product</button>
                <button type="button" className="btn btn-warning ms-2" onClick={() => updateProduct(product.productId)}>Update Product</button>

            </form>
            <h3 className="mt-4">List of Products</h3>
            <table className="table">
                <thead>
                    <tr>
                        <th>Product Name</th><th>Description</th><th>Price</th><th>Category ID</th><th>Action</th>
                    </tr>
                </thead>
                <tbody>
                    {products.map(product => (
                        <tr key={product.productId}>
                            <td>{product.productName}</td><td>{product.description}</td><td>{product.price}</td><td>{product.categoryId}</td>
                            <td>
                                <button type="button" className="btn btn-danger" onClick={() => deleteProduct(product.productId)}>Delete</button>
                            </td>
                        </tr>
                    ))}
                </tbody>
            </table>
        </div>
    );
}

export default AdminProducts;
                                                                                                                                          
