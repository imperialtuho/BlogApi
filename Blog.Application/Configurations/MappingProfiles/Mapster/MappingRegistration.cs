using Blog.Application.Dtos.Category;
using Blog.Application.Dtos.Comment;
using Blog.Application.Dtos.Interaction;
using Blog.Application.Dtos.Post;
using Blog.Domain.Entities;
using Mapster;

namespace Blog.Application.Configurations.MappingProfiles.Mapster
{
    public class MappingRegistration : IRegister
    {
        public void Register(TypeAdapterConfig config)
        {
            config.Default.Settings.IgnoreNullValues = true;
            // Mapping from Entity to DTO.
            config.NewConfig<Post, PostDto>()
                .Map(dest => dest.PostId, src => src.Id)
                .Map(dest => dest.Title, src => src.Title)
                .Map(dest => dest.Content, src => src.Content)
                .Map(dest => dest.Comments, src => src.Comments != null ? src.Comments.Adapt<IList<CommentDto>>() : null)
                .Map(dest => dest.Interactions, src => src.Interactions != null ? src.Interactions.Adapt<IList<InteractionDto>>() : null);

            config.NewConfig<Comment, CommentDto>().Map(dest => dest.CommentId, src => src.Id);
            config.NewConfig<Interaction, InteractionDto>().Map(dest => dest.InteractionId, src => src.Id);
            config.NewConfig<Category, CategoryDto>().Map(dest => dest.Id, src => src.Id);

            // Mapping from DTO to Entity
            config.NewConfig<PostDto, Post>()
                .Map(dest => dest.Id, src => src.PostId)
                .Map(dest => dest.Title, src => src.Title)
                .Map(dest => dest.Content, src => src.Content)
                .Map(dest => dest.Comments, src => src.Comments != null ? src.Comments.Adapt<IList<Comment>>() : null)
                .Map(dest => dest.Interactions, src => src.Interactions != null ? src.Comments.Adapt<IList<Interaction>>() : null);

            config.NewConfig<CommentDto, Comment>().Map(dest => dest.Id, src => src.CommentId);
            config.NewConfig<InteractionDto, Interaction>().Map(dest => dest.Id, src => src.InteractionId);
            config.NewConfig<CategoryDto, Category>().Map(dest => dest.Id, src => src.Id);
        }
    }
}