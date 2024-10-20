using AutoMapper;
using IMS_Dashboard.Models.Entities;
using IMS_Dashboard.ViewModels.ProductsVM;
using System.Net.Http.Headers;

namespace IMS_Dashboard.Configurations
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Product, CreateProductViewModel>().ReverseMap();
        }
    }
}
