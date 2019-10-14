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

        public async Task<IActionResult> GetUsers([FromQuery]UserParams userParams)
        {
            var currentUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            var user = await _unitOfWork.userRepository.GetEntity(currentUserId);

            userParams.UserId = currentUserId;
            if (string.IsNullOrEmpty(userParams.Gender))
                userParams.Gender = user.Gender == "male" ? "female" : "male";


            var users = await _unitOfWork.userRepository.PaginateWithFilter(userParams);
            var returnedUsers = _mapper.Map<IEnumerable<UserListDto>>(users);
            Response.AddPagination(users.CurrentPage, users.PageSize, users.TotalItems, users.TotalPages);
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