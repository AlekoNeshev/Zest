using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zest.DBModels.Models;
using Zest.DBModels;
using Zest.Services.Infrastructure.Services;
using Zest.ViewModels.ViewModels;
using System.Linq.Expressions;
using Zest.Services.Infrastructure.Interfaces;

namespace Zest.UnitTests.Services
{
	[TestFixture]
	public class PostServiceTests
	{
		private PostService _postService;
		private ZestContext _context;

		[SetUp]
		public void Setup()
		{
			var options = new DbContextOptionsBuilder<ZestContext>()
				.UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
				.Options;
			_context = new ZestContext(options);
			_context.Database.EnsureCreated();

			var mapperConfig = new MapperConfiguration(cfg =>
			{
				cfg.CreateMap<Post, PostViewModel>()
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
			});
			IMapper mapper = mapperConfig.CreateMapper();

			_postService = new PostService(_context, mapper);
		}

		[TearDown]
		public void TearDown()
		{
			_context.Dispose();
		}

		[Test]
		public async Task DoesExist_ReturnsTrue_IfPostExists()
		{
			// Arrange
			var post = new Post { Id = 1, Title = "Test Post", Text = "Test Text", CreatedOn = DateTime.Now, AccountId="test" };
			await _context.Posts.AddAsync(post);
			await _context.SaveChangesAsync();

			// Act
			var result = await _postService.DoesExist(post.Id);

			// Assert
			Assert.IsTrue(result);
		}

		[Test]
		public async Task DoesExist_ReturnsFalse_IfPostDoesNotExist()
		{
			// Arrange

			// Act
			var result = await _postService.DoesExist(999);

			// Assert
			Assert.IsFalse(result);
		}
	
		[Test]
		public async Task FindAsync_ReturnsNull_IfPostDoesNotExist()
		{
			// Arrange
			var accountId = "testAccountId";
			var postId = 999;

			// Act
			var result = await _postService.FindAsync(postId, accountId);

			// Assert
			Assert.IsNull(result);
		}
		[Test]
		public async Task AddAsync_AddsPostToDatabase()
		{
			// Arrange
			var title = "Test Post";
			var text = "Test Text";
			var accountId = "testAccountId";
			var communityId = 1;

			// Act
			await _postService.AddAsync(title, text, accountId, communityId);

			// Assert
			var post = await _context.Posts.FirstOrDefaultAsync();
			Assert.IsNotNull(post);
			Assert.AreEqual(title, post.Title);
			Assert.AreEqual(text, post.Text);
			Assert.AreEqual(accountId, post.AccountId);
			Assert.AreEqual(communityId, post.CommunityId);
		
		}
		[Test]
		public async Task RemoveAsync_MarksPostAsDeleted()
		{
			// Arrange
			var post = new Post
			{
				Id = 1,
				Title = "Test Post",
				Text = "Test Text",
				AccountId = "testAccountId",
				CommunityId = 1,
				CreatedOn = DateTime.Now
			};
			await _context.Posts.AddAsync(post);
			await _context.SaveChangesAsync();

			// Act
			await _postService.RemoveAsync(post.Id);

			// Assert
			var deletedPost = await _context.Posts.FindAsync(post.Id);
			Assert.IsTrue(deletedPost.IsDeleted);
		}
		
		

		[Test]
		public async Task IsOwnerAsync_ReturnsTrueIfUserIsOwnerOfPost()
		{
			// Arrange
			var postId = 1;
			var accountId = "testAccountId";
			var post = new Post { Id = postId, AccountId = accountId, CreatedOn=DateTime.Now,Text="test" , Title = "test"};
			await _context.Posts.AddAsync(post);
			await _context.SaveChangesAsync();

			// Act
			var isOwner = await _postService.IsOwnerAsync(postId, accountId);

			// Assert
			Assert.IsTrue(isOwner); 
		}
		
	}




}
