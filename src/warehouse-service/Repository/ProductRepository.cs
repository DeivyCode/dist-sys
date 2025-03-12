using Microsoft.EntityFrameworkCore;
using warehouse_service.Repository.Entities;

namespace warehouse_service.Repository;

public interface IProuctRepository
{
    Task<List<Product>> GetAllProducts();
    Task<Product> GetProductById(int productId);
    Task<Product> AddProduct(Product product);
    Task<Product> UpdateProduct(Product product);
    Task<Product> DeleteProduct(int productId);
}

public class ProductRepository : IProuctRepository
{
    private readonly WarehouseDbContext _dbContext;

    public ProductRepository(WarehouseDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<List<Product>> GetAllProducts()
    {
        return await _dbContext.Products.AsNoTracking().ToListAsync();
    }

    public async Task<Product> GetProductById(int productId)
    {
        return await _dbContext.Products.AsNoTracking().FirstOrDefaultAsync(p => p.ProductId == productId) ??
               new Product();
    }

    public async Task<Product> AddProduct(Product product)
    {
        //add a detached entity to the context
        var entry = _dbContext.Entry(product);

        //add a new entity to the context
        entry.State = EntityState.Added;

        //save the changes
        if (await _dbContext.SaveChangesAsync() <= 0) throw new ApplicationException("Failed to add product");

        return product;
    }

    public async Task<Product> UpdateProduct(Product product)
    {
        var entry = await _dbContext.Set<Product>().FindAsync(product);

        if (entry == null) throw new ApplicationException("Product not found");

        //update the entity
        _dbContext.Entry(entry).State = EntityState.Detached;
        _dbContext.Set<Product>().Attach(product);
        _dbContext.Entry(product).State = EntityState.Modified;

        //save the changes
        if (await _dbContext.SaveChangesAsync() <= 0) throw new ApplicationException("Failed to update product");

        return product;
    }

    public Task<Product> DeleteProduct(int productId)
    {
        throw new NotImplementedException();
    }
}