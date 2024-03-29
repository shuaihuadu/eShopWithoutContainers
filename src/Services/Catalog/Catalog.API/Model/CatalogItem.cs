﻿namespace eShopWithoutContainers.Services.Catalog.API.Model;
public class CatalogItem
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public decimal Price { get; set; }
    public string PictureFileName { get; set; }
    public string PictureUri { get; set; }
    public int CatalogTypeId { get; set; }
    public CatalogType CatalogType { get; set; }
    public int CatalogBrandId { get; set; }
    public CatalogBrand CatalogBrand { get; set; }
    public int AvailableStock { get; set; }
    public int RestockThreshold { get; set; }
    public int MaxStockThreshold { get; set; }
    public bool OnReorder { get; set; }
    public CatalogItem() { }

    public int RemoveStock(int quanitityDesired)
    {
        if (AvailableStock == 0)
        {
            throw new CatalogDomainException($"Empty stock, product item {Name} is sold out.");
        }
        if (quanitityDesired <= 0)
        {
            throw new CatalogDomainException($"Item uniys desired should be greater than zero.");
        }

        int removed = Math.Min(quanitityDesired, AvailableStock);

        AvailableStock -= removed;

        return removed;
    }

    public int AddStock(int quantity)
    {
        int original = AvailableStock;

        if ((AvailableStock + quantity) > MaxStockThreshold)
        {
            AvailableStock += MaxStockThreshold - AvailableStock;
        }
        else
        {
            AvailableStock += quantity;
        }
        OnReorder = false;
        return AvailableStock - original;
    }
}