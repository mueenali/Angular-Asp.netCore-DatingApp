
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using DatingApp.API.Dtos;
using System.Security.Claims;
using DatingApp.API.Data.RepositoryInterfaces;
using DatingApp.API.Models;

namespace DatingApp.API.Controllers
{
    [Authorize]
    [Route("api/users/{userId}/photos")]
    [ApiController]
    public class PhotosController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;


        public PhotosController(IUnitOfWork unitOfWork, IMapper mapper)
        {

            _mapper = mapper;
            _unitOfWork = unitOfWork;

        }

        [HttpGet("{id}", Name = "GetPhoto")]
        public async Task<IActionResult> GetPhoto(int id)
        {
            var photo = await _unitOfWork.photoRepository.GetEntity(id);
            var photoToReturn = _mapper.Map<PhotoToReturnDto>(photo);
            return Ok(photoToReturn);
        }

        [HttpPost]
        public async Task<IActionResult> AddPhoto(int userId, [FromForm]PhotoCreateDto photoCreateDto)
        {
            if (userId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
                return Unauthorized();

            var user = await _unitOfWork.userRepository.GetEntityWithInclude(u => u.Photos, u => u.ID == userId);
            var file = photoCreateDto.File;

            if (file != null)
            {
                var blob = await _unitOfWork.storageService.UploadImage(file);
                photoCreateDto.Url = blob.Uri.ToString();
                photoCreateDto.PublicId = blob.Name.ToString();
                var photo = _mapper.Map<Photo>(photoCreateDto);
                _unitOfWork.photoRepository.AddPhoto(user, photo);

                if (await _unitOfWork.Commit())
                {
                    var photoToReturn = _mapper.Map<PhotoToReturnDto>(photo);
                    return CreatedAtRoute("GetPhoto", new { id = photo.Id }, photoToReturn);
                }
            }

            return BadRequest("Failed to add the photo");
        }

        [HttpPost("{id}/setMain")]
        public async Task<IActionResult> SetPhotoToMain(int userId, int id)
        {
            if (userId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
                return Unauthorized();

            var user = await _unitOfWork.userRepository.GetEntityWithInclude(u => u.Photos, u => u.ID == userId);
            var result = await _unitOfWork.photoRepository.ValidatePhoto(user, id);
            if (result == "Unauthorized")
                return Unauthorized();

            else if (result == "bad request")
                return BadRequest("The photo is already the main photo");


            var photo = await _unitOfWork.photoRepository.GetEntity(id);
            var mainPhoto = await _unitOfWork.photoRepository.FindMainPhoto(user.ID);
            mainPhoto.IsMain = false;
            photo.IsMain = true;

            if (await _unitOfWork.Commit())
                return NoContent();

            return BadRequest("Could not set the photo to main");
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePhoto(int userId, int id)
        {
            if (userId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
                return Unauthorized();

            var user = await _unitOfWork.userRepository.GetEntityWithInclude(u => u.Photos, u => u.ID == userId);
            var result = await _unitOfWork.photoRepository.ValidatePhoto(user, id);

            if (result == "Unauthorized")
                return Unauthorized();

            else if (result == "bad request")
                return BadRequest("You cannot delete your main photo");


            var photo = await _unitOfWork.photoRepository.GetEntity(id);
            var deleted = await _unitOfWork.storageService.DeleteImage(photo.PublicId);

            if (deleted)
            {
                _unitOfWork.photoRepository.Delete(photo);
                if (await _unitOfWork.Commit())
                    return Ok();
            }

            return BadRequest("Failed to delete the photo");
        }
    }
}