# ملف مستوى المطوّر — مرجع معايرة المراجعات (UniNet)

> **الغرض من هذا الملف:** أساس ثابت تُبنى عليه مراجعات الجلسات المستقبلية، بحيث تكون المعايير مطابقة لمستوى المطوّر الفعلي — لا متشددة بمعايير Senior Architect، ولا متهاونة بمعايير مبتدئ.
>
> **تاريخ آخر تحديث:** 2026-08-07 · **الفرع وقتها:** `refactor/v1Branch` · **آخر Commit مُقيَّم:** `25c99d2`
> **الجولة السابقة:** `d58be80` (2026-08-06) · **البناء وقت المراجعة:** ناجح، **10 تحذيرات** (نفس العدد ونفس المواقع تقريبًا منذ الجولة السابقة).
> **حالة المشروع:** قيد التطوير النشط (v1/MVP) — ليس منتجًا جاهزًا للنشر.

---

## 1. التقييم العام للمستوى

**التصنيف: Strong Junior — على حافة Mid-level، لكن لم يعبرها بعد.**

هذا التصنيف مبني على أدلة من الكود وليس على انطباع. المطوّر يتخذ قرارات معمارية صحيحة في المسائل الصعبة، لكن ينقصه الانضباط الآلي (Systematic Discipline) في تعميم ما يقرره وفي التحقق من صحة ما ينسخه.

**المعادلة التي تلخّص مستواه:**
> جودة القرارات التصميمية **أعلى** من جودة التنفيذ التفصيلي.

هذه معادلة غير شائعة — الأغلب في هذا المستوى يكون العكس (تنفيذ نظيف لقرارات ضعيفة). وهذا يعني أن سقف تطوّره مرتفع، وأن أخطاءه قابلة للإصلاح بعادات عمل وليس بإعادة تعلّم أساسيات.

**ما تغيّر عن الجولة السابقة:** طُرح في §7 من النسخة السابقة سؤالٌ صريح: *هل بدأ يقرأ تحذيرات المُصرِّف ويتتبّع منطق ما ينسخه؟* — **الإجابة المُتحقَّق منها: لا.** البناء ما زال يُخرج **10 تحذيرات**، ومنها تحذيران يشيران إلى عطل يمنع تسجيل دخول كل حسابات الموظفين (§3.3 و§6.1). لذلك **لم تتم ترقية التصنيف** رغم تقدّم حقيقي في محاور أخرى.

---

## 2. نقاط القوة المثبتة بالأدلة

هذه ليست مجاملات — كل بند مقترن بدليل من الكود، ويجب **عدم** إعادة اقتراحها عليه كأنها ناقصة:

| القوة | الدليل |
|---|---|
| **انضباط معماري حقيقي** | فصل الطبقات مفروض على مستوى `csproj` وليس مجرد مجلدات: `Domain` لا يرجع لغير `Contracts`، و`Application`/`DataAccessLayer` لا يرجع أحدهما للآخر. نادر في هذا المستوى. |
| **فهم عميق لـ EF Core** | فهرسة محكمة، لا Lazy Loading، `AsNoTracking` في مسارات القراءة، Projection عبر `Expression<Func<>>` بدل جلب كيانات كاملة، `DeleteBehavior.Restrict` كافتراضي مع استثناء واحد مبرّر. |
| **أمان متقدّم نسبيًا لمستواه** | تدوير Refresh Token مع تخزين الـ Hash فقط (SHA-256) وربط `ReplacedByTokenId` — تنفيذ يتجاوز المتوقع من حديثي التخرج. لا أسرار في المستودع (`appsettings.*.json` المتتبَّعة نظيفة، والمفاتيح في User Secrets). |
| **فهم صحيح لمتى تلزم المعاملات** | `Transaction` صريحة فقط في `CreateEmployee`/`CreateStudent` (عمليات متعددة الخطوات)، وغيابها حيث لا تلزم. |
| **فصل التحقق عن منطق العمل** | لا ازدواجية بين FluentValidation (شكل) وفحوصات الخدمة (وجود/تفرّد). فصل واعٍ ومتّسق. |
| **قرارات أمنية واعية** | إرجاع 404 بدل 403 عند فشل الملكية لمنع تسريب وجود المورد. وفي كل دوال `Get*PerParent` جعل `scope` يتقدّم على `filter` القادم من العميل عبر `else if` — ترتيب صحيح أمنيًا، ومُطبَّق الآن بشكل متطابق في College/Department/Batch/Section. |
| **قدرة على تجاوز الحل المقترح عليه** | `catch (DbUpdateException ex) when (ex.InnerException is SqlException { Number: 547 })` في `UnitOfWorkRepository` — أنظف من الحل الذي اقتُرح عليه، مع تعليق يشرح السبب. |
| **تعلّم ذاتي موجَّه** | `My_Notes_UniNet_VS_ZadByan.txt` — يقارن تصميمه (Anemic Entity + حماية عبر المسار) بمشروع آخر (Rich Entity) ويحلل السبب. مؤشر نضج مهني حقيقي. |
| **تنظيم DI متّسق** | `PolicyExtension` و`AuthorizationHandlerExtension` مستخرجان من `Program.cs` تبعًا لاتفاقية المشروع. |
| **🆕 تعميم قرار عبر منطقة كاملة (جديد هذه الجولة)** | حذف `GetByName` من College سابقًا، ثم **تعميم نفس القرار** على Department و Section في نفس الموجة (`500ef16`, `25c99d2`). التحقق: لم يبقَ في `Domain/Interfaces` أي `GetDTOByName` أكاديمي — فقط `ExistsByName` المستخدَم للتفرّد. **هذا عكس نمط ضعفه الجذري تمامًا، وهو أهم مؤشر إيجابي في هذه الجولة.** |
| **🆕 إنجاز البند المؤجَّل بالكامل** | الفلترة عُمِّمت فعليًا على **الكيانات الخمسة كلها** (University/College/Department/Batch/Section) عبر `AcademicFilter` مشترك. البند الذي كان "مؤجَّلًا بقرار واعٍ" أصبح مُنجَزًا. |

---

## 3. أنماط الضعف المتكررة — التشخيص الأهم

### 3.1 النمط الأول (الأخطر): تعميم ناقص للنمط المُصمَّم
هذا **أبرز نمط ضعف لديه**، وتكرر عبر أربع موجات متتالية:

| الموجة | ما طُبِّق | ما نُسِي |
|---|---|---|
| بناء `CollegeOwnerHandler` أولًا | `GetCollegeById` فقط | كل عمليات الكتابة |
| بعد المراجعة الأولى | `Delete` + `Add` لـ College/Department/Batch | **`Update` في الثلاثة معًا** |
| بناء الـ Handlers الأكاديمية | `GetById` + `Delete` | `GetAll*` و`Get*PerParent` |
| **🆕 هذه الجولة** | **`Get*PerParent`** حصل على `scope` في Batch و Section ✅ | **`GetAll*`** بقيت بلا `scope` **وبقيت مفتوحة لأدوار مُقيَّدة النطاق** (§6.3)، و**`DeleteSection` بقيت بلا أي فحص ملكية إطلاقًا** (§6.2) |

**الدليل القاطع أن المشكلة سلوكية لا معرفية:** في نفس الجولة، `CollegeController.GetAllColleges` و`UniversityController.GetAllUniversities` مُقيَّدتان بـ `[Authorize(Roles = "Super Admin")]` ✅ — أي أنه **يعرف القاعدة وطبّقها في 2 من 5**، ونسيها في Department و Batch و Section.

**الخلاصة التشخيصية:** يطبّق النمط على نقاط النهاية التي يفكر فيها لحظتها، ولا يُجري جردًا منهجيًا لكل النقاط المتأثرة. **الضحية تنتقل كل جولة: كانت `Update`، ثم أصبحت `GetAll*` و`Delete`.**

**العلاج الموصى به:** جدول تحقق مكتوب لكل نمط أمني قبل اعتباره منجزًا — صفوفه الكيانات الخمسة، وأعمدته: `GetAll` / `GetPerParent` / `GetById` / `Add` / `Update` / `Delete`، وخانة "الأدوار المسموحة" لكل خلية. لا يُغلق النمط قبل أن تمتلئ كل الخلايا.

### 3.2 النمط الثاني: أخطاء نسخ-ولصق غير مُتحقَّق منها
عند تكرار كتلة مشابهة، ينسخ ويعدّل جزئيًا فيسقط تعديل لازم. أمثلة مؤكدة هذه الجولة:

- **المُنتِج نُسي بينما كُتب كل المستهلكين بشكل صحيح:** `ToUserScope()` في `UniNet/Extensions/ControllersExtensions.cs:102-110` **لا يُسنِد `BatchId`** — بينما `StudentRepository.GetStudents:96`، و`IsWithinScope:16`، و`BatchOwnerHandler`، و`SectionOwnerHandler` كلها مكتوبة لتتعامل معه. أربعة مستهلكين صحيحين ومُنتِج ناقص بسطر واحد (§6.4).
- **قاعدة `StudentNumber`** ما زالت منسوخة من قاعدة الهاتف بالكامل — Regex الهاتف، رسالة الهاتف، وشرط `.When(x => !string.IsNullOrEmpty(x.PhoneNumber))` (`AddStudentValidator.cs:62-66`). لم تُلمَس منذ المراجعة السابقة (§6.6).
- **حارس ميت مُكرَّر ثلاث مرات:** `if (scope == null || (كل الحقول null)) scope = new UserScope();` في `CollegeRepository:98`، `BatchRepository:130`، `SectionRepository:144` — إسناد كائن فارغ مكان كائن فارغ. نُسخ حرفيًا ثلاث مرات دون التساؤل عمّا يفعله.

**الملاحظة المهمة:** في كل حالة، **النسخة الصحيحة موجودة في المشروع نفسه** — فالمشكلة ليست جهلًا بالصواب، بل غياب مراجعة ذاتية بعد النسخ.

### 3.3 النمط الثالث: تجاهل تحذيرات المُصرِّف — **لم يتحسّن إطلاقًا**
هذا هو **البند الوحيد الذي لم يتقدّم بأي قدر** بين الجولتين. البناء ما زال يُخرج **10 تحذيرات**، وأخطرها ليس تحذيرًا شكليًا:

```
Application/Mappers/TokenUserInfoMapper.cs(29,16): CS8602 + CS0472
Application/Mappers/TokenUserInfoMapper.cs(70,16): CS8602 + CS0472
```

هذان السطران بالضبط هما موقع العطل الذي **يمنع تسجيل دخول كل حسابات الموظفين** (§6.1). أي أن المُصرِّف كان يشير بإصبعه على العطل الحرج، مرتين، في كل عملية بناء، لأكثر من جولة كاملة.

**التحذيرات العشرة الحالية:**

| الملف | السطر | التحذير |
|---|---|---|
| `Contracts/Results/UserTypeResult.cs` | 28 | CS8603 — `SystemAdmin()` تُرجع `null` دائمًا |
| `Contracts/Responses/AddUpdateServiceResponse.cs` | 16 | CS8618 — `ErrorMessage` غير قابل للـ null بلا قيمة |
| `Application/Services/Login Service/RefreshTokenService.cs` | 52 | CS8613 — عدم تطابق nullability مع الواجهة |
| `Application/Mappers/TokenUserInfoMapper.cs` | 29, 70 | **CS8602 ×2 + CS0472 ×2 — العطل الحرج** |
| `Application/Services/EmployeeService/EmployeeService.cs` | 334 | CS0168 — `ex` معرَّف وغير مستخدم |
| `DataAccessLayer/Repos/AcademicRepositories/CollegeRepository.cs` | 73 | CS8602 — سببه `filter?.Search` في :68 بعد إسناد غير-null |
| `DataAccessLayer/Repos/StudentRepository/StudentRepository.cs` | 96 | CS8602 — `scope` بلا فحص null |

**العلاج (لم يعد اختياريًا):** إضافة `<TreatWarningsAsErrors>true</TreatWarningsAsErrors>` أو على الأقل `<WarningsAsErrors>CS8602;CS8603</WarningsAsErrors>` في ملفات المشاريع. الاعتماد على "تذكُّر قراءة المخرجات" ثبت فشله عبر جولتين.

### 3.4 النمط الرابع: بقايا كود غير منظَّفة
- معاملات مُستقبَلة وغير مستخدمة: `CollegeIdParameter @collegeParameter` في `DepartmentController.cs:52` (وهو معامل **إلزامي** بـ `[Range(1,...)]` — يُجبر العميل على إرساله ثم يُتجاهَل).
- تعليق `// Not completed yet` في `UserService.cs:81` فوق دالة مكتملة.
- دالة مصنع تُرجع `null` وتتجاهل معاملها: `UserTypeResult.SystemAdmin(int? AdminId) => null;`
- `services.AddAuthorization(...)` مُستدعاة **7 مرات** في `PolicyExtension.cs` بدل مرة واحدة بـ 7 سياسات.
- ملفان XML بحجم **1.3 ميجابايت** متتبَّعان في جذر المستودع: `StudentBranch.xml`, `UniNet(StatusBranch).xml`.
- `.github/workflows/` موجود وفارغ.
- أخطاء إملائية في المعرّفات العامة: `Coulmns`, `UniverseId`, `Parametre`, `Requsets`, `ToPagedActioneResult`, `AccesseToken`, `proprty`, `RepoSitories`, `Deletaion faield`, `incommingHash`.

### 3.5 فجوات معرفية (وليست أخطاء)
- **لا خبرة باختبارات الوحدة** — لا مشروع اختبارات إطلاقًا. **أكبر فجوة مهارية مقابلةً بالسوق، ولم تتحرك منذ ثلاث جولات.**
- **لا خبرة بالـ CI/CD أو Docker** — `.github/workflows/` مجلد فارغ.
- **`ILogger` موجود في الـ Middleware فقط** — لم يصل إلى طبقة الخدمات بعد.
- **لا خبرة بتشغيل الواجهة الأمامية فعليًا** — دليل ذلك في §6.7.

---

## 4. معايير المعايرة للمراجعات المستقبلية

> **هذا هو القسم العملي — اقرأه قبل أي مراجعة قادمة.**

### 4.1 ما يُصنَّف 🔴 Critical عند هذا المستوى
- ثغرة أمنية قابلة للاستغلال فعليًا (تجاوز نطاق، تصعيد صلاحيات، IDOR).
- خطأ يكسر وظيفة معلنة كمكتملة (مثل NRE في مسار تسجيل دخول).
- خطأ توجيه/تعاقد يجعل نقطة نهاية تفعل شيئًا مختلفًا عن اسمها.
- سر مكشوف في مستودع Git.
- نمط ناقص التعميم **إن كان كل يوم تأخير يضاعف تكلفة إصلاحه** (لأنه يُنسخ لميزات جديدة).

### 4.2 ما يُصنَّف 🟠 Important
- خطأ منطقي حقيقي لكنه لا يُستغل أمنيًا اليوم، **أو ثغرة كامنة تُفعَّل بإصلاح جزئي لاحق** (انظر الفخ في §6.5 — نمط جديد يستحق التصنيف).
- غياب فحص `null` قد ينتج استثناءً غير معالج.
- تناقض يُنتج سلوكًا مختلفًا بين مسارين متشابهين، أو بين ما تعلنه نقطة النهاية وما تفعله.
- قرارات مخطَّطة ومؤجَّلة عمدًا — **تُذكر كتذكير لا كلوم**.

### 4.3 ما يُصنَّف 🟡 Improvement
تكرار بنيوي، توحيد أسماء، أخطاء إملائية، استيرادات ميتة، تنظيف تعليقات، تحسينات أسلوبية. **لا يُطلب تنفيذها أثناء بناء ميزة** — تُجمَّع لتمريرة تنظيف.

### 4.4 ما يجب **عدم** طرحه إطلاقًا (تشتيت غير مبرَّر)
- CQRS، MediatR، Event Sourcing، DDD الكامل، Microservices، GraphQL.
- Redis / Distributed Caching / Message Queues — لا حِمل يبرّرها.
- AutoMapper — أسلوبه اليدوي عبر `Expression` **أفضل** أداءً لحالته، ولا يجب اقتراح استبداله.
- Generic Repository — قرار واعٍ بعدم استخدامه، موثّق في `CLAUDE.md`.
- أي Design Pattern لمجرد وجوده.

### 4.5 قواعد أسلوب المراجعة معه
1. **ابدأ بالتحقق الفعلي من الكود قبل الحكم** — لا تفترض أن ما ورد في `CLAUDE.md` أو في مراجعة سابقة لا يزال صحيحًا؛ فقد ثبت أكثر من مرة أن التوثيق تأخر عن الكود (مثال حيّ هذه الجولة: `CLAUDE.md` و`README.md` يقولان إن الواجهة الأمامية تحت `UniNet/wwwroot/` — المجلد غير موجود أصلًا، §6.7).
2. **ابنِ المشروع فعليًا واقرأ التحذيرات قبل أي شيء آخر** — في هذه الجولة، تحذيران من المُصرِّف كانا يشيران مباشرة إلى أخطر عطل في المشروع.
3. **قدّم الأدلة بأرقام أسطر** — يتعامل معها جيدًا ويتحقق بنفسه.
4. **لا مجاملة ولا تهويل** — طلب صراحةً الصراحة، ويستجيب لها بإصلاحات فعلية سريعة.
5. **اشرح "لماذا" وليس "ماذا" فقط** — يستوعب التعليل ويبني عليه.
6. **حين يطلب الإرشاد دون كود** — التزم بذلك حرفيًا: وجّه، اطرح القرارات التي يجب أن يحسمها بنفسه، ولا تكتب الحل.
7. **اعترف بما أنجزه صراحةً قبل ذكر ما تبقّى** — يتابع الإصلاحات بجدية ويستحق تثبيت الصحيح منها.
8. **🆕 حين يكون هناك عيبان يُخفي أحدهما الآخر — قل ذلك صراحة وحدِّد ترتيب الإصلاح.** ثبت هذه الجولة أن إصلاح `ToUserScope` وحده **يفتح** ثغرة بدل أن يغلقها (§6.5).

---

## 5. لقطة حالة المشروع (وقت كتابة الملف)

**الحجم:** ~10,163 سطر C# (خارج الـ Migrations) + 4,421 سطر Migrations · 5 مشاريع · 12 متحكمًا · 13 مستودعًا · 0 اختبارات.

### مُنجَز ومُثبَت
- الهيكل الأكاديمي (University → College → Department → Batch → Section): CRUD كامل.
- الهوية والمصادقة: JWT + Refresh Token مع تدوير و Hash + BCrypt + صلاحيات مبنية على الأدوار.
- ميزتا Employee و Student.
- تقييد متحكمات الهوية على `Super Admin` ✅.
- Handlers الملكية الخمسة (University/College/Department/Batch/Section) + Employee + Student، ومسجَّلة في DI ✅.
- `ILogger` في `GlobalExceptionHandlingMiddleware` مع تمييز صحيح بين `LogWarning` (قيد عمل) و`LogError` (عطل) ✅.
- معالجة `SqlException 547` في `UnitOfWorkRepository` ✅.
- **الفلترة مُعمَّمة على الكيانات الخمسة كلها** ✅ (البند المؤجَّل سابقًا — أُغلق).
- **`GetByName` محذوفة من كل الكيانات الأكاديمية** ✅ (قرار مُعمَّم بالكامل).
- `scope` مُطبَّق في `Get*PerParent` للكيانات الأربعة ✅.
- `TestDataSeeder` للتطوير (idempotent, BCrypt).

### لم يبدأ بعد
- `Semester` / `Subject` / `SectionSubject` / `StudentResult` — موجودة في قاعدة البيانات بلا أي Repository/Service/Controller. **هذه أهم قيمة وظيفية متبقية (الدرجات).**
- `Post` / `Announcement` — مُهيكَلة في القاعدة بلا تنفيذ (قرار منتج مطلوب: تُبنى أم تُزال).
- مشروع اختبارات، CI، `ILogger` في الخدمات، تدوير مفتاح JWT، توحيد `UpdatedAt`.

---

## 6. البنود المفتوحة — مرتبة حسب الأولوية

كلها **مُتحقَّق منها في الكود** وقت كتابة هذا الملف (Commit `25c99d2`).

### ✅ أُغلقت في هذه الجولة (تحقّقتُ منها، لا تُعِد فتحها)
- تعميم الفلترة على University و Batch و Section — مُنجَز.
- حذف `GetByName` من Department و Section — مُنجَز ومتّسق مع قرار College.
- إضافة `scope` إلى `GetBatchesPerDepartment` و`GetSectionsPerBatch` — مُنجَز (البند #1 من القائمة السابقة، **جزئيًا**: `Get*PerParent` أُغلقت، `GetAll*` لم تُغلق — انظر §6.3).

---

### 🔴 6.1 — عطل يمنع تسجيل دخول كل حسابات الموظفين

**الموقع:** `Application/Mappers/TokenUserInfoMapper.cs:29` و`:70`

```csharp
// السطر 18: يخرج مبكرًا فقط إذا كان الاثنان null
if (typeResult == null || (typeResult.EmployeeAuthorizationInfo == null && typeResult.StudentAuthorizationInfo == null))
    return new TokenUserInfoDTO { ... };

// السطر 29: يصل هنا الموظف — و StudentAuthorizationInfo هي null
if (typeResult.StudentAuthorizationInfo.BatchId != null)   // 💥 NullReferenceException
```

**مسار الفشل بالكامل:**
1. موظف (`UniversityAdmin` / `CollegeAdmin` / `DepartmentAdmin`) يستدعي `POST /api/Login/login`.
2. `UserService.GetAuthorizationInfoResult` تُرجع `UserTypeResult.Employee(...)` → `EmployeeAuthorizationInfo != null`, `StudentAuthorizationInfo == null`.
3. الحارس في السطر 18 لا يعمل (لأن الشرط `&&`).
4. السطر 29 → `NullReferenceException` → 500.

**نفس العطل حرفيًا في `:70`** للنسخة الثانية من الدالة، أي أن `POST /api/Login/refresh` معطوب بنفس الطريقة.

**الأثر:** كل سطح الإدارة (كل ما هو ليس `Super Admin` أو `Student`) غير قابل للاستخدام. `README.md` يُصنّف ميزة Employee بـ "✅ Complete".

**ملاحظة إضافية على نفس الدالة:** الشرط `typeResult.StudentAuthorizationInfo.BatchId != null` يقارن `int` بـ `null` (CS0472) — أي أنه **دائمًا `true`**. حتى بعد إصلاح الـ NRE، التفريع لن يعمل كما هو مقصود.

---

### 🔴 6.2 — `DeleteSection` بلا أي فحص ملكية

**الموقع:** `UniNet/Controllers/AcademicControllers/SectionController.cs:80-93`

```csharp
[Authorize(Roles = "Super Admin,UniversityAdmin,CollegeAdmin,DepartmentAdmin")]
[HttpDelete(Name = "DeleteSection")]
public async Task<ActionResult> DeleteSection([FromQuery] SectionIdParameter sectionIdParameter)
{
    var result = await _sectionService.Delete(sectionIdParameter.SectionId);   // ← لا scope، لا Policy
    return result.ToDeleteActionResult<Section>(sectionIdParameter.SectionId);
}
```

`SectionService.Delete(int sectionId)` لا تستقبل `UserScope` أصلًا (`SectionService.cs:73`).

**الأثر:** أي `DepartmentAdmin` في أي جامعة يستطيع حذف أي شعبة في النظام كله بمعرفة رقمها فقط (IDOR كامل على عملية تدميرية).

**المقارنة التي تُثبت أنه سهو لا جهل:** في نفس المشروع، `CollegeController.Delete` و`DepartmentController.Delete` يستخدمان `AuthorizeAsync + Policy`، و`BatchController.Delete` يفحص `IsWithinScope` يدويًا. **الشعبة وحدها بلا شيء** — و`SectionOwnerHandler` و`SectionOwnerPolicy` مبنيان ومسجَّلان وجاهزان للاستخدام، ومستخدَمان فعلًا في `GetSectionById` في نفس الملف (`:70`).

---

### 🔴 6.3 — نقاط `GetAll*` بلا نطاق ومفتوحة لأدوار مُقيَّدة النطاق

| نقطة النهاية | الأدوار المسموحة | الدالة المستدعاة | فيها scope؟ |
|---|---|---|---|
| `GET /api/Department` (`DepartmentController.cs:33`) | Super Admin, UniversityAdmin, **CollegeAdmin** | `GetAllDepartments(filter, ...)` | ❌ |
| `GET /api/Batch` (`BatchController.cs:34`) | Super Admin, UniversityAdmin, CollegeAdmin, **DepartmentAdmin** | `GetAllBatches(filter, ...)` | ❌ |
| `GET /api/Section` (`SectionController.cs:33`) | Super Admin, UniversityAdmin, CollegeAdmin, **DepartmentAdmin** | `GetAllSections(filter, ...)` | ❌ |
| `GET /api/College` (`CollegeController.cs:33`) | **Super Admin فقط** | `GetColleges(filter, ...)` | ✅ (لا حاجة) |
| `GET /api/University` (`UniversityController.cs:33`) | **Super Admin فقط** | `GetAllUniversities(filter, ...)` | ✅ (لا حاجة) |

**الأثر:** `DepartmentAdmin` في جامعة (أ) يستدعي `GET /api/Batch?pageNumber=1&pageSize=1000` فيحصل على كل الدفعات في **كل الجامعات**. تسريب بيانات عابر للمستأجرين (Cross-Tenant) بقراءة واحدة، بلا أي استغلال ذكي.

**لاحظ:** المسار الآمن موجود بجواره مباشرة — `GET /api/Batch/by-departmentId` يمرّر `ToUserScope()` ويفلتر بشكل صحيح. المستخدم الخبيث ببساطة لا يستعمله.

**قراران ممكنان (اختر واحدًا لكل كيان، ووثّقه):**
- **أ)** تقييد `GetAll*` بـ `Super Admin` فقط — يطابق ما فعلته في College و University، ويجعل القاعدة موحّدة عبر الكيانات الخمسة.
- **ب)** تمرير `ToUserScope()` إليها كما في `Get*PerParent` — يجعل الفرق بين نقطتَي النهاية بلا معنى.
> **التوصية: (أ)** — لأنها تجعل القاعدة قابلة للتذكّر: *`GetAll*` = Super Admin، `Get*PerParent` = بقية الأدوار مع scope.* قاعدة من سطر واحد أصعب في النسيان من قاعدة تحتاج مراجعة كل دالة على حدة.

---

### 🟠 6.4 — `ToUserScope()` يُسقِط `BatchId`

**الموقع:** `UniNet/Extensions/ControllersExtensions.cs:102-110`

```csharp
public static UserScope ToUserScope(this ICurrentUserService currentUserService)
{
    return new UserScope
    {
        UniversityId = currentUserService.UniversityId,
        CollegeId    = currentUserService.CollegeId,
        DepartmentId = currentUserService.DepartmentId,
        // BatchId = currentUserService.BatchId,   ← مفقود
    };
}
```

`ICurrentUserService.BatchId` موجودة ومنفَّذة (`CurrentUserService.cs:72-82`)، و`JwtTokenFactory` يُصدر مطالبة `BatchId` (`:39-42`)، و`UserScope.BatchId` معرَّفة — لكن الجسر بينهما مفقود.

**المستهلكون الأربعة المكتوبون بشكل صحيح والمعطَّلون بسبب هذا السطر:**
- `StudentRepository.GetStudents:96` — `if (!scope.BatchId.HasValue) {...} else { query.Where(s => s.BatchId == scope.BatchId); }` — الفرع `else` كود ميت.
- `EmployeeScopeExtension.IsWithinScope:16` — فرع `scope.BatchId.HasValue` كود ميت.
- `BatchOwnerHandler` و`SectionOwnerHandler` — يمرّران `resource.BatchId` كوسيط رابع بلا أثر.

**الأثر العملي اليوم:** محدود، لأن دور `BatchAdmin` غير مزروع أصلًا (§6.8). **لكن لحظة زرعه، كل مسؤول دفعة يرى كل طلاب القسم بدل دفعته.**

---

### 🟠 6.5 — ⚠️ فخ: إصلاح §6.4 وحده **يفتح** ثغرة تصعيد صلاحيات

**الموقع:** `Contracts/Common/Extensions/EmployeeScopeExtension.cs:14`

```csharp
if (scope == null || (scope.UniversityId == null && scope.CollegeId == null && scope.DepartmentId == null))
    return true;   // ← "هذا Super Admin، اسمح بكل شيء"
```

الحارس يفحص ثلاثة حقول من أربعة — **`BatchId` غير مذكور.**

**السيناريو:** مستخدم مطالبته الوحيدة `BatchId` (طالب أو مسؤول دفعة بلا مطالبات أعلى) → الحقول الثلاثة `null` → الدالة تُرجع `true` → **يُعامَل كـ Super Admin في كل `IsWithinScope` في المشروع.**

**لماذا هو غير قابل للاستغلال الآن:** لأن `ToUserScope` لا يملأ `BatchId` أصلًا (§6.4). **العيبان يُخفي أحدهما الآخر.**

> **ترتيب الإصلاح إلزامي: أصلح §6.5 أولًا (أضف `&& scope.BatchId == null` إلى الحارس)، ثم §6.4.** العكس يفتح ثغرة حقيقية بين الإصلاحين.

هذا النوع من الاقتران بين عيبين هو أرقى ما يستحق الانتباه في هذه المراجعة — وهو أيضًا الحجة الأقوى لصالح وجود اختبارات وحدة على `IsWithinScope` تحديدًا.

---

### 🟠 6.6 — قاعدة `StudentNumber` معطَّلة بالكامل

**الموقع:** `Application/Validators/StudentValidator/AddStudentValidator.cs:62-66`

```csharp
RuleFor(x => x.StudentNumber)
    .NotEmpty().WithMessage("Student number is required.")
    .Matches(@"^\+?[1-9]\d{1,14}$").WithMessage("Invalid phone number format...")  // ← Regex ورسالة الهاتف
    .When(x => !string.IsNullOrEmpty(x.PhoneNumber));                              // ← الشرط على PhoneNumber!
```

`.When()` في نهاية السلسلة تنطبق على **كل** القواعد السابقة فيها — بما فيها `NotEmpty()`. النتيجة: **إن لم يُرسل العميل رقم هاتف، لا يُتحقَّق من رقم الطالب إطلاقًا** — لا وجودًا ولا شكلًا. ورقم الطالب حقل مطلوب وفريد في القاعدة، فالفشل ينتقل إلى مستوى قيد قاعدة البيانات بدل رسالة تحقق نظيفة.

بند مُبلَّغ عنه في المراجعة السابقة ولم يُلمَس.

---

### 🟠 6.7 — الواجهة الأمامية غير مخدومة إطلاقًا

`Program.cs:196-197` يستدعي `UseDefaultFiles()` + `UseStaticFiles()`، وكلاهما يخدم من `{ContentRoot}/wwwroot` أي `UniNet/wwwroot/`.

**التحقق:** `UniNet/wwwroot/` **غير موجود**. الملفات الثمانية (`index.html`, `js/api.js`, `js/auth.js`, ...) متتبَّعة في **جذر المستودع** تحت `wwwroot/`.

**الأثر:** طلب `GET /` يُرجع 404. الواجهة لم تُشغَّل قط.

**التوثيق يناقض الكود في موضعين:** `CLAUDE.md` يقول "Static files under `UniNet/wwwroot/`"، و`README.md` يُدرج الواجهة كـ "in active development".

**الإصلاح:** انقل المجلد إلى `UniNet/wwwroot/` (`git mv wwwroot UniNet/wwwroot`) وصحّح الملفين.

---

### 🟠 6.8 — تناقضات في العقد والأدوار

| # | البند | الموقع | التفصيل |
|---|---|---|---|
| 1 | **409 يُرجَع كـ 400** | `ControllersExtensions.cs:53-57` | `EnErrorTypes.ExistedResource => new BadRequestObjectResult(new { StatusCode = 409, ... })` — حالة HTTP الفعلية **400**، والجسم يدّعي 409. كل المتحكمات تعلن `[ProducesResponseType(409)]`. العميل الذي يفحص `response.status === 409` لن يعمل أبدًا. |
| 2 | **`Add*` تُرجع 200 لا 201** | `ControllersExtensions.cs:19-22` | `ToActionResult` لا تُنتج `201 Created` ولا رأس `Location` رغم `[ProducesResponseType(201)]` على 9 نقاط إنشاء. |
| 3 | **دور `BatchAdmin` غير موجود** | `StudentController.cs:31,43,92,105` | مذكور في 4 سمات `[Authorize]`، وله نقطتا نهاية مخصصتان (`AddBatchAdmin`/`UpdateBatchAdmin`)، لكنه **غير مزروع في `SeedData.cs`** (الأدوار المزروعة: Super Admin, UniversityAdmin, CollegeAdmin, DepartmentAdmin, Lecturer, Student). لا مستخدم يمكنه امتلاكه → الميزة غير قابلة للتشغيل. |
| 4 | **دور `Lecturer` مزروع وغير مستخدم** | `SeedData.cs:29` | لا يظهر في أي `[Authorize]`. |
| 5 | **معامل إلزامي مُتجاهَل** | `DepartmentController.cs:52` | `CollegeIdParameter @collegeParameter` مربوط بـ `[Range(1,int.MaxValue)]` — يُجبر العميل على إرساله (وإلا 400)، ثم لا يُستخدم في السطر 54 إطلاقًا. |
| 6 | **`filter.UniversityId` مُتجاهَل** | `UniversityRepository.cs:59-81` | كل الأشقاء الأربعة يطبّقون فلتر المعرّف الخاص بهم؛ University وحدها لا. |
| 7 | **`filter.DepartmentId` مُتجاهَل** | `DepartmentRepository.GetDepartmentsPerCollege` | موجود في `GetAllDepartments:87` ومفقود في النسخة المُنطَّقة — نفس نمط "أصلح دالة وانسَ توأمها". |

---

### 🟡 6.9 — تنظيف (تُجمَّع لتمريرة واحدة، لا تُطلب أثناء بناء ميزة)

| البند | الموقع |
|---|---|
| حارس ميت `if (scope == null \|\| كل الحقول null) scope = new UserScope();` | `CollegeRepository:98`, `BatchRepository:130`, `SectionRepository:144` |
| `UserTypeResult.SystemAdmin(int? AdminId) => null;` — تتجاهل معاملها وتُرجع null | `UserTypeResult.cs:28` |
| `services.AddAuthorization()` ×7 بدل مرة واحدة | `PolicyExtension.cs` |
| `// Not completed yet` فوق دالة مكتملة | `UserService.cs:81` |
| `catch (Exception ex)` و`ex` غير مستخدم | `EmployeeService.cs:334` |
| `filter?.Search` بعد إسناد غير-null (سبب CS8602) | `CollegeRepository.cs:68` |
| Middleware الاستثناءات مسجَّل بعد CORS/StaticFiles بدل أن يكون أول شيء | `Program.cs:199` |
| ملفان XML بحجم 1.3MB في جذر المستودع | `StudentBranch.xml`, `UniNet(StatusBranch).xml` |
| `.github/workflows/` فارغ | — |
| أخطاء إملائية في معرّفات عامة | `Coulmns`, `UniverseId`, `Parametre`, `Requsets`, `ToPagedActioneResult`, `AccesseToken`, `proprty`, `RepoSitories`, `Deletaion faield`, `incommingHash` |

---

## 7. مؤشرات التقدّم المرصودة

للمقارنة في الجلسات القادمة:

| المؤشر | الجولة السابقة (`d58be80`) | هذه الجولة (`25c99d2`) | الاتجاه |
|---|---|---|---|
| **سرعة الاستجابة للمراجعة** | عالية | عالية — أغلق البند المؤجَّل (تعميم الفلترة) بالكامل + عمّم حذف `GetByName` | ↗ ثابت ممتاز |
| **تعميم القرار عبر منطقة كاملة** | ضعيف | **تحسّن حقيقي** — `GetByName` والفلترة عُمِّمتا على الكيانات الخمسة دون طلب | ↗ **أهم تحسّن** |
| **تعميم النمط عبر Endpoints** | ضعيف | ما زال ضعيفًا — `GetAll*` و`DeleteSection` (§6.2, §6.3) | → لم يتغيّر |
| **قراءة تحذيرات المُصرِّف** | 10 تحذيرات | **10 تحذيرات، نفس المواقع** | → **لم يتغيّر إطلاقًا** |
| **إغلاق بنود Phase 0 القديمة** | مفتوحة | `TokenUserInfoMapper` و`StudentNumber` ما زالتا مفتوحتين | → لم يتغيّر |
| **الاختبارات** | 0 | 0 | → لم يتغيّر |

**القراءة الدقيقة للنتيجة:** التقدّم هذه الجولة كان في **البُعد الذي يستمتع به** (تصميم تجريدات وتعميمها معماريًا) وليس في **البُعد الممل** (قراءة المخرجات، وإغلاق بنود قديمة، وجرد نقاط النهاية). هذا نمط شخصية مهنية واضح، وهو نفسه سبب المعادلة في §1.

**السؤال الذي يجب طرحه في المراجعة القادمة:**
> هل أضاف `TreatWarningsAsErrors` أو أغلق التحذيرات العشرة؟ وهل أنشأ مشروع اختبارات ولو بثلاثة اختبارات على `IsWithinScope`؟
>
> **إن نعم لكليهما — يستحق الترقية إلى Mid-level فورًا،** لأن هذين البندين تحديدًا هما ما يحوّلان الانضباط من "تذكُّر" إلى "آلية"، وهو الفارق الوحيد المتبقي بينه وبين المستوى التالي.

---

## About Developer

> قسم مستقل موجَّه للقارئ الخارجي (مُوظِّف، قائد فريق، أو المطوّر نفسه عند إعداد سيرته الذاتية). كل ما فيه مستخلَص من الكود المراجَع في هذا المستودع، لا من تصريحات المطوّر.

### الملف المهني في سطرين

مطوّر **.NET Backend** يبني — منفردًا — نظام إدارة جامعية متعدد المستأجرين (Multi-Tenant) بمعمارية طبقية مفروضة على مستوى المشاريع، مع نظام مصادقة وتفويض مخصّص من الصفر. **يفكّر كمعماري ويُنفِّذ كمبتدئ متقدّم** — وهذا التباين هو أصدق وصف لملفه.

### التقنيات المُثبَتة بالكود

| المجال | التقنيات | مستوى الإتقان الظاهر |
|---|---|---|
| **اللغة والمنصّة** | C# 12, .NET 8, ASP.NET Core Web API | **جيد جدًا** — يستخدم `record`-like DTOs، nullable reference types، pattern matching (`is SqlException { Number: 547 }`)، expression trees |
| **قواعد البيانات** | EF Core 8, SQL Server, Migrations, Fluent API Configurations | **جيد جدًا** — أقوى مهاراته التقنية. Projection عبر `Expression<Func<>>`، `AsNoTracking`، فهارس مركّبة فريدة تطابق قواعد العمل، `DeleteBehavior.Restrict` كسياسة واعية |
| **المعمارية** | Layered/Clean-ish, Repository + Unit of Work, Result Envelope Pattern | **جيد جدًا** — الفصل مفروض بمراجع المشاريع لا بالاتفاق. `Contracts` كمكتبة مشتركة قرار ناضج |
| **الأمن** | JWT Bearer, Refresh Token Rotation (SHA-256 hash-at-rest), BCrypt, Claims-Based Scope, Resource-Based Authorization (`IAuthorizationHandler` + Policies) | **جيد على مستوى التصميم، متوسط على مستوى التغطية** — الآليات مبنية بشكل احترافي، لكن تطبيقها على كل نقاط النهاية غير مكتمل |
| **التحقق** | FluentValidation | **جيد** — فصل واعٍ بين تحقق الشكل وتحقق منطق العمل |
| **الأدوات** | Git (فروع لكل ميزة), Swagger/OpenAPI, .NET User Secrets, EF CLI | **متوسط إلى جيد** |
| **الواجهة الأمامية** | HTML/CSS/Vanilla JS (SPA بسيطة) | **مبتدئ** — موجودة لكنها غير مُشغَّلة قط (§6.7) |

### المهارات غير التقنية (المُثبَتة بالسلوك، لا بالادعاء)

- **✅ يستقبل النقد الصريح ويحوّله إلى كود خلال دورة واحدة.** أغلق كل بنود الموجة الأولى في دورتين، وأنجز بندًا مؤجَّلًا قبل موعده.
- **✅ يحسّن الحل المقترح عليه بدل نسخه.** حل `UnitOfWork` (`catch...when`) جاء أنظف مما اقتُرح، مع تعليق يشرح السبب.
- **✅ يتعلّم بالمقارنة النقدية.** ملف `My_Notes_UniNet_VS_ZadByan.txt` يقارن تصميمه بمشروع آخر ويحلّل التنازلات — سلوك نادر في هذا المستوى.
- **✅ يوثّق قراراته** بتعليقات تشرح "لماذا" لا "ماذا".
- **⚠️ لا يتحقق ذاتيًا بعد التنفيذ.** لا يقرأ مخرجات البناء، ولا يتتبّع منطق ما نسخه، ولا يجرد نقاط النهاية المتأثرة بقرار أمني.

### الفجوات المهارية — مرتبة حسب أثرها على التوظيف

| # | الفجوة | لماذا تهم | الجهد التقديري لسدّها |
|---|---|---|---|
| 1 | **لا اختبارات وحدة إطلاقًا** | أول سؤال في أي مقابلة Backend جادّة. غيابها التام يُقرأ كإشارة إنذار مستقلة عن جودة الكود | أسبوع لمشروع xUnit + 20 اختبارًا على المُحقِّقات و`IsWithinScope` |
| 2 | **لا CI/CD ولا Docker** | معيار حدّ أدنى في كل فريق حديث. `.github/workflows/` فارغ | يوم واحد لـ workflow يبني ويشغّل الاختبارات + Dockerfile |
| 3 | **لا انضباط آلي (التحذيرات)** | 10 تحذيرات ثابتة عبر جولتين، منها اثنان يشيران لعطل حرج | ساعة واحدة (`TreatWarningsAsErrors`) + يوم للتنظيف |
| 4 | **Logging سطحي** | `ILogger` في الـ Middleware فقط، لا في الخدمات | يومان |
| 5 | **لا خبرة تشغيل فعلي (Runtime)** | الواجهة لم تُخدَم قط؛ عطل تسجيل دخول الموظف بقي جولتين — كلاهما يُكتشف بتشغيل واحد | أهم من كل ما سبق: **شغّل ما تكتب** |

> **الملاحظة الأهم في هذا القسم:** الفجوات الخمس كلها **إجرائية، لا معرفية**. لا واحدة منها تتطلب تعلّم مفهوم جديد صعب. مجموع الجهد لسدّها كلها **أقل من أسبوعين**، وأثرها على جاهزيته للتوظيف أكبر من أثر بناء ميزة جديدة كاملة.

### نسبة الاستعداد للوظيفة الأولى أو التدريب

| المسار | النسبة | التبرير |
|---|---|---|
| **تدريب (Internship / Trainee)** | **95%** | **جاهز الآن.** مستواه يتجاوز المتوقع من متدرّب بفارق واضح؛ سيكون من أقوى المتدرّبين في أي فريق. لا شيء يمنع التقديم اليوم |
| **وظيفة Junior .NET Backend أولى** | **75%** | جاهز تقنيًا. الناقص هو "إثبات الاحتراف" لا "القدرة": اختبارات + CI + بناء نظيف. **يصل إلى 90% خلال أسبوعين من العمل المركّز على الفجوات الخمس** |
| **وظيفة Mid-level مباشرة** | **40%** | معماريًا يستحق النقاش؛ لكن غياب الاختبارات والانضباط الآلي وعدم اكتمال تطبيق النمط الأمني تمنعه اليوم |

**تقدير صادق:** لو تقدّم غدًا لوظيفة Junior، سيتفوّق في الأسئلة المعمارية وأسئلة EF Core على أغلب المتقدّمين في مستواه، **وسيتعثّر** في: "أرِني اختباراتك"، "كيف تنشر؟"، و"كيف تضمن أن التعديل لم يكسر شيئًا؟". هذه الثلاثة هي كل ما يفصله عن عرض عمل.

**نقطة قوة تفاوضية ينبغي أن يستخدمها صراحةً:** مشروع واقعي بـ ~10,000 سطر، متعدد المستأجرين، بمصادقة وتفويض مبنيين من الصفر، وبفصل طبقات حقيقي — هذا **أقوى بكثير** من محفظة مليئة بمشاريع تعليمية (To-Do / Blog). في المقابلة، الحديث عن *لماذا* اختار Anemic Entities وحماية عبر المسار (كما في ملف مقارنته الذاتية) سيميّزه فورًا.

### الأقسام التي يمكنه المساهمة فيها بفاعلية في مشاريع Production

**🟢 يمكنه المساهمة فيها من اليوم الأول، بمراجعة كود عادية:**
- **بناء نقاط نهاية CRUD كاملة** ضمن نمط قائم — أثبت قدرته على ذلك عبر 12 متحكمًا متّسقًا.
- **طبقة الوصول للبيانات (EF Core)**: كتابة Repositories، استعلامات Projection، تكوينات Fluent API، فهرسة، Migrations. **هذه أقوى مناطقه — يمكن الاعتماد عليه فيها بثقة.**
- **تصميم عقود البيانات (DTOs/Requests/Responses)** وقواعد التحقق بـ FluentValidation.
- **كتابة استعلامات فلترة وترقيم صفحات (Pagination/Filtering)** — بنى `AcademicFilter` مشتركًا وطبّقه على خمسة كيانات.
- **قراءة قاعدة بيانات قائمة وبناء طبقة وصول فوقها** — يفهم العلاقات وسلوك الحذف والفهارس جيدًا.

**🟡 يمكنه المساهمة فيها بإشراف ومراجعة أدق:**
- **ميزات المصادقة والتفويض** — يفهم الآليات جيدًا (بنى Refresh Token Rotation صحيحًا)، لكن تغطيته للحالات غير مكتملة. مناسب للعمل هنا **بشرط** وجود قائمة تحقق ومراجع ثانٍ.
- **إعادة هيكلة (Refactoring) لكود قائم** — يتخذ قرارات تجريد جيدة، لكنه يحتاج من يتحقق من اكتمال التطبيق.
- **تصميم معماري لوحدة جديدة** — قراراته جيدة على غير المتوقع؛ يستفيد من نقاش تصميمي قبل التنفيذ.

**🔴 لا يُسنَد إليه منفردًا في الوقت الحالي:**
- **أي عمل أمني حسّاس بلا مراجعة** — ليس لضعف الفهم، بل لأن نمط "التعميم الناقص" الموثّق يجعل التغطية غير مكتملة بشكل متكرر.
- **البنية التحتية / DevOps / النشر** — لا خبرة مثبتة.
- **كتابة اختبارات لنظام قائم** — لا خبرة إطلاقًا (لكن هذه أول ما ينبغي تدريبه عليه، والعائد سيكون سريعًا).
- **تطوير الواجهات الأمامية** — مبتدئ.

### التوصية الختامية

> مطوّر **يستحق الاستثمار فيه**، لا مجرد التوظيف. سقفه أعلى بوضوح من مستواه الحالي، لأن ما ينقصه **عادات** لا **مفاهيم** — وهذا أسرع أنواع الفجوات في الإغلاق.
>
> في فريق لديه مراجعة كود جادّة و CI يمنع دمج كود بتحذيرات، سيصل إلى **Mid-level خلال 6–9 أشهر**. بلا هذا الإطار، سيبقى يكرر نفس نمط الضعف مهما زادت خبرته، لأن النمط ليس نقص معرفة بل نقص آلية تُمسك الخطأ نيابةً عنه.
>
> **أقصر طريق أمامه:** لا ميزة جديدة قبل: (1) `TreatWarningsAsErrors`، (2) مشروع xUnit بعشرين اختبارًا على `IsWithinScope` والمُحقِّقات، (3) workflow في GitHub Actions يبني ويشغّل الاختبارات. أسبوعان يرفعان جاهزيته من 75% إلى 90%، وهو عائد لا تعطيه أي ميزة جديدة.
