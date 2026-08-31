using Contracts.Enums;
using Contracts.Requests.ContentRequests;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Validators.ContentValidators
{
    /// <summary>
    /// شكل فقط. الوجود والنطاق والاحتواء فحوصُ خدمة لا مُتحقِّق — الفصل نفسه المتبع في المشروع.
    /// </summary>
    public class AddContentValidator : AbstractValidator<AddContentDTO>
    {
        public AddContentValidator()
        {
            RuleFor(c => c.Title)
                .NotEmpty().WithMessage("Title is required.")
                .MaximumLength(500).WithMessage("Title must not exceed 500 characters.");

            RuleFor(c => c.Body)
                .NotEmpty().WithMessage("Body is required.");

            // IsInEnum يرفض القيمة 0 — وهي بالضبط ما يربطه ASP.NET حين يُغفل العميل حقل Scope
            // في نموذج multipart. و 0 لا يحقق أي فرع من CK_ContentItems_ScopeTargets،
            // فبدون هذه القاعدة يصل الطلب إلى SQL ليعود بخطأ قيد غامض.
            RuleFor(c => c.Scope)
                .IsInEnum().WithMessage("Scope must be one of: Public, Batch, Department, College, University.");

            RuleFor(c => c.TargetId)
                .Null()
                .When(c => c.Scope == EnContentScope.Public)
                .WithMessage("TargetId must be empty for public content.");

            RuleFor(c => c.TargetId)
                .NotNull().WithMessage("TargetId is required for scoped content.")
                .GreaterThan(0).WithMessage("TargetId must be greater than 0.")
                .When(c => c.Scope != EnContentScope.Public && c.Scope != default);
        }
    }
}
