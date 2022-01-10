﻿using System.Threading.Channels;
using ChannelExample.API.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<Database>(config => config.UseInMemoryDatabase("test"));
builder.Services.AddHttpClient();

builder.Services.AddHostedService<NotificationDispatcher>();
builder.Services.AddSingleton(Channel.CreateUnbounded<string>());


builder.Services.AddTransient<Notifications>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
