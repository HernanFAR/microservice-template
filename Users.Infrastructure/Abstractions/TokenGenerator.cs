using System;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.IdentityModel.Tokens;
using SharedKernel.Application.Configurations;
using Users.Application.Abstractions;
using Users.Domain.Entities.Users;
using Users.EntityFramework.Identity;

namespace Users.Infrastructure.Abstractions
{
    public class TokenGenerator : ITokenGenerator
    {
        private readonly ApplicationSignInManager _SignInManager;
        private readonly JwtConfiguration _JwtConfiguration;

        public TokenGenerator(ApplicationSignInManager signInManager, JwtConfiguration jwtConfiguration)
        {
            _SignInManager = signInManager;
            _JwtConfiguration = jwtConfiguration;
        }

        public async Task<(string Token, DateTime ExpireDate)> GetIdentityTokenAsync(ApplicationUser user,
            CancellationToken cancellationToken)
        {
            var identity = await _SignInManager.CreateUserPrincipalAsync(user);
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_JwtConfiguration.IssuerSigningKey));

            var tokenHandler = new JwtSecurityTokenHandler();
            var jwt = new JwtSecurityToken(
                issuer: _JwtConfiguration.Issuer,
                audience: _JwtConfiguration.Audience,
                claims: identity.Claims,
                expires: DateTime.Now.AddDays(_JwtConfiguration.Duration),
                signingCredentials: new SigningCredentials(key, SecurityAlgorithms.HmacSha256)
            );

            return (tokenHandler.WriteToken(jwt), jwt.ValidTo);
        }
    }
}
