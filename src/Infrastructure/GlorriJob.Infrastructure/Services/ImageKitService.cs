using GlorriJob.Application.Abstractions.Services;
using GlorriJob.Common.Shared;
using Imagekit;
using Imagekit.Models;
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
		private ImagekitClient _imagekit { get; }
		public ImageKitService(IConfiguration configuration)
		{
			var publicKey = configuration["ImageKitSettings:PublicKey"];
			var privateKey = configuration["ImageKitSettings:PrivateKey"];
			var urlEndpoint = configuration["ImageKitSettings:UrlEndpoint"];
			_imagekit = new ImagekitClient(publicKey, privateKey, urlEndpoint);
		}

		public async Task<string?> AddImageAsync(string imagePath, string imageName)
		{
			byte[] imageBytes = File.ReadAllBytes(imagePath);
			string base64Image = Convert.ToBase64String(imageBytes);
			FileCreateRequest fileCreateRequest = new()
			{
				file = base64Image,
				fileName = imageName,
				useUniqueFileName = true
			};
			var uploadResult = await _imagekit.UploadAsync(fileCreateRequest);
			if (uploadResult.HttpStatusCode != 200)
			{
				return null;
			}
			return uploadResult.url;
		}

		public Task<string> UpdateImageAsync(string imagePath, string imageName)
		{
			throw new NotImplementedException();
		}
		public async Task<bool> DeleteImageAsync(string imageId)
		{
			var response = await _imagekit.DeleteFileAsync(imageId);
			if (response.HttpStatusCode != 204)
			{
				return false;
			}
			return true;
		}
		public async Task<string?> GetImageId(string imagePath)
		{
			var imageName = imagePath.Substring(imagePath.LastIndexOf('/') + 1);
			var fileListResponse = await _imagekit.GetFileListRequestAsync(new GetFileListRequest
			{
				Name = imageName
			});
			var file = fileListResponse.FileList.FirstOrDefault();
			if(file is null)
			{
				return null;
			}
			return file.fileId;
		}
	}
}
