import React, { useState, useEffect } from 'react';
import axios from 'axios';

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
        const formData = new FormData();
        formData.append("productName", product.productName);
        formData.append("description", product.description);
        formData.append("price", product.price);
        formData.append("categoryId", product.categoryId);
        formData.append("productImage", product.productImage);

        axios.put(`http://localhost:5183/api/Product/${productId}`, formData, {
            headers: {
                'Content-Type': 'multipart/form-data'
            }
        })
            .then(response => {
                alert('Product updated');
                fetchAllProducts();
                setProduct({ productId: 0, productName: '', description: '', price: 0, categoryId: 0, productImage: null });
            })
            .catch(error => {
                console.error('Error updating product:', error);
                alert('Failed to update product. Please try again.');
            });
    };

    const addProduct = () => {
        const formData = new FormData();
        formData.append("productName", product.productName);
        formData.append("description", product.description);
        formData.append("price", product.price);
        formData.append("categoryId", product.categoryId);
        formData.append("productImage", product.productImage);

        axios.post("http://localhost:5183/api/Product", formData, {
            headers: {
                'Content-Type': 'multipart/form-data'
            }
        })
            .then(response => {
                alert('Product added');
                fetchAllProducts();
                setProduct({ productId: 0, productName: "", description: "", price: 0, categoryId: 0, productImage: "" });
            })
            .catch(error => {
                console.error('Error adding product:', error);
                alert('Failed to add product. Please try again.');
            });
    };

    const handleImageChange = (event) => {
        setProduct(prev => ({ ...prev, productImage: event.target.files[0] }));
    };

    return (
        <div className="container mt-4">
            <h2 className="main-heading">Product Form</h2>
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
                    <input type="file" className="form-control" id="productImage" onChange={handleImageChange} />
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
