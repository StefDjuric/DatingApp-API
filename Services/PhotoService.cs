﻿using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using DatingApp_API.Helpers;
using DatingApp_API.Interfaces;
using Microsoft.Extensions.Options;

namespace DatingApp_API.Services
{
    public class PhotoService : IPhotoService
    {
        private readonly Cloudinary _cloudinary;
        public PhotoService(IOptions<CloudinarySettings> opt) 
        {
            var acc = new Account(opt.Value.CloudName, opt.Value.ApiKey, opt.Value.ApiSecret);

            _cloudinary = new Cloudinary(acc);
        }
        public async Task<ImageUploadResult> AddPhotoAsync(IFormFile file)
        {
            var uploadResult = new ImageUploadResult();

            if (file.Length > 0)
            {
                using var stream = file.OpenReadStream();
                var uploadParams = new ImageUploadParams
                {
                    File = new FileDescription(file.FileName, stream),
                    Transformation = new Transformation()
                    .Height(500).Width(500).Crop("fill").Gravity("face"),
                    Folder = "DatingApp"
                };

                uploadResult = await _cloudinary.UploadAsync(uploadParams);

            }
                return uploadResult;
        }

        public async Task<DeletionResult> DeletePhotoAsync(string publicId)
        {
            var deletionParams = new DeletionParams(publicId);

            return await _cloudinary.DestroyAsync(deletionParams);
        }
    }
}
