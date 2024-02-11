Image.cs:
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageLibrary.Models
{
    public class Image
    {
        public int Id { get; set; }
        public string FileName { get; set; }
        public byte[] Data { get; set; }
    }   
}
ImageRepository.cs:
using ImageLibrary.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageLibrary.Repos
{
    public class ImageRepository : IImageRepository
    {
        ImageDBContext ctx = new ImageDBContext();

        public async Task<IEnumerable<Image>> GetAllImages()
        {
            return await ctx.Images.ToListAsync();
        }

        public async Task<Image> GetImageById(int id)
        {
            return await ctx.Images.FindAsync(id);
        }

        public async Task AddImage(Image image)
        {
            ctx.Images.Add(image);
            await ctx.SaveChangesAsync();
        }

        public async Task DeleteImage(int id)
        {
            var image = await ctx.Images.FindAsync(id);
            if (image != null)
            {
                ctx.Images.Remove(image);
                await ctx.SaveChangesAsync();
            }
        }
    }
}
ImageController.cs:
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
import React, { useState } from "react";
import axios from "axios";

const ImageUploader = () => {
    const [selectedFile, setSelectedFile] = useState(null);

    const handleFileChange = (event) => {
        setSelectedFile(event.target.files[0]);
    };

    const handleUpload = async () => {
        const formData = new FormData();
        formData.append("image", selectedFile);

        try {
            await axios.post("http://localhost:5094/api/Image", formData, {
                headers: {
                    "Content-Type": "multipart/form-data",
                },
            });
            alert("Image uploaded successfully!");
        } catch (error) {
            console.error("Error uploading image:", error);
            alert("Failed to upload image. Please try again.");
        }
    };

    return (
        <div>
            <h1>Image Uploader</h1>
            <input type="file" onChange={handleFileChange} />
            <button onClick={handleUpload}>Upload</button>
        </div>
    );
};

export default ImageUploader;
Like this apply the update the code in AdminProduct.jsx and ProductController to perform How to upload the image to the database.


AdminProduct.jsx:
import React, { useState } from "react";
import axios from "axios";

const AdminProduct = () => {
    const [productData, setProductData] = useState({
        productName: "",
        description: "",
        price: 0,
        categoryId: 0,
        file: null,
    });

    const handleInputChange = (event) => {
        const { name, value } = event.target;
        setProductData({
            ...productData,
            [name]: value,
        });
    };

    const handleFileChange = (event) => {
        setProductData({
            ...productData,
            file: event.target.files[0],
        });
    };

    const handleSubmit = async (event) => {
        event.preventDefault();

        const formData = new FormData();
        formData.append("productName", productData.productName);
        formData.append("description", productData.description);
        formData.append("price", productData.price);
        formData.append("categoryId", productData.categoryId);
        formData.append("file", productData.file);

        try {
            await axios.post("http://localhost:5094/api/Product", formData, {
                headers: {
                    "Content-Type": "multipart/form-data",
                },
            });
            alert("Product added successfully!");
        } catch (error) {
            console.error("Error adding product:", error);
            alert("Failed to add product. Please try again.");
        }
    };

    return (
        <div>
            <h1>Add Product</h1>
            <form onSubmit={handleSubmit}>
                <div>
                    <label>Product Name:</label>
                    <input type="text" name="productName" onChange={handleInputChange} />
                </div>
                <div>
                    <label>Description:</label>
                    <input type="text" name="description" onChange={handleInputChange} />
                </div>
                <div>
                    <label>Price:</label>
                    <input type="number" name="price" onChange={handleInputChange} />
                </div>
                <div>
                    <label>Category ID:</label>
                    <input type="number" name="categoryId" onChange={handleInputChange} />
                </div>
                <div>
                    <label>Image:</label>
                    <input type="file" onChange={handleFileChange} />
                </div>
                <button type="submit">Submit</button>
            </form>
        </div>
    );
};

export default AdminProduct;

ProductController.cs:
[HttpPost]
[Consumes("multipart/form-data")]
public async Task<ActionResult> AddProduct([FromForm] IFormFile file, [FromForm] Product product)
{
    if (file == null || product == null)
    {
        return BadRequest();
    }

    try
    {
        using (var memoryStream = new MemoryStream())
        {
            await file.CopyToAsync(memoryStream);
            product.ProductImage = memoryStream.ToArray();
        }

        await _productRepo.AddProduct(product);
        return Created($"api/product/{product.ProductId}", product);
    }
    catch (Exception ex)
    {
        return StatusCode(500, ex.Message);
    }
}

