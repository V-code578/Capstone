Product.cs:
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineShopping.Models
{
    [Table("Product")]
    public class Product
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ProductId { get; set; }

        [Required]
        [StringLength(30)] // Adjust the length as needed
        public string ProductName { get; set; }

        [Required]
        [StringLength(50)]
        public string Description { get; set; }

        [Required]
        [Column(TypeName = "decimal(18, 2)")] // Adjust precision and scale as needed
        public decimal Price { get; set; }

        [ForeignKey("Category")]
        public int CategoryId { get; set; }

        public virtual Category? Category { get; set; }

        public string ProductFileName { get; set; }
        public byte[] ProductImage { get; set; } // New attribute for storing the image data

        public virtual ICollection<Cart> Carts { get; set; }
        public virtual ICollection<WishList> WishLists { get; set; }

        public Product()
        {
            Carts = new HashSet<Cart>();
            WishLists = new HashSet<WishList>();
        }
    }
}
IProductRepo.cs:
using OnlineShopping.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineShopping.Repos
{
    public interface IProductRepo
    {
        Task<Product> GetProductById(int productId);
        Task<Product> GetProductByName(string productName);
        Task<List<Product>> GetAllProducts();
        Task<List<Product>> GetProductsByCategory(int categoryId);
        Task AddProduct(Product product);
        Task UpdateProduct(int productId, Product product);
        Task DeleteProduct(int productId);
    }
}

EFProductRepo.cs:
using Microsoft.EntityFrameworkCore;
using OnlineShopping.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineShopping.Repos
{
    public class EFProductRepo : IProductRepo
    {
        OnlineShoppingDbContext ctx = new OnlineShoppingDbContext();
        public async Task AddProduct(Product product)
        {
            try
            {
                 ctx.Products.Add(product);
                 await ctx.SaveChangesAsync();
            }
            catch (Exception ex)
            {

                throw new Exception(ex.Message);
            }
        }

        public async Task DeleteProduct(int productId)
        {
            try
            {
                Product product = await GetProductById(productId);
                ctx.Products.Remove(product);
                await ctx.SaveChangesAsync();
            }
            catch (Exception ex)
            {

                throw new Exception(ex.Message);
            }
        }

        public async Task<List<Product>> GetAllProducts()
        {
            try
            {
                List<Product> products = await ctx.Products.ToListAsync();
                return products;
            }
            catch (Exception ex)
            {

                throw new Exception(ex.Message);
            }
        }

        public async Task<Product> GetProductById(int productId)
        {
            try
            {
                Product product = await (from p in ctx.Products where p.ProductId == productId select p).FirstAsync();
                return product;
            }
            catch (Exception ex)
            {

                throw new Exception(ex.Message);
            }
        }

        public async Task<Product> GetProductByName(string productName)
        {
            try
            {
                Product product = await (from p in ctx.Products where p.ProductName == productName select p).FirstAsync();
                return product;
            }
            catch (Exception ex)
            {

                throw new Exception(ex.Message);
            }
        }

        public async Task<List<Product>> GetProductsByCategory(int categoryId)
        {
            try
            {
                List<Product> products = await (from p in ctx.Products where p.CategoryId== categoryId select p).ToListAsync();
                return products;
            }
            catch (Exception ex)
            {

                throw new Exception(ex.Message);
            }
        }

        public async Task UpdateProduct(int productId, Product product)
        {
            try
            {
                Product product1 = await GetProductById(productId);
                product1.ProductName = product.ProductName;
                product1.Description = product.Description;
                product1.Price = product.Price;
                product1.CategoryId = product.CategoryId;
                product1.ProductFileName = product.ProductFileName;
                product1.ProductImage = product.ProductImage;
                await ctx.SaveChangesAsync();
            }
            catch (Exception ex)
            {

                throw new Exception(ex.Message);
            }
        }
    }
}

ProductController.cs:
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OnlineShopping.Models;
using OnlineShopping.Repos;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace webapi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly IProductRepo _productRepo;

        public ProductController(IProductRepo repo)
        {
            _productRepo = repo;
        }

        [HttpGet("AllProducts")]
        public async Task<ActionResult> GetAllProducts()
        {
            try
            {
                List<Product> products = await _productRepo.GetAllProducts();
                return Ok(products);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet("ProductById/{productId}")]
        public async Task<ActionResult> GetProductById(int productId)
        {
            try
            {
                Product product = await _productRepo.GetProductById(productId);
                return Ok(product);
            }
            catch (Exception ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpGet("ProductByName/{productName}")]
        public async Task<ActionResult> GetProductByName(string productName)
        {
            try
            {
                Product product = await _productRepo.GetProductByName(productName);
                return Ok(product);
            }
            catch (Exception ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpGet("ProductByCategory/{categoryId}")]
        public async Task<ActionResult> GetProductsByCategory(int categoryId)
        {
            try
            {
                List<Product> products = await _productRepo.GetProductsByCategory(categoryId);
                return Ok(products);
            }
            catch (Exception ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpPost]
        [Consumes("multipart/form-data")]
        public async Task<ActionResult> AddProduct([FromForm] IFormFile productImage, [FromForm] Product product)
        {
            if (productImage == null || product == null)
            {
                return BadRequest();
            }

            try
            {
                using (var memoryStream = new MemoryStream())
                {
                    await productImage.CopyToAsync(memoryStream);
                    product.ProductImage = memoryStream.ToArray();
                    product.ProductFileName = productImage.FileName;
                }

                await _productRepo.AddProduct(product);
                return Created($"api/product/{product.ProductId}", product);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPut("{productId}")]
        public async Task<ActionResult> UpdateProduct(int productId, [FromForm] IFormFile productImage, [FromForm] Product product)
        {
            try
            {
                if (productImage != null)
                {
                    using (var memoryStream = new MemoryStream())
                    {
                        await productImage.CopyToAsync(memoryStream);
                        product.ProductImage = memoryStream.ToArray();
                    }
                }

                await _productRepo.UpdateProduct(productId, product);
                return Ok(product);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpDelete("{productId}")]
        public async Task<ActionResult> DeleteProduct(int productId)
        {
            try
            {
                await _productRepo.DeleteProduct(productId);
                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }
}

AdminProducts.jsx:
import React, { useState, useEffect } from 'react';
import axios from 'axios';

function AdminProducts() {
    const [products, setProducts] = useState([]);
    const [product, setProduct] = useState({ productName: "", description: "", price: 0, categoryId: 0, fileName: "", productImage: null });

    useEffect(() => {
        fetchAllProducts();
    }, []);

    const fetchAllProducts = () => {
        axios.get("http://localhost:5183/api/Product/AllProducts")
            .then(response => {
                setProducts(response.data);
            })
            .catch(error => {
                console.error('Error fetching products:', error);
            });
    };

    const deleteProduct = (productId) => {
        axios.delete(`http://localhost:5183/api/Product/${productId}`)
            .then(response => {
                alert("Product deleted");
                fetchAllProducts();
            })
            .catch(error => {
                console.error('Error deleting product:', error);
                alert("Failed to delete product. Please try again.");
            });
    };

    const addProduct = async () => {
        const formData = new FormData();
        formData.append("productName", product.productName);
        formData.append("description", product.description);
        formData.append("price", product.price);
        formData.append("categoryId", product.categoryId);
        formData.append("fileName", product.fileName);
        formData.append("productImage", product.productImage);

        try {
            await axios.post("http://localhost:5183/api/Product", formData, {
                headers: {
                    'Content-Type': 'multipart/form-data'
                }
            });
            alert('Product added');
            fetchAllProducts();
            setProduct({ productName: "", description: "", price: 0, categoryId: 0, fileName: "", productImage: null });
        } catch (error) {
            console.error('Error adding product:', error);
            alert('Failed to add product. Please try again.');
        }
    };

    const handleImageChange = (event) => {
        setProduct(prev => ({ ...prev, productImage: event.target.files[0], fileName: event.target.files[0].name }));
    };

    return (
        <div className="container mt-4">
            <h2 className="main-heading">Product Form</h2>
            <div className="underline"></div>
            <form>
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
            </form>
            <h3 className="mt-4">List of Products</h3>
            <table className="table">
                <thead>
                    <tr>
                        <th>Product Name</th>
                        <th>Description</th>
                        <th>Price</th>
                        <th>Category ID</th>
                        <th>File Name</th>
                        <th>Action</th>
                    </tr>
                </thead>
                <tbody>
                    {products.map(product => (
                        <tr key={product.productId}>
                            <td>{product.productName}</td>
                            <td>{product.description}</td>
                            <td>{product.price}</td>
                            <td>{product.categoryId}</td>
                            <td>{product.fileName}</td>
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
When i upload the image, i get this error:
Failed to load resource: the server responded with a status of 400 (Bad Request)
AdminProducts.jsx:53  Error adding product: AxiosError
