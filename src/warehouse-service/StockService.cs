using Microsoft.EntityFrameworkCore;
using warehouse_service.Repository;
using warehouse_service.Repository.Entities;

namespace warehouse_service;

public record ProductStock
{
    public int ProductId { get; init; }
    public int Quantity { get; init; }
}

public interface IStockService
{
    Task<Stock> GetStock(int productId);
    Task<Stock> FeedWarehouse(ProductStock productStock);
}

public class StockService : IStockService
{
    private readonly ILogger<StockService> _logger;
    private readonly WarehouseDbContext _dbContext;

    public StockService(ILogger<StockService> logger, WarehouseDbContext dbContext)
    {
        _logger = logger;
        _dbContext = dbContext;
    }

    public async Task<Stock> GetStock(int productId)
    {
        _logger.LogInformation("Getting stock for product {ProductId}", productId);
        return await _dbContext.Stocks
            .AsNoTracking()
            .Where(s => s.ProductId == productId)
            .Include(x => x.Product)
            .FirstOrDefaultAsync<Stock>()
            .ConfigureAwait(false) ?? new Stock();
    }

    public async Task<Stock> FeedWarehouse(ProductStock productStock)
    {
        _logger.LogInformation("Feeding warehouse with product {ProductId} and quantity {Quantity}",
            productStock.ProductId, productStock.Quantity);

        var prodouct = await _dbContext.Products
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.ProductId == productStock.ProductId)
            .ConfigureAwait(false);

        if (prodouct is null) throw new ApplicationException("Product not found");

        //feed the warehouse
        var stock = await _dbContext.Stocks
            .AsNoTracking()
            .FirstOrDefaultAsync(s => s.ProductId == productStock.ProductId)
            .ConfigureAwait(false);

        if (stock is null)
        {
            stock = new Stock
            {
                ProductId = productStock.ProductId,
                Quantity = productStock.Quantity
            };
            await _dbContext.Stocks.AddAsync(stock).ConfigureAwait(false);
        }
        else
        {
            _dbContext.Entry(stock).State = EntityState.Modified;
            stock.Quantity += productStock.Quantity;
        }

        await _dbContext.SaveChangesAsync().ConfigureAwait(false);

        return stock;
    }
}