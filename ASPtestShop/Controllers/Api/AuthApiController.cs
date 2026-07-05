using ASPtestShop.Data;
using ASPtestShop.Models.DTO.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace ASPtestShop.Controllers.Api
{
    [Route("api/auth")]
    [ApiController]
    public class AuthApiController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IConfiguration _configuration;

        //cái này là để quản lý người dùng, nó cung cấp các phương thức để tạo, xóa, cập nhật người dùng và quản lý mật khẩu, vai trò
        public AuthApiController(UserManager<ApplicationUser> userManager, IConfiguration configuration)
        {
            _userManager = userManager;
            _configuration = configuration;
        }

        // Đăng kí người dùng mới Phương thức Post
        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterDto dto)
        {
            var user = new ApplicationUser//tạo một đối tượng ApplicationUser mới với thông tin từ DTO
            {
                UserName = dto.UserName,
                Email = dto.Email,
                FullName = dto.FullName
            };
            var result = await _userManager.CreateAsync(user, dto.Password);//sử dụng UserManager để tạo người dùng mới với mật khẩu được cung cấp
            if (!result.Succeeded)
            {
                return BadRequest(result.Errors);
            }
            return Ok(new { Message = "Đăng kí thành công" });
        }

        //LOGIN
        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDto dto)
        {
            //tìm user theo username
            var user = await _userManager.FindByNameAsync(dto.UserName);
            if (user == null)
            {
                return Unauthorized(new { Message = "Tên đăng nhập hoặc mật khẩu không đúng" });
            }

            //check password
            bool checkPassword = await _userManager.CheckPasswordAsync(user, dto.Password);
            if (!checkPassword)
            {
                return Unauthorized(new { Message = "Tên đăng nhập hoặc mật khẩu không đúng" });
            }

            //tạo claims để lưu thông tin người dùng trong token
            var claims = new List<Claim> {
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(ClaimTypes.Name, user.UserName!)
                //nếu muốn lưu thêm thông tin khác như email, fullname thì có thể thêm vào đây
            };

            //Tạo Key để mã hóa token
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));

            //Tạo SigningCredentials để ký token
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            //Tạo token
            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.Now.AddDays(7),
                signingCredentials: creds);
            return Ok(new { Message = "Đăng nhập thành công", token = new JwtSecurityTokenHandler().WriteToken(token) });
        }

        //Phương thức này được bảo vệ bằng [Authorize], nghĩa là chỉ những người dùng đã xác thực mới có thể truy cập. Khi người dùng gửi yêu cầu GET đến /api/auth/profile, phương thức này sẽ lấy thông tin người dùng từ token JWT và trả về thông tin đó trong phản hồi.
        [Authorize]
        [HttpGet("profile")]
        public IActionResult Profile()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var userName = User.Identity?.Name;

            return Ok(new
            {
                message = "Token hợp lệ",
                userId,
                userName
            });
        }
    }
}