using StackoverflowService.Application.Abstractions;
using System.Security.Claims;
using System;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using System.Configuration;
using System.Text;

namespace StackoverflowService.Infrastructure.Security
{
    public class IdentityValidator : IIdentityValidator
    {
        private readonly string _issuer;
        private readonly string _audience;
        private readonly SymmetricSecurityKey _key;
        private readonly TokenValidationParameters _parameters;
        private readonly JsonWebTokenHandler _handler = new JsonWebTokenHandler();

        public IdentityValidator()
        {
            _issuer = ConfigurationManager.AppSettings["Jwt:Issuer"] ?? "StackOverflowService";
            _audience = ConfigurationManager.AppSettings["Jwt:Audience"] ?? "StackOverflowService.Client";
            var secret = ConfigurationManager.AppSettings["Jwt:Key"];

            _key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret));

            _parameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidIssuer = _issuer,
                ValidateAudience = true,
                ValidAudience = _audience,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = _key,
                RequireSignedTokens = true,
                ValidateLifetime = true,
                ClockSkew = TimeSpan.FromMinutes(2)
            };
        }

        public bool TryValidate(string token, out ClaimsPrincipal principal, out string error)
        {
            principal = null;
            error = null;
            if (string.IsNullOrWhiteSpace(token))
            {
                error = "Missing token.";
                return false;
            }

            var result = _handler.ValidateToken(token, _parameters);
            if (!result.IsValid)
            {
                error = result.Exception?.Message ?? "Invalid token.";
                return false;
            }

            principal = new ClaimsPrincipal(result.ClaimsIdentity);
            return true;
        }
    }
}
