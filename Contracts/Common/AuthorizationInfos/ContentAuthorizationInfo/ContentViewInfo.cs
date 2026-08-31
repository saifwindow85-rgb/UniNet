using Contracts.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contracts.Common.AuthorizationInfos.ContentAuthorizationInfo
{
    // الجمهور المستهدف بعنصر محتوى: المستوى + الكيان عند ذلك المستوى.
    // منفصل عن GeneralAuthorizationInfo عمدًا لأنه يجيب سؤالًا مختلفًا (انظر ContentScopeExtension).
    public class ContentViewInfo
    {
        public EnContentScope Scope { get; set; }
        public int? UniversityId { get; set; }
        public int? CollegeId { get; set; }
        public int? DepartmentId { get; set; }
        public int? BatchId { get; set; }
    }
}
