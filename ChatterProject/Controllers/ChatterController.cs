using ChatterProject.Models;
using ChatterProject.Services;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace ChatterProject.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ChatterController : ControllerBase
    {
        private readonly IChatterService _chatterService;

        public ChatterController(IChatterService chatterService)
        {
            _chatterService = chatterService;
        }

        [HttpGet]
        public async Task<ActionResult<Chatter>> Get()
        {
            return await _chatterService.GetChatters();
        }
    }
}
