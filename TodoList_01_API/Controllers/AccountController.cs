using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using TodoList_01_API.Data;
using TodoList_01_API.DTOs;
using TodoList_01_API.Helpers;
using TodoList_01_API.Models;
using TodoList_01_API.Repository.IRepository;

namespace TodoList_01_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IConfiguration _appSettings;
        private readonly TokenValidationParameters _tokenValidationParams;
        private readonly ILogger<AccountController> _logger;

        public AccountController(
            AppDbContext context,
            IUnitOfWork unitOfWork, //
            UserManager<AppUser> userManager,
            SignInManager<AppUser> signInManager,
            RoleManager<IdentityRole> roleManager,
            IConfiguration appSettings, // (Access app settings data for Jwt token)
            TokenValidationParameters tokenValidationParams, // (Jwt token)
            ILogger<AccountController> logger
            )
        {
            _context = context;
            _unitOfWork = unitOfWork;
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
            _appSettings = appSettings;
            _tokenValidationParams = tokenValidationParams;
            _logger = logger;
        }

        [HttpGet("GetAllUsers")]
        public async Task<ActionResult> GetAllUsers()
        {
            //var users = _mapper.Map<List<AppUserDto>>(await _userManager.Users.ToListAsync());
            //// Convert date to string so JavaScript can parse the value
            //foreach (var user in users)
            //{
            //    user.DateAddedStr = user.DateAdded.ToString("MM/dd/yy");
            //}


            //return Ok(users);

            var users = await _userManager.Users.ToListAsync();
            if (users.Count() == 0)
            {
                return BadRequest(new AuthResult()
                {
                    Success = false,
                    Messages = new List<string>() { "Could not retrieve any users" }
                });
            }
            else
            {
                var userDtoList = new List<AppUserDto>();

                // Map all the users into user DTOs
                foreach (var u in users)
                {
                    var newUserDto = new AppUserDto()
                    {
                        Id = u.Id,
                        UserName = u.UserName,
                        Email = u.Email,
                        DateCreated = u.DateCreated,
                        DateCreatedStr = u.DateCreated.ToString(DateFormat.Month_Day_Year)
                    };

                    userDtoList.Add(newUserDto);
                }

                return Ok(userDtoList);
            }
        }

        [HttpGet("GetUserById/{userId}")]
        public async Task<ActionResult> GetUserById(string userId)
        {
            //var user = _mapper.Map<AppUserDto>(await _userManager.FindByIdAsync(userId));
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return BadRequest(new AuthResult()
                {
                    Success = false,
                    Messages = new List<string>() { "User doesn't exist" }
                });
            }
            else
            {
                var userDto = new AppUserDto()
                {
                    Id = user.Id,
                    UserName = user.UserName,
                    Email = user.Email,
                    DateCreated = user.DateCreated,
                    DateCreatedStr = user.DateCreated.ToString(DateFormat.Month_Day_Year) // Convert date to string so JavaScript can parse the value
                };

                return Ok(userDto);
            }


            //return Ok(user);
        }

        [HttpGet("GetUserProfile")]
        //[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = UserRoles.User)]
        public async Task<ActionResult> GetUserProfile()
        {
            // Extract the user's Id from the token(claims)
            string userId = User.Claims.First(c => c.Type == "Id").Value;
            if (userId == null)
            {
                return BadRequest(new AuthResult()
                {
                    Success = false,
                    Messages = new List<string>() { $"userId is null: {userId}" }
                });
            }

            //var user = _mapper.Map<AppUserDto>(await _userManager.FindByIdAsync(userId));
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return BadRequest(new AuthResult()
                {
                    Success = false,
                    Messages = new List<string>() { "User is not found" }
                });
            }

            // Map values
            var userDto = new AppUserDto()
            {
                Id = user.Id,
                UserName = user.UserName,
                Email = user.Email,
                DateCreated = user.DateCreated,
                DateCreatedStr = user.DateCreated.ToString()
            };

            return Ok(userDto);
        }


        [HttpPost("Register")]
        public async Task<ActionResult> Register(RegistrationReqDto registrationReqDto)
        {
            if (!ModelState.IsValid)
                return BadRequest();

            // Check if email already exists
            AppUser existingUser = await _userManager.FindByEmailAsync(registrationReqDto.Email);
            if (existingUser != null)
            {
                //// Check if username already exists (Identity doesn't like registering duplicates)
                //if (existingUser.UserName == (registrationReqDto.UserName))
                //{
                //    return BadRequest(new AuthResult()
                //    {
                //        Success = false,
                //        Messages = new List<string>() { "This username is already in use" }
                //    });
                //}

                return BadRequest(new AuthResult()
                {
                    Success = false,
                    Messages = new List<string>() { "This email address is already in use" }
                });
            }

            //AppUser newUser = _mapper.Map<AppUser>(requestDto);
            AppUser newUser = new AppUser()
            {
                UserName = registrationReqDto.UserName ?? registrationReqDto.Email,
                Email = registrationReqDto.Email,
                // Password is to be encrypted and stored via Identity Framework
                DateCreated = DateTime.UtcNow,
            };


            var newUserIsCreated = await _userManager.CreateAsync(newUser, registrationReqDto.Password);
            if (!newUserIsCreated.Succeeded)
            {
                return BadRequest(new AuthResult()
                {
                    Success = false,
                    Messages = new List<string>() { "Unable to create new user (Check that the username doesn't already exist, and that the password meets Identity standards)" }
                });
            }

            //// Prevent new user from being created if a default Identity role don't exist
            //if (await _roleManager.FindByNameAsync(UserRoles.User) == null)
            //{
            //    return BadRequest(new AuthResult()
            //    {
            //        Success = false,
            //        Messages = new List<string>() { "An Identity default account role doesn't exist" }
            //    });
            //}

            // Grab the new user from db
            AppUser newUserFromDb = await _userManager.FindByEmailAsync(registrationReqDto.Email);
            if (newUserFromDb == null)
            {
                return BadRequest(new AuthResult()
                {
                    Success = false,
                    Messages = new List<string>() { "Failed to return newly created user from database" }
                });
            }
            //else
            //{
            //    // Add user to a default role after user has been created
            //    await _userManager.AddToRoleAsync(newUserFromDb, UserRoles.User);
            //}

            // Give token to user (to be stored in browser local storage client-side)
            AuthResult jwtTokenResult = await GenerateJwtTokenAsync(newUserFromDb);

            // Prepare to send data back to the client to indicate user has logged in
            //AppUserLoggedInDto loggedInUser = _mapper.Map<AppUserLoggedInDto>(newUserFromDb);
            LoginResDto loginResDto = new LoginResDto()
            {
                Id = newUserFromDb.Id,
                UserName = newUserFromDb.UserName,
                Email = newUserFromDb.Email,
                DateCreated = newUserFromDb.DateCreated,
                DateCreatedStr = newUserFromDb.DateCreated.ToString(DateFormat.Month_Day_Year),
                Token = jwtTokenResult.Token,
                RefreshToken = jwtTokenResult.RefreshToken
            };

            // Then map the token data
            //_mapper.Map<AuthResult, AppUserLoggedInDto>(jwtTokenResult, loggedInUser);

            return Ok(loginResDto);
        }

        [HttpPost("Login")]
        public async Task<ActionResult> Login(LoginReqDto loginReqDto)
        {
            if (!ModelState.IsValid)
                return BadRequest();

            // Check if user exists
            AppUser existingUser = await _userManager.FindByEmailAsync(loginReqDto.Email);
            if (existingUser == null)
            {
                return BadRequest(new AuthResult()
                {
                    Success = false,
                    Messages = new List<string>() { "Email doesn't exist" }
                });
            }

            // Verify password
            var passwordIsCorrect = await _userManager.CheckPasswordAsync(existingUser, loginReqDto.Password);
            if (!passwordIsCorrect)
            {
                return BadRequest(new AuthResult()
                {
                    Success = false,
                    Messages = new List<string>() { "Invalid credentials" }
                });
            }

            // Give token to user (to be stored in browser local storage client-side)
            AuthResult jwtTokenResult = await GenerateJwtTokenAsync(existingUser);

            // Map user details
            //AppUserLoggedInDto loggedInUser = _mapper.Map<AppUserLoggedInDto>(existingUser);
            LoginResDto loginResDto = new LoginResDto()
            {
                Id = existingUser.Id,
                UserName = existingUser.UserName,
                Email = existingUser.Email,
                DateCreated = existingUser.DateCreated,
                DateCreatedStr = existingUser.DateCreated.ToString(DateFormat.Month_Day_Year),
                Token = jwtTokenResult.Token,
                RefreshToken = jwtTokenResult.RefreshToken
            };

            //// Then map the token data
            //_mapper.Map<AuthResult, AppUserLoggedInDto>(jwtTokenResult, loggedInUser);

            return Ok(loginResDto);
        }

        [HttpPost("UpdateUser")]
        public async Task<ActionResult> UpdateUser(AppUserUpdateDto updatedUserDto)
        {
            if (!ModelState.IsValid)
                return BadRequest();

            // Reference user
            var existingUser = await _userManager.FindByIdAsync(updatedUserDto.Id);
            if (existingUser == null)
            {
                return BadRequest(new AuthResult()
                {
                    Success = false,
                    Messages = new List<string>() { "User doesn't exist" }
                });
            }

            #region DEMO: Prevent demo Admin user from changing/deleting users
            //if (existingUser.Email == "admin@example.com")
            //{
            //    return BadRequest(new AuthResult()
            //    {
            //        Success = false,
            //        Messages = new List<string>() { "Demo Admin not allowed to edit user data" }
            //    });
            //}
            #endregion

            // Overwrite values
            existingUser.UserName = updatedUserDto.UserName;
            existingUser.Email = updatedUserDto.Email;

            // Save updated values to db
            await _userManager.UpdateAsync(existingUser);

            // Prepare to send updated user data back to client via DTO
            var userDto = new AppUserDto()
            {
                Id = existingUser.Id,
                UserName = existingUser.UserName,
                Email = existingUser.Email,
                DateCreated = existingUser.DateCreated,
                DateCreatedStr = existingUser.DateCreated.ToString()
            };

            return Ok(userDto);
        }


        [HttpDelete("DeleteUser/{userId}")]
        public async Task<ActionResult> DeleteUser(string userId)
        {
            // Reference user by id
            var existingUser = await _userManager.FindByIdAsync(userId);
            if (existingUser == null)
            {
                return BadRequest(new AuthResult()
                {
                    Success = false,
                    Messages = new List<string>() { "User doesn't exist" }
                });
            }

            #region DEMO: Prevent demo Admin user from changing/deleting users
            //if (existingUser.Email == "admin@example.com")
            //{
            //    return BadRequest(new AuthResult()
            //    {
            //        Success = false,
            //        Messages = new List<string>() { "Demo Admin not allowed to edit user data" }
            //    });
            //}
            #endregion

            // Delete user from db
            await _userManager.DeleteAsync(existingUser);

            return Ok("User successfully deleted");
        }

        [HttpPost("RefreshToken")]
        public async Task<ActionResult> RefreshToken([FromBody] TokenReqDto tokenReq)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new AuthResult()
                {
                    Success = false,
                    Messages = new List<string>() { "Invalid payload" }
                });
            }

            AuthResult? result = await VerifyAndGenerateToken(tokenReq);
            if (result == null) // In case something goes wrong in the middle of the process
            {
                return BadRequest(new AuthResult()
                {
                    Success = false,
                    Messages = new List<string>() { "Invalid token" }
                });
            }
            return Ok(result);
        }



        #region Jwt Token logic
        private async Task<AuthResult> GenerateJwtTokenAsync(AppUser user)
        {

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_appSettings.GetSection("JwtConfig:Secret").Value));
            var currentDate = DateTime.UtcNow;
            var claims = await GetAllValidClaimsAsync(user);

            // Define the token
            var tokenDescriptor = new SecurityTokenDescriptor()
            {
                Subject = new ClaimsIdentity(claims),
                Expires = currentDate.AddDays(1), // Expires = currentDate.AddSeconds(10), // Temp: For refresh token demo purposes
                NotBefore = currentDate,
                SigningCredentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256Signature)
            };

            var jwtTokenHandler = new JwtSecurityTokenHandler();

            // Create the token
            SecurityToken token = jwtTokenHandler.CreateToken(tokenDescriptor);
            string jwtToken = jwtTokenHandler.WriteToken(token);

            var refreshToken = new RefreshToken()
            {
                JwtId = token.Id,
                IsUsed = false,
                IsRevoked = false,
                UserId = user.Id,
                AddedDate = currentDate,
                ExpiryDate = currentDate.AddMonths(6),
                Token = Guid.NewGuid().ToString() + Guid.NewGuid(),
                //Token = RandomString(35) + Guid.NewGuid(),
            };

            await _context.RefreshTokens.AddAsync(refreshToken); // Add changes to memory
            await _context.SaveChangesAsync(); // Save changes to db

            return new AuthResult()
            {
                Token = jwtToken,
                RefreshToken = refreshToken.Token,
                Success = true,
            };
        }

        private async Task<List<Claim>> GetAllValidClaimsAsync(AppUser user)
        {
            var claims = new List<Claim>
        {
            new Claim("Id", user.Id), // There is no ClaimTypes.Id
            new Claim(ClaimTypes.NameIdentifier, user.UserName),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()), // For "JwtId"
            // new Claim(JwtRegisteredClaimNames.Iat, DateTime.UtcNow.ToUniversalTime().ToString())
            // (The role claim will be added here)
        };

            // Getting the claims that we have assigned to the user
            var userClaims = await _userManager.GetClaimsAsync(user);
            claims.AddRange(userClaims);

            // Get the user role, convert it, and add it to the claims
            var userRoles = await _userManager.GetRolesAsync(user);
            foreach (var userRole in userRoles)
            {
                var role = await _roleManager.FindByNameAsync(userRole);

                if (role != null)
                {
                    claims.Add(new Claim(ClaimTypes.Role, userRole));

                    var roleClaims = await _roleManager.GetClaimsAsync(role);
                    foreach (var roleClaim in roleClaims)
                    { claims.Add(roleClaim); }
                }
            }

            return claims;
        }

        //Used in RefreshToken() POST
        private async Task<AuthResult> VerifyAndGenerateToken(TokenReqDto tokenReq)
        {
            var jwtTokenHandler = new JwtSecurityTokenHandler();

            try // Run the token request through validations
            {
                // Check that the string is actually in jwt token format
                // (The token validation parameters were defined in the Program.cs class)
                var tokenInVerification = jwtTokenHandler.ValidateToken(tokenReq.Token,
                    _tokenValidationParams, out var validatedToken);

                // Check if the encryption algorithm matches
                if (validatedToken is JwtSecurityToken jwtSecurityToken)
                {
                    bool result = jwtSecurityToken.Header.Alg
                        .Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCulture);

                    if (result == false)
                        return null;
                }

                // Check if token has expired (don't generate new token if current token is still usable)
                // "long" was used because of the long utc time string
                var utcExpiryDate = long.Parse(tokenInVerification.Claims
                    .FirstOrDefault(x => x.Type == JwtRegisteredClaimNames.Exp).Value);

                // Convert into a usable date type
                var expiryDate = UnixTimeStampToDateTime(utcExpiryDate);

                if (expiryDate > DateTime.UtcNow)
                {
                    return new AuthResult()
                    {
                        Success = false,
                        Messages = new List<string>() { "Token has not yet expired" }
                    };
                }

                // Check if token exists in db
                var storedToken = await _context.RefreshTokens
                    .FirstOrDefaultAsync(x => x.Token == tokenReq.RefreshToken);

                if (storedToken == null)
                {
                    return new AuthResult()
                    {
                        Success = false,
                        Messages = new List<string>() { "Token does not exist" }
                    };
                }

                // Check if token is already used
                if (storedToken.IsUsed)
                {
                    return new AuthResult()
                    {
                        Success = false,
                        Messages = new List<string>() { "Token has been used" }
                    };
                }

                // Check if token has been revoked
                if (storedToken.IsRevoked)
                {
                    return new AuthResult()
                    {
                        Success = false,
                        Messages = new List<string>() { "Token has been revoked" }
                    };
                }

                // Check if jti matches the id of the refresh token that exists in our db (validate the id)
                var jti = tokenInVerification.Claims
                    .FirstOrDefault(x => x.Type == JwtRegisteredClaimNames.Jti).Value;

                if (storedToken.JwtId != jti)
                {
                    return new AuthResult()
                    {
                        Success = false,
                        Messages = new List<string>() { "Token does not match" }
                    };
                }

                // First, update current token
                storedToken.IsUsed = true; // Prevent the current token from being used in the future
                _context.RefreshTokens.Update(storedToken);
                await _context.SaveChangesAsync(); // Save changes

                // Then, generate a new jwt token, then assign it to the user
                var dbUser = await _userManager.FindByIdAsync(storedToken.UserId); // Find the AppUser by the user id on the current token
                return await GenerateJwtTokenAsync(dbUser);
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        private DateTime UnixTimeStampToDateTime(long unixTimeStamp)
        {
            var dateTimeVal = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            dateTimeVal = dateTimeVal.AddSeconds(unixTimeStamp).ToUniversalTime();
            return dateTimeVal;
        }

        // NOTE: Maybe delete this and generate a new GUID instead
        private string RandomString(int length)
        {
            var random = new Random();
            var chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length)
                .Select(x => x[random.Next(x.Length)]).ToArray());
        }

        #endregion
    }
}
