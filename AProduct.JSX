import React, { useState, useEffect } from 'react';

//This function enables the Admin to See all the products and perform the CRUD operaions on it.
function AProduct() {
    const [products, setProducts] = useState([]);
    const [product, setProduct] = useState({ productName: "", description: "", price: 0, categoryId: 0, stockQuantity: 0 });

    useEffect(() => {
        fetch("https://localhost:7077/api/Product/AllProducts")
            .then(result => result.json())
            .then(result => {
                setProducts(result);
            });
    }, []);


    //Enables the admin to delete the particular Product using the Product's ID.
    const deleteProduct = (productId) => {
        fetch('https://localhost:7077/api/Product/' + productId, {
            method: 'DELETE',
        }).then(result => result.text()).then(result => {
            alert("Product deleted");
            setProducts(products.filter(p => p.productId !== productId));
        });
    }

    //This function fetches all the Products from the Database.
    const fetchAllProducts = () => {
        fetch("https://localhost:7077/api/Product/AllProducts")
            .then(result => result.json())
            .then(result => {
                setProducts(result);
            });
    };

    //This function enables the Admin to perform Update operation using the product ID.
    const updateProduct = (productId) => {
        fetch('https://localhost:7077/api/Product/' + productId, {
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
                setProduct({
                    productId: 0,
                    productName: '',
                    description: '',
                    price: 0,
                    categoryId: 0,
                    stockQuantity: 0,
                });
            });
    };

    //This function enables the admin to Add the New Product to the Database.
    const addProduct = () => {
        fetch("https://localhost:7077/api/Product", {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json'
            },
            body: JSON.stringify(product)
        }).then(result => result.json()).then(result => {
            alert("Product added");
            fetchAllProducts();
            setProduct({ productName: "", description: "", price: 0, categoryId: 0, stockQuantity: 0 });
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
                    <label htmlFor="stockQuantity" className="form-label">Stock Quantity:</label>
                    <input type="number" className="form-control" id="stockQuantity" value={product.stockQuantity}
                        onChange={(e) => setProduct(prev => ({ ...prev, stockQuantity: e.target.value }))} />
                </div>
                <button type="button" className="btn btn-primary" onClick={fetchAllProducts}>Show All Products</button>
                <button type="button" className="btn btn-success ms-2" onClick={addProduct}>Add Product</button>
                <button type="button" className="btn btn-warning ms-2" onClick={() => updateProduct(product.productId)}>Update Product</button>

            </form>
            <h3 className="mt-4">List of Products</h3>
            <table className="table">
                <thead>
                    <tr>
                        <th>Product Name</th><th>Description</th><th>Price</th><th>Category ID</th><th>Stock Quantity</th><th>Action</th>
                    </tr>
                </thead>
                <tbody>
                    {products.map(product => (
                        <tr key={product.productId}>
                            <td>{product.productName}</td><td>{product.description}</td><td>{product.price}</td><td>{product.categoryId}</td><td>{product.stockQuantity}</td>
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

export default AProduct;
