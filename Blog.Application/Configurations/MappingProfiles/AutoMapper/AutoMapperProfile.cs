using AutoMapper;
using Blog.Application.Dtos.Category;
using Blog.Application.Dtos.Post;
using Blog.Domain.Entities;
using Blog.Domain.Helpers;

namespace Blog.Application.Configurations.MappingProfiles.AutoMapper
{
    /// <summary>
    /// Configures the mappings for AutoMapper, defining how domain entities map to DTOs and vice versa.
    /// </summary>
    /// <remarks>
    /// This class inherits from <see cref="Profile"/> and defines the mapping configurations between domain
    /// entities and their corresponding data transfer objects (DTOs). The mappings can be used by AutoMapper to
    /// automatically convert between the two types. Additionally, the mappings can be customized as needed, including
    /// using specific member mappings, value conversions, or reverse mappings.
    /// </remarks>
    public class AutoMapperProfile : Profile
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AutoMapperProfile"/> class and defines mapping configurations.
        /// </summary>
        public AutoMapperProfile()
        {
            CreateMap<Post, PostDto>()
                .ForMember(p => p.MinutesToRead, opt => opt.MapFrom(src => ReadingTimeEstimatorHelper.EstimateMinutesToRead(src.Content, 200)))
                .ForMember(p => p.Slug, opt => opt.MapFrom(src => StringHelper.ToSlug(src.Title)))
                .ReverseMap();

            CreateMap<Category, CategoryDto>()
                .ForMember(c => c.ImageUrl, opt => opt.MapFrom(src => src.CoverImageUrl)).ReverseMap();
        }
    }
}