﻿using AutoMapper;
using Zest.DBModels.Models;
using Zest.ViewModels.ViewModels;

namespace Zest.Services.Mapper
{
	public class MappingProfile : Profile
	{
		public MappingProfile()
		{
			CreateMap<Account, AccountViewModel>()
				.ForMember(dest => dest.CreatedOn1, op => op.MapFrom(src => src.CreatedOn))
			.ForMember(dest => dest.IsAdmin, op => op.MapFrom(src => src.IsAdmin));
			CreateMap<Account, BaseAccountViewModel>();
			CreateMap<Post, PostViewModel>()
				.ForMember(dest => dest.Publisher, op => op.MapFrom(src => src.Account.Username))
				.ForMember(dest => dest.PostedOn, op => op.MapFrom(src => src.CreatedOn))
				.ForMember(dest => dest.Likes, op => op.MapFrom(src => src.Likes.Where(x => x.Value == true).Count()))
				.ForMember(dest => dest.Dislikes, op => op.MapFrom(src => src.Likes.Where(x => x.Value == false).Count()))
				.ForMember(dest => dest.CommunityName, op => op.MapFrom(src => src.Community.Name))
				.ForMember(dest => dest.CommunityId, op => op.MapFrom(src => src.Community.Id))
				.ForMember(dest => dest.ResourceType, op => op.MapFrom(src => src.PostResources.FirstOrDefault().Type))
			   .AfterMap((src, dest) =>
			   {
				   if (src.IsDeleted == true)
				   {
					   dest.Title = "Deleted";
					   dest.Text = "Deleted";
					   dest.Publisher = "Unknown";
				   }
			   });

			CreateMap<Comment, CommentViewModel>()
				.ForMember(dest => dest.Publisher, op => op.MapFrom(src => src.Account.Username))
				.ForMember(dest => dest.PostedOn, op => op.MapFrom(src => src.CreatedOn))
				  .ForMember(dest => dest.Likes, op => op.MapFrom(src => src.Likes.Where(x => x.Value == true).Count()))
				.ForMember(dest => dest.Dislikes, op => op.MapFrom(src => src.Likes.Where(x => x.Value == false).Count()))
				 .AfterMap((src, dest) =>
				 {
					 if (src.IsDeleted == true)
					 {
						 dest.Text = "Deleted";
						 dest.Publisher = "Unknown";
					 }
				 }); ;

			CreateMap<Community, CommunityViewModel>()
				.ForMember(dest => dest.Description, op => op.MapFrom(src => src.Information))
				.ForMember(dest => dest.Creator, op => op.MapFrom(src => src.Creator.Username));

			CreateMap<Follower, BaseAccountViewModel>()
				.ForMember(dest => dest.Id, op => op.MapFrom(src => src.FollowerId))
				.ForMember(dest => dest.Username, op => op.MapFrom(src => src.FollowerNavigation.Username));

			CreateMap<Message, MessageViewModel>()
			   .ForMember(dest => dest.SenderUsername, op => op.MapFrom(src => src.Sender.Username))
			   .ForMember(dest => dest.Text, op => op.MapFrom(src => src.Text));
			CreateMap<PostResources, PostRescourcesViewModel>()
				.ForMember(dest => dest.Source, op => op.MapFrom(src => src.Path + " "));
			CreateMap<Account, UserViewModel>()
				.ForMember(dest => dest.CreatedOn1, op => op.MapFrom(src => src.CreatedOn));
			CreateMap<Like, LikeViewModel>();
		}

	}
}
