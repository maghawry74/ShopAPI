using AutoMapper;
using Microsoft.AspNetCore.Http;
using ShopAPI.BL.CustomClasses;
using ShopAPI.BL.DTOs;
using ShopAPI.DAL.Data.Models;

namespace ShopAPI.BL.Managers;

public class ProductManager : IProductManager
{
    public IProductRepository ProductRepository { get; }
    public IMapper Mapper { get; }

    public ProductManager(IProductRepository productRepository, IMapper mapper)
    {
        ProductRepository = productRepository;
        Mapper = mapper;
    }


    public async Task<ProductReadDTO> AddAsync(NewProductDTO newProduct)
    {
        var fileName = await AddNewFile(newProduct.ImageFile);
        var product = Mapper.Map<Product>(newProduct);
        product.Image = fileName;
        await ProductRepository.Add(product);
        await ProductRepository.SaveChanges();
        return Mapper.Map<ProductReadDTO>(product);
    }

    public async Task<ProductReadDTO?> GetByConditionAsync(int id)
    {
        var product = await ProductRepository.GetByCondition(P => P.Id == id);
        return product == null ? null : Mapper.Map<ProductReadDTO>(product);
    }

    public async Task<IEnumerable<ProductReadDTO>> GetAllAsync(int skip, int take)
    {
        var products = await ProductRepository.GetPage(skip, take);
        return Mapper.Map<IEnumerable<ProductReadDTO>>(products);
    }

    public async Task DeleteAsync(int id)
    {
        var product = await ProductRepository.GetByCondition(P => P.Id == id);
        if (product == null)
        {
            var ex = new StatusCodeEx(404, "Prodcut Not Founded");
            throw ex;
        }
        ProductRepository.Delete(product);
        var path = Path.Combine(Directory.GetCurrentDirectory(), "Public", product.Image);
        File.Delete(path);
        await ProductRepository.SaveChanges();
    }

    public async Task UpdateAsync(int id, ProductUpdateDTO EditedProduct)
    {
        var product = await ProductRepository.GetByCondition(P => P.Id == id, true);
        if (product == null) throw new StatusCodeEx(404, "Product Not Founded");
        product.Price = EditedProduct.Price;
        product.Description = EditedProduct.Description;
        product.Amount = EditedProduct.Amount;
        product.Category = EditedProduct.Category;
        product.ProductName = EditedProduct.ProductName;
        if (EditedProduct.ImageFile == null)
        {
            await ProductRepository.SaveChanges();
            return;
        }
        var oldImagePath = Path.Combine(Directory.GetCurrentDirectory(), "Public", product.Image);
        File.Delete(oldImagePath);
        var fileName = await AddNewFile(EditedProduct.ImageFile);
        product.Image = fileName;
        await ProductRepository.SaveChanges();
    }

    async Task<string> AddNewFile(IFormFile file)
    {
        var FileExtension = file.FileName.Split('.').Last();
        var fileName = $"{Guid.NewGuid()}.{FileExtension}";
        var path = Path.Combine(Directory.GetCurrentDirectory(), "Public");
        if (!Directory.Exists(path))
        {
            Directory.CreateDirectory(path);
        }
        string filePath = Path.Combine(path, fileName);
        using (var stream = new FileStream(path: filePath, FileMode.Create))
        {
            await file.CopyToAsync(stream);
        }
        return fileName;
    }

    public async Task<IEnumerable<Product>> GetRange(int[] ids)
    {
        return await ProductRepository.GetRange(ids);
    }
}
