
using Microsoft.EntityFrameworkCore;
using Zest.DBModels;
using Zest.Hubs;

namespace Zest
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            builder.Services.AddSignalR();
            builder.Services.AddAutoMapper(bl =>
            {
                bl.AddProfile(new MappingProfile());
            });
            builder.Services.AddDbContext<ZestContext>(b=> b.UseLazyLoadingProxies());
            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();

            app.MapHub<LikesHub>("/likeshub");

            app.MapControllers();

            app.Run();
        }
    }
}