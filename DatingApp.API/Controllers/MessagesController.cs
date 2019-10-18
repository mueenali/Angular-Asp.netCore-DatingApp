
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using System.Security.Claims;
using DatingApp.API.Data.RepositoryInterfaces;
using DatingApp.API.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using DatingApp.API.Models;
using DatingApp.API.Dtos;
using System;

namespace DatingApp.API.Controllers
{
    [ServiceFilter(typeof(LogUserActivity))]
    [Authorize]
    [Route("api/users/{userId}/[controller]")]
    [ApiController]
    public class MessagesController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        public MessagesController(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;

        }

        [HttpGet("{id}", Name = "GetMessage")]
        public async Task<IActionResult> GetMessage(int userId, int id)
        {

            if (userId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
                return Unauthorized();

            var message = await _unitOfWork.messageRepository.GetEntity(id);

            if (message == null)
                return NotFound();

            return Ok(message);
        }


        [HttpGet]
        public async Task<IActionResult> GetUserMessages(int userId, [FromQuery]MessageParams messageParams)
        {
            if (userId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
                return Unauthorized();

            messageParams.UserId = userId;
            var messages = await _unitOfWork.messageRepository.GetUserMessages(messageParams);
            var messagesToReturn = _mapper.Map<IEnumerable<MessageDetailsDto>>(messages);
            Response.AddPagination(messages.CurrentPage, messages.PageSize, messages.TotalItems, messages.TotalPages);

            return Ok(messagesToReturn);
        }

        [HttpPost]
        public async Task<IActionResult> CreateMessage(int userId, MessageCreateDto messageCreateDto)
        {
            var sender = await _unitOfWork.userRepository.GetEntityWithInclude(u => u.Photos, u => u.ID == userId);
            if (sender.ID != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
                return Unauthorized();

            messageCreateDto.SenderId = userId;
            var receiver = await _unitOfWork.userRepository.GetEntityWithInclude(u => u.Photos, u => u.ID == messageCreateDto.ReceiverId);

            if (receiver == null)
                return BadRequest("Could not find receiver");

            var message = _mapper.Map<Message>(messageCreateDto);
            _unitOfWork.messageRepository.Add(message);

            if (await _unitOfWork.Commit())
            {
                var messageToReturn = _mapper.Map<MessageDetailsDto>(message);
                return CreatedAtRoute("GetMessage", new { id = message.Id }, messageToReturn);
            }

            throw new Exception("Creating new message failed on save");
        }

        [HttpPost("{id}/read")]
        public async Task<IActionResult> MarkMessageAsRead(int userId, int id)
        {
            if (userId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
                return Unauthorized();

            var message = await _unitOfWork.messageRepository.GetEntity(id);
            if (message.ReceiverId != userId)
                return Unauthorized();

            message.IsRead = true;
            message.DateRead = DateTime.Now;

            if (await _unitOfWork.Commit())
                return NoContent();

            throw new Exception("Marking the message as read failed on update");
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteMessage(int userId, int id)
        {
            if (userId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
                return Unauthorized();

            var message = await _unitOfWork.messageRepository.GetEntity(id);
            if (message.SenderId == userId)
                message.SenderDeleted = true;

            if (message.ReceiverId == userId)
                message.ReceiverDeleted = true;

            if (message.SenderDeleted && message.ReceiverDeleted)
                _unitOfWork.messageRepository.Delete(message);


            if (await _unitOfWork.Commit())
                return NoContent();

            throw new Exception("Message failed on delete");

        }

        [HttpGet("thread/{receiverId}")]
        public async Task<IActionResult> GetMessageThread(int userId, int receiverId)
        {
            if (userId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
                return Unauthorized();

            var messages = await _unitOfWork.messageRepository.GetMessageThread(userId, receiverId);
            if (messages == null)
                return BadRequest("Could not find message thread");


            var messagesThread = _mapper.Map<IEnumerable<MessageDetailsDto>>(messages);

            return Ok(messagesThread);
        }



    }
}