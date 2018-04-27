using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using Newtonsoft.Json;
using SimplCommerce.Module.Catalog.Models;
using SimplCommerce.Module.Catalog.Services.Dtos;
using SimplCommerce.Module.Catalog.ViewModels;

namespace SimplCommerce.Module.Catalog
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<Product, ProductListItem>()
                ;
            CreateMap<Product, ProductVm>()
                .ForMember(dest => dest.Slug, opt => opt.MapFrom(src => src.SeoTitle))
                .ForMember(dest => dest.Stock, opt => opt.MapFrom(src => src.StockQuantity))
                .ForMember(dest => dest.IsOutOfStock, opt => opt.MapFrom(src => src.StockQuantity == 0))
                .ForMember(dest => dest.CategoryIds, opt => opt.MapFrom(src => src.Categories.Select(x => x.CategoryId).ToList()))
                .ReverseMap()
                .ForMember(dest => dest.HasOptions, opt => opt.MapFrom(src => src.Variations.Any() ? true : false))
                .ForMember(dest => dest.StockQuantity, opt => opt.MapFrom(src => src.IsOutOfStock ? 0 : src.Stock))
                ;
            CreateMap<Product, ProductVariationVm>()
                .AfterMap((src, dest) => dest.OptionCombinations = dest.OptionCombinations.OrderBy(i => i.SortIndex).ToList())
                ;
            CreateMap<Product, ProductLinkVm>();
            CreateMap<ProductOptionCombination, ProductOptionCombinationVm>()
                .ForMember(dest => dest.OptionName, opt => opt.MapFrom(src => src.Option.Name))
                ;
            CreateMap<ProductAttributeValue, ProductAttributeVm>()
                .ForMember(dest => dest.AttributeValueId, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.AttributeId))
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Attribute.Name))
                .ForMember(dest => dest.GroupName, opt => opt.MapFrom(src => src.Attribute.Group.Name))
                ;
            CreateMap<ProductOptionValue, ProductOptionVm>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.OptionId))
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Option.Name))
                .ForMember(dest => dest.Values, opt => opt.MapFrom(src => JsonConvert.DeserializeObject<IList<ProductOptionValueVm>>(src.Value)))
                ;
            CreateMap<ProductMedia, ProductMediaVm>()
                .ForMember(dest => dest.Caption, opt => opt.MapFrom(src => src.Media.Caption))
                ;
            CreateMap<Product, ProductDto>();
        }
    }
}