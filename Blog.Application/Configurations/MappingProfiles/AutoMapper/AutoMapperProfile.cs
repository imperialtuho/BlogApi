using AutoMapper;
using Blog.Application.Dtos;
using Blog.Domain.Entities;

namespace Blog.Application.Configurations.MappingProfiles.AutoMapper
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<PostDto, Post>().ReverseMap();
        }
    }
}