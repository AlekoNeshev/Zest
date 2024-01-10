using AutoMapper;
using Microsoft.AspNetCore.Routing.Constraints;
using Zest.DBModels.Models;
using Zest.ViewModels.ViewModels;

namespace Zest
{
    public class MappingProfile : Profile
    {
        public MappingProfile() 
        {
            CreateMap<Account, AccountViewModel>();
            CreateMap<Post, PostViewModel>()
                .ForMember(dest => dest.Publisher, op => op.MapFrom(src => src.Account.Username))
                .ForMember(dest => dest.PostedOn, op => op.MapFrom(src => src.CreatedOn))
                .ForMember(dest => dest.Likes, op=> op.MapFrom(src=>src.Likes.Where(x=>x.Value == true).Count()))
                .ForMember(dest => dest.Dislikes, op => op.MapFrom(src => src.Likes.Where(x => x.Value == false).Count()));

            CreateMap<Comment, CommentViewModel>()
                .ForMember(dest => dest.Publisher, op => op.MapFrom(src => src.Account.Username))
                .ForMember(dest => dest.PostedOn, op => op.MapFrom(src => src.CreatedOn))
                 .ForMember(dest => dest.Likes, op => op.MapFrom(src => src.Likes.Where(x => x.Value == true).Count()))
                .ForMember(dest => dest.Dislikes, op => op.MapFrom(src => src.Likes.Where(x => x.Value == false).Count()));

            CreateMap<Community, CommunityViewModel>()
                .ForMember(dest => dest.Description, op => op.MapFrom(src => src.Information))
                .ForMember(dest => dest.Creator, op => op.MapFrom(src => src.Creator.Username));

            CreateMap<Follower, FollowerViewModel>()
                .ForMember(dest => dest.FollowerId, op => op.MapFrom(src =>src.FollowerId))
                .ForMember(dest => dest.FollowerUsername, op => op.MapFrom(src => src.FollowerNavigation.Username));

			CreateMap<Message, MessageViewModel>()
			   .ForMember(dest => dest.SenderUsername, op => op.MapFrom(src => src.Sender.Username))
			   .ForMember(dest => dest.Text, op => op.MapFrom(src => src.Text));
            CreateMap<PostResources, PostRescourcesViewModel>();
		}

    }
}
