using E_Commerce.Data;
using E_Commerce.Interface;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace E_Commerce.Service
{
    public class TokenMiddleware(RequestDelegate next, IServiceScopeFactory serviceScopeFactory)
    {
        private readonly RequestDelegate _next = next;
        private readonly IServiceScopeFactory _serviceScopeFactory = serviceScopeFactory;

        public async Task Invoke(HttpContext context)
        {
            var token = context.Request.Cookies["TestToken"];
            if (string.IsNullOrEmpty(token))
            {
                await _next(context);
                return;
            }

            using var scope = _serviceScopeFactory.CreateScope();
            //Resolve `SqldbContext` and `IJasonToken` services from the created scope
            var dbcontext = scope.ServiceProvider.GetRequiredService<SqldbContext>();
            var _jsontoken = scope.ServiceProvider.GetRequiredService<IJasonToken>();

            var userid = _jsontoken.VerifyToken(token);
            //If userid is empty
            if (userid == Guid.Empty)
            {
                context.Response.Cookies.Delete("TestToken");  //Delete token
                context.Response.Redirect("/User/Login");
                return; // Stop further execution of this middleware
            }

            var user = await dbcontext.Users.FirstOrDefaultAsync(u => u.UserId == userid);


            // If the user exists, attach it to `HttpContext.Items`
            // This allows controllers and other middleware to access the user details easily
            if (user != null)
            {
                context.Items["User"] = user;
            }

            await _next(context);
        }

    }
}
       
            

           
        
    

