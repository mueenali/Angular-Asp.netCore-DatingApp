using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using DatingApp.API.Data;
using DatingApp.API.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
namespace DatingApp.API.Controllers

{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IRepository _repo;
        private readonly IMapper _mapper;

        public UsersController(IRepository repo, IMapper mapper)
        {
            this._repo = repo;
            this._mapper = mapper;
        }

        [HttpGet]

        public async Task<IActionResult> GetUser()
        {
            var users = await _repo.GetUsers();
            var returnedUsers = _mapper.Map<IEnumerable<UserListDto>>(users);
            return Ok(returnedUsers);
        }

        [HttpGet("{id}")]

        public async Task<IActionResult> GetUser(int id)
        {
            var user = await _repo.GetUser(id);
            var returnedUser = _mapper.Map<UserDetailsDto>(user);
            return Ok(returnedUser);
        }
    }
}