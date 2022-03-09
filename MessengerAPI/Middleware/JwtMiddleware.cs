using MessengerAPI.Interfaces;
using MessengerAPI.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;

namespace MessengerAPI.Middleware
{
    public class JwtMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ISessionRepository _sessionRepository;

        public JwtMiddleware(RequestDelegate next, ISessionRepository sessionRepository)
        {
            _next = next;
            _sessionRepository = sessionRepository;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            string jwtToken = context.Request.Headers.Authorization;
            var token = jwtToken.Split(' ')[1];
            Console.WriteLine(jwtToken);
            Console.WriteLine(token);

            var handler = new JwtSecurityTokenHandler();
            var jsonToken = handler.ReadToken(token);
            var tokenS = jsonToken as JwtSecurityToken;

            Guid sessionId = Guid.Parse(tokenS.Claims.First(claim => claim.Type == ClaimsIdentity.DefaultNameClaimType).Value);
            DateTime dateEnd = DateTime.Parse(tokenS.Claims.First(claim => claim.Type == "DateEnd").Value);

            if (dateEnd > DateTime.Now)
            {
                context.Response.StatusCode = (int)HttpStatusCode.Forbidden;
                await context.Response.WriteAsync(ResponseErrors.TOKEN_EXPIRED);
            }
            else
            {
                Session session = await _sessionRepository.GetAsync(sessionId);
                if(session == null)
                {
                    context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                    await context.Response.WriteAsync(ResponseErrors.SESSION_NOT_FOUND);
                }
                else
                {
                    await _next(context);
                }
            }
        }
    }
}
