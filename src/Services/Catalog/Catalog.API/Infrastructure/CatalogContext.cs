﻿namespace eShopWithoutContainers.Services.Catalog.API.Infrastructure;
public class CatalogContext : DbContext
{
    public CatalogContext(DbContextOptions<CatalogContext> options) : base(options)
    {

    }

    public DbSet<CatalogItem> CatalogItems { get; set; }
    public DbSet<CatalogBrand> CatalogBrands { get; set; }
    public DbSet<CatalogType> CatalogTypes { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new CatalogBrandEntityTypeConfiguration());
        modelBuilder.ApplyConfiguration(new CatalogTypeEntityTypeConfiguration());
        modelBuilder.ApplyConfiguration(new CatalogItemEntityTypeConfiguration());
    }
}