using GlorriJob.Common.Shared;


namespace GlorriJob.Application.Abstractions.Services
{
	public interface IImageKitService
	{
		Task<string?> AddImageAsync(string imagePath, string imageName);
		Task<string> UpdateImageAsync(string imagePath, string imageName);
		Task<bool> DeleteImageAsync(string imageId);
		Task<string?> GetImageId(string imagePath);
	}
}
