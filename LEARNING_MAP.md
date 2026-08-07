# دليل التعلّم: من UniNet إلى ZadByan

> خريطة عملية لتطبيق مفاهيم ZadByan على الأجزاء غير المبنية من UniNet.
>
> **الجمهور:** أنا (Saif) — مطوّر بنى UniNet بنفسه، ويريد رفع مستواه عبر دراسة نظام ABP إنتاجي.

---

## المحتويات

| القسم | الموضوع |
|-------|---------|
| ٠ | الحالة الفعلية للمشروعين |
| ١ | القاعدة الحاكمة — ما ينتقل وما لا |
| ٢ | الخريطة العملية — سبعة مفاهيم بأمثلة |
| ٣ | ترتيب التنفيذ |
| ٤ | أهم الفروقات الجوهرية |
| ٥ | المفاهيم التي ترفع مستواي |
| ٦ | كيف أفهم ZadByan بسهولة |
| ٧ | جرد المفاهيم في المشروعين + تقييم دقة التطبيق |

---

## ٠. الحالة الفعلية للمشروعين

### UniNet — ما بُني وما لم يُبنَ

الكيانات موجودة، وطبقة الخدمات فوقها غير موجودة:

| الوحدة | الكيان | إعداد EF | مستودع | خدمة | Controller |
|--------|:------:|:--------:|:------:|:----:|:----------:|
| الهيكل الأكاديمي | ✅ | ✅ | ✅ | ✅ | ✅ |
| الهوية والمصادقة | ✅ | ✅ | ✅ | ✅ | ✅ |
| الموظفون | ✅ | ✅ | ✅ | ✅ | ✅ |
| الطلاب | ✅ | ✅ | ✅ | ✅ | ✅ |
| **Study** — `Subject`, `Semester`, `SectionSubject` | ✅ | ✅ | ❌ | ❌ | ❌ |
| **Results** — `StudentResult` | ✅ | ✅ | ❌ | ❌ | ❌ |
| **Content** — `Post`, `Announcement` | ✅ | ✅ | ❌ | ❌ | ❌ |
| **Images** — `Image` | ✅ | ✅ | ❌ | ❌ | ❌ |

**النتيجة المهمّة:** سبعة كيانات مبنية وصفر خدمات فوقها. لا يوجد كود يعتمد على تصميمها بعد — أي أن **تعديل تصميمها الآن بلا مخاطرة إطلاقاً**.

هذا عكس الوحدات المكتملة تماماً: تلك تعمل الآن، وإعادة هيكلتها مخاطرة بلا عائد يُذكر.

### ZadByan — البنية

| الطبقة | الملفات | الأسطر |
|--------|:-------:|:------:|
| Domain.Shared | 27 | 899 |
| Domain | 88 | 4,176 |
| Application.Contracts | 131 | 3,410 |
| Application | 48 | 5,753 |
| EntityFrameworkCore | 37 | 37,627\* |
| HttpApi | 5 | 272 |

\* الرقم الضخم لأن مجلد Migrations مولَّد آلياً.

---

## ١. القاعدة الحاكمة — ما ينتقل وما لا ينتقل

> **انقل الأفكار التصميمية. لا تنقل البنية التحتية التي يوفّرها ABP مجاناً.**

هذه القاعدة تحكم كل ما يأتي بعدها. الفرق عملي لا نظري:

| النوع | مثال | الكلفة عليّ | القرار |
|-------|------|--------------|--------|
| **فكرة تصميمية** | الكيان يحرس ثوابته بنفسه | أسطر معدودة | ✅ انقلها |
| **بنية تحتية** | توليد Controllers تلقائياً | إعادة بناء ABP | ❌ لا تنقلها |

**لماذا هذا مهم؟** لأن نقل كل ما أراه بلا تمييز هو **التقليد الأعمى (Cargo Cult)** — استيراد شكل الحلّ بلا سببه.

القدرة على قول "هذا لا يناسب مشروعي" هي بذاتها علامة نضج أعلى من تطبيق كل نمط أراه.

---

## ٢. الخريطة العملية — سبعة مفاهيم

```
🔴 أولوية عليا
  ①  الكيان الغني              →  StudentResult.Total
  ②  الحذف الناعم              →  StudentResult
  ③  خدمة المجال (Manager)     →  SectionSubject + Results

🟡 أولوية متوسطة — تحتاج قراراً واعياً
  ④  وراثة TPH                 →  Post / Announcement
  ⑤  التحقق من انتقال الحالة   →  Semester.IsCurrent
  ⑥  حدود التجميع              →  StudentResult (درس دقيق)

🟢 أولوية منخفضة — تحسين تدريجي
  ⑦  ثوابت مركزية              →  كل الكيانات الجديدة
```

---

### ① الكيان الغني — من حقيبة بيانات إلى حارس لنفسه

#### الفكرة في سطر

الكيان لا يسمح لنفسه بأن يصير في حالة خاطئة، بغضّ النظر عن الكود الذي يناديه.

#### المثال في ZadByan

```csharp
// TS.ZadByan.Domain/Sections/Section.cs
public class Section : AuditedAggregateRoot<int>
{
    public string Name { get; private set; }        // ← لا أحد يكتبها من الخارج

    public Section SetName(string name)             // ← الطريق الوحيد
    {
        Check.NotNullOrEmpty(name, nameof(Name));
        Check.Length(name, nameof(Name),
            ZadByanDomainConsts.Section.NameMaxLength,
            ZadByanDomainConsts.Section.NameMinLength);
        this.Name = name;
        return this;                                // ← يسمح بالتسلسل
    }
}
```

#### المثال المقابل في UniNet — الوضع الحالي

```csharp
// Domain/Entities/Academic Structure/College.cs
public class College : BaseEntity
{
    public string Name { get; set; } = null!;       // ← أي كود يكتبها مباشرة
}

// Application/Services/AcademicServices/CollegeService.cs
// المنطق هنا، خارج الكيان:
var collegeEntity = new College
{
    Name = newCollege.CollegeName,
    CreatedAt = DateTime.UtcNow,
    CreatedByUserId = currentUserId,
};
```

#### الاكتشاف المهم في StudentResult

```csharp
// Domain/Entities/Study/StudentResult.cs — الوضع الحالي
public decimal Midterm   { get; set; }          // عام
public decimal Practical { get; set; }          // عام
public decimal Final     { get; set; }          // عام
public decimal Total     { get; private set; }  // ← خاصّة!
```

**الغريزة كانت صحيحة** — أدركتُ أن `Total` قيمة محسوبة لا تُكتب من الخارج، فجعلتها `private set`.

**لكن الأمر لم يكتمل:** لا توجد أي دالة تحسبها. حالياً `Total` تبقى صفراً إلى الأبد ولا سبيل لضبطها.

#### التطبيق المقترح

```csharp
public class StudentResult : BaseEntity
{
    public decimal Midterm   { get; private set; }
    public decimal Practical { get; private set; }
    public decimal Final     { get; private set; }
    public decimal Total     { get; private set; }

    public StudentResult SetGrades(decimal midterm, decimal practical, decimal final)
    {
        if (midterm   < 0 || midterm   > GradeConstants.MidtermMax)
            throw new ArgumentOutOfRangeException(nameof(midterm));
        if (practical < 0 || practical > GradeConstants.PracticalMax)
            throw new ArgumentOutOfRangeException(nameof(practical));
        if (final     < 0 || final     > GradeConstants.FinalMax)
            throw new ArgumentOutOfRangeException(nameof(final));

        Midterm   = midterm;
        Practical = practical;
        Final     = final;
        Total     = midterm + practical + final;   // ← الثابت المحفوظ
        return this;
    }
}
```

#### لماذا هذا أقوى من FluentValidation وحده؟

ليس بسبب فحص المدى — يمكن فعله في الاثنين. السبب هو **الاتساق الداخلي**:

| السيناريو | تصميم فقير | تصميم غني |
|-----------|-----------|-----------|
| كود يعدّل `Midterm` وينسى تحديث `Total` | ⚠️ ممكن ← درجة خاطئة في شهادة طالب | ❌ مستحيل بنيوياً |
| درجات تصل من استيراد Excel بلا مرور بالخدمة | ⚠️ تفلت من كل تحقّق | ❌ تمرّ بنفس الحارس |
| اختبار وحدة يبني الكيان مباشرة | ⚠️ يفلت | ❌ يمرّ بالحارس |

> `Total` ليست مجرد حقل — هي **علاقة بين حقول** يجب أن تبقى صحيحة دائماً.
> حماية العلاقات هي جوهر الكيان الغني.

#### ملاحظة تنفيذية

`private set` يحتاج تهيئة EF ليقرأ القيمة عند التحميل. في `StudentResultConfiguration`:

```csharp
builder.Property(r => r.Total).HasPrecision(5, 2);
// EF يتعامل مع private set عبر backing field تلقائياً،
// لكن تأكّد بتشغيل استعلام فعلي بعد التطبيق
```

#### القاعدة العامة: متى private set؟

اسأل عن كل خاصية: **"هل توجد قيمة ممكنة تكسر صحّة الكيان؟"**

- **نعم** ← `private set` + دالة `SetX` تتحقّق
- **لا** ← `public set` عادية، ولا تتكلّف حماية بلا فائدة

مثال من `Section` في ZadByan:

```csharp
public string Name     { get; private set; }   // قيد طول → محمي
public string About    { get; private set; }   // قيد طول → محمي
public bool   IsActive { get; set; }           // كل قيمة صحيحة → غير محمي
public string ImageURL { get; set; }           // لا قيد حالياً → غير محمي
```

---

### ② الحذف الناعم — الدرجات لا تختفي بلا أثر

#### الفكرة في سطر

بعض السجلات لا يجوز أن تُمحى فعلياً من قاعدة البيانات — تُعلَّم كمحذوفة وتبقى للتدقيق.

#### المثال في ZadByan — قرار مقصود لا صدفة

```csharp
// Section: يُحذف فعلياً — لا بأس
public class Section : AuditedAggregateRoot<int>

// PaymentTransaction: لا يُحذف أبداً — سجل مالي
public class PaymentTransaction : FullAuditedAggregateRoot<Guid>
```

الفرق في الأعمدة الناتجة عن الترحيل:

| العمود | `AuditedAggregateRoot` | `FullAuditedAggregateRoot` |
|--------|:----------------------:|:--------------------------:|
| `CreationTime`, `CreatorId` | ✅ | ✅ |
| `LastModificationTime`, `LastModifierId` | ✅ | ✅ |
| `IsDeleted`, `DeleterId`, `DeletionTime` | ❌ | ✅ |

#### الوضع الحالي في UniNet — ثلاث مشكلات في كتلة واحدة

```csharp
// Domain/Entities/Study/StudentResult.cs
public class StudentResult                          // ⚠️ لا يرث BaseEntity!
{
    public int EnteredByUserId { get; set; }        // ⚠️ تكرار لـ CreatedByUserId
    public User EnterdByUser { get; set; } = null!; // ⚠️ خطأ إملائي: Enterd
    public DateTime CreatedAt { get; set; }
    public int? UpdatedByUserId { get; set; }
    public User UpdatedByUser { get; set; } = null!; // ⚠️ null! رغم أن الـ Id هو int?
}
```

1. **لا يرث `BaseEntity`** — يكرّر حقول التدقيق بأسماء مختلفة
2. **خطأ إملائي** (`Enterd`) سيلاحقني في كل استعلام
3. **تضارب قابلية الإفراغ** — المُعرِّف `int?` والتنقّل `null!` متناقضان

#### التطبيق المقترح

```csharp
public class StudentResult : BaseEntity
{
    public bool IsDeleted { get; private set; }
    public DateTime? DeletedAt { get; private set; }
    public int? DeletedByUserId { get; private set; }

    public StudentResult MarkDeleted(int byUserId)
    {
        IsDeleted = true;
        DeletedAt = DateTime.UtcNow;
        DeletedByUserId = byUserId;
        return this;
    }
}
```

ثم في `AppDbcontext` — ترشيح تلقائي:

```csharp
builder.Entity<StudentResult>().HasQueryFilter(r => !r.IsDeleted);
```

#### لماذا يرفع مستواي؟

الدرجات الأكاديمية من نفس فئة السجلات المالية: **لو حُذفت درجة بالخطأ أو بسوء نيّة، يجب أن يبقى أثر يُدقَّق.**

وبناء هذا يدوياً يعلّمني **ما يخفيه ABP خلف `FullAuditedAggregateRoot`** — وهذه ميزتي كمن بنى البنية التحتية بيده.

---

### ③ خدمة المجال (Manager) — أين يعيش المنطق متعدد الكيانات؟

#### الفكرة في سطر

منطق يمسّ كيانين فأكثر لا يخصّ أيّاً منهما وحده، ولا يخصّ طبقة الخدمة (مهمّتها التنسيق والصلاحيات) — مكانه **خدمة مجال**.

#### المثال في ZadByan

```csharp
// TS.ZadByan.Domain/Students/Enrollments/EnrollmentManager.cs
public class EnrollmentManager : DomainService
{
    public async Task<Enrollment> CreateEnrollmentAsync(int semesterId, Guid studentId, decimal price)
    {
        Guid enrollmentId = GuidGenerator.Create();
        Enrollment enrollment = new Enrollment(enrollmentId, semesterId, studentId, price);

        var lessons = await repository.GetSemesterLessonsAsync(semesterId);
        foreach (var lesson in lessons)              // ← ينشئ الأبناء تلقائياً
            enrollment.EnrolledLessons.Add(
                new EnrolledLesson(GuidGenerator.Create(), enrollmentId, lesson.UnitId, lesson.Id, ...));

        return enrollment;
    }
}
```

**المنطق:** تسجيل طالب في برنامج *يستلزم* إنشاء سجلّ تتبّع لكل درس فيه. عملية واحدة منطقياً، تمسّ كيانين.

مثال آخر — `StudentManager`:

```csharp
// إنشاء طالب = IdentityUser (للدخول) + Student (للبيانات) بنفس الـ Id
public async Task<Student> CreateStudentAsync(string name, string userName, string password, ...)
{
    Guid id = GuidGenerator.Create();

    if (await userManager.FindByNameAsync(userName) != null)
        throw new UserFriendlyException(ZadByanDomainErrorCodes.UserAlreadyExistsException);

    IdentityUser user = new IdentityUser(id, userName, email);
    IdentityResult result = await userManager.CreateAsync(user, password);

    if (result.Succeeded)
    {
        await userManager.AddToRoleAsync(user, ZadByanRoles.Student);
        return new Student(id, name, ...);          // ← نفس الـ id
    }
    throw new UserFriendlyException(...);
}
```

#### الوضع الحالي في UniNet

**لا يوجد أي `Manager` في المشروع كلّه.** المنطق موزّع بين الخدمات والكيانات الفقيرة.

#### التطبيق المقترح — المكافئ الحرفي

إسناد مادة لشعبة *يستلزم* تجهيز سجلّ درجات فارغ لكل طالب فيها:

```csharp
public class SectionSubjectManager
{
    private readonly IUnitOfWorkRepository _uow;

    public async Task<SectionSubject> AssignSubjectAsync(
        int sectionId, int subjectId, int semesterId, string lecturerName)
    {
        // تحقّقات تمسّ كيانات متعددة — لا مكان لها داخل كيان واحد:
        //  · هل المادة تنتمي لقسم هذه الشعبة؟
        //  · هل الفصل الدراسي مفتوح؟
        //  · هل المادة مُسندة مسبقاً لنفس الشعبة في نفس الفصل؟

        var sectionSubject = new SectionSubject(sectionId, subjectId, semesterId, lecturerName);

        var students = await _uow.StudentRepository.GetSectionStudentsAsync(sectionId);
        foreach (var student in students)
            sectionSubject.StudentResults.Add(new StudentResult(student.StudentId, ...));

        return sectionSubject;
    }
}
```

#### لماذا يرفع مستواي؟

**هذا المفهوم جديد كلياً عليّ** — أعلى بند في القائمة من حيث الجديد المعرفي.

يعلّمني الإجابة على سؤال يتكرر في كل مشروع: *"هذا المنطق يمسّ ثلاثة كيانات — أين أضعه؟"*

---

### ④ وراثة TPH — جدول واحد لأنواع متشابهة

#### الفكرة في سطر

أنواع تشترك في معظم حقولها تعيش في جدول واحد يميّزها عمود `Discriminator`.

#### المثال في ZadByan — موضعان

```csharp
// TS.ZadByan.Domain/Semesters/
public abstract class Semester : AuditedAggregateRoot<int>
{
    public SemesterType Discriminator { get; set; }
    // Name, CurriculumId, StaffId, Duration, Price, About, Goals...
}

public class Diploma : Semester { }
public class Course  : Semester { }
public class Level   : Semester { }

// وأيضاً: QuestionBank → Activity / Exercise
```

كلها في جدول `Semester` واحد.

#### الوضع الحالي في UniNet — الكيانان متطابقان حرفياً

```csharp
// Domain/Entities/Content/Post.cs
public class Post : BaseEntity
{
    public int PostId { get; set; }
    public string Title { get; set; } = null!;
    public string Content { get; set; } = null!;
    public EnContentType Type { get; set; }
    public int? ImageId { get; set; }
    public Image? Image { get; set; }
}

// Domain/Entities/Content/Announcement.cs
public class Announcement : BaseEntity
{
    public int AnnouncementId { get; set; }      // ← الفرق الوحيد: اسم المفتاح
    public string Title { get; set; } = null!;
    public string Content { get; set; } = null!;
    public EnContentType Type { get; set; }
    public int? ImageId { get; set; }
    public Image? Image { get; set; }
}
```

ونتيجة ذلك في `Image` — نمط علاقة متعدّدة الأشكال هشّ:

```csharp
// Domain/Entities/Images/Image.cs
public Post? Post { get; set; }
public Announcement? Announcement { get; set; }   // ⚠️ تنقّلان اختياريان
```

#### التطبيق المقترح

```csharp
public abstract class ContentItem : BaseEntity
{
    public int ContentItemId { get; set; }
    public string Title { get; set; } = null!;
    public string Body { get; set; } = null!;
    public EnContentType Scope { get; set; }
    public int? ImageId { get; set; }
    public Image? Image { get; set; }
}

public class Post         : ContentItem { }
public class Announcement : ContentItem { }
```

**المكاسب الملموسة:**

- `Image` يحمل مفتاحاً خارجياً واحداً نظيفاً بدل تنقّلين اختياريين
- مستودع واحد وخدمة واحدة بدل اثنين متطابقين
- استعلام موحّد: "كل المحتوى المرئي لنطاق هذا المستخدم"

#### معيار القرار قبل التنفيذ

| السؤال | إن نعم | إن لا |
|--------|--------|-------|
| هل سيتباعد سلوكهما لاحقاً؟ (إعلان له تاريخ انتهاء ومنشور لا؟) | احتفظ بهما منفصلين | TPH مناسب |
| هل أحتاج استعلامهما معاً غالباً؟ | TPH مناسب | الفصل أبسط |

**تنبيه حاسم:** التحويل لـ TPH بعد إنشاء الجداول والبيانات يتطلب Migration معقّدة.
**الآن — قبل أي بيانات فعلية — كلفته شبه صفر.** إن ترددت، فهذا وقت الحسم لا لاحقاً.

---

### ⑤ التحقق من انتقال الحالة

#### الفكرة في سطر

الحالة لا تقفز عشوائياً — كل انتقال له شروط تُفحص.

#### المثال في ZadByan

```csharp
// TS.ZadByan.Domain.Shared/ZadByanDomainErrorCodes.cs

/// <summary>محاولة انتقال حالة غير مسموحة (مثل بدء حصة منتهية).</summary>
public const string LiveSessionInvalidStatusTransition = "Exception:LiveSession:InvalidStatusTransition";

/// <summary>الحصة خارج نافذة الانضمام الزمنية.</summary>
public const string LiveSessionOutsideJoinWindow = "Exception:LiveSession:OutsideJoinWindow";
```

مع `LiveSessionStatus` — لا يمكن بدء حصة منتهية.

#### الثغرة الحالية في UniNet

```csharp
// Domain/Entities/Study/Semester.cs
public bool IsCurrent { get; set; }
```

**لا شيء يمنع وجود ثلاثة فصول دراسية `IsCurrent = true` في وقت واحد.**

هذا ثابت على مستوى **المجموعة كلها** لا الكيان الواحد — ولذلك مكانه خدمة مجال:

```csharp
public async Task SetCurrentSemesterAsync(int semesterId)
{
    await _repo.ClearCurrentFlagAsync();       // ← إلغاء الحالي أولاً
    var semester = await _repo.GetAsync(semesterId);
    semester.MarkAsCurrent();
}
```

#### امتداد أقوى — حالة للدرجة نفسها

```
Draft  →  Submitted  →  Approved
```

بعد الاعتماد لا يجوز للمدرّس تعديل الدرجة إلا بمسار خاص موثّق.

**هذا يحوّل النظام من "تخزين بيانات" إلى "نظام يفرض قواعد مؤسسية".**

---

### ⑥ حدود التجميع — ليس كل ما له أب هو كيان تابع

#### السؤال

`StudentResult` يتبع `Student` أم `SectionSubject`؟

#### الجواب: لا هذا ولا ذاك — هو جذر تجميعي مستقل

والدليل من ZadByan نفسه:

```csharp
// Enrollment يربط Student بـ Semester — ومع ذلك:
public class Enrollment : AuditedAggregateRoot<Guid>   // ← جذر مستقل!
{
    public int SemesterId { get; set; }
    public Guid StudentId { get; set; }
    public ICollection<EnrolledLesson> EnrolledLessons { get; set; } = [];
}
```

بينما:

```csharp
// SectionTranslation كيان تابع — يُدار من داخل Section فقط
public class Section : AuditedAggregateRoot<int>
{
    public Section AddTranslation(...) { Translations.Add(new SectionTranslation(...)); return this; }
    public Section EditTranslation(int id, ...) { ... }
    public Section RemoveTranslation(int id) { ... }
}
```

#### القاعدة المميِّزة

| السؤال | `SectionTranslation` | `Enrollment` / `StudentResult` |
|--------|:--------------------:|:------------------------------:|
| له معنى منفصلاً عن أبيه؟ | ❌ ترجمة بلا قسم = لا شيء | ✅ درجة لها سياق مستقل |
| يُستعلم عنه مباشرة؟ | ❌ دائماً عبر القسم | ✅ "أرني كشف درجات الطالب" |
| يمسّ أكثر من أب؟ | ❌ أب واحد | ✅ طالب **و** مادة/شعبة |
| **النتيجة** | كيان تابع | **جذر تجميعي مستقل** |

#### لماذا يهمّ عملياً؟

الجذر المستقل يحصل على مستودع وخدمة خاصّين، ويمكن الاستعلام عنه مباشرة.

لو عاملت `StudentResult` ككيان تابع لـ `SectionSubject`، فكل استعلام عن كشف درجات طالب سيمرّ عبر كل الشُّعب — **تصميم خاطئ يظهر ثمنه في الأداء لاحقاً**.

---

### ⑦ ثوابت مركزية — مصدر واحد للحقيقة

#### المثال في ZadByan

```csharp
// TS.ZadByan.Domain.Shared/ZadByanDomainConsts.cs
public static class Section
{
    public const int NameMaxLength = 200;
    public const int NameMinLength = 2;
    public const int AboutMaxLength = 500;
}
```

يُستخدم في موضعين — والقيمة واحدة دائماً:

```csharp
// في الكيان:
Check.Length(name, nameof(Name), ZadByanDomainConsts.Section.NameMaxLength, ...);

// في الـ DTO:
[Length(ZadByanDomainConsts.Language.NameMinLength, ZadByanDomainConsts.Language.NameMaxLength)]
public string Name { get; set; }
```

#### الوضع الحالي في UniNet — مصدران لا يعرف أحدهما الآخر

```csharp
// DataAccessLayer/Configurations/AcademicConfigurations/CollegeConfiguration.cs
builder.Property(c => c.Name).HasColumnType("NVARCHAR(250)").IsRequired();

// Application/Validators/... — رقم منفصل تماماً
RuleFor(x => x.CollegeName).MaximumLength(/* ??? */);
```

#### الخطر الحقيقي

| ماذا لو غيّرت... | النتيجة |
|------------------|---------|
| العمود إلى 300 ونسيت الـ Validator | ⚠️ يُقبل 300 حرف بصمت — لا خطأ، لكن التحقّق أصبح بلا معنى |
| الـ Validator إلى 300 ونسيت العمود | ⚠️ `SqlException 8152` (اقتطاع نص) |

**والحالة الثانية بالذات** هي نوع الخطأ الذي كان يُبتلَع قبل إصلاح `CompleteAsync`.
**البنود مترابطة.**

#### الحل

```csharp
// Contracts/Common/EntityConstants.cs
public static class CollegeConstants
{
    public const int NameMaxLength = 250;
    public const int NameMinLength = 2;
}
```

يُستخدم في الثلاثة: إعداد EF، والـ Validator، والكيان الغني.

---

## ٣. ترتيب التنفيذ

```
الآن ──────────────────────────────────────────────────────────►

  1. StudentResult: ورث BaseEntity + أصلح Enterd            ← ١٠ دقائق، يمهّد لما بعده
  2. StudentResult: SetGrades + Total محسوبة                ← أول كيان غني أكتبه بيدي
  3. StudentResult: حذف ناعم + QueryFilter                  ← أبني يدوياً ما يخفيه ABP
  4. قرار TPH لـ Post/Announcement                           ← ⚠️ الآن أو لن يعود سهلاً
  5. SectionSubjectManager (خدمة المجال الأولى)              ← المفهوم الأجدّ عليّ
  6. Semester: ثابت "فصل حالي واحد فقط"
  7. ثوابت الأطوال المركزية أثناء بناء الخدمات
```

**البند ٢ هو نقطة التحوّل الحقيقية** — أول مرة أكتب كياناً يحرس نفسه من الصفر، بقراري لا نقلاً.

وهو أوضح ما يمكن أن أحكيه في مقابلة عمل:

> "بدأت بكيانات فقيرة، ثم تعلّمت متى يستحق الكيان أن يحمي ثوابته، وطبّقت ذلك على الدرجات تحديداً لأن اتساق `Total` مسألة صحّة لا أسلوب."

---

## ٤. أهم الفروقات الجوهرية بين المشروعين

### الجدول الشامل

| المحور | UniNet | ZadByan |
|--------|--------|---------|
| **الإطار** | ASP.NET Core 8 خام | ABP Framework 9.1 فوق .NET 9 |
| **عدد المشاريع** | 5 | 10 (7 منها backend) |
| **تصميم الكيان** | فقير (حقيبة بيانات) | غني (يحرس ثوابته) |
| **مكان التحقّق** | FluentValidation خارجي | `Check.*` داخل الكيان + DataAnnotations على الـ DTO |
| **معالجة الأخطاء** | مظروف نتيجة `AddUpdateServiceResponse<T>` | استثناءات `UserFriendlyException` + middleware |
| **حفظ التغييرات** | `CompleteAsync()` صريح | Unit of Work تلقائي — بلا `SaveChanges` |
| **المستودعات** | مكتوبة يدوياً + `IUnitOfWorkRepository` | مولَّدة: `AddDefaultRepositories(includeAllEntities: true)` |
| **التحويل** | إسقاطات `Expression<Func<T,DTO>>` يدوية | AutoMapper `CreateMap` |
| **الـ Controllers** | مكتوبة يدوياً (17 ملف) | مولَّدة تلقائياً (ملفان فقط يدويان في المشروع كلّه) |
| **المصادقة** | JWT مخصّص + BCrypt + RefreshToken | OpenIddict + ABP Identity |
| **التفويض** | أدوار + `OwnershipRequirement` | صلاحيات شجرية دقيقة |
| **مكان `[Authorize]`** | على الـ Controller | على الـ AppService |
| **حقول التدقيق** | `BaseEntity` يدوي، تُملأ في كل خدمة | `AuditedAggregateRoot`، تُملأ تلقائياً |
| **تسجيل DI** | يدوي في 3 ملفات extension | اصطلاحي تلقائي |
| **اللغات** | أحادي | متعدد (كيانات ترجمة منفصلة) |
| **الاختبارات** | ❌ لا يوجد | 11 ملف (معظمها قوالب) |

### الفروقات الثلاثة الأهم — بالتفصيل

#### الفرق ١: أين تعيش قواعد العمل؟

```
UniNet:                              ZadByan:

Controller                           AppService
   ↓ يستدعي                             ↓ يستدعي
Validator ← القواعد هنا               Entity ← القواعد هنا
   ↓                                     ↓
Service ← البناء هنا                  Repository
   ↓
Repository
   ↓
Entity ← حقيبة بيانات فقط
```

**الأثر:** في UniNet، صحّة البيانات مضمونة *إن مرّ الطلب بالمسار المصمَّم*.
في ZadByan، مضمونة *دائماً* لأن الكيان لا يملك مساراً آخر.

#### الفرق ٢: من يقرّر متى تُحفظ البيانات؟

```csharp
// UniNet — أنا أقرّر
_unitOfWorkRepository.CollegeRepository.Add(collegeEntity);
await _unitOfWorkRepository.CompleteAsync();     // ← قراري

// ZadByan — الإطار يقرّر
await _repository.InsertAsync(language);
// لا شيء بعدها! AbpUnitOfWorkMiddleware يحفظ عند نجاح الطلب
```

**المقايضة:**

| | UniNet | ZadByan |
|---|--------|---------|
| الوضوح للقارئ | ✅ ترى نقطة الحفظ | ⚠️ خفيّة |
| خطر النسيان | ⚠️ تنسى `CompleteAsync` ← لا حفظ بصمت | ✅ مستحيل |
| التحكّم الدقيق | ✅ كامل | ⚠️ يحتاج تجاوز الافتراضي |

**لا أحدهما "أفضل" مطلقاً** — لكن معرفة المقايضة هي المهارة.

#### الفرق ٣: أين يوضع فحص الصلاحية؟

```csharp
// UniNet — على الـ Controller (صحيح لأن HTTP هو المدخل الوحيد)
[Authorize(Roles = "Super Admin,UniversityAdmin")]
public async Task<ActionResult<CollegeDTO>> GetCollegeById(...)

// ZadByan — على الخدمة (لأن لها أكثر من مدخل)
[Authorize(ZadByanPermissions.Languages.Default)]
public class LanguageAppService : ApplicationService, ILanguageAppService
{
    [Authorize(ZadByanPermissions.Languages.Create)]
    public async Task<LanguageDto> CreateAsync(...) { }
}
```

**السبب في ZadByan:** المشروع Blazor بوضعي تصيير:

```
Blazor Server (على الخادم)
   يحقن ILanguageAppService عبر DI ← يناديها مباشرة بلا HTTP!

Blazor WebAssembly (في المتصفح)
   يمرّ عبر HttpApi.Client → HTTP → Controller مولَّد
```

لو وُضعت الصلاحية على الـ Controller فقط، لتجاوزها مكوّن Blazor Server تماماً.

> **القاعدة المستخلصة:** ضع الفحوصات العابرة (Authorization, Validation) عند **أضيق نقطة مشتركة بين كل الاستدعاءات الممكنة**، لا عند أول نقطة تخطر ببالي.

---

## ٥. المفاهيم التي ترفع مستواي

مرتّبة بالقيمة التعليمية:

### المستوى الأول — تغيّر طريقة تفكيري

| المفهوم | ما يعلّمني | أين أطبّقه |
|---------|-----------|------------|
| **الكيان الغني** | الفرق بين "تحقّق من البيانات" و"حماية الثوابت" | `StudentResult.Total` |
| **حدود التجميع** | متى يكون الكيان تابعاً ومتى يكون جذراً مستقلاً | `StudentResult` vs `SectionSubject` |
| **خدمة المجال** | أين يعيش المنطق متعدد الكيانات | `SectionSubjectManager` |

**لماذا هذه أولاً؟** لأنها **أفكار تصميمية خالصة** — لا تعتمد على ABP ولا على أي إطار. تنفعني في أي مشروع مستقبلي بأي لغة.

### المستوى الثاني — تحسّن جودة كودي

| المفهوم | ما يعلّمني |
|---------|-----------|
| **اختيار مستوى التدقيق** | لماذا `PaymentTransaction` يرث `FullAudited` و`Section` لا |
| **وراثة TPH** | متى تستحق الأنواع المتشابهة جدولاً واحداً |
| **انتقالات الحالة** | تحويل النظام من تخزين إلى فرض قواعد |
| **الثوابت المركزية** | منع انحراف مصادر الحقيقة |

### المستوى الثالث — أفهمها ولا أنقلها

| المفهوم | لماذا لا أنقله | ما هو الأصوب لمشروعي |
|---------|----------------|------------------------|
| Auto API Controllers | يحتاج Castle DynamicProxy وبنية ABP كاملة | Controllers يدوية — أوضح وأتحكّم بمساراتي |
| UnitOfWork تلقائي | يحتاج AOP كاملاً | `CompleteAsync()` صريح — أوضح للقارئ |
| نظام الصلاحيات الشجري | وحدة ABP بجداول ومزوّدي قيم | أدوار + `OwnershipRequirement` — يكفي لنطاقي |
| AutoMapper | انعكاس وقت التشغيل | إسقاطات `Expression` عندي **أسرع فعلياً** |
| كيانات الترجمة | ZadByan متعدد اللغات بطبيعته | UniNet أحادي — تعقيد بلا مقابل |

> **ملاحظة مهمة:** في ثلاثة من هذه الخمسة، اختياري الحالي **أفضل لسياقي** لا مجرد "مقبول".
> لا أغيّر ما يعمل جيداً لمجرد أن مشروعاً آخر يفعله بشكل مختلف.

---

## ٦. كيف أفهم ZadByan بسهولة

### الفكرة المفتاحية: ABP ليس لغزاً — هو مجموعة اصطلاحات

معظم ما يبدو "سحراً" في ZadByan هو في الحقيقة **اصطلاح متوقَّع**. حين أعرف الاصطلاحات الخمسة التالية، أفهم 80% من الكود.

### الاصطلاحات الخمسة

#### ١. اتجاه الاعتماد بين الطبقات

```
Domain.Shared  ←  لا يعتمد على شيء
      ↑
   Domain
      ↑
Application.Contracts
      ↑
 Application        EntityFrameworkCore
      ↑                    ↑
   HttpApi  ──────────  Blazor (المضيف)
```

**القاعدة:** السهم للأعلى فقط. `Domain` **لا يعرف** `Application` إطلاقاً — ولو حاولت، لن يُترجم المشروع.

**كيف يساعدني؟** حين أبحث عن كود، أعرف أين لا يمكن أن يكون.

#### ٢. أي صنف يرث ApplicationService يصبح API تلقائياً

```csharp
public class LanguageAppService : ApplicationService, ILanguageAppService
```

يتحوّل تلقائياً إلى:

| الدالة | المسار المولَّد |
|--------|-----------------|
| `GetListAsync` | `GET /api/app/language` |
| `GetAsync(1)` | `GET /api/app/language/1` |
| `CreateAsync` | `POST /api/app/language` |
| `UpdateAsync` | `PUT /api/app/language/1` |
| `DeleteAsync` | `DELETE /api/app/language/1` |

السطر المسؤول عن ذلك في `ZadByanBlazorModule.cs`:

```csharp
options.ConventionalControllers.Create(typeof(ZadByanApplicationModule).Assembly);
```

**كيف يساعدني؟** حين أبحث عن Controller لخدمة ما ولا أجده — **هذا طبيعي**، لا تضيّع وقتاً.

#### ٣. المستودعات مولَّدة، لا مكتوبة

```csharp
// ZadByanEntityFrameworkCoreModule.cs
options.AddDefaultRepositories(includeAllEntities: true);
```

لذلك `IRepository<Language, int>` يعمل رغم أن أحداً لم يكتبه.

**متى أجد مستودعاً مكتوباً؟** فقط حين يحتاج استعلاماً معقّداً — مثل `SectionRepository` بإسقاطاته.

#### ٤. الحفظ تلقائي — لا SaveChanges في أي مكان

```
AbpUnitOfWorkMiddleware   ← يفتح UoW لكل طلب HTTP
        ↓
UnitOfWorkInterceptor     ← ينضمّ إليها في كل استدعاء خدمة
        ↓
عند نجاح الطلب: SaveChanges + COMMIT تلقائياً
عند أي استثناء: ROLLBACK كامل تلقائياً
```

**كيف يساعدني؟** حين لا أجد `SaveChanges` — **لا تبحث عنها**، هي في البنية التحتية.

#### ٥. أسماء الملفات تكشف أدوارها

| اللاحقة | الدور | الطبقة |
|---------|-------|--------|
| `*AppService` | حالة استخدام | Application |
| `*Manager` | منطق متعدد الكيانات | Domain |
| `*Repository` | استعلام مخصّص | EntityFrameworkCore |
| `*Dto` | ما يخرج للعميل | Application.Contracts |
| `CreateUpdate*Dto` | ما يدخل من العميل | Application.Contracts |
| `*FilterDto` | معايير بحث وترقيم | Application.Contracts |
| `*WithDetails` | نموذج قراءة (إسقاط) | Domain |
| `*Translation` | ترجمة كيان | Domain |
| `*Consts` | ثوابت | Domain.Shared |

**كيف يساعدني؟** أعرف دور أي ملف من اسمه قبل فتحه.

---

### استراتيجية القراءة: عمودياً لا أفقياً

#### الطريقة الخاطئة (تُنهك ولا تُفهِم)

```
اقرأ كل Domain (88 ملف)
   ثم كل Application.Contracts (131 ملف)
      ثم كل Application (48 ملف)
```

النتيجة: 267 ملفاً بلا سياق يربطها.

#### الطريقة الصحيحة — الشريحة العمودية

اختر ميزة واحدة، وتتبّعها من أولها لآخرها:

```
مثال: ميزة "اللغات"

1. Domain/Languages/Language.cs                              ← الكيان
2. Application.Contracts/Languages/LanguageDto.cs            ← ما يخرج
3. Application.Contracts/Languages/CreateUpdateLanguageDto.cs ← ما يدخل
4. Application.Contracts/Languages/ILanguageAppService.cs     ← العقد
5. Application/Languages/LanguageAppService.cs               ← التنفيذ
6. Application/ZadByanApplicationAutoMapperProfile.cs        ← التحويل
7. Application.Contracts/Permissions/ZadByanPermissions.cs   ← الصلاحيات
```

**سبعة ملفات = فهم كامل لميزة كاملة.** ثم كرّر مع ميزة أعقد (`Section`)، ثم أعقد (`Enrollment`).

---

### ما أتجاهله تماماً

| الموضع | لماذا |
|--------|-------|
| `Migrations/` | مولَّد آلياً — 37 ألف سطر لا أقرؤها |
| `LiveSessionAppService.cs` (821 سطر) | أضخم ملف، متشابك مع LiveKit وwebhooks |
| `Payments/Gateways/` | يمسّ المال — خطأ فيه يُكلّف فعلياً |
| ملفات `*Module.cs` | تهيئة وربط — أعود لها عند الحاجة فقط |

---

### خريطة "أين أضع هذا الكود؟"

| ما أريد فعله | الطبقة | مثال يُحتذى |
|--------------|--------|-------------|
| قاعدة عمل تخصّ كياناً واحداً | Domain (داخل الكيان) | `Section.SetName` |
| منطق يمسّ كيانين فأكثر | Domain (Manager) | `EnrollmentManager` |
| تنسيق حالة استخدام + صلاحيات | Application | `LanguageAppService` |
| شكل بيانات للواجهة | Application.Contracts | `LanguageDto` |
| استعلام معقّد | EntityFrameworkCore | `SectionRepository` |
| ثابت مشترك / enum | Domain.Shared | `ZadByanRoles` |

---

### ثلاث قواعد لا أكسرها في ZadByan

1. **`Domain` لا يعرف `Application`** — الاتجاه للأعلى فقط
2. **لا أُرجع كياناً من AppService** — أُرجع DTO دائماً
3. **أسجّل `CreateMap` عند إضافة أي DTO** — نسيانه = خطأ وقت تشغيل، وهو أشهر خطأ للمبتدئين

---

### مقياس الفهم

| السؤال | إن أجبت بنعم |
|--------|--------------|
| أفهم لماذا لا يوجد `SaveChanges` في أي AppService؟ | استوعبت Unit of Work |
| أعرف الفرق بين `[Authorize(Roles=...)]` و`[Authorize(Permission)]`؟ | استوعبت نموذج الصلاحيات |
| أضع قاعدة العمل داخل الكيان تلقائياً؟ | استوعبت الكيان الغني |
| أميّز الكيان التابع من الجذر المستقل؟ | استوعبت حدود التجميع |
| أعرف متى **لا** أنقل نمطاً من ZadByan؟ | **نضجت هندسياً** — وهذه أهمّها |

---

## ٧. جرد المفاهيم في المشروعين + تقييم دقة التطبيق

### كيف تُقرأ النِّسب

> **أساس المعايرة:** النِّسب مبنية على `DEVELOPER_PROFILE.md` — تصنيف **Strong Junior على حافة Mid-level**.
> **ليست** مقارنة بمعايير Senior Architect، ولا بمشروع ABP ناضج بفريق كامل.
> كل نسبة مقترنة بدليل من الكود، لا بانطباع.

**عمودان لا عمود واحد** — لأن المعادلة المركزية في ملف المستوى هي:

> جودة القرارات التصميمية **أعلى** من جودة التنفيذ التفصيلي.

عمود واحد يخفي هذه الحقيقة. عمودان يجعلانها مرئية: **الفجوة بين الرقمين هي التشخيص نفسه.**

| المدى | المعنى |
|-------|--------|
| **90–100%** | متقن — لا يحتاج عملاً |
| **75–89%** | جيد مع ثغرات محدودة |
| **60–74%** | الفكرة صحيحة والتطبيق ناقص |
| **40–59%** | مطبَّق جزئياً — فجوة حقيقية |
| **0–39%** | غائب أو معطَّل |

---

### أ. مفاهيم مستخدَمة في المشروعين معاً

| # | المفهوم | في ZadByan | في UniNet | القرار التصميمي | دقة التنفيذ | الفجوة |
|:-:|---------|------------|-----------|:---------------:|:-----------:|:------:|
| 1 | **الفصل الطبقي** | 7 مشاريع بفرض ABP | 5 مشاريع مفروضة على مستوى `csproj` | **95%** | **95%** | — |
| 2 | **المعاملات (Transactions)** | UoW تلقائي | صريحة في `CreateEmployee`/`CreateStudent` فقط | **90%** | **90%** | — |
| 3 | **التحويل (Mapping)** | AutoMapper | `Expression<Func<T,DTO>>` يدوية | **95%** | **90%** | 5 |
| 4 | **فصل العقود (DTO)** | `Application.Contracts` | `Contracts` مكتبة مشتركة | **95%** | **90%** | 5 |
| 5 | **Repository** | مولَّد + مخصَّص للمعقّد | يدوي لكل كيان | **90%** | **85%** | 5 |
| 6 | **Unit of Work** | تلقائي (Middleware + Interceptor) | `CompleteAsync()` صريح | **90%** | **85%** | 5 |
| 7 | **الترقيم (Pagination)** | `PagedResultDto` | `PagedResult<T>` + امتداد | **85%** | **80%** | 5 |
| 8 | **حقول التدقيق** | `AuditedAggregateRoot` تلقائي | `BaseEntity` يدوي | **80%** | **70%** | 10 |
| 9 | **معالجة الأخطاء** | استثناءات + Middleware | مظروف `AddUpdateServiceResponse<T>` | **90%** | **75%** | 15 |
| 10 | **التحقق من المدخلات** | `Check.*` + DataAnnotations | FluentValidation | **90%** | **65%** | 25 |
| 11 | **نطاق متعدد المستأجرين** | Tenant من ABP | مطالبات JWT مخصّصة | **95%** | **60%** | **35** |
| 12 | **المصادقة** | OpenIddict + ABP Identity | JWT + Refresh Rotation + BCrypt | **95%** | **60%** | **35** |
| 13 | **التسجيل (Logging)** | Serilog عبر الطبقات | `ILogger` في الـ Middleware فقط | **70%** | **40%** | 30 |
| 14 | **التفويض** | صلاحيات شجرية | أدوار + `OwnershipRequirement` | **95%** | **55%** | **40** |

**المتوسط:** القرار التصميمي **≈ 90%** · دقة التنفيذ **≈ 74%** · **الفجوة ≈ 16 نقطة**

> هذه الـ 16 نقطة هي **الفارق الوحيد المتبقّي** بين Strong Junior و Mid-level.
> ليست فجوة معرفية — بل فجوة انضباط.

---

### الأدلة وراء النِّسب المنخفضة

#### التفويض — 95% تصميم / 55% تنفيذ (أكبر فجوة)

**لماذا التصميم 95%؟** بنيت `OwnershipRequirement` + 7 handlers مسجَّلة في DI، واخترت إرجاع 404 بدل 403 لمنع تسريب وجود المورد — **قرار أمني واعٍ يتجاوز المتوقَّع في هذا المستوى**.

**لماذا التنفيذ 55%؟** الآلية مبنية باحتراف لكنها غير معمَّمة:

| النقطة | الحالة |
|--------|--------|
| `DeleteSection` | ❌ بلا أي فحص ملكية إطلاقاً — و`SectionOwnerPolicy` **جاهز ومستخدَم في نفس الملف** |
| `GetAll*` في Department/Batch/Section | ❌ بلا `scope`، ومفتوحة لأدوار مُقيَّدة النطاق |
| `GetAll*` في College/University | ✅ مُقيَّدة بـ `Super Admin` |

**التشخيص:** طبّقت القاعدة في 2 من 5 — أي أنك **تعرفها** ونسيت تعميمها. سلوكي لا معرفي.

#### المصادقة — 95% تصميم / 60% تنفيذ

**التصميم:** تدوير Refresh Token مع تخزين الـ Hash فقط (SHA-256) وربط `ReplacedByTokenId` — تنفيذ يتجاوز المتوقَّع من حديثي التخرج.

**التنفيذ:** `TokenUserInfoMapper.cs:29` و`:70` — الحارس في السطر 18 يستخدم `&&` بدل `||`، فيصل الموظف إلى سطر يُلغي مرجعاً فارغاً ← **تسجيل دخول كل حسابات الموظفين معطَّل**، بينما `README.md` يصنّف الميزة "✅ Complete".

والمُصرِّف كان يشير على السطرين بالضبط (`CS8602` + `CS0472`) في **كل عملية بناء لأكثر من جولة**.

#### التحقق من المدخلات — 90% تصميم / 65% تنفيذ

**التصميم:** فصل واعٍ ومتّسق بين تحقق الشكل (FluentValidation) وتحقق منطق العمل (الخدمة) — بلا ازدواجية.

**التنفيذ:** `AddStudentValidator.cs:62-66` — قاعدة `StudentNumber` منسوخة من قاعدة الهاتف بالكامل: Regex الهاتف، ورسالة الهاتف، و`.When(x => !string.IsNullOrEmpty(x.PhoneNumber))`.

`.When()` تنطبق على **كل** القواعد السابقة في السلسلة — فإن لم يُرسل العميل هاتفاً، **لا يُتحقَّق من رقم الطالب إطلاقاً**.

#### نطاق متعدد المستأجرين — 95% تصميم / 60% تنفيذ

`ToUserScope()` في `ControllersExtensions.cs:102-110` **لا يُسنِد `BatchId`** — بينما أربعة مستهلكين مكتوبون بشكل صحيح لاستخدامه (`StudentRepository:96`، `IsWithinScope:16`، `BatchOwnerHandler`، `SectionOwnerHandler`).

> ⚠️ **تنبيه مقترن:** `EmployeeScopeExtension.cs:14` يفحص ثلاثة حقول من أربعة — `BatchId` غير مذكور.
> **إصلاح `ToUserScope` وحده يفتح ثغرة تصعيد صلاحيات.** الترتيب إلزامي: أصلح الحارس أولاً، ثم `ToUserScope`.

#### معالجة الأخطاء — 90% تصميم / 75% تنفيذ

**التصميم:** مظروف النتيجة اختيار ناضج — الاستثناءات مكلفة ولا تصلح للتحكم بالتدفّق.

**التنفيذ:** `ControllersExtensions.cs:53-57` — `ExistedResource` يُرجع `BadRequestObjectResult` (حالة **400**) وجسمه يدّعي `StatusCode = 409`. كل المتحكمات تعلن `[ProducesResponseType(409)]`. العميل الذي يفحص `response.status === 409` لن يعمل أبداً.

وكذلك `Add*` تُرجع 200 لا `201 Created` بلا رأس `Location`، رغم `[ProducesResponseType(201)]` على 9 نقاط إنشاء.

---

### ✅ ما لا يحتاج عملاً — لا تُعِد فتحه

| المفهوم | لماذا هو مغلق |
|---------|----------------|
| **الفصل الطبقي (95/95)** | مفروض على مستوى `csproj` لا بالاتفاق — نادر في هذا المستوى |
| **التحويل بالإسقاطات (95/90)** | أسرع فعلياً من AutoMapper لحالتك — **لا تستبدله** |
| **المعاملات (90/90)** | صريحة حيث تلزم فقط، وغائبة حيث لا تلزم — فهم صحيح |
| **`CompleteAsync`** | أصلحته بـ `catch...when` **أنظف من الحل المقترح عليك**، مع تعليق يشرح السبب |
| **`ILogger` في الـ Middleware** | مع تمييز صحيح بين `LogWarning` (قيد عمل) و`LogError` (عطل) |
| **تعميم الفلترة وحذف `GetByName`** | عُمِّما على الكيانات الخمسة كلها دون طلب — **أهم تحسّن مرصود** |

---

### ب. مفاهيم في ZadByan وغائبة عن UniNet

هذه هي **فرص النقل** — راجع القسم ٢ لتفاصيل كل بند.

| # | المفهوم | في ZadByan | في UniNet | النسبة | أين أطبّقه |
|:-:|---------|------------|-----------|:------:|------------|
| 1 | **خدمة المجال (Manager)** | `EnrollmentManager`, `StudentManager` | لا يوجد أي `Manager` | **0%** | `SectionSubjectManager` |
| 2 | **الحذف الناعم** | `PaymentTransaction : FullAudited` | لا يوجد | **0%** | `StudentResult` |
| 3 | **اختبارات الوحدة** | 11 ملف (معظمها قوالب) | لا مشروع اختبارات | **0%** | `IsWithinScope` أولاً |
| 4 | **الكيان الغني** | كل الكيانات | `Total` فقط بـ `private set` — بلا دالة تحسبها | **10%** | `StudentResult.SetGrades` |
| 5 | **ثوابت مركزية** | `ZadByanDomainConsts` | الأطوال مكرّرة في EF Config والـ Validator | **20%** | `EntityConstants` |
| 6 | **حدود التجميع** | `Section→Translation`, `Enrollment` جذر | العلاقات موجودة بلا تمييز تابع/جذر | **30%** | `StudentResult` كجذر مستقل |
| 7 | **وراثة TPH** | `Semester→Diploma/Course/Level` | `Post` و`Announcement` متطابقان ومنفصلان | **0%** | `ContentItem` |
| 8 | **انتقالات الحالة** | `LiveSessionStatus` + رموز أخطاء | `IsCurrent` بلا أي حماية | **0%** | `Semester` + حالة الدرجة |

> **ملاحظة على البند 4:** الـ 10% ليست صفراً لأن الغريزة كانت صحيحة — أدركت أن `Total` قيمة محسوبة فجعلتها `private set`. الناقص هو **إكمال النمط**، لا فهمه.

---

### ج. قراءة الأرقام — ماذا تقول فعلاً؟

#### النمط الواضح

```
الفجوة صغيرة (0–5 نقاط)          الفجوة كبيرة (25–40 نقطة)
─────────────────────            ──────────────────────────
الفصل الطبقي                      التفويض
المعاملات                          المصادقة
الإسقاطات                          النطاق متعدد المستأجرين
فصل العقود                         التسجيل
Repository / UoW                   التحقق من المدخلات
```

**ما الذي يميّز العمود الأيسر؟** كلها مفاهيم تُطبَّق **مرة واحدة في مكان واحد**.

**ما الذي يميّز العمود الأيمن؟** كلها مفاهيم تحتاج **تطبيقاً متكرراً عبر نقاط متعددة**.

> **التشخيص في جملة:** المشكلة ليست في فهم المفهوم — بل في **تعميمه على كل النقاط المتأثرة**.

#### العلاج — ليس تعلّماً بل آلية

| الأداة | ماذا تحلّ |
|--------|-----------|
| **جدول تحقّق مكتوب** — صفوفه الكيانات الخمسة، وأعمدته `GetAll`/`GetPerParent`/`GetById`/`Add`/`Update`/`Delete` | لا يُغلق النمط قبل امتلاء كل الخلايا |
| **`<TreatWarningsAsErrors>`** في ملفات المشاريع | يحوّل "تذكُّر القراءة" إلى **إجبار** — والمُصرِّف كان يشير على أخطر عطل مرتين في كل بناء |
| **مشروع اختبارات** ولو بثلاثة اختبارات على `IsWithinScope` | يمسك اقتران العيبين في §6.4/§6.5 قبل أن يصير ثغرة |

**الثلاثة تحوّل الانضباط من "تذكُّر" إلى "آلية" — وهذا هو الفارق المتبقّي مع Mid-level.**

---

### د. مؤشر التقدّم — للمقارنة لاحقاً

| المفهوم | دقة التنفيذ اليوم | الهدف | كيف أصل |
|---------|:-----------------:|:-----:|---------|
| التفويض | 55% | 85% | `DeleteSection` + تقييد `GetAll*` بـ Super Admin |
| المصادقة | 60% | 90% | إصلاح `TokenUserInfoMapper:29,70` |
| النطاق | 60% | 90% | الحارس أولاً ثم `ToUserScope` — **بهذا الترتيب** |
| التحقق | 65% | 85% | قاعدة `StudentNumber` |
| التسجيل | 40% | 75% | `ILogger` في طبقة الخدمات |
| الكيان الغني | 10% | 70% | `StudentResult.SetGrades` |
| خدمة المجال | 0% | 60% | `SectionSubjectManager` |
| الاختبارات | 0% | 40% | 3 اختبارات على `IsWithinScope` |

**أعِد التقييم بعد إغلاق البنود الحمراء** — والفجوة بين العمودين هي المقياس الحقيقي للتقدّم، لا الرقم المطلق.

---

## خاتمة

**الخلاصة في ثلاث نقاط:**

1. **الأجزاء غير المبنية في UniNet هي فرصتي** — لا كود يعتمد عليها، فالتجريب فيها بلا مخاطرة.

2. **أنقل الأفكار لا البنية التحتية** — الكيان الغني وخدمة المجال وحدود التجميع تنفعني في أي مشروع. أمّا Auto Controllers وUoW التلقائي فتحتاج ABP كاملاً.

3. **ميزتي أنني بنيت البنية التحتية بيدي** — أفهم ما يخفيه الإطار. من يستخدم ABP دون أن يبني `IUnitOfWorkRepository` بنفسه لا يعرف ما يجري تحته.

**البداية العملية:** `StudentResult` — ورث `BaseEntity`، ثم اكتب `SetGrades`.
