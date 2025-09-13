using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using StackoverflowService.Application.Abstractions;
using StackoverflowService.Application.DTOs.Auth;
using StackoverflowService.Domain.Entities;
using System;
using System.Configuration;
using System.Security.Claims;
using System.Text;

namespace StackoverflowService.Infrastructure.Security
{
    public class IdentityService : IIdentityService
    {
        private readonly string _issuer;
        private readonly string _audience;
        private readonly SymmetricSecurityKey _key;
        private readonly SigningCredentials _creds;
        private readonly TimeSpan _lifetime;

        public IdentityService()
        {
            _issuer = ConfigurationManager.AppSettings["Jwt:Issuer"] ?? "StackOverflowService";
            _audience = ConfigurationManager.AppSettings["Jwt:Audience"] ?? "StackOverflowService.Client";
            var secret = ConfigurationManager.AppSettings["Jwt:Key"];

            var minutes = int.TryParse(ConfigurationManager.AppSettings["Jwt:AccessTokenMinutes"], out var m) ? m : 60;

            _key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret));
            _creds = new SigningCredentials(_key, SecurityAlgorithms.HmacSha256);
            _lifetime = TimeSpan.FromMinutes(minutes);
        }

        public AuthResponseDto CreateAccessToken(User user)
        {
            if (user == null) throw new ArgumentNullException(nameof(user));

            var nowUtc = DateTime.UtcNow;
            var expires = nowUtc.Add(_lifetime);

            var claims = new[]
                        {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id),
                new Claim(JwtRegisteredClaimNames.Email, user.Email ?? ""),
                new Claim("name", user.Name ?? ""),
                new Claim("family_name", user.Lastname ?? ""),
                new Claim(JwtRegisteredClaimNames.Iat, Epoch(nowUtc), ClaimValueTypes.Integer64)
            };

            var descriptor = new SecurityTokenDescriptor
            {
                Issuer = _issuer,
                Audience = _audience,
                Subject = new ClaimsIdentity(claims),
                NotBefore = nowUtc.AddMinutes(-1),
                Expires = expires,
                SigningCredentials = _creds
            };

            var handler = new JsonWebTokenHandler();
            var token = handler.CreateToken(descriptor);

            var response = new AuthResponseDto
            {
                AccessToken = token,
                TokenType = "Bearer",
                ExpiresAt = new DateTimeOffset(expires, TimeSpan.Zero),
                UserId = user.Id,
                Email = user.Email ?? "",
                Name = user.Name ?? "",
                Lastname = user.Lastname ?? ""
            };

            return response;
        }

        private static string Epoch(DateTime utc)
            => ((long)(utc - new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalSeconds).ToString();

    }
}
