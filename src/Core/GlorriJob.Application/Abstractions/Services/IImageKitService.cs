using GlorriJob.Common.Shared;


namespace GlorriJob.Application.Abstractions.Services
{
	public interface IImageKitService
	{
		Task<BaseResponse<string>> AddImageAsync(string imagePath, string imageName);
		Task<string> UpdateImageAsync(string imagePath, string imageName);
		Task<BaseResponse<object>> DeleteImageAsync(string imageId);
		Task<BaseResponse<string>> GetImageId(string imagePath);
	}
}
