using Microsoft.EntityFrameworkCore;
using Sweets.Models;
using Npgsql;

public class SweetContext : DbContext
{
    public SweetContext(DbContextOptions<SweetContext> options) : base(options) { }

    public DbSet<Unit> Units { get; set; }
    public DbSet<RawMaterial> RawMaterials { get; set; }
    public DbSet<FinishedProduct> FinishedProducts { get; set; }
    public DbSet<Position> Positions { get; set; }
    public DbSet<Employee> Employees { get; set; }
    public DbSet<Ingredient> Ingredients { get; set; }
    public DbSet<Budget> Budgets { get; set; }
    public DbSet<RawMaterialPurchase> RawMaterialPurchases { get; set; }
    public DbSet<ProductSale> ProductSales { get; set; }
    public DbSet<ProductManufacturing> ProductManufacturings { get; set; }
    public DbSet<EmployeeSalary> EmployeeSalaries { get; set; }
    public DbSet<Credit> Credits { get; set; }
    public DbSet<Payment> Payments { get; set; }
 
    public async Task GeneratePaymentsAsync(int creditId)
    {
        await Database.ExecuteSqlRawAsync("CALL generate_payments({0})", creditId);
    }
    
    public async Task MakePaymentAsync(int creditId, DateTime paymentDate)
    {
        
        var creditIdParam = new NpgsqlParameter("p_creditid", NpgsqlTypes.NpgsqlDbType.Integer)
        {
            Value = creditId
        };

        var paymentDateParam = new NpgsqlParameter("p_paymentdate", NpgsqlTypes.NpgsqlDbType.Date)
        {
            Value = paymentDate.Date 
        };

        await Database.ExecuteSqlRawAsync(
            "CALL create_payment(@p_creditid, @p_paymentdate)",
            creditIdParam,
            paymentDateParam);
    }
}
