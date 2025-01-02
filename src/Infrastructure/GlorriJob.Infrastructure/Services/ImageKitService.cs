using GlorriJob.Application.Abstractions.Services;
using GlorriJob.Common.Shared;
using Imagekit.Sdk;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace GlorriJob.Infrastructure.Services
{
	public class ImageKitService : IImageKitService
	{
		private ImagekitClient _imagekit {  get; }
        public ImageKitService(IConfiguration configuration)
        {
			var publicKey = configuration["ImageKitSettings:PublicKey"];
			var privateKey = configuration["ImageKitSettings:PrivateKey"];
			var urlEndpoint = configuration["ImageKitSettings:UrlEndpoint"];
			_imagekit = new ImagekitClient(publicKey, privateKey, urlEndpoint);
		}

		public async Task<BaseResponse<string>> AddImageAsync(string imagePath, string imageName)
		{
			if(!File.Exists(imagePath))
			{
				return new BaseResponse<string>
				{
					StatusCode = HttpStatusCode.BadRequest,
					Message = "Image file not found.",
					Data = null
				};
			}
			byte[] imageBytes = File.ReadAllBytes(imagePath);
			string base64Image = Convert.ToBase64String(imageBytes);
			FileCreateRequest fileCreateRequest = new()
			{
				file = base64Image,
				fileName = imageName,
				useUniqueFileName = true
			};
			var uploadResult = await _imagekit.UploadAsync(fileCreateRequest);
			if(uploadResult.HttpStatusCode != 200)
			{
				return new BaseResponse<string>
				{
					StatusCode = HttpStatusCode.BadRequest,
					Message = $"Error occurred while uploading image.",
					Data = null
				};
			}
			return new BaseResponse<string>
			{
				StatusCode = HttpStatusCode.OK,
				Message = $"Image is successfully uploaded.",
				Data = uploadResult.url
			};
		}

		public Task<string> UpdateImageAsync(string imagePath, string imageName)
		{
			throw new NotImplementedException();
		}
	}
}
