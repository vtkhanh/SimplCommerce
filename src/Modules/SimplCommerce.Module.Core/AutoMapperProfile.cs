using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using Newtonsoft.Json;
using SimplCommerce.Module.Core.Models;
using SimplCommerce.Module.Core.Services.Dtos;

namespace SimplCommerce.Module.Core
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<User, CustomerDto>();
        }
    }
}