using AutoMapper;
using ShopAPI.BL.DTOs;
using ShopAPI.BL.DTOs.User;
using ShopAPI.DAL.Data.Models;

namespace ShopAPI.BL.Mapper;

public class MapperProfile : Profile
{
    public MapperProfile()
    {
        CreateMap<Product, NewProductDTO>().ReverseMap();
        CreateMap<Product, ProductReadDTO>().ReverseMap();
        CreateMap<Product, OrderProductReadDTO>()
            .ReverseMap();
        CreateMap<ApplicationUser, UserReadDTO>().ReverseMap();
        CreateMap<NewUserDTO, ApplicationUser>().ReverseMap();

        CreateMap<OrderProducts, OrderProductDTO>().ReverseMap();
    }
}
