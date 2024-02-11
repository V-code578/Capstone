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
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace webapi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        IProductRepo _productRepo;

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
        public async Task<ActionResult> AddProduct([FromForm] IFormFile productImage, [FromForm] Product product)
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

fail: Microsoft.AspNetCore.Diagnostics.DeveloperExceptionPageMiddleware[1]
      An unhandled exception has occurred while executing the request.
      Swashbuckle.AspNetCore.SwaggerGen.SwaggerGeneratorException: Failed to generate Operation for action - webapi.Controllers.ProductController.AddProduct (webapi). See inner exception
       ---> System.ArgumentException: An item with the same key has already been added. Key: FileName
         at System.Collections.Generic.Dictionary`2.TryInsert(TKey key, TValue value, InsertionBehavior behavior)
         at System.Collections.Generic.Dictionary`2.Add(TKey key, TValue value)
         at Swashbuckle.AspNetCore.SwaggerGen.SwaggerGenerator.GenerateSchemaFromFormParameters(IEnumerable`1 formParameters, SchemaRepository schemaRepository)
         at Swashbuckle.AspNetCore.SwaggerGen.SwaggerGenerator.GenerateRequestBodyFromFormParameters(ApiDescription apiDescription, SchemaRepository schemaRepository, IEnumerable`1 formParameters)
         at Swashbuckle.AspNetCore.SwaggerGen.SwaggerGenerator.GenerateRequestBody(ApiDescription apiDescription, SchemaRepository schemaRepository)
         at Swashbuckle.AspNetCore.SwaggerGen.SwaggerGenerator.GenerateOperation(ApiDescription apiDescription, SchemaRepository schemaRepository)
         --- End of inner exception stack trace ---
         at Swashbuckle.AspNetCore.SwaggerGen.SwaggerGenerator.GenerateOperation(ApiDescription apiDescription, SchemaRepository schemaRepository)
         at Swashbuckle.AspNetCore.SwaggerGen.SwaggerGenerator.GenerateOperations(IEnumerable`1 apiDescriptions, SchemaRepository schemaRepository)
         at Swashbuckle.AspNetCore.SwaggerGen.SwaggerGenerator.GeneratePaths(IEnumerable`1 apiDescriptions, SchemaRepository schemaRepository)
         at Swashbuckle.AspNetCore.SwaggerGen.SwaggerGenerator.GetSwaggerDocumentWithoutFilters(String documentName, String host, String basePath)
         at Swashbuckle.AspNetCore.SwaggerGen.SwaggerGenerator.GetSwaggerAsync(String documentName, String host, String basePath)
         at Swashbuckle.AspNetCore.Swagger.SwaggerMiddleware.Invoke(HttpContext httpContext, ISwaggerProvider swaggerProvider)
         at Microsoft.AspNetCore.Diagnostics.DeveloperExceptionPageMiddlewareImpl.Invoke(HttpContext context)

Program.cs:
using OnlineShopping.Models;
using OnlineShopping.Repos;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddScoped<IUserRepo, EFUserRepo>();
builder.Services.AddScoped<IProductRepo, EFProductRepo>();
builder.Services.AddScoped<ICategoryRepo, EFCategoryRepo>();
builder.Services.AddScoped<ICartRepo, EFCartRepo>();
builder.Services.AddScoped<IOrderRepo, EFOrderRepo>();
builder.Services.AddScoped<IWishListRepo, EFWishListRepo>();
builder.Services.AddCors(options =>
{
    options.AddPolicy("MyPolicy", builder => builder.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod());
});

builder.Services.AddDbContext<OnlineShoppingDbContext>();
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors("MyPolicy");
app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();
