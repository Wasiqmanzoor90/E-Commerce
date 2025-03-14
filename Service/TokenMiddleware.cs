using E_Commerce.Data;
using E_Commerce.Interface;

using Microsoft.EntityFrameworkCore;

namespace E_Commerce.Service
{
    public class TokenMiddleware(RequestDelegate next, IServiceScopeFactory serviceScopeFactory)
    {
        private readonly RequestDelegate _next= next;
        private readonly IServiceScopeFactory _serviceScopeFactory = serviceScopeFactory;

      

        public async Task Invoke(HttpContext context)
        {
            var token = context.Request.Cookies["TestToken"];

            // ✅ If token is missing, do NOT attempt validation
            if (string.IsNullOrEmpty(token))
            {
                await _next(context); // Let the request continue
                return;
            }

            // ✅ Create a scoped service provider for DbContext
            using var scope = _serviceScopeFactory.CreateScope();
            var dbcontext = scope.ServiceProvider.GetRequiredService<SqldbContext>();
            var _jsontoken = scope.ServiceProvider.GetRequiredService<IJasonToken>();


            // ✅ Verify token only if it's not null or empty
            var userid = _jsontoken.VerifyToken(token);
            if (userid == Guid.Empty)
            {
                context.Response.Cookies.Delete("TestToken");
                context.Response.Redirect("/User/Login");
                return;
            }

            // ✅ Fetch user from DB
            var user = await dbcontext.Users.FirstOrDefaultAsync(u => u.UserId == userid);
            if (user != null)
            {
                context.Items["User"] = user;
            }
            await _next(context); // ✅ Ensure the request continues
        }
    }
}
       
            

           
        
    

