using Contracts.Enums;
using Contracts.Requests.ContentRequests;

namespace UniNet.Models.ContentModels
{
    /// <summary>
    /// نموذج multipart — يسكن في UniNet وحده لأنه يحمل IFormFile.
    /// هذا هو الحدّ الذي لا يعبره نوع ASP.NET إلى Contracts: مشروع Contracts بلا أي
    /// مرجع مشروعٍ أو إطار ويب، فوضع IFormFile في AddContentDTO كان سيجرّ ASP.NET
    /// إلى Domain نفسه عبر سلسلة المراجع.
    /// </summary>
    public class AddContentForm
    {
        public string Title { get; set; } = null!;

        public string Body { get; set; } = null!;

        public EnContentScope Scope { get; set; }

        public int? TargetId { get; set; }

        public IFormFile? Image { get; set; }

        public AddContentDTO ToDTO() => new AddContentDTO
        {
            Title = Title,
            Body = Body,
            Scope = Scope,
            TargetId = TargetId,
        };
    }
}
