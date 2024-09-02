

using api.Midlleware;
using infrastructure;
using Microsoft.Net.Http.Headers;

var builder = WebApplication.CreateBuilder(args);

/*if (builder.Environment.IsDevelopment())
{
    builder.Services.AddNpgsqlDataSource(Utilities.ProperlyFormattedConnectionString,
        dataSourceBuilder => dataSourceBuilder.EnableParameterLogging());
}

if (builder.Environment.IsProduction())
{
    builder.Services.AddNpgsqlDataSource(Utilities.ProperlyFormattedConnectionString);
}*/

builder.Services.AddSingleton<Repository>(); // all the repository need to be added hear
//builder.Services.AddSingleton<Service>();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


 //var frontEndRelativePath = "./../factoryfrontend/www/";  //path to the frontend fail

/*
builder.Services.AddSpaStaticFiles(configuration => 
    { configuration.RootPath = "./../factoryfrontend/www/"; });
    */

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowOrigin",
        builder => builder
                .AllowAnyOrigin()
                .AllowAnyMethod()
                .AllowAnyHeader());
});

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseSwagger();
app.UseSwaggerUI();

app.UseCors(options =>
{
    options.SetIsOriginAllowed(origin => true)
        .AllowAnyMethod()
        .AllowAnyHeader()
        .AllowCredentials();
});

app.UseSpaStaticFiles(new StaticFileOptions()
{
    OnPrepareResponse = ctx =>
    {
        const int durationInSeconds = 60 * 60 * 24;
        ctx.Context.Response.Headers[HeaderNames.CacheControl] =
            "public,max-age=" + durationInSeconds;
    }
});

app.Map("/factoryfrontend",
    (IApplicationBuilder frontendApp) =>
    { frontendApp.UseSpa(spa =>
    { spa.Options.SourcePath = "./app/www/"; }); });


app.UseSpaStaticFiles();
// app.UseSpa(conf =>
// {
//     conf.Options.SourcePath = frontEndRelativePath;
// });


app.MapControllers();
app.UseMiddleware<Middleware>();

app.Run();