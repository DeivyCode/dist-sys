using Microsoft.AspNetCore.Mvc;

namespace warehouse_service;

public static class StockEndpoints
{
    public static void MapStockEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/stock");

        group.MapGet("/{productId}", async ([FromServices] IStockService stockService, int productId) =>
        {
            var stock = await stockService.GetStock(productId);
            return stock.ProductId > 0
                ? Results.Ok(stock)
                : Results.NotFound("Product not found");
        });

        group.MapPost("/", async ([FromServices] IStockService stockService, ProductStock productStock) =>
        {
            var stock = await stockService.FeedWarehouse(productStock);
            return Results.Created($"/stock/{stock.ProductId}", stock);
        });
    }
}