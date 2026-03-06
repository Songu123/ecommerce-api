using API.DTOs.Request;
using API.DTOs.Response;
using API.Models;
using AutoMapper;

namespace API.Mappings
{
    /// <summary>
    /// AutoMapper profile for mapping entities to DTOs
    /// </summary>
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
    // Product mappings
       CreateMap<Product, ProductResponse>()
         .ForMember(dest => dest.CategoryName, 
      opt => opt.MapFrom(src => src.Category != null ? src.Category.Name : null));
   
    CreateMap<CreateProductRequest, Product>()
    .ForMember(dest => dest.ImageUrl, opt => opt.Ignore());
  
   CreateMap<UpdateProductRequest, Product>()
        .ForMember(dest => dest.ImageUrl, opt => opt.Ignore())
           .ForMember(dest => dest.Id, opt => opt.Ignore())
   .ForMember(dest => dest.CreatedAt, opt => opt.Ignore());

    // Category mappings
     CreateMap<Category, CategoryResponse>()
          .ForMember(dest => dest.ProductCount, 
        opt => opt.MapFrom(src => src.Products.Count));
 
   CreateMap<CreateCategoryRequest, Category>();

      // User mappings
     CreateMap<RegisterRequest, User>()
      .ForMember(dest => dest.PasswordHash, opt => opt.Ignore());

      // Order mappings
 CreateMap<Order, OrderResponse>()
       .ForMember(dest => dest.CustomerName, 
        opt => opt.MapFrom(src => src.User != null ? src.User.FullName : "N/A"))
 .ForMember(dest => dest.CustomerEmail, 
        opt => opt.MapFrom(src => src.User != null ? src.User.Email : "N/A"))
            .ForMember(dest => dest.Items, 
       opt => opt.MapFrom(src => src.OrderItems));

       CreateMap<Order, OrderSummaryResponse>()
    .ForMember(dest => dest.CustomerName, 
     opt => opt.MapFrom(src => src.User != null ? src.User.FullName : "N/A"))
      .ForMember(dest => dest.ItemsCount, 
     opt => opt.MapFrom(src => src.OrderItems.Count));

     CreateMap<OrderItem, OrderItemResponse>()
     .ForMember(dest => dest.ProductName, 
     opt => opt.MapFrom(src => src.Product != null ? src.Product.Name : "N/A"))
    .ForMember(dest => dest.ProductImage, 
    opt => opt.MapFrom(src => src.Product != null ? src.Product.ImageUrl : null));
        }
    }
}
