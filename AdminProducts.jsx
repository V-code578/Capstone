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

Implement the functionality to upload the product image in above controller like what i used in below imageController.cs

ImageController:
using ImageLibrary.Models;
using ImageLibrary.Repos;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace webapi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ImageController : ControllerBase
    {
         private readonly IImageRepository _imageRepository;

    public ImageController(IImageRepository imageRepository)
    {
        _imageRepository = imageRepository;
    }

    [HttpGet]
    public async Task<IActionResult> GetImages()
    {
        var images = await _imageRepository.GetAllImages();
        return Ok(images);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetImage(int id)
    {
        var image = await _imageRepository.GetImageById(id);
        if (image == null)
        {
            return NotFound();
        }
        return Ok(image);
    }

        [HttpPost]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> AddImage([FromForm] IFormFile image)
        {
            if (image == null)
            {
                return BadRequest();
            }

            // Convert IFormFile to Image model if necessary
            var imageData = new Image
            {
                FileName = image.FileName,
                Data = await GetByteArrayFromFormFile(image)
            };

            await _imageRepository.AddImage(imageData);
            return Created($"api/Image/{imageData.Id}", imageData);
        }

        private async Task<byte[]> GetByteArrayFromFormFile(IFormFile file)
        {
            using (var memoryStream = new MemoryStream())
            {
                await file.CopyToAsync(memoryStream);
                return memoryStream.ToArray();
            }
        }

        [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteImage(int id)
    {
        await _imageRepository.DeleteImage(id);
        return Ok();
    }
    }
}
