
using System.Threading.Tasks;
using AutoMapper;
using DatingApp.API.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using CloudinaryDotNet;
using DatingApp.API.Dtos;
using System.Security.Claims;
using DatingApp.API.Data.RepositoryInterfaces;
using CloudinaryDotNet.Actions;
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
        private Cloudinary _cloudinary;

        private readonly IOptions<CloudinarySettings> _cloudinaryConfig;
        public PhotosController(IUnitOfWork unitOfWork, IMapper mapper, IOptions<CloudinarySettings> cloudinaryConfig)
        {
            _cloudinaryConfig = cloudinaryConfig;
            _mapper = mapper;
            _unitOfWork = unitOfWork;

            Account account = new Account(
                _cloudinaryConfig.Value.CloudName,
                _cloudinaryConfig.Value.ApiKey,
                _cloudinaryConfig.Value.ApiSecret
            );
            _cloudinary = new Cloudinary(account);
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
            var uploadedResult = new ImageUploadResult();
            if (file.Length > 0)
            {
                using (var stream = file.OpenReadStream())
                {
                    var uploadParams = new ImageUploadParams
                    {
                        File = new FileDescription(file.Name, stream),
                        Transformation = new Transformation().Width(500).Height(500).Crop("fill").Gravity("face")
                    };

                    uploadedResult = _cloudinary.Upload(uploadParams);
                }
            }
            photoCreateDto.Url = uploadedResult.Uri.ToString();
            photoCreateDto.PublicId = uploadedResult.PublicId;
            var photo = _mapper.Map<Photo>(photoCreateDto);
            _unitOfWork.photoRepository.AddPhoto(user, photo);

            if (await _unitOfWork.Commit())
            {
                var photoToReturn = _mapper.Map<PhotoToReturnDto>(photo);
                return CreatedAtRoute("GetPhoto", new { id = photo.Id }, photoToReturn);
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
            if (photo.PublicId != null)
            {
                var deleteParams = new DeletionParams(photo.PublicId);
                var deleteResult = _cloudinary.Destroy(deleteParams);

                if (deleteResult.Result == "ok")
                    _unitOfWork.photoRepository.Delete(photo);

            }

            if (await _unitOfWork.Commit())
                return Ok();

            return BadRequest("Failed to delete the photo");
        }
    }
}