using Contracts.Requests.ImageRequests;

namespace UniNet.Extensions
{
    public static class FormFileExtensions
    {
        public static UploadedFileDTO? ToUploadedFile(this IFormFile? file) => file is null ? null : new UploadedFileDTO
        {
            FileName = file.FileName,
            ContentType = file.ContentType,
            Length = file.Length,
            Content = file.OpenReadStream(),
        };
    }
}
