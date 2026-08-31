using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contracts.Enums
{
    // نُقلا من Domain.Entities.Enums إلى Contracts لأن طلبات المحتوى واستجاباته تحتاجهما،
    // و Contracts هي المكتبة الوحيدة التي تراها كل الطبقات. نفس سابقة UserType.
    public enum EnContentScope { Public = 1, Batch = 2, Department = 3, College = 4, University = 5 }

    public enum EnContentType { Post = 1, Announcement = 2 }
}
