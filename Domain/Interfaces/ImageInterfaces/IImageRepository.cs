using Contracts.Responses.ImageResponses;
using Domain.Entities.Images;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Interfaces.ImageInterfaces
{
    /// <summary>
    /// عمليتا قراءة فقط — وهذا مقصود:
    /// • لا Add: الصورة تُربَط بإسناد ContentItem.Image فيتولّى EF الإدراج والمفتاح الأجنبي معًا.
    /// • لا Delete: العلاقة Cascade، فحذف المحتوى يحذف صف الصورة في نفس الأمر.
    /// إضافة الطريقتين "للاكتمال" كانت ستُنتج كودًا ميتًا لا يستدعيه أحد.
    /// </summary>
    public interface IImageRepository
    {
        Task<ImageFileDTO?> GetFileInfoByContentItemIdAsync(int contentItemId);

        Task<Image?> GetByContentItemIdAsync(int contentItemId);
    }
}
