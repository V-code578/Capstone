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
