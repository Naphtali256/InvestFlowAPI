using InvestFlowAPI.Data;
using InvestFlowAPI.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace InvestFlowAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly InvestFlowDbContext _context;
        private readonly PasswordHasher<User> _passwordHasher;

        public UsersController(InvestFlowDbContext context)
        {
            _context = context;
            _passwordHasher = new PasswordHasher<User>();
        }

        // GET: api/users
        [HttpGet]
        public async Task<IActionResult> GetUsers()
        {
            var users = await _context.Users
                .Select(u => new
                {
                    u.UserID,
                    u.FullName,
                    u.Email,
                    u.Phone,
                    u.AccountBalance,
                    u.CreatedAt,
                    u.IsActive
                })
                .ToListAsync();

            return Ok(users);
        }

        // GET: api/users/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetUser(int id)
        {
            var user = await _context.Users.FindAsync(id);

            if (user == null)
            {
                return NotFound(new
                {
                    message = "User not found."
                });
            }

            return Ok(new
            {
                user.UserID,
                user.FullName,
                user.Email,
                user.Phone,
                user.AccountBalance,
                user.CreatedAt,
                user.IsActive
            });
        }

        // POST: api/users
        [HttpPost]
        public async Task<IActionResult> CreateUser(User user)
        {
            if (string.IsNullOrWhiteSpace(user.FullName))
            {
                return BadRequest(new
                {
                    message = "Full name is required."
                });
            }

            if (string.IsNullOrWhiteSpace(user.Email))
            {
                return BadRequest(new
                {
                    message = "Email is required."
                });
            }

            if (string.IsNullOrWhiteSpace(user.PasswordHash))
            {
                return BadRequest(new
                {
                    message = "Password is required."
                });
            }

            var existingUser = await _context.Users
                .FirstOrDefaultAsync(u => u.Email == user.Email);

            if (existingUser != null)
            {
                return BadRequest(new
                {
                    message = "A user with this email already exists."
                });
            }

            // Store a securely hashed password
            var password = user.PasswordHash;

            user.PasswordHash = _passwordHasher.HashPassword(
                user,
                password
            );

            // Default account values
            user.AccountBalance = 0;
            user.CreatedAt = DateTime.UtcNow;
            user.IsActive = true;

            _context.Users.Add(user);

            await _context.SaveChangesAsync();

            return Ok(new
            {
                message = "User registered successfully.",
                userId = user.UserID
            });
        }

        // POST: api/users/login
        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Email))
            {
                return BadRequest(new
                {
                    message = "Email is required."
                });
            }

            if (string.IsNullOrWhiteSpace(request.Password))
            {
                return BadRequest(new
                {
                    message = "Password is required."
                });
            }

            // Find user by email
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Email == request.Email);

            if (user == null)
            {
                return Unauthorized(new
                {
                    message = "Invalid email or password."
                });
            }

            // Check whether account is active
            if (!user.IsActive)
            {
                return Unauthorized(new
                {
                    message = "This account is inactive."
                });
            }

            // Verify the password against the stored hash
            var passwordResult = _passwordHasher.VerifyHashedPassword(
                user,
                user.PasswordHash,
                request.Password
            );

            if (passwordResult == PasswordVerificationResult.Failed)
            {
                return Unauthorized(new
                {
                    message = "Invalid email or password."
                });
            }

            // Login successful
            return Ok(new
            {
                message = "Login successful.",
                user = new
                {
                    user.UserID,
                    user.FullName,
                    user.Email,
                    user.Phone,
                    user.AccountBalance,
                    user.CreatedAt,
                    user.IsActive
                }
            });
        }
    }
}