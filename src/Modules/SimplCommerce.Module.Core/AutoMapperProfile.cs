using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using Newtonsoft.Json;
using SimplCommerce.Module.Core.Models;
using SimplCommerce.Module.Core.Services.Dtos;
using SimplCommerce.Module.Core.ViewModels;

namespace SimplCommerce.Module.Core
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<User, CustomerDto>()
                .ForMember(dest => dest.Address, opt => opt.MapFrom(src => src.DefaultShippingAddress.AddressLine1));
            CreateMap<User, UserForm>()
                .ForMember(dest => dest.Password, opt => opt.Ignore())
                .ForMember(dest => dest.Address, opt => opt.MapFrom(src => src.DefaultShippingAddress.AddressLine1))
                .ForMember(dest => dest.RoleIds, opt => opt.MapFrom(src => src.Roles.Select(item => item.RoleId).ToList()))
                .ForMember(dest => dest.CustomerGroupIds, opt => opt.MapFrom(src => src.CustomerGroups.Select(item => item.CustomerGroupId).ToList()))
                ;
        }
    }
}
