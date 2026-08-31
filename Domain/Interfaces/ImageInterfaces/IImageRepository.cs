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
    /// لا Add: الصورة تُربَط بإسناد ContentItem.Image فيتولّى EF الإدراج والمفتاح الأجنبي معًا.
    ///
    /// Delete موجود — والتعليل السابق ("العلاقة Cascade فلا حاجة له") كان صحيحًا لحذف المحتوى
    /// وخاطئًا لاستبدال الصورة أو إزالتها، حيث يبقى ContentItem حيًّا ويجب أن يزول صف الصورة وحده.
    /// وبدونه يصطدم إدراج الصورة الجديدة بالفهرس الفريد IX_Images_ContentItemId.
    /// </summary>
    public interface IImageRepository
    {
        Task<ImageFileDTO?> GetFileInfoByContentItemIdAsync(int contentItemId);

        Task<Image?> GetByContentItemIdAsync(int contentItemId);

        bool Delete(Image image);
    }
}
