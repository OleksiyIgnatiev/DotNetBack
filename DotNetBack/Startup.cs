using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;
using DotNetBack.Repositories;
using Microsoft.OpenApi.Models;
using System;

public class Startup
{
    public Startup(IConfiguration configuration)
    {
        Configuration = configuration;
    }

    public IConfiguration Configuration { get; }

    public void ConfigureServices(IServiceCollection services)
    {
        services.AddControllers();

        services.AddHttpContextAccessor();

        services.AddDistributedMemoryCache();
        services.AddSession(options =>
        {
            options.IdleTimeout = TimeSpan.FromMinutes(30); // Время жизни сессии
            options.Cookie.HttpOnly = true;
            options.Cookie.IsEssential = true;
        });

        services.AddSwaggerGen();
        services.AddScoped<IMessageRepository, MessageRepository>();
        services.AddScoped<ICategoryRepository, CategoryRepository>();
        services.AddScoped<IWordRepository, WordRepository>();
        services.AddScoped<IUserRepository, UserRepository>();

        services.AddCors(options =>
        {
            options.AddPolicy("AllowAllOrigins", builder =>
            {
                builder.AllowAnyOrigin()
                       .AllowAnyMethod()
                       .AllowAnyHeader();
            });
        });
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Your API v1");
            });
        }
        else
        {
            app.UseExceptionHandler("/Home/Error");
            app.UseHsts();
        }

        app.UseRouting();

        app.UseAuthorization();

        app.UseSession();

        app.UseCors(builder =>
        {
            builder
            .WithOrigins("http://localhost:3000", "http://localhost:5264") // my client app url
            .AllowAnyMethod()
            .AllowAnyHeader()
            .AllowCredentials();
        });

        app.UseHttpsRedirection();
        app.UseStaticFiles();

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers();
        });
    }
}
