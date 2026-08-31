using Contracts.Requests.ImageRequests;
using Contracts.Responses.ImageResponses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Interfaces.ImageInterfaces
{
    /// <summary>
    /// منفذ التخزين الفيزيائي — إدخال/إخراج ملفات خالص، بلا أي معرفة بـ ContentItem أو قاعدة البيانات.
    /// هذا العزل هو ما يجعل استبدال القرص المحلي بـ Azure Blob أو S3 لاحقًا تغييرَ تسجيلٍ واحد في DI.
    /// </summary>
    public interface IImageStorage
    {
        Task<StoredFileResult> SaveAsync(UploadedFileDTO file, CancellationToken cancellationToken = default);

        void Delete(string relativePath);

        string GetAbsolutePath(string relativePath);
    }
}
