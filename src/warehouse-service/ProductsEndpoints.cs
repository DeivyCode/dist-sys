using Microsoft.AspNetCore.Mvc;
using warehouse_service.Repository;
using warehouse_service.Repository.Entities;

namespace warehouse_service;

public static class ProductsEndpoints
{
    public static void MapProductsEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/products");


        group.MapGet("/", async ([FromServices] IProuctRepository productService) =>
        {
            var products = await productService.GetAllProducts();
            return Results.Ok(products);
        });


        group.MapPost("/", async ([FromServices] IProuctRepository productService, Product product) =>
        {
            var newProduct = await productService.AddProduct(product);
            return Results.Created($"/products/{newProduct.ProductId}", newProduct);
        });

        group.MapGet("/{productId}", async ([FromServices] IProuctRepository productService, int productId) =>
        {
            var product = await productService.GetProductById(productId);
            return product.ProductId > 0
                ? Results.Ok(product)
                : Results.NotFound("Product not found");
        });
    }
}