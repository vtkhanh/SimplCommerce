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
                .ForMember(dest => dest.Stock, opt => opt.MapFrom(src => src.Stock))
                .ForMember(dest => dest.IsOutOfStock, opt => opt.MapFrom(src => src.Stock <= 0))
                .ForMember(dest => dest.CategoryIds, opt => opt.MapFrom(src => src.Categories.Select(x => x.CategoryId).ToList()))
                .ForMember(dest => dest.Attributes, opt => opt.Ignore())
                .ForMember(dest => dest.Options, opt => opt.Ignore())
                .ForMember(dest => dest.Variations, opt => opt.Ignore())
                .ForMember(dest => dest.ThumbnailImageUrl, opt => opt.Ignore())
                .ForMember(dest => dest.ProductImages, opt => opt.Ignore())
                .ForMember(dest => dest.ProductDocuments, opt => opt.Ignore())
                .ForMember(dest => dest.DeletedMediaIds, opt => opt.Ignore())
                .ForMember(dest => dest.RelatedProducts, opt => opt.Ignore())
                .ForMember(dest => dest.CrossSellProducts, opt => opt.Ignore())
                .ReverseMap()
                .ForMember(dest => dest.HasOptions, opt => opt.MapFrom(src => src.Variations.Any() ? true : false))
                .ForMember(dest => dest.Stock, opt => opt.MapFrom(src => src.Stock))
                .ForMember(dest => dest.SeoTitle, opt => opt.MapFrom(src => src.Slug))
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
            CreateMap<Product, ProductDto>()
                .ForMember(dest => dest.ThumbnailImageUrl, opt => opt.Ignore())
                ;
        }
    }
}
