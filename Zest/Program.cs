
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Zest.DBModels;
using Zest.Hubs;
using Zest.Services;

namespace Zest
{
	public class Program
	{
		public static void Main(string[] args)
		{
			var builder = WebApplication.CreateBuilder(args);

			// Add services to the container.

			builder.Services.AddControllers();
			builder.Services.AddSingleton<UserConnectionService>();
			builder.Services.AddSingleton<SignaRGroupsPlaceholder>();

			// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
			builder.Services.AddEndpointsApiExplorer();
			builder.Services.AddSwaggerGen(options =>
			{



			});
			builder.Services.AddSignalR();
			builder.Services.AddAutoMapper(bl =>
			{
				bl.AddProfile(new MappingProfile());
			});
			builder.Services.AddDbContext<ZestContext>(b => b.UseLazyLoadingProxies());

			builder.Services.AddAuthentication(options =>
			{
				options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
				options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
			
			}).AddJwtBearer(options =>
			{
				options.Authority = "https://dev-kckk4xk2mvwnhizd.us.auth0.com/";
				options.Audience = "https://localhost:7183";

			}).AddOpenIdConnect("Auth0", options =>
			{
				options.SignInScheme = JwtBearerDefaults.AuthenticationScheme;
				options.Authority = "https://dev-kckk4xk2mvwnhizd.us.auth0.com/";
				options.ClientId = "65def0009bb0ca8e79b027e6";
				options.ClientSecret = "PSTa40M0QNRtqdtIW9AqJSHD3LFGACtv6EXFjbErp9VL5JEE8Lf61y_oS9xyjBkm";
				options.ResponseType = "code";
				options.SaveTokens = true;
			});

			var app = builder.Build();

			app.UseAuthentication();
			app.UseAuthorization();
			
			// Configure the HTTP request pipeline.
			if (app.Environment.IsDevelopment())
			{
				app.UseSwagger();
				app.UseSwaggerUI();
			}

			app.UseHttpsRedirection();

			
			
			app.MapHub<LikesHub>("/likeshub");
			app.MapHub<CommentsHub>("/commentshub");
			app.MapHub<MessageHub>("/messagehub");

			app.MapControllers();

			app.Run();
		}
	}
}