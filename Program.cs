var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
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

app.UseRouting();

app.UseCors("MyPolicy");

app.UseAuthentication();
app.UseAuthorization();

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
});

app.Run();

const handleSubmit = async (event) => {
  event.preventDefault();

  const formData = new FormData();
  formData.append("productName", productData.productName);
  formData.append("description", productData.description);
  formData.append("price", productData.price);
  formData.append("categoryId", productData.categoryId);

  // Convert file to byte array
  const reader = new FileReader();
  reader.onload = async (e) => {
    const buffer = e.target.result;
    const bytes = new Uint8Array(buffer);
    formData.append("file", bytes);
    
    try {
      const response = await fetch("http://localhost:5094/api/Product", {
        method: "POST",
        body: formData,
      });

      if (response.ok) {
        alert("Product added successfully!");
      } else {
        throw new Error("Failed to add product. Please try again.");
      }
    } catch (error) {
      console.error("Error adding product:", error);
      alert("Failed to add product. Please try again.");
    }
  };
  reader.readAsArrayBuffer(productData.file);
};
