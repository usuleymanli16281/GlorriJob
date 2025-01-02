using GlorriJob.Common.Shared;


namespace GlorriJob.Application.Abstractions.Services
{
	public interface IImageKitService
	{
		public Task<BaseResponse<string>> AddImageAsync(string imagePath, string imageName);
		public Task<string> UpdateImageAsync(string imagePath, string imageName);
	}
}
