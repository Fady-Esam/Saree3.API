using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Saree3.API.BL.Interfaces;
using Saree3.API.Domains;
using Saree3.API.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Saree3.API.BL.Classes
{
    public class AuthService : IAuthService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly UserManager<AppUser> _userManager;
        private readonly JWTModel _jwt;
        private readonly ApplicationDBContext _context;
        public AuthService(UserManager<AppUser> userManager, IOptions<JWTModel> jwt, IHttpClientFactory httpClientFactory, ApplicationDBContext context)
        {
            _userManager = userManager;
            _jwt = jwt.Value;
            _httpClientFactory = httpClientFactory;
            _context = context;
        }
        public async Task<APIResponse> RegisterCustomerWithEmailAndPassword(RegisterModel RegisterModel)
        {
            try
            {
                if (await _userManager.FindByNameAsync(RegisterModel.UserName) is not null)
                {
                    return new APIResponse
                    {
                        ErrMessage = "UserName already exists",
                        Success = false
                    };
                }
                if (await _userManager.FindByEmailAsync(RegisterModel.Email) is not null)
                {
                    return new APIResponse
                    {
                        ErrMessage = "Email already exists",
                        Success = false
                    };
                }
                var user = new AppUser
                {
                    UserName = RegisterModel.UserName,
                    Email = RegisterModel.Email,
                };
                await _userManager.AddToRoleAsync(user, "Customer");
                var res = await _userManager.CreateAsync(user, RegisterModel.Password);
                if (!res.Succeeded)
                {
                    return new APIResponse
                    {
                        Success = false,
                        ErrMessage = string.Join(", ", res.Errors.Select(i => i.Description))
                    };
                }
                var token = await GetAccessToken(user);
                var authModel = new AuthModel
                {
                    UserId = user.Id,
                    IsAuthenticated = true,
                    Roles = new List<string> { "Customer" },
                    ExpiresOn = token.ValidTo,
                    UserName = user.UserName,
                    Email = user.Email,
                    Token = new JwtSecurityTokenHandler().WriteToken(token),
                };
                return new APIResponse
                {
                    Success = true,
                    Data = authModel,
                };
            }
            catch (Exception ex)
            {
                return new APIResponse
                {
                  Success = false,
                  ErrMessage = "Server Failure, Please try again"
                };
            }

        }


        public async Task<APIResponse> LoginWithEmailAndPassword(LogInModel LogInModel)
        {
            try
            {
                var user = await _userManager.FindByEmailAsync(LogInModel.Email);
                if (user is null || !await _userManager.CheckPasswordAsync(user, LogInModel.Password))
                {
                    return new APIResponse
                    {
                        Success = false,
                        ErrMessage = "Invalid Email or Password",
                    };
                }
                var token = await GetAccessToken(user);

                var authModel = new AuthModel
                {
                    UserId = user.Id,
                    IsAuthenticated = true,
                    Roles = (await _userManager.GetRolesAsync(user)).ToList(),
                    ExpiresOn = token.ValidTo,
                    UserName = user.UserName!,
                    Email = user.Email!,
                    Token = new JwtSecurityTokenHandler().WriteToken(token),

                };
                return new APIResponse
                {
                    Success = true,
                    Data = authModel,
                };

            }
            catch (Exception ex)
            {
                return new APIResponse
                {
                    Success = false,
                    ErrMessage = "Server Failure, Please try again"
                };
            }
        }
        public async Task<APIResponse> RegisterRiderWithEmailAndPassword(RegisterModel RegisterModel)
        {
            try
            {
                if (await _userManager.FindByNameAsync(RegisterModel.UserName) is not null)
                {
                    return new APIResponse
                    {
                        ErrMessage = "UserName already exists",
                        Success = false
                    };
                }
                if (await _userManager.FindByEmailAsync(RegisterModel.Email) is not null)
                {
                    return new APIResponse
                    {
                        ErrMessage = "Email already exists",
                        Success = false
                    };
                }
                var user = new AppUser
                {
                    UserName = RegisterModel.UserName,
                    Email = RegisterModel.Email,
                };
                await _userManager.AddToRoleAsync(user, "Rider");
                var res = await _userManager.CreateAsync(user, RegisterModel.Password);
                if (!res.Succeeded)
                {
                    return new APIResponse
                    {
                        Success = false,
                        ErrMessage = string.Join(", ", res.Errors.Select(i => i.Description))
                    };
                }
                var token = await GetAccessToken(user);

                var authModel = new AuthModel
                {
                    UserId = user.Id,
                    IsAuthenticated = true,
                    Roles = new List<string> { "Rider" },
                    ExpiresOn = token.ValidTo,
                    UserName = user.UserName,
                    Email = user.Email,
                    Token = new JwtSecurityTokenHandler().WriteToken(token),
                };
                return new APIResponse
                {
                    Success = true,
                    Data = authModel,
                };
            }
            catch (Exception ex)
            {
                return new APIResponse
                {
                    Success = false,
                    ErrMessage = "Server Failure, Please try again"
                };
            }
        }
        private async Task<JwtSecurityToken> GetAccessToken(AppUser appUser)
        {
            var claims = new List<Claim>
            { 
                new Claim(ClaimTypes.NameIdentifier, appUser.Id),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Sub, appUser.UserName ?? ""),
                new Claim(JwtRegisteredClaimNames.Email, appUser.Email ?? ""),
            };
            var roles = await _userManager.GetRolesAsync(appUser!);
            foreach (var item in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, item));
            }
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwt.Key));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var jwtToken = new JwtSecurityToken(
                    issuer: _jwt.Issuer,
                    audience: _jwt.Audience,
                    claims: claims,
                    expires: DateTime.Now.AddMonths(_jwt.DurationInMonths),
                    signingCredentials: credentials
                );
            return jwtToken;

        }


        #region RefreshTokenSection
        //private string GenerateRefreshToken()
        //{
        //    var refreshToken = Guid.NewGuid().ToString();
        //    return refreshToken;
        //}
        //public async Task StoreRefreshToken(string userId, string refreshToken)
        //{
        //    try
        //    {
        //        var existingToken = await _context.UserRefreshTokens
        //            .FirstOrDefaultAsync(t => t.UserId == userId && t.IsRevoked == false);

        //        if (existingToken != null)
        //        {
        //            // If a valid token exists, revoke it (mark as revoked)
        //            existingToken.IsRevoked = true;
        //            _context.UserRefreshTokens.Update(existingToken);
        //            await _context.SaveChangesAsync();
        //        }

        //        // Generate new expiration date for the refresh token (e.g., 7 days from now)
        //        var expiryDate = DateTime.UtcNow.AddDays(7);

        //        // Create the new refresh token entry
        //        var newRefreshToken = new UserRefreshToken
        //        {
        //            UserId = userId.ToString(),
        //            RefreshToken = refreshToken,
        //            ExpiryDate = expiryDate,
        //            CreatedAt = DateTime.UtcNow,
        //            IsRevoked = false
        //        };

        //        // Insert the new refresh token into the database
        //        await _context.UserRefreshTokens.AddAsync(newRefreshToken);
        //        await _context.SaveChangesAsync();
        //    }
        //    catch (Exception ex)
        //    {

        //    }
        //}
        //public async Task<APIResponse> Refresh( UserRefreshToken model)
        //{
        //    try
        //    {
        //        // Step 1: Validate the Refresh Token (check if it exists and is valid)
        //        var userId = await ValidateRefreshToken(model.RefreshToken);
        //        if (userId == null)
        //        {
        //            return new APIResponse
        //            {
        //                Success = false,
        //                ErrMessage = "Invalid or expired refresh token."
        //            };
        //        }

        //        // Step 2: Retrieve user and generate new access token
        //        var user = await _userManager.FindByIdAsync(userId.ToString());
        //        if (user == null)
        //        {
        //            return new APIResponse
        //            {
        //                Success = false,
        //                ErrMessage = "Invalid User"
        //            };
        //        }

        //        var newToken = await GetAccessToken(user);

        //        // Step 3: Generate a new refresh token (optional, if refresh tokens are rotating)
        //        var newRefreshToken = GenerateRefreshToken();

        //        // Step 4: Store the new refresh token securely in the database (optional)
        //        await StoreRefreshToken(user.Id, newRefreshToken);

        //        // Step 5: Return new tokens to client
        //        var authModel = new AuthModel
        //        {
        //            UserId = user.Id,
        //            IsAuthenticated = true,
        //            Token = new JwtSecurityTokenHandler().WriteToken(newToken),
        //            RefreshToken = newRefreshToken 
        //        };
        //        return new APIResponse
        //        {
        //            Data = authModel,
        //            Success = true
        //        };

        //    }
        //    catch (Exception ex)
        //    {
        //        return new APIResponse 
        //        { 
        //            ErrMessage = "Server Error, Please try again", 
        //            Success = false 
        //        };
        //    }
        //}




        //public async Task RevokeRefreshToken(Guid userId)
        //{
        //    try
        //    {
        //        // Find the refresh token for the user and mark it as revoked
        //        var refreshToken = await _context.UserRefreshTokens
        //            .FirstOrDefaultAsync(t => t.UserId == userId.ToString() && t.IsRevoked == false);

        //        if (refreshToken != null)
        //        {
        //            refreshToken.IsRevoked = true;
        //            _context.UserRefreshTokens.Update(refreshToken);
        //            await _context.SaveChangesAsync();
        //        }
        //    }
        //    catch (Exception ex)
        //    {

        //    }
        //}
        //public async Task<string?> ValidateRefreshToken(string refreshToken)
        //{
        //    try
        //    {
        //        // Find the refresh token in the database and check if it's valid
        //        var token = await _context.UserRefreshTokens
        //            .FirstOrDefaultAsync(t => t.RefreshToken == refreshToken && t.IsRevoked == false && t.ExpiryDate > DateTime.UtcNow);

        //        if (token == null)
        //        {
        //            return null; // Token is invalid or expired
        //        }

        //        return token.UserId;
        //    }
        //    catch (Exception ex)
        //    {
        //        return null;
        //    }
        //}
        #endregion

    }
}
