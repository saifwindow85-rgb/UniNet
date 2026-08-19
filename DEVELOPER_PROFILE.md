# ملف مستوى المطوّر — مرجع معايرة المراجعات (UniNet)

> **الغرض من هذا الملف:** أساس ثابت تُبنى عليه مراجعات الجلسات المستقبلية، بحيث تكون المعايير مطابقة لمستوى المطوّر الفعلي — لا متشددة بمعايير Senior Architect، ولا متهاونة بمعايير مبتدئ.
>
> **تاريخ آخر تحديث:** 2026-08-19 · **الفرع:** `refactor/v1Branch` · **آخر Commit مُقيَّم:** `d24bef7`
> **الجولة السابقة:** `7aeac1e` · **البناء وقت المراجعة:** ناجح، **صفر تحذيرات** (كانت 3) مع `TreatWarningsAsErrors`.
> **حالة التشغيل وقت المراجعة:** ❌ **التطبيق لا يعمل** — نموذج EF لا يُبنى (§6.1). البناء ينجح، التشغيل لا.
> **حالة المشروع:** قيد التطوير النشط (v1/MVP) — ليس منتجًا جاهزًا للنشر.

---

## 1. التقييم العام للمستوى

**التصنيف: Mid-level مبتدئ — مع تحفّظ جديد على الانضباط التشغيلي.**

هذه الجولة **متناقضة بشكل حاد**، ويجب قراءتها كما هي بلا تلطيف:

| المحور | النتيجة |
|---|---|
| **جودة القرارات التصميمية** | ↗↗ **قفزة حقيقية.** أعاد تصميم `IsWithinScope` لتأخذ `GeneralAuthorizationInfo` — فقتل فئة خطأ كاملة بدل معالجة 12 حالة. |
| **الانضباط الآلي** | ↗↗ `TreatWarningsAsErrors` + `build.yml` + صفر تحذيرات. أغلق بندين من فجواته الخمس. |
| **جودة قاعدة البيانات** | ↗ أغلق كل بنود §7.3 السابقة: الفهارس الفريدة الثلاثة + `Semester.UniversityId` + `StudentResult : BaseEntity`. |
| **الانضباط التشغيلي** | ↘↘ **تراجُع.** آخر Commit يترك التطبيق **غير قابل للتشغيل**، وهجرة قاعدة البيانات **تفشل على قاعدة جديدة**. |
| **الاختبارات** | → صفر. للجولة الرابعة. |

**المعادلة التي تلخّص مستواه (تغيّرت):**
> السابق: «جودة القرارات التصميمية أعلى من جودة التنفيذ التفصيلي.»
>
> **الآن:** «جودة القرارات التصميمية **ممتازة**، وجودة التحقق من أن ما بناه **يعمل فعلًا** هي الفجوة الوحيدة الحقيقية المتبقية.»

**التشخيص الدقيق:** المطوّر ركّب هذه الجولة آليتَي انضباط (تحذيرات كأخطاء + CI)، لكن كلتيهما تقيسان **التصريف** لا **التشغيل**. فمرّ كودٌ يُصرَّف بصفر تحذيرات ويسقط عند أول لمسة لقاعدة البيانات. هذا **بالضبط** ما تقيسه الاختبارات، ولهذا صار غيابها الآن أغلى مما كان.

---

## 2. نقاط القوة المثبتة بالأدلة

كل بند مقترن بدليل من الكود، ويجب **عدم** إعادة اقتراحها عليه كأنها ناقصة:

| القوة | الدليل |
|---|---|
| **انضباط معماري حقيقي** | فصل الطبقات مفروض على مستوى `csproj` لا بمجرد مجلدات: `Domain` لا يرجع لغير `Contracts`، و`Application`/`DataAccessLayer` لا يرجع أحدهما للآخر. لا تسرّب EF إلى `Application`، ولا تسرّب `HttpContext` إلى ما دون الويب (عدا `CurrentUserService` وهو استثناء مبرَّر). |
| **فهم عميق لـ EF Core — أقوى مهاراته** | فهرسة مركّبة فريدة تطابق قواعد العمل في كل الكيانات، فهرس مُصفّى على `Email`، `AsNoTracking`، Projection عبر `Expression<Func<>>`، `DeleteBehavior.Restrict` كسياسة واعية، وعمود محسوب مخزَّن (`StudentResult.Total`). |
| **🆕 إصلاح على مستوى النوع لا الحالة — للمرة الرابعة** | بدل تمرير الوسيط الرابع في 12 موضعًا (الحل المقترح)، أعاد تصميم التوقيع إلى `IsWithinScope(this UserScope?, GeneralAuthorizationInfo)` وأضاف `UserScope.IsGlobal`. النتيجة: **إغفال وسيط صار مستحيلًا نحويًا**، والحارس الناقص صار خاصية محسوبة واحدة. هذا يغلق §6.1 و§6.2 السابقتين معًا. **حل أفضل من المقترح — للمرة الرابعة.** |
| **🆕 ثبّت الانضباط بآلية لا بذاكرة** | `Directory.Build.props` بـ `TreatWarningsAsErrors` + `.github/workflows/build.yml`. التحذيرات: 3 ← **0**. |
| **🆕 أغلق كل ديون مخطط Study** | `SectionSubject` و`StudentResult` لهما الآن فهرسان فريدان مركّبان، و`Semester` صار له `UniversityId` + فهرس مُصفّى `IsCurrent` **لكل جامعة**، و`StudentResult` يرث `BaseEntity`. خمسة بنود في هجرة واحدة. |
| **🆕 أول سلوك Domain حقيقي** | `StudentResult.SetGrades()` + `GradeConstants` — أول مرة يخرج المنطق من الخدمة إلى الكيان. الاتجاه صحيح (انظر §6.7 للتحفّظ). |
| **أمان متقدّم نسبيًا لمستواه** | تدوير Refresh Token مع تخزين الـ Hash فقط (SHA-256، 64 بايت عشوائية) وربط `ReplacedByTokenId`. BCrypt. لا أسرار في المستودع (`UserSecretsId`). |
| **قرارات أمنية واعية** | 404 بدل 403 لمنع تسريب وجود المورد. `scope` يتقدّم على `filter` عبر `else if`. فحص `IsActive` **بعد** التحقق من كلمة المرور (يمنع تعداد الحسابات الموقوفة). |
| **فصل التحقق عن منطق العمل** | لا ازدواجية بين FluentValidation (شكل) وفحوصات الخدمة (وجود/تفرّد/نطاق). |
| **تعلّم ذاتي موثَّق** | `LEARNING_MAP.md` + `My_Notes_UniNet_VS_ZadByan.txt`. مؤشر نضج مهني نادر في هذا المستوى. |

---

## 3. أنماط الضعف المتكررة — التشخيص الأهم

### 3.1 النمط الأول (الأخطر): تعميم ناقص — **الموجة السادسة، وأغلى تجلٍّ حتى الآن**

| الموجة | ما طُبِّق | ما نُسِي |
|---|---|---|
| 1–5 (سابقًا) | كيان ← نقطة نهاية ← وسيط دالة | (انظر تاريخ الملف) |
| **🆕 6** | دمج `Post` و`Announcement` في `ContentItem` مع TPH، وتعطيل الإعدادين القديمين | **لم يجرد ملاحيات `Image`**: `Image.Post` و`Image.Announcement` بقيتا تشيران لأنواع مشتقة، وإعداد `ContentItem` يربط `WithOne(i => i.Announcement)` بمفتاح `HasForeignKey<Announcement>`. **النتيجة: نموذج EF لا يُبنى إطلاقًا.** |

**التطوّر المزدوج — اقرأه بدقّة:**

- في بدائية النطاق: **شُفي النمط**. لم يعالج 12 موضعًا، بل غيّر التوقيع حتى لا توجد مواضع تُنسى. هذا أعلى مستوى من الإصلاح.
- في إعادة هيكلة المحتوى: **تكرر النمط بحذافيره** — عدّل مُنتِجًا (`Post`/`Announcement`) ولم يجرد مستهلكيه (`Image`).

**القراءة الصحيحة:** هو **يعرف** كيف يُصلح على مستوى النوع حين يُنبَّه؛ لكنه **لا يجرد تلقائيًا** حين يبادر. الجرد ما زال ردّ فعل لا عادة.

**العلاج (تغيّر مرة أخرى):** لم يعد `grep` كافيًا — لم يكن ليكشف §6.1، لأن العطل في **بناء النموذج** لا في نصّ الكود. المطلوب الآن:

> بعد أي تعديل على كيان أو إعداد EF، شغّل قبل الـ Commit:
>
> ```bash
> dotnet ef dbcontext info --project DataAccessLayer --startup-project UniNet
> ```
>
> ثانية واحدة، وكانت ستمنع §6.1 و§6.3 معًا. وإضافتها إلى `build.yml` تجعلها آلية لا تذكُّرًا.

### 3.2 النمط الثاني: أخطاء نسخ-ولصق غير مُتحقَّق منها — **قائم**

- **حارس ميت مُكرَّر أربع مرات:** `if (scope == null || (كل الحقول null)) scope = new UserScope();` في `CollegeRepository:98`، `BatchRepository:130`، `SectionRepository:141`، `StudentRepository:97`. صار الآن **مكرَّرًا وميتًا ومهجورًا معًا** — لأن `UserScope.IsGlobal` تؤدي الغرض في مكان واحد.
- **قاعدة `StudentNumber`** — **الجولة الرابعة.** أصلح الرسائل هذه الجولة (`d24bef7`) ونقل `.When()` إلى الحقل الصحيح، لكن `.When(x => !string.IsNullOrEmpty(x.StudentNumber))` ما زالت تُبطل `NotEmpty()` — الشرط والقاعدة يتناقضان (§6.8).
- **`filter.UniversityId` مُتجاهَل** في `UniversityRepository` وحدها — بند مفتوح منذ جولتين.

### 3.3 النمط الثالث: تجاهل مخرجات الأدوات — ✅ **عولج آليًا**

`TreatWarningsAsErrors` + CI. **أُغلق بندًا وآليةً.** لا يُعاد طرحه.

**لكن:** CI يبني ولا يشغّل شيئًا. أضِف خطوة `dotnet ef dbcontext info` إلى `build.yml`.

### 3.4 النمط الرابع: بقايا كود غير منظَّفة — **قائم وتراكمي**

- `services.AddAuthorization(...)` مُستدعاة **7 مرات** في `PolicyExtension.cs` — بلا تغيير.
- معامل إلزامي مُستقبَل ومُتجاهَل: `CollegeIdParameter @collegeParameter` في `DepartmentController.cs:188`.
- معامل `roleId` مُستقبَل ومُتجاهَل في `StudentService.UpdateStudent` الخاصة — يجعل `batch_admin` و`student` نقطتَي تحديث **متطابقتين تمامًا** (§6.7).
- **12 تسجيل Repository في DI ميتة** — `UnitOfWorkRepository` يبنيها يدويًا بـ `new`، ولا أحد يحقنها مباشرة.
- `IValidator<LoginRequest>` مسجَّل ولا يُستدعى أبدًا — `LoginRequestValidator` كود ميت (§6.9).
- `UserHelper.cs` و`ServicesExtensions.cs` ملفان فارغان. `ILoginService` واجهة بلا تنفيذ.
- ملفان XML بحجم **1.3 ميجابايت** في جذر المستودع.
- أخطاء إملائية في معرّفات عامة: `Coulmns`, `UnivresityId`, `Parametre`, `Requsets`, `ToPagedActioneResult`, `AccesseToken`, `proprty`, `RepoSitories`, `incommingHash`, `Dbcontext`.

### 3.5 فجوات معرفية (وليست أخطاء)

- **لا خبرة باختبارات الوحدة** — صفر. **الفجوة الوحيدة المتبقية، وقد صارت هذه الجولة الأغلى ثمنًا.**
- **`ILogger` في الـ Middleware فقط** — لم يصل لطبقة الخدمات.
- **لا خبرة تشغيل فعلي** — **مثبتة بالدليل هذه الجولة:** لو شُغِّل التطبيق مرة واحدة قبل الـ Commit، لظهر العطل فورًا.
- **لا Docker** (CI صار موجودًا).

---

## 4. معايير المعايرة للمراجعات المستقبلية

> **هذا هو القسم العملي — اقرأه قبل أي مراجعة قادمة.**

### 4.1 ما يُصنَّف 🔴 Critical

- ثغرة أمنية قابلة للاستغلال فعليًا (تجاوز نطاق، تصعيد صلاحيات، IDOR).
- خطأ يكسر وظيفة معلنة كمكتملة.
- خطأ توجيه/تعاقد يجعل نقطة نهاية تفعل شيئًا مختلفًا عن اسمها.
- سر مكشوف في مستودع Git.
- عطل يجعل ميزة وُصِّلت للتو غير قابلة للعمل بالبناء.
- **🆕 عطل يمنع التطبيق أو الهجرة من العمل أصلًا** — حتى لو نجح التصريف.

### 4.2 ما يُصنَّف 🟠 Important

- خطأ منطقي حقيقي لا يُستغل أمنيًا اليوم، أو ثغرة كامنة يحميها اليوم سببٌ عرضي لا تصميمي.
- غياب فحص `null` قد ينتج استثناءً غير معالج (500 بدل 400).
- تناقض بين ما تعلنه نقطة النهاية وما تفعله.
- قرارات مخطَّطة ومؤجَّلة عمدًا — **تُذكر كتذكير لا كلوم**.

### 4.3 ما يُصنَّف 🟡 Improvement

تكرار بنيوي، توحيد أسماء، أخطاء إملائية، كود ميت، تنظيف تعليقات. **لا يُطلب تنفيذها أثناء بناء ميزة** — تُجمَّع لتمريرة تنظيف.

### 4.4 ما يجب **عدم** طرحه إطلاقًا

- CQRS، MediatR، Event Sourcing، DDD الكامل، Microservices، GraphQL.
- Redis / Distributed Caching / Message Queues / Kubernetes / Observability stack.
- AutoMapper — أسلوبه اليدوي عبر `Expression` **أفضل** أداءً لحالته.
- Generic Repository — قرار واعٍ موثّق.
- أي Design Pattern لمجرد وجوده.
- **الواجهة الأمامية / `wwwroot`** — خارج نطاق المراجعة بقرار صريح من المطوّر (2026-08-19).

### 4.5 قواعد أسلوب المراجعة معه

1. **ابدأ بالتحقق الفعلي من الكود قبل الحكم.**
2. **ابنِ المشروع أولًا — و🆕 شغّل `dotnet ef dbcontext info` بعده.** البناء وحده لم يعد مؤشرًا كافيًا؛ هذه الجولة أثبتت ذلك.
3. **عند مراجعة تعديل على نوع/دالة مشتركة، اجرد مواضع الاستدعاء بـ `grep` قبل أي شيء آخر.**
4. **قدّم الأدلة بأرقام أسطر** — يتعامل معها جيدًا ويتحقق بنفسه.
5. **لا مجاملة ولا تهويل** — طلب صراحةً الصراحة، ويستجيب لها بإصلاحات فعلية سريعة.
6. **اشرح "لماذا" لا "ماذا"** — يستوعب التعليل ويبني عليه، وغالبًا يتجاوزه.
7. **حين يطلب الإرشاد دون كود** — التزم حرفيًا.
8. **اعترف بما أنجزه صراحةً قبل ذكر ما تبقّى.** هذه الجولة تحديدًا: إنجازه في §2 حقيقي وكبير، ولا يجوز أن تبتلعه بنود §6.
9. **حين تُحذِّر من ترتيب إصلاح إلزامي، افترض أنه قد يُنفَّذ بترتيب معكوس.**
10. **🆕 لا تفترض أن نجاح البناء يعني أن الكود يعمل.** آخر Commit يبني بصفر تحذيرات ولا يُقلع.

---

## 5. لقطة حالة المشروع

**الحجم:** ~10,400 سطر C# (خارج الـ Migrations) · 5 مشاريع · 12 متحكمًا · 13 مستودعًا · 7 معالجات ملكية · **0 اختبارات** · **0 تحذيرات** · هجرتان.

### مُنجَز ومُستقر

- الهيكل الأكاديمي (University → College → Department → Batch → Section): CRUD كامل + فلترة + ترقيم + نطاق.
- الهوية والمصادقة: JWT + Refresh Token مع تدوير و Hash + BCrypt + أدوار. تسجيل دخول الأنواع الثلاثة يعمل.
- ميزتا Employee و Student، ودور `BatchAdmin` صار له معنًى محدَّد: **طالب** يدير دفعته (`StudentService.AddBatchAdmin`).
- بدائية النطاق موحّدة عبر `GeneralAuthorizationInfo` + `UserScope.IsGlobal` — 21 موضع استدعاء متّسق.
- `GetAll*` مقيَّدة بـ `Super Admin` في الكيانات الخمسة.
- `ILogger` في `GlobalExceptionHandlingMiddleware` مع تمييز `LogWarning`/`LogError`، ومعالجة `SqlException 547`.
- `TestDataSeeder` للتطوير (idempotent, BCrypt).
- **مخطط Study كامل ومُفهرَس بشكل صحيح** (Semester / Subject / SectionSubject / StudentResult).
- `TreatWarningsAsErrors` + GitHub Actions.

### لم يبدأ بعد (ليس عيبًا — خارج النطاق الحالي)

- **طبقة تطبيق Study**: صفر Repository/Service/Controller لـ `Semester`/`Subject`/`SectionSubject`/`StudentResult`. **أهم قيمة وظيفية متبقية.**
- `ContentItem` (Post/Announcement/Image): مُهيكَل بلا تنفيذ — وهو الآن **مكسور** (§6.1).
- مشروع اختبارات، `ILogger` في الخدمات، تدوير مفتاح JWT، Rate limiting.

---

## 6. البنود المفتوحة — مرتبة حسب الأولوية

كلها **مُتحقَّق منها في الكود** عند Commit `d24bef7`.

### ✅ أُغلقت هذه الجولة (تحقّقتُ منها بنفسي — لا تُعِد فتحها)

| البند السابق | كيف أُغلق |
|---|---|
| 🔴 §6.1 — 12 موضع استدعاء ثلاثي الوسائط لـ `IsWithinScope` | **أُلغيت المشكلة من جذرها**: التوقيع صار `(UserScope?, GeneralAuthorizationInfo)`، و21 موضعًا كلها متّسقة الآن |
| 🔴 §6.2 — الحارس يفحص 3 حقول من 4 | صار `scope.IsGlobal` خاصية محسوبة على `UserScope` تشمل `BatchId` |
| 🟠 §6.3 — `BatchAdmin` بلا معنى | حُسِم: `BatchAdmin` = **طالب** يدير دفعته. يحمل `BatchId` عبر `StudentAuthorizationInfo`. (يبقى عيب البذر — §6.5) |
| 🟠 §7.2-2 — `Semester` بلا `UniversityId` | أُضيف + فهرس فريد `(UniversityId, Name)` + فهرس مُصفّى `IsCurrent` لكل جامعة |
| 🟠 §7.3-1,2 — لا فهارس فريدة على `SectionSubject`/`StudentResult` | أُضيفت كلتاهما |
| 🟠 §7.3-4 — `StudentResult` لا يرث `BaseEntity` | يرثه الآن، و`EnteredByUserId` صار `CreatedByUserId` بهجرة نظيفة |
| 🟠 §7.3-5 — لا حدود على الدرجات | `SetGrades()` + `GradeConstants` (جزئيًا — §6.7) |
| 🟡 §3.3 — الانضباط يدوي | `TreatWarningsAsErrors` + `build.yml`. 3 تحذيرات ← 0 |
| 🟡 الفجوة #2 — لا CI | `.github/workflows/build.yml` يعمل |
| — §6.5 السابق (`wwwroot`) | **خارج النطاق بقرار المطوّر** — لا يُذكر مجددًا |

---

### 🔴 6.1 — نموذج EF لا يُبنى: التطبيق لا يعمل

**الموقع:** `DataAccessLayer/Configurations/ContentConfigurations/ContentItemConfiguration.cs:20-21` + `Domain/Entities/Images/Image.cs:16-17`

```text
dotnet ef dbcontext info --project DataAccessLayer --startup-project UniNet
→ The navigation 'Announcement' cannot be added to the entity type 'Image'
  because its CLR type 'Announcement' does not match the expected CLR type 'ContentItem'.
```

**السبب:** بعد دمج `Post` و`Announcement` في `ContentItem` (TPH)، بقيت في `Image` ملاحيتان منفصلتان:

```csharp
public Post? Post { get; set; }
public Announcement? Announcement { get; set; }
```

وإعداد الأساس يربطهما هكذا:

```csharp
builder.HasOne(c => c.Image).WithOne(i => i.Announcement)
       .HasForeignKey<Announcement>(p => p.ImageId);   // ImageId مُعرَّف على ContentItem لا Announcement
```

في TPH لا يمكن إعلان مفتاح أجنبي على نوع مشتق لخاصية معرَّفة على الأساس، ولا يمكن لملاحية عكسية أن تشير لنوع مشتق في علاقة معرَّفة على الأساس. و`Image.Post` لم تُعرَّف أصلًا بعد تعطيل `PostConfiguration`.

**الأثر:** في التطوير، `Program.cs:184` يستدعي `GetRequiredService<AppDbcontext>()` → **استثناء عند الإقلاع**. في الإنتاج، أول طلب يلمس قاعدة البيانات → 500. **كل نقاط النهاية معطّلة.**

**لماذا لم يمسكه أي شيء:** التصريف ينجح، والتحذيرات صفر، وCI يبني فقط. `PostConfiguration` و`AnnouncementConfiguration` مُعطَّلان بالتعليق فلا يُصرَّفان.

**الإصلاح:** استبدل ملاحيتَي `Image` بواحدة `public ContentItem? ContentItem { get; set; }`، واربط الطرف على الأساس:

```csharp
builder.HasOne(c => c.Image).WithOne(i => i.ContentItem)
       .HasForeignKey<ContentItem>(c => c.ImageId)
       .IsRequired(false).OnDelete(DeleteBehavior.Restrict);
```

ثم ولّد هجرة — لا توجد هجرة للدمج أصلًا.

---

### 🔴 6.2 — `refresh` يُسقِط `UniversityId` فيمنح `UniversityAdmin` صلاحيات عالمية

**الموقع:** `DataAccessLayer/Repos/RefreshTokenRepository.cs:18-29`

`ToDetaieldTokenDTO` تُسقِط كل حقل عدا `UniversityId`:

```csharp
t => new UserToken { RefreshTokenId = …, Type = t.User.Type, TokenHash = …,
                     UserId = …, UserName = t.User.UserName, … }
                     //  UniversityId ← غير مُسنَد، يبقى null
```

**السلسلة الكاملة:**

`POST /api/Login/refresh`

→ `UserToken.UniversityId = null`

→ `TokenUserInfoMapper.ToInfoDTO(UserToken…):103` يمرّر `UniversityId = token.UniversityId` = `null`

→ `JwtTokenFactory:24` لا يُصدر مطالبة `UniversityId`

→ `UniversityAdmin` ليس له `CollegeId` ولا `DepartmentId` (`CreateEmployee` يمرّرهما `null`)

→ الرمز الجديد **بلا أي مطالبة نطاق**

→ `ToUserScope()` → `UserScope.IsGlobal == true`

→ `IsWithinScope` تُرجع `true` لكل مورد في النظام

**سيناريو الاستغلال (خطوتان، بلا أدوات):**

1. `UniversityAdmin` لجامعة A يسجّل الدخول ثم يستدعي `/api/Login/refresh` مرة واحدة.
2. بالرمز الجديد: `DELETE /api/College?CollegeId=<كلية في جامعة B>` → **ينجح**. و`GET /api/College/by-universityId` يُرجع **كليات كل الجامعات**. و`GET /api/Employee` يُرجع **كل الموظفين**. و`POST /api/Employee/college_admin` ينشئ مسؤولًا في جامعة أخرى.

**لماذا لم تنكشف:** فحوص `[Authorize(Roles=…)]` ما زالت تمرّ (الدور لم يتغيّر)، والعطل صامت تمامًا. و`UniversityOwnerHandler` وحده ينجو لأنه **لا يستخدم** `IsWithinScope` بل يقارن مباشرة.

**الإصلاح:** أضف `UniversityId = t.User.UniversityId` إلى `ToDetaieldTokenDTO`. سطر واحد.

**الدرس البنيوي (الأهم):** `IsGlobal` تعني «بلا نطاق ⇒ صلاحية كاملة». هذا **fail-open**. أي مسار يفقد المطالبات يتحوّل تلقائيًا إلى Super Admin. الأمتن: أن يكون العالمي **دورًا صريحًا** (`context.User.IsInRole("Super Admin")`) لا **غياب بيانات**.

---

### 🔴 6.3 — الهجرة الثانية تفشل على أي قاعدة بيانات

**الموقع:** `DataAccessLayer/Migrations/20260810081206_Update-StudyConfigurations.cs`

```csharp
migrationBuilder.AddColumn<int>("UniversityId", "Semesters", nullable: false, defaultValue: 0);
migrationBuilder.UpdateData("Semesters", "SemesterId", 1, "UniversityId", value: 0);
// …
migrationBuilder.AddForeignKey("FK_Semesters_Universities_UniversityId",
    "Semesters", "UniversityId", "Universities", "UniversityId", ReferentialAction.Restrict);
```

`SeedData.GetSemesters():210-222` لا تُسنِد `UniversityId` → القيمة 0 → لا توجد جامعة بمعرّف 0 → **إنشاء المفتاح الأجنبي يفشل بخطأ 547**.

**الأثر:** `dotnet ef database update` يفشل على قاعدة جديدة وعلى قاعدة قائمة معًا. مع §6.1، **لا توجد اليوم طريقة لتشغيل المشروع من الصفر.**

**الإصلاح:** أسنِد `UniversityId = 1` في `SeedData.GetSemesters()`، وأعد توليد الهجرة.

---

### 🔴 6.4 — `UpdateCollegeAdmin` / `UpdateDepartmentAdmin`: اختطاف موظف عبر المستأجرين

**الموقع:** `Application/Services/EmployeeService/EmployeeService.cs:182-212` و`224-257`

الفحص يتم على **الكلية/القسم القادم في جسم الطلب**، لا على **الموظف المستهدَف**:

```csharp
var collegeInfo = await …GetCollegeAuthorizationInfo(updatedCollegeAdmin.CollegeId);
if (!scope.IsWithinScope(collegeInfo.ToCollegeInfo())) return …;   // يفحص الوجهة

var employee = await GetEntityById(employeeId);                     // ← بلا أي فحص نطاق
employee.CollegeId    = collegeInfo.CollegeId;
employee.UniversityId = collegeInfo.UniversityId;
await UpdateEmployee(user, updatedCollegeAdmin, currentUserId);     // يكتب فوق بيانات المستخدم
```

**سيناريو:** `UniversityAdmin` لجامعة A يرسل:

`PUT /api/Employee/college_admin?EmployeeId=<موظف في جامعة B>`

مع `CollegeId` = كلية داخل جامعة A.

→ فحص النطاق يمرّ (الوجهة داخل نطاقه)

→ الموظف الأجنبي **يُنقل إلى جامعة A**

→ `UpdateEmployee` يكتب فوق `FullName` و`UserName` و`Email` و`PhoneNumber` و`IsActive`

→ بإرسال `IsActive = false` يُعطَّل حساب مسؤول في جامعة أخرى.

**عيب سلامة إضافي:** `UpdateCollegeAdmin` يغيّر `CollegeId` ولا يمسّ `DepartmentId` → يمكن أن يصبح للموظف قسمٌ لا ينتمي لكليته. لا قيد في قاعدة البيانات يمنع ذلك (لا مفتاح أجنبي مركّب).

**النسخة الصحيحة موجودة في المشروع نفسه:** `StudentService.UpdateStudent:218-231` يجلب الكيان أولًا ثم يشتق `batchInfo` **منه** ثم يفحص النطاق. **هذا هو الترتيب الصحيح.**

**الإصلاح:** اجلب `employee` أولًا، اشتق `EmployeeAuthorizationInfo` منه، افحص `IsWithinScope` عليه، **ثم** افحص الوجهة.

---

### 🟠 6.5 — `BatchAdmin` غير مزروع في هجرة الإنتاج

`SeedData.GetRoles():24-31` تزرع ستة أدوار — **بلا `BatchAdmin`**. و`TestDataSeeder:238` تزرعه، لكنها تعمل في التطوير فقط.

**الأثر:** على قاعدة إنتاج، `POST /api/Student/batch_admin` يُرجع 404 `Role Doesnt Exists`، ودور مذكور في **10 سمات `[Authorize]`** لا يمكن لأحد امتلاكه.

**الإصلاح:** `new Role { RoleId = 7, Name = "BatchAdmin" }` + هجرة.

---

### 🟠 6.6 — `GetSectionsPerBatch` لا تُصفّي بـ `scope.BatchId`

**الموقع:** `DataAccessLayer/Repos/AcademicRepositories/SectionRepository.cs:336-393`

الدالة تُصفّي بـ `UniversityId` و`CollegeId` و`DepartmentId` — و`BatchId` **غائبة**. و`BatchAdmin` **مصرَّح له** على هذه النقطة (`SectionController.cs:313`)، وهو يحمل `BatchId` فعلًا لأنه طالب.

**الأثر:** `BatchAdmin` يرى **كل شُعَب قسمه**، لا شُعَب دفعته فقط. تسريب معلومات عبر الدفعات.

**النسخة الصحيحة موجودة في المشروع:** `StudentRepository.GetStudents:100-125` تتعامل مع `scope.BatchId` بفرع `else` صريح.

**ملاحظة:** `BatchRepository.GetBatchesPerDepartment` بها الإغفال نفسه، لكنها غير مستغَلّة اليوم لأن `BatchAdmin` غير مصرَّح له عليها. **أصلحهما معًا** — هذا حرفيًا نمط §3.1.

---

### 🟠 6.7 — بنود منطق ودقّة متفرقة

| # | البند | الموقع | التفصيل |
|---|---|---|---|
| 1 | **`UpdateStudent` يُعيد تعيين الحالة قسرًا** | `StudentService.cs:258` | `studentEntity.StatusId = statusId;` حيث `statusId` هو دائمًا "Enrolled". تحديث بيانات طالب **متخرّج أو موقوف** يُعيده إلى "مُقيَّد" صامتًا. |
| 2 | **`batch_admin` و`student` نقطتا تحديث متطابقتان** | `StudentService.cs:211` | المعامل `roleId` مُستقبَل وغير مستخدم. `UpdateBatchAdmin` لا يمنح ولا يتحقق من دور `BatchAdmin`. |
| 3 | **`sectionInfo!` → 500 بدل 400** | `StudentService.cs:161, 245` | `SectionId` غير موجود ⇒ `GetSectionAuthorizationInfoAsync` تُرجع `null` ⇒ `NullReferenceException`. |
| 4 | **`UpdateUser` يُرجع بيانات قديمة** | `UserService.cs:141-142` | `FindById(...)` تُستدعى **قبل** `CompleteAsync()`. استعلامات EF لا تُفرِّغ التغييرات المعلّقة ⇒ الاستجابة تحمل القيم السابقة. كل الخدمات الأخرى تعكس الترتيب. |
| 5 | **`AddUser` لا يُسنِد `Type`** | `UserService.cs:46-57` | مستخدم بـ `Type == null` ⇒ `GetAuthorizationInfoResult` تُرجع `null!` ⇒ `ToInfoDTO` تسقط إلى `new TokenUserInfoDTO()` ⇒ `UserName == null` ⇒ **`ArgumentNullException` في `JwtTokenFactory` ⇒ 500 عند تسجيل الدخول.** ينطبق أيضًا على المستخدم المزروع في `SeedData.GetUsers()`. |
| 6 | **`refresh` لا يفحص `IsActive`** | `LoginController.cs:75-107` | حساب مُعطَّل يظل يُصدر رموز وصول حتى 7 أيام. `Login` يفحص، `Refresh` لا. |
| 7 | **`SetGrades` قابل للتجاوز** | `StudentResult.cs:14-16, 26` | `Midterm`/`Practical`/`Final` لها `set` عام، فالحارس اختياري لا إلزامي. كما أن `ArgumentException(nameof(practical))` تمرّر اسم المعامل مكان الرسالة. لا قيد `CHECK` في قاعدة البيانات كشبكة أمان. |
| 8 | **هاش كلمة مرور تالف في البذور** | `SeedData.cs:41` | `"AQAAAAEAACcQ…"` صيغة ASP.NET Identity لا BCrypt ⇒ `BCrypt.Verify` يرمي `SaltParseException` ⇒ **500 بدل 401**. |

---

### 🟠 6.8 — تناقضات في العقد (بنود قائمة من الجولة السابقة)

| # | البند | الموقع | الحالة |
|---|---|---|---|
| 1 | **409 يُرجَع كـ 400** | `ControllersExtensions.cs:53-57` | ما زال. الجسم يقول `StatusCode = 409` والحالة الفعلية 400. |
| 2 | **`Add*` تُرجع 200 لا 201** | `ControllersExtensions.cs:21` | ما زال. لا `201 Created` ولا رأس `Location` رغم `[ProducesResponseType(201)]` على 9 نقاط. |
| 3 | **معامل إلزامي مُتجاهَل** | `DepartmentController.cs:188` | ما زال. |
| 4 | **`filter.UniversityId` مُتجاهَل** | `UniversityRepository.cs:59-81` | ما زال. |
| 5 | **`filter.DepartmentId` مُتجاهَل** | `DepartmentRepository.GetDepartmentsPerCollege` | ما زال. |
| 6 | **`StudentNumber`: `.When` تُبطل `NotEmpty`** | `AddStudentValidator.cs:62-67` | **الجولة الرابعة.** الرسائل صُحِّحت والشرط نُقل للحقل الصحيح — لكن `.When(!IsNullOrEmpty(StudentNumber))` تجعل `NotEmpty()` بلا معنى. `StudentNumber = ""` يمرّ. |
| 7 | **`PageSize` بلا حدّ أعلى** | `PagedResultParameters.cs:12` | `[Range(1, int.MaxValue)]` يسمح بـ `PageSize = 2147483647` ⇒ مسح كامل للجدول. |

---

### 🟡 6.9 — تنظيف (تُجمَّع لتمريرة واحدة)

| البند | الموقع |
|---|---|
| حارس ميت مُكرَّر 4 مرات (أُغني عنه بـ `IsGlobal`) | `CollegeRepository:98`, `BatchRepository:130`, `SectionRepository:141`, `StudentRepository:97` |
| `services.AddAuthorization()` ×7 بدل مرة واحدة | `PolicyExtension.cs` |
| 7 سياسات متطابقة بلا مُميِّز — نوع المورد هو الفارق الفعلي لا اسم السياسة | `PolicyExtension.cs` + المعالجات |
| 12 تسجيل Repository ميت في DI | `RepositoriesToDIContainer.cs` |
| `IValidator<LoginRequest>` مسجَّل ولا يُستدعى؛ وقاعدة كلمة المرور فيه (`^[a-zA-Z0-9_]+$`) **تناقض** قاعدة الإنشاء (تُلزم برمز خاص) | `AddServicesToDIContainer.cs:79` + `LoginRequestValidator.cs` |
| `AddSwaggerGen()` مُستدعاة مرتين | `Program.cs:122-123` |
| Middleware الاستثناءات مسجَّل بعد CORS/StaticFiles | `Program.cs:199` |
| فهرس فريد مُكرِّر للمفتاح الأساسي | `UserRoleConfigurations.cs` |
| ملفان فارغان + واجهة بلا تنفيذ | `UserHelper.cs`, `ServicesExtensions.cs`, `ILoginService` |
| `IUnitOfWorkRepository : IDisposable` يتخلّص من `DbContext` تملكه الحاوية | `UnitOfWorkRepository.cs:94` |
| تناقض `AsNoTracking`: موجود في `CollegeRepository.GetCollegeDTOById` ومفقود في `DepartmentRepository.GetDTOById` و`BatchRepository.GetDTOById` | — |
| `GradeConstants` في مجلد `DomainConstant` بينما مساحة الاسم `StudyConstants` | `GradeConstants.cs` |
| ملفان XML بحجم 1.3MB في جذر المستودع | `StudentBranch.xml`, `UniNet(StatusBranch).xml` |
| أخطاء إملائية في معرّفات عامة | `Coulmns`, `UnivresityId`, `Parametre`, `Requsets`, `AccesseToken`, `proprty`, `RepoSitories`, `Dbcontext` |

---

## 7. الترتيب الموصى به للعمل القادم

> **القاعدة:** لا سطر جديد في وحدة Study قبل أن يُقلع التطبيق ويُثبَّت النطاق.

### المرحلة صفر — استعادة القابلية للتشغيل (ساعة واحدة، إلزامية)

1. **§6.1** — أصلح ملاحيات `Image` وإعداد `ContentItem`، ثم `dotnet ef dbcontext info` حتى ينجح.
2. **§6.3** — `UniversityId = 1` في بذور `Semester`، وأعد توليد الهجرة.
3. **ولّد هجرة للدمج** (`ContentItems`) — لا توجد واحدة اليوم.
4. **أضف إلى `build.yml`** خطوةً تشغّل `dotnet ef dbcontext info`. هذا يحوّل §6.1 من عطل يمرّ إلى عطل يُوقف الدمج.

### المرحلة الأولى — إغلاق الأمن (ساعتان)

5. **§6.2** — سطر واحد في `ToDetaieldTokenDTO`. **ثم فكّر جديًا في جعل «العالمي» دورًا صريحًا بدل غياب نطاق.**
6. **§6.4** — اقلب ترتيب الفحص في دالتَي تحديث الموظف لتطابق `StudentService.UpdateStudent`.
7. **§6.6** — أضف فرع `scope.BatchId` في `SectionRepository` **و**`BatchRepository` معًا.
8. **§6.7-6** — فحص `IsActive` في `refresh`.

### المرحلة الثانية — الاختبارات (نصف يوم، وهي أعلى عائد في المشروع كله)

9. مشروع xUnit + خمسة اختبارات فقط:
   - اختبار واحد يبني `AppDbcontext` (`OnModelCreating`) → **كان سيمنع §6.1**.
   - ثلاثة على `IsWithinScope` (عالمي / نطاق مطابق / نطاق مخالف).
   - واحد على `EmployeeService.UpdateCollegeAdmin` بموظف خارج النطاق → **كان سيمنع §6.4**.

> **الربط الذي ينبغي أن يقتنع به:** في الجولة السابقة قيل إن الاختبارات «الآلية الوحيدة التي تجرد المستهلكين نيابةً عنه». هذه الجولة قدّمت الدليل: بنى آليتَي انضباط (تحذيرات + CI)، وكلتاهما تقيسان التصريف، فمرّ عطلٌ لا علاقة له بالتصريف. **الاختبار الأول أعلاه سطرٌ واحد ويغطي فئة العطل كلها.**

### المرحلة الثالثة — ثم Study

10. `SubjectRepository` → `SemesterService` → … بعد أن يصبح الأساس قابلًا للتشغيل ومختبَرًا.

**قبل `StudentResult` تحديدًا، احسم:** مَن يُدخِل الدرجات؟ `Lecturer` مزروع بلا صلاحيات، و`SectionSubject.LecturerName` نصّ حرّ بلا مفتاح أجنبي. السؤال بلا جواب منذ ثلاث جولات.

---

## 8. مؤشرات التقدّم المرصودة

| المؤشر | جولة `7aeac1e` | جولة `d24bef7` | الاتجاه |
|---|:---:|:---:|:---:|
| **تحذيرات البناء** | 3 | **0** + `TreatWarningsAsErrors` | ↗↗ **مثبَّت آليًا** |
| **CI** | لا يوجد | `build.yml` يعمل | ↗↗ 🆕 |
| **جودة الإصلاح** | ممتازة | **ممتازة** — `GeneralAuthorizationInfo` أفضل من كل الخيارات المقترحة | ↗ |
| **ديون مخطط قاعدة البيانات** | 5 بنود مفتوحة | **0** | ↗↗ |
| **جرد مستهلكي التعديل** | ضعيف (12 موضعًا) | **مزدوج**: مثالي في النطاق، غائب تمامًا في `Image` | ↔ |
| **التطبيق يعمل** | ✅ نعم | ❌ **لا** | ↘↘ 🆕 **أخطر تراجُع** |
| **الهجرة تعمل** | ✅ نعم | ❌ **لا** | ↘↘ 🆕 |
| **بنود 🔴 مفتوحة** | 2 | **4** (كلها جديدة — القديمة أُغلقت) | ↘ |
| **بنود قديمة عالقة** | `StudentNumber` (3) | `StudentNumber` (**4**)، 409/201، معاملات مُتجاهَلة | → |
| **الاختبارات** | 0 | 0 | → |

**القراءة الدقيقة:** الجولة السابقة أثبتت أنه ينجز «العمل الممل» حين يُطلَب منه صراحةً. هذه الجولة تضيف تمييزًا أدقّ: **ينجز الممل المُوجَّه، ولا ينجز الممل غير المطلوب** — أي التحقق من أن ما بناه يعمل. البنود الأربعة 🔴 كلها من نوع «كان سيظهر عند أول تشغيل».

**السؤال للمراجعة القادمة:**

> هل أصبح `dotnet ef dbcontext info` (أو أول اختبار وحدة) جزءًا من `build.yml`؟
>
> إن نعم، فقد أُغلق نمط §3.1 آليًا لا سلوكيًا، وهذه هي الترقية الكاملة إلى Mid-level.

---

## About Developer

> قسم مستقل موجَّه للقارئ الخارجي (مُوظِّف، قائد فريق، أو المطوّر نفسه عند إعداد سيرته). كل ما فيه مستخلَص من الكود المراجَع، لا من تصريحات المطوّر.

### الملف المهني في سطرين

مطوّر **.NET Backend** يبني — منفردًا — نظام إدارة جامعية متعدد المستأجرين (Multi-Tenant) بمعمارية طبقية مفروضة على مستوى المشاريع، مع نظام مصادقة وتفويض مخصّص من الصفر. **يفكّر كمعماري، ويُصلح كمهندس، ويتحقق كمبتدئ** — أعاد تصميم بدائية التفويض بحلٍّ يقتل فئة الخطأ كلها، ثم دفع في اليوم نفسه Commit يترك التطبيق غير قابل للإقلاع.

### التقنيات المُثبَتة بالكود

| المجال | التقنيات | مستوى الإتقان الظاهر |
|---|---|---|
| **اللغة والمنصّة** | C# 12, .NET 8, ASP.NET Core Web API | **جيد جدًا** — nullable reference types، pattern matching (`is SqlException { Number: 547 }`)، expression trees، أنواع نتائج مُميَّزة، خصائص محسوبة كبدائل تفويض |
| **قواعد البيانات** | EF Core 8, SQL Server, Migrations, Fluent API | **جيد جدًا — أقوى مهاراته.** Projection عبر `Expression<Func<>>`، `AsNoTracking`، فهارس مركّبة ومُصفّاة، أعمدة محسوبة مخزَّنة، TPH، `DeleteBehavior.Restrict` كسياسة. **التحفّظ الوحيد: لا يتحقق من أن النموذج يُبنى بعد التعديل.** |
| **المعمارية** | Layered/Clean-ish, Repository + Unit of Work, Result Envelope | **جيد جدًا** — الفصل مفروض بمراجع المشاريع لا بالاتفاق |
| **الأمن** | JWT Bearer, Refresh Token Rotation (SHA-256 at rest), BCrypt, Claims-Based Scope, Resource-Based Authorization | **جيد جدًا على التصميم، متوسط على التغطية** — الآليات محترفة والبدائية صارت موحّدة؛ الفجوة في مسارات لم تُراجَع (`refresh`، تحديث الموظف) |
| **التحقق** | FluentValidation | **جيد** — فصل واعٍ بين تحقق الشكل ومنطق العمل |
| **الأدوات** | Git (فرع لكل ميزة), GitHub Actions, Swagger, .NET User Secrets, EF CLI, MSBuild (`Directory.Build.props`) | **متوسط إلى جيد — تحسّن ملموس هذه الجولة** |
| **التوثيق الذاتي** | `CLAUDE.md`, `LEARNING_MAP.md`, `DEVELOPER_PROFILE.md` | **ممتاز — نادر في أي مستوى** |

### المهارات غير التقنية (مُثبَتة بالسلوك)

- **✅ يستقبل النقد الصريح ويحوّله إلى كود خلال دورة واحدة.**
- **✅ يُحسِّن الحل المقترح بدل نسخه — أربع مرات موثَّقة.** آخرها `GeneralAuthorizationInfo`: بدل إصلاح 12 موضعًا، غيّر التوقيع فصار الإغفال مستحيلًا.
- **✅ يبني آليات لا يعتمد على الذاكرة.** `TreatWarningsAsErrors` + CI في جولة واحدة.
- **⚠️ لا يشغّل ما يكتب.** أخطر فجوة حالية، ومثبتة بأربعة بنود 🔴 كلها كانت ستظهر عند أول إقلاع.
- **⚠️ لا يجرد مستهلكي ما يُعدِّله — حين يبادر.** يجرد ممتازًا حين يُنبَّه.
- **⚠️ يترك بنودًا صغيرة معلَّقة لأربع جولات** (`StudentNumber`) بينما يغلق البنود الكبيرة فورًا.

### الفجوات المهارية — مرتبة حسب أثرها على التوظيف

| # | الفجوة | لماذا تهم | الجهد |
|---|---|---|---|
| 1 | **لا اختبارات وحدة إطلاقًا** | أول سؤال في أي مقابلة Backend جادّة. **والأهم: خمسة اختبارات كانت ستمنع ثلاثة من بنود 🔴 الأربعة في هذه الجولة** | نصف يوم لأول خمسة، أسبوع لعشرين |
| 2 | **لا تحقق من التشغيل قبل الدفع** | البناء الأخضر لا يعني تطبيقًا يعمل. هذا **سلوك** لا معرفة | خطوة واحدة في CI |
| 3 | **Logging سطحي** | `ILogger` في الـ Middleware فقط | يومان |
| 4 | **لا Docker** | معيار شائع؛ CI صار موجودًا فبقي التغليف | يوم |

> **الفجوات كلها إجرائية لا معرفية.** مجموع الجهد أقل من أسبوع، وأثرها على جاهزيته أكبر من بناء وحدة Study كاملة.

### نسبة الاستعداد للوظيفة الأولى أو التدريب

| المسار | النسبة | التغيّر | التبرير |
|---|:---:|:---:|---|
| **تدريب (Internship / Trainee)** | **97%** | → | جاهز الآن بلا تحفّظ |
| **وظيفة Junior .NET Backend أولى** | **85%** | → | جاهز تقنيًا. مكسب CI و`TreatWarningsAsErrors` (+5) عادله انكشاف فجوة «لا يشغّل ما يكتب» (−5). **يصل إلى 95% بنصف يوم: إصلاح §6.1–6.4 + خمسة اختبارات** |
| **وظيفة Mid-level مباشرة** | **58%** | ▲ 3 | إعادة تصميم `IsWithinScope` وحدها عمل Mid-level حقيقي، ومخطط قاعدة البيانات فوق المستوى. يمنعه: صفر اختبارات، ودفع كود غير مُشغَّل |

**تقدير صادق:** لو تقدّم غدًا لوظيفة Junior، سيتفوّق بوضوح في أسئلة EF Core والمعمارية وتصميم التفويض. **وسيتعثّر** في سؤالين: «أرِني اختباراتك» و«كيف تتأكد أن ما دفعته يعمل؟». وهذه الجولة أعطت السؤال الثاني وزنًا لم يكن له من قبل.

**نقطة قوة تفاوضية ينبغي أن يستخدمها صراحةً:** أقوى قصة يرويها ليست ميزة بناها، بل **عيبٌ في التفويض أُبلِغ عنه في 12 موضعًا، فبدل إصلاح الاثني عشر أعاد تصميم التوقيع حتى صار العيب غير قابل للكتابة أصلًا** (`GeneralAuthorizationInfo` + `IsGlobal`). هذه قصة مهندس، لا كاتب كود. وأصدق ما يُتبعها به: «ثم اكتشفت في المراجعة التالية أنني دفعت كودًا لا يقلع، فأضفت التحقق من النموذج إلى CI.»

### الأقسام التي يمكنه المساهمة فيها بفاعلية في مشاريع Production

**🟢 من اليوم الأول، بمراجعة كود عادية:**

- **طبقة الوصول للبيانات (EF Core)**: Repositories، استعلامات Projection، تكوينات Fluent API، فهرسة، Migrations. **أقوى مناطقه.**
- **بناء نقاط نهاية CRUD كاملة** ضمن نمط قائم — 12 متحكمًا متّسقًا.
- **تصميم عقود البيانات** (DTOs/Requests/Responses) وقواعد FluentValidation.
- **استعلامات الفلترة والترقيم.**
- **إصلاح العيوب المُبلَّغ عنها** — سرعة وجودة استجابة عالية بشكل متكرر ومثبت.

**🟡 بإشراف ومراجعة أدقّ:**

- **ميزات المصادقة والتفويض** — التصميم قوي؛ يحتاج مراجعًا ثانيًا يجرد المسارات الجانبية (`refresh` مثال حيّ).
- **إعادة هيكلة كود قائم** — قراراته ممتازة، والتحقق بعدها هو نقطة الضعف (دمج `ContentItem` مثال حيّ).
- **تصميم معماري لوحدة جديدة.**

**🔴 لا يُسنَد إليه منفردًا حاليًا:**

- **أي دمج إلى `master` بلا بوابة تشغيل آلية** — ليس لضعف الفهم، بل لأن التحقق من التشغيل لم يصبح عادة بعد.
- **البنية التحتية / DevOps / النشر** — بدأ (CI) ولم ينضج.
- **كتابة اختبارات لنظام قائم** — لا خبرة (وهي أول ما ينبغي تدريبه عليه، والعائد فوري).

### التوصية الختامية

> مطوّر **يستحق الاستثمار فيه**، لا مجرد التوظيف. وهذه الجولة قدّمت الدليلين معًا في يوم واحد: أفضل إصلاح تصميمي في تاريخ المشروع، وأسوأ Commit من حيث القابلية للتشغيل.
>
> ما ينقصه **عادة واحدة** لا مفاهيم: **أن يشغّل ما يكتب قبل أن يدفعه** — وأرخص صورة لهذه العادة هي اختبار واحد وسطر في CI.
>
> **أقصر طريق أمامه — بهذا الترتيب:**
>
> 1. §6.1 + §6.3 — أعِد التطبيق للعمل (ساعة).
> 2. §6.2 + §6.4 + §6.6 — أغلق الأمن (ساعتان).
> 3. خمسة اختبارات xUnit، أولها يبني `AppDbcontext` (نصف يوم).
> 4. خطوة `dotnet ef dbcontext info` + تشغيل الاختبارات في `build.yml` (نصف ساعة).
> 5. **ثم** وحدة Study.
>
> يومان يرفعان جاهزيته من 85% إلى 95% — عائد لا تعطيه أي ميزة جديدة.
