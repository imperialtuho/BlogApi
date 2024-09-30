using Blog.Application.Dtos;
using Blog.Domain.Entities;
using Mapster;

namespace Blog.Application.Configurations.MappingProfiles.Mapster
{
    public class MappingRegistration : IRegister
    {
        public void Register(TypeAdapterConfig config)
        {
            config.NewConfig<Post, PostDto>()
                .Map(dest => dest.PostId, src => src.Id)
                .Map(dest => dest.Title, src => src.Title)
                .Map(dest => dest.Content, src => src.Content)
                .Map(dest => dest.CreatedDate, src => src.CreatedDate)
                .Map(dest => dest.ModifiedDate, src => src.ModifiedDate)
                .Map(dest => dest.User, src => src.User.Adapt<UserDto>())
                .Map(dest => dest.Comments, src => src.Comments.Adapt<IList<CommentDto>>())
                .Map(dest => dest.Interactions, src => src.Interactions.Adapt<IList<InteractionDto>>());

            config.NewConfig<User, UserDto>().Map(dest => dest.UserId, src => src.Id);
            config.NewConfig<Comment, CommentDto>().Map(dest => dest.CommentId, src => src.Id);
            config.NewConfig<Interaction, InteractionDto>().Map(dest => dest.InteractionId, src => src.Id);

            // Reverse mappings
            config.NewConfig<PostDto, Post>()
                .Map(dest => dest.Id, src => src.PostId)
                .Map(dest => dest.Title, src => src.Title)
                .Map(dest => dest.Content, src => src.Content)
                .Map(dest => dest.CreatedDate, src => src.CreatedDate)
                .Map(dest => dest.ModifiedDate, src => src.ModifiedDate)
                .Map(dest => dest.User, src => src.User.Adapt<User>())
                .Map(dest => dest.Comments, src => src.Comments.Adapt<ICollection<Comment>>())
                .Map(dest => dest.Interactions, src => src.Interactions.Adapt<ICollection<Interaction>>());

            config.NewConfig<UserDto, User>().Map(dest => dest.Id, src => src.UserId);
            config.NewConfig<CommentDto, Comment>().Map(dest => dest.Id, src => src.CommentId);
            config.NewConfig<InteractionDto, Interaction>().Map(dest => dest.Id, src => src.InteractionId);
        }
    }
}