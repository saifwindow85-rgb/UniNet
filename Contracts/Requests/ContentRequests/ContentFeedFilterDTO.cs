using Contracts.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contracts.Requests.ContentRequests
{
    /// <summary>
    /// مرشِّح الخلاصة — متعمَّد الفقر. لا حقول نطاق فيه إطلاقًا:
    /// نطاق المُشاهِد يأتي من مطالباته لا من طلبه، وقبول UniversityId من العميل هنا
    /// كان سيعني تمكينه من تصفّح محتوى جامعة أخرى بتغيير وسيط في الرابط.
    /// </summary>
    public class ContentFeedFilterDTO
    {
        public string? Title { get; set; }

        public EnContentType? Type { get; set; }
    }
}
