using Contracts.Requests.ContentRequests;

namespace UniNet.Models.ContentModels
{
    public class UpdateContentForm
    {
        public string Title { get; set; } = null!;

        public string Body { get; set; } = null!;

        /// <summary>
        /// الحالة الثالثة: بلا هذا العلم يبقى غياب الملف غامضًا بين إبقاء الصورة وحذفها.
        /// </summary>
        public bool RemoveImage { get; set; }

        public IFormFile? Image { get; set; }

        public UpdateContentDTO ToDTO() => new UpdateContentDTO
        {
            Title = Title,
            Body = Body,
            RemoveImage = RemoveImage,
        };
    }
}
