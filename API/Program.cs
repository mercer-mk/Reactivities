using API.Middleware;
using Application.Activities.Queries;
using Application.Validators;
using Application.Core;

using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Persistence;
using Microsoft.AspNetCore.Identity;
using Domain;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Authorization;
using Application.Interfaces;

using Infrastructure.Security;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers(opt =>
 {
     var policy = new AuthorizationPolicyBuilder().RequireAuthenticatedUser().Build();
     opt.Filters.Add(new AuthorizeFilter(policy));
 });
builder.Services.AddDbContext<AppDbContext>(opt =>
{
    opt.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
});

builder.Services.AddCors();
builder.Services.AddMediatR(x =>
{
    x.RegisterServicesFromAssemblyContaining<GetActivityList.Handler>();
    x.AddOpenBehavior(typeof(ValidationBehavior<,>));
});
builder.Services.AddScoped<IUserAccessor, UserAccessor>();
builder.Services.AddAutoMapper(typeof(MappingProfiles).Assembly);
builder.Services.AddValidatorsFromAssemblyContaining<CreateActivityValidator>();
builder.Services.AddTransient<ExceptionMiddleware>();

builder.Services.AddIdentityApiEndpoints<User>(opt =>
 {
     opt.User.RequireUniqueEmail = true;
 })
 .AddRoles<IdentityRole>()
 .AddEntityFrameworkStores<AppDbContext>();
builder.Services.AddAuthorizationBuilder()
    .AddPolicy("IsActivityHost", policy => 
     {
         policy.Requirements.Add(new IsHostRequirement());
     });
builder.Services.AddTransient<IAuthorizationHandler, IsHostRequirementHandler>();
var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseMiddleware<ExceptionMiddleware>();
app.UseCors(x => x.AllowAnyHeader().
AllowAnyMethod().AllowCredentials().WithOrigins("http://localhost:3000", "https://localhost:3000"));

app.UseAuthentication();
app.UseAuthorization();

app.UseDefaultFiles();
app.UseStaticFiles();
app.MapControllers();
app.MapGroup("api").MapIdentityApi<User>(); // api/login
app.MapFallbackToController("Index","Fallback");

using var scope = app.Services.CreateScope();
var services = scope.ServiceProvider;

try
{
    var context = services.GetRequiredService<AppDbContext>();
    var userManager = services.GetRequiredService<UserManager<User>>();
    await context.Database.MigrateAsync();
    await DbIntializer.SeedData(context, userManager);
}
catch (Exception ex)
{
    var logger = services.GetRequiredService<ILogger<Program>>();
    logger.LogError(ex, "An error occured during db migration");
}

app.Run();
