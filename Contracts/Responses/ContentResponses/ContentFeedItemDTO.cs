using Contracts.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contracts.Responses.ContentResponses
{
    /// <summary>
    /// صفّ الخلاصة — أخفّ استجابة في المشروع لأنه أسخن استعلام فيه.
    /// بلا DTOsBaseEntity عمدًا: حقول التدقيق الستة ليست بيانات قارئ.
    /// وبلا معرّفات النطاق: من يرى منشورًا لا يحتاج أن يعرف الدفعة التي استُهدفت به.
    /// </summary>
    public class ContentFeedItemDTO
    {
        public int ContentItemId { get; set; }

        public string Title { get; set; } = null!;

        public string Body { get; set; } = null!;

        public EnContentType Type { get; set; }

        public string AuthorName { get; set; } = null!;

        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// علم لا رابط ولا ImageId: الرابط معروف اشتقاقًا من معرّف المحتوى،
        /// و ImageId تفصيلة تخزين لا يحتاجها العميل. يُسقَط في المستودع كـ c.Image != null
        /// فيكلّف EXISTS واحدًا لا رحلة ثانية.
        /// </summary>
        public bool HasImage { get; set; }
    }
}
