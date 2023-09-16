using AutoMapper;
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
                .ForMember(dest => dest.PostedOn, op => op.MapFrom(src => src.CreatedOn)); ;

            CreateMap<Comment, CommentViewModel>()
                .ForMember(dest => dest.Publisher, op =>op.MapFrom(src=>src.Account.Username))
                .ForMember(dest=> dest.PostedOn, op=>op.MapFrom(src=>src.CreatedOn));

            CreateMap<Community, CommunityViewModel>()
                .ForMember(dest => dest.Description, op=>op.MapFrom(src=>src.Information))
                .ForMember(dest => dest.Creator, op=>op.MapFrom(src=>src.Creator.Username));
        }

    }
}
