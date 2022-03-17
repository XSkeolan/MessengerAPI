using MessengerAPI.Contexts;
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

        public async Task InvokeAsync(HttpContext context, IServiceContext serviceContext)
        {
            Console.WriteLine(context.Request.Path.Value);
            if (context.Request.Path.Value.StartsWith("/api/private"))
            {
                string jwtToken = context.Request.Headers.Authorization;
                if (jwtToken == null)
                {
                    context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                    await context.Response.WriteAsync(ResponseErrors.UNAUTHORIZE);
                    return;
                }

                string token = jwtToken.Split(' ')[1];

                var handler = new JwtSecurityTokenHandler();
                var jsonToken = handler.ReadToken(token);
                var tokenS = jsonToken as JwtSecurityToken;

                Guid sessionId = Guid.Parse(tokenS.Claims.First(claim => claim.Type == ClaimsIdentity.DefaultNameClaimType).Value);
                DateTime dateEnd = DateTime.Parse(tokenS.Claims.First(claim => claim.Type == "DateEnd").Value);

                if (dateEnd < DateTime.UtcNow)
                {
                    context.Response.StatusCode = (int)HttpStatusCode.Forbidden;
                    await context.Response.WriteAsync(ResponseErrors.TOKEN_EXPIRED);
                    return;
                }

                Session session = await _sessionRepository.GetAsync(sessionId);
                if (session == null)
                {
                    context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                    await context.Response.WriteAsync(ResponseErrors.SESSION_NOT_FOUND);
                    return;
                }
                serviceContext.ConfigureSession(session);
            }
            await _next(context);
        }
    }
}
