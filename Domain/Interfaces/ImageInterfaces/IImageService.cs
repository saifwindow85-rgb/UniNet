using Contracts.Requests.ImageRequests;
using Contracts.Responses;
using Contracts.Responses.ImageResponses;
using Domain.Entities.Images;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Interfaces.ImageInterfaces
{
    public interface IImageService
    {
        /// <summary>
        /// يتحقق من الملف ثم يكتبه على القرص ويُرجع كيان Image **غير محفوظ** جاهزًا لإسناده
        /// إلى ContentItem.Image. لا يستدعي CompleteAsync إطلاقًا: المُنسِّق (PostService) هو من يحفظ،
        /// فيُدرَج المحتوى وصورته في SaveChanges واحد — أي ذرّيًا وبلا معاملة صريحة.
        /// </summary>
        Task<AddUpdateServiceResponse<Image>> PrepareAsync(UploadedFileDTO file, int currentUserId);

        /// <summary>
        /// يحذف الملف الفيزيائي لصورة. يُستدعى في حالتين:
        ///   (1) تعويض: فشلت قاعدة البيانات بعد نجاح كتابة الملف ⇒ الملف صار يتيمًا.
        ///   (2) تنظيف: نجح حذف المحتوى ⇒ Cascade أزال الصف والملف باقٍ على القرص.
        /// آمن للاستدعاء على ملف غير موجود.
        /// </summary>
        void DeletePhysicalFile(Image image);

        /// <summary>
        /// نفس العملية انطلاقًا من المسار النسبي وحده.
        /// يحتاجها مسارا الاستبدال والحذف: كلاهما يلتقط المسار قبل أن يُعلَّم الصف محذوفًا،
        /// فالتعويض من نصٍّ ملتقَط أسلم من الإمساك بكيان قد يكون EF أزاله من المتتبِّع.
        /// </summary>
        void DeletePhysicalFile(string relativePath);

        Task<ImageFileDTO?> GetFileInfoByContentItemIdAsync(int contentItemId);

        /// <summary>يحوّل المسار النسبي المخزَّن إلى مسار مطلق — يحتاجه PhysicalFile في الـ Controller.</summary>
        string GetAbsolutePath(string relativePath);
    }
}
