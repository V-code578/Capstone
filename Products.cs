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

        public string FileName { get; set; }

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
                product1.FileName = product.FileName;
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

namespace webapi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        IProductRepo productRepo;

        public ProductController(IProductRepo repo)
        {
             productRepo = repo;
        }

        [HttpGet("AllProducts")]
        public async Task<ActionResult> GetAllProducts()
        {
            List<Product> products = await productRepo.GetAllProducts();
            return Ok(products);
        }

        [HttpGet("ProductById/{productId}")]
        public async Task<ActionResult> GetProductById(int productId)
        {
            try
            {
                Product product = await productRepo.GetProductById(productId);
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
                Product product = await productRepo.GetProductByName(productName);
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
                List<Product> product = await productRepo.GetProductsByCategory(categoryId);
                return Ok(product);
            }
            catch (Exception ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpPost]
        public async Task<ActionResult> AddProduct([FromForm] IFormFile imageFile, [FromForm] Product product)
        {
            if (imageFile != null)
            {
                using (var memoryStream = new MemoryStream())
                {
                    await imageFile.CopyToAsync(memoryStream);
                    product.ProductImage = memoryStream.ToArray();
                }
            }

            await productRepo.AddProduct(product);
            return Created($"api/product/{product.ProductId}", product);
        }


        [HttpPut("{productId}")]
        public async Task<ActionResult> UpdateProduct(int productId, IFormFile imageFile, [FromForm] Product product)
        {
            if (imageFile != null)
            {
                using (var memoryStream = new MemoryStream())
                {
                    await imageFile.CopyToAsync(memoryStream);
                    product.ProductImage = memoryStream.ToArray();
                }
            }

            await productRepo.UpdateProduct(productId, product);
            return Ok(product);
        }


        [HttpDelete]
        public async Task<ActionResult> DeleteProduct(int productId)
        {
            await productRepo.DeleteProduct(productId);
            return Ok();
        }
    }
}

Then Please implement the Front end web page using React JS and Bootstrap for the admin side to adding , updating, deleting, fetching all products, Fetching Single product using ProductId, Fetching Products using CategoryName.
