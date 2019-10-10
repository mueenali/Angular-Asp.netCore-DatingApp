using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using DatingApp.API.Data.RepositoryInterfaces;
using DatingApp.API.Dtos;
using DatingApp.API.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
namespace DatingApp.API.Controllers

{
    [ServiceFilter(typeof(LogUserActivity))]
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public UsersController(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        [HttpGet]

        public async Task<IActionResult> GetUsers()
        {
            var users = await _unitOfWork.userRepository.GetAllWithInclude(u => u.Photos);
            var returnedUsers = _mapper.Map<IEnumerable<UserListDto>>(users);
            return Ok(returnedUsers);
        }

        [HttpGet("{id}", Name = "GetUser")]

        public async Task<IActionResult> GetUser(int id)
        {
            var user = await _unitOfWork.userRepository.GetEntityWithInclude(u => u.Photos, u => u.ID == id);
            var returnedUser = _mapper.Map<UserDetailsDto>(user);
            return Ok(returnedUser);
        }

        [HttpPut("{id}")]

        public async Task<IActionResult> UpdateUser(int id, UserUpdateDto userUpdateDto)
        {
            if (id != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
                return Unauthorized();

            var user = await _unitOfWork.userRepository.GetEntity(id);
            _mapper.Map(userUpdateDto, user);
            _unitOfWork.userRepository.Update(user);
            if (await _unitOfWork.Commit())
                return NoContent();

            throw new Exception($"Updating user {id} failed on save");
        }
    }
}