using System.Security;
using ChMS.Modules.Auth.Core.DTOs;
using ChMS.Modules.Auth.Core.Entities;
using ChMS.Modules.Auth.Database;
using ChMS.Modules.Auth.Infrastructure;
using EZXception.Authorization;
using EZXception.Business;
using EZXception.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace ChMS.Modules.Auth.Application.Services
{
    public class AuthService(
        AuthDbContext authDbContext,
        JwtService jwtService,
        IHttpContextAccessor httpContextAccessor
    )
    {
        private readonly IHttpContextAccessor _httpContextAccess = httpContextAccessor;
        private readonly AuthDbContext _db = authDbContext;
        private readonly JwtService _jwt = jwtService;

        public async Task<Guid> Signup(SignUpRequest signUpRequest)
        {
            bool duplicateEmail = await _db.Users.AnyAsync(u => u.Email == signUpRequest.Email);

            if (duplicateEmail)
                throw new DuplicateEntityException("Email", signUpRequest.Email);

            var user = new User
            {
                Id = Guid.NewGuid(),
                Name = signUpRequest.Name,
                Email = signUpRequest.Email,
                PasswordHash = PasswordHasher.HashPassword(signUpRequest.Password),
                Role = signUpRequest.Role,
                IsActive = signUpRequest.Role != Core.Enums.UserRole.Admin,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
            };

            await _db.Users.AddAsync(user);
            await _db.SaveChangesAsync();

            return user.Id;
        }

        public async Task<(SignInResponse, string)> Signin(string email, string password)
        {
            var httpContext = _httpContextAccess.HttpContext;
            var user =
                await _db.Users.FirstOrDefaultAsync(u => u.Email == email) ?? throw new InvalidCredentialsException();

            if (!PasswordHasher.ValidatePassword(password, user.PasswordHash))
                throw new InvalidCredentialsException();

            if (!user.IsActive)
                throw new OperationNotAllowedException("Login", $"Access Denied: Account is not active");

            var (accessToken, _) = _jwt.GenerateToken(user);
            var (refreshToken, _) = _jwt.GenerateToken(user, isRefreshToken: true);

            var refreshTokenRecord = new RefreshToken
            {
                Id = Guid.NewGuid(),
                UserId = user.Id,
                HashedToken = TokenHasher.HashToken(refreshToken),
                CreatedAt = DateTime.UtcNow,
                ExpiresAt = DateTime.UtcNow.AddDays(30),
                IpAddress = httpContext!.Connection.RemoteIpAddress?.ToString(),
                UserAgent = httpContext.Request.Headers.UserAgent.ToString(),
            };

            await _db.RefreshTokens.AddAsync(refreshTokenRecord);
            await _db.SaveChangesAsync();

            return (
                new SignInResponse
                {
                    AccessToken = accessToken,
                    User = new SignInResponse.UserInfo
                    {
                        Id = user.Id,
                        Name = user.Name,
                        Email = user.Email,
                        Role = user.Role,
                    },
                    HasOnboarded = user.HasOnboarded,
                },
                refreshToken
            );
        }

        public async Task<(string AccessToken, string RefreshToken)> RefreshTokenRotation(string currentRefreshToken)
        {
            var httpContext = _httpContextAccess.HttpContext;
            currentRefreshToken = TokenHasher.HashToken(currentRefreshToken);

            var currentRefreshTokenRecord =
                await _db
                    .RefreshTokens.Include(rt => rt.User)
                    .FirstOrDefaultAsync(rt => rt.HashedToken == currentRefreshToken)
                ?? throw new SecurityTokenValidationException("Refresh Aborted: Inactive or invalid refresh token");

            if (!currentRefreshTokenRecord.IsRevoked)
            {
                await RevokeUserTokenChainById(currentRefreshTokenRecord.UserId);
                throw new SecurityTokenValidationException("Refresh Aborted: Inactive or invalid refresh token");
            }

            if (currentRefreshTokenRecord.IsExpired)
                throw new UnauthorizedException("Refresh Failed: Refresh token expired");

            var (accessToken, _) = _jwt.GenerateToken(currentRefreshTokenRecord.User!);
            var (refreshToken, _) = _jwt.GenerateToken(currentRefreshTokenRecord.User!, isRefreshToken: true);

            var rotatedTokenRecord = new RefreshToken
            {
                Id = Guid.NewGuid(),
                UserId = currentRefreshTokenRecord.UserId,
                HashedToken = PasswordHasher.HashPassword(refreshToken),
                CreatedAt = DateTime.UtcNow,
                ExpiresAt = DateTime.UtcNow.AddDays(30),
                IpAddress = httpContext!.Connection.RemoteIpAddress?.ToString(),
                UserAgent = httpContext.Request.Headers.UserAgent.ToString(),
            };

            currentRefreshTokenRecord.RevokedAt = DateTime.UtcNow;
            currentRefreshTokenRecord.ReplacedByTokenId = rotatedTokenRecord.Id;

            await _db.AddAsync(rotatedTokenRecord);
            await _db.SaveChangesAsync();

            return (accessToken, refreshToken);
        }

        public async Task RevokeUserTokenChainById(Guid userId)
        {
            var matchingUserTokenRecords = await _db
                .RefreshTokens.Where(rt => rt.UserId == userId && rt.RevokedAt == null)
                .ToListAsync();

            foreach (var matchingUserTokenRecord in matchingUserTokenRecords)
                matchingUserTokenRecord.RevokedAt = DateTime.UtcNow;

            await _db.SaveChangesAsync();
        }
    }
}
