using Contracts.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contracts.Requests.ContentRequests
{
    /// <summary>
    /// شكل واحد مشترك للمنشور والإعلان: Post و Announcement جسمان فارغان يرثان ContentItem
    /// بلا حقل واحد مختلف، فنوعان متطابقان من الـ DTO كانا سيتطلبان مُتحقِّقَين ومسارَي خدمة متطابقين.
    /// النوع (EnContentType) يمرَّر كوسيط للخدمة من مسار الـ Controller لا كحقل يرسله العميل —
    /// وإلا صار بإمكان العميل نشر إعلان عبر نقطة المنشورات متجاوزًا صلاحياتها.
    /// </summary>
    public class AddContentDTO
    {
        public string Title { get; set; } = null!;

        public string Body { get; set; } = null!;

        public EnContentScope Scope { get; set; }

        /// <summary>
        /// معرّف الكيان المستهدف عند مستوى Scope وحده: جامعة أو كلية أو قسم أو دفعة.
        /// معرّف واحد لا أربعة: أعمدة ContentItem الأربعة تحمل سلسلة الأجداد كاملة،
        /// لكن قيد CK_ContentItems_ScopeTargets يفحص NULL-ness فقط ولا يستطيع إثبات أن
        /// الكلية المُرسَلة تتبع الجامعة المُرسَلة. فالخادم يستنتج السلسلة ولا يثق بأربعة مُدخلات.
        /// يبقى null عند Scope = Public.
        /// </summary>
        public int? TargetId { get; set; }
    }
}
