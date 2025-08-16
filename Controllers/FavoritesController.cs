using Hirsch.Jwt;
using Hirsch.Models;
using Hirsch.Services;
using Hirsch.Ws;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

namespace hirsch.Controllers
{
    [ApiController]
    [Route("api/users")]
    public class FavoritesController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IHubContext<FavoritesHub> _hubContext;
        private readonly IJwtHelper _jwtHelper;

        public FavoritesController(IUserService userService, IHubContext<FavoritesHub> hubContext, IJwtHelper jwtHelper)
        {
            _userService = userService;
            _hubContext = hubContext;
            _jwtHelper = jwtHelper;

        }

        [HttpPost("register")]
        public IActionResult Register([FromBody] UserRequest user)
        {
            try
            {
                _userService.RegisterUser(user);
                return Ok("Usuario registrado exitosamente.");
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("login")]
        public IActionResult Login([FromBody] UserRequest loginRequest)
        {
            try
            {
                var user = _userService.Login(loginRequest.Email, loginRequest.Password);
                var token = _jwtHelper.GenerateToken(user.Email);
                return Ok(new { Token = token });
            }
            catch
            {
                return Unauthorized("Credenciales inválidas.");
            }
        }

        [HttpGet()]
        [Authorize]
        public IActionResult GetAllUsers()
        {
            string username = User.Identity?.Name ?? "anonymous";
            List<UserResponse> users = _userService.GetAllUsers(username);
            return Ok(users);
        }

        [HttpGet("{userId}")]
        [Authorize]
        public IActionResult GetUserById(int userId)
        {
            string username = User.Identity?.Name ?? "anonymous";
            UserDetail user = _userService.GetUserById(userId, username);
            return Ok(user);
        }

        [HttpPost("{userId}/favorites")]
        [Authorize]
        public async Task<IActionResult> AddFavorite(int userId)
        {
            string username = User.Identity?.Name ?? "anonymous";
            _userService.AddFavorite(username, userId);            
                        
            await _hubContext.Clients.All.SendAsync("FavoritesUpdated", username);

            // Alternativa: Notificar solo a usuarios específicos
            // await _hubContext.Clients.Group(username).SendAsync("FavoritesUpdated", username);            

            return Ok(new
            {
                success = true,
                message = "Favorito actualizado correctamente",
                username = username
            });            
            
        }

        [HttpDelete("{userId}/favorites")]
        [Authorize]
        public async Task<IActionResult> RemoveFavorite(int userId)
        {
            var username = User.Identity.Name; // Extraer el username del token
            _userService.RemoveFavorite(username, userId);

            await _hubContext.Clients.All.SendAsync("FavoritesUpdated", username);
            return Ok();
        }
        [HttpGet("users/{userId}")]
        [Authorize]
        public IActionResult GetUserEntityById(int userId)
        {            
            User user = _userService.GetUserEntityById(userId);
            return Ok(user);
        }
    }
}