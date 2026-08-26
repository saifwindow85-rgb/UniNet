using Contracts.Enums;
using DataAccessLayer.Dbcontext;
using Domain.Entities.Academic_Structure;
using Domain.Entities.Content;
using Domain.Entities.Enums;
using Domain.Entities.Employees;
using Domain.Entities.Identity;
using Domain.Entities.Images;
using Domain.Entities.Students;
using Domain.Entities.Study;
using Microsoft.EntityFrameworkCore;

namespace DataAccessLayer.Seeds
{
    /// <summary>
    /// مُغذٍّ تطويري كبير وواقعي يملأ كامل الكيانات المترابطة (آلاف الصفوف):
    /// جامعات ← كليات ← أقسام ← مواد + دفعات ← شُعَب، فصول دراسية، ربط المواد بالشُّعَب،
    /// موظفون (مسؤولو الجامعة/الكلية/القسم)، محاضرون، طلاب، ونتائج بدرجات ضمن المدى الصحيح.
    /// بيانات عربية واقعية موزّعة على عدّة جامعات وأقسام متنوّعة.
    ///
    /// مُقسَّم إلى مراحل مستقلّة قابلة للاستئناف: كل مرحلة تتحقّق من وجود بياناتها وتتخطّى إن كانت موجودة،
    /// وتقرأ ما تحتاجه من القاعدة مباشرةً — فإن فشلت مرحلة (أو توقّف التشغيل) يُكمل التشغيل التالي الناقص
    /// دون إسقاط القاعدة ودون تكرار.
    /// كلمات المرور مُجزّأة بـ BCrypt فعليًّا فتعمل حسابات الدخول. كلمة المرور الموحّدة: P@ssw0rd123!
    /// </summary>
    public static class TestDataSeeder
    {
        private const string DefaultPassword = "P@ssw0rd123!";
        private static string Hash() => BCrypt.Net.BCrypt.HashPassword(DefaultPassword);
        private static readonly Random Rnd = new Random(20260826);

        private static int _nameCounter;
        private static int _studentCounter;

        // ---- مجمّعات أسماء عربية واقعية ----
        private static readonly string[] FirstNames =
        {
            "أحمد","محمد","علي","حسن","خالد","عمر","يوسف","إبراهيم","عبدالله","سالم","فهد","ناصر","طارق","بلال",
            "ليلى","فاطمة","سارة","مريم","نور","هدى","أسماء","رنا","دعاء","شيماء"
        };
        private static readonly string[] LastNames =
        {
            "الحضرمي","الصنعاني","اليافعي","الكندي","المقطري","العدني","الريمي","الشامي","باعوم","بامطرف",
            "العولقي","الحداد","الأهدل","السقاف","المخلافي","الوصابي"
        };
        private static readonly string[] Lecturers =
        {
            "د. عبدالرحمن باحاج","د. منى الصلاحي","د. سمير العمودي","أ.د. فاروق باعباد","د. هالة الكاف",
            "د. وليد بن سعيد","د. أروى الشعيبي","د. ماجد باوزير","د. ريم الحكيمي","د. عصام باديب",
            "د. نجاة القباطي","د. أنور باشراحيل"
        };

        // ---- كتالوج الكليات/الأقسام/المواد (واقعي) ----
        private static readonly (string College, string Prefix, (string Dept, string[] Subjects)[] Depts)[] Catalog =
        {
            ("كلية علوم الحاسوب وتقنية المعلومات", "CS", new[]
            {
                ("علوم الحاسوب",  new[]{"مقدمة في البرمجة","هياكل البيانات","تحليل الخوارزميات","نظم التشغيل","قواعد البيانات"}),
                ("تقنية المعلومات", new[]{"شبكات الحاسوب","أمن المعلومات","برمجة الويب","إدارة الأنظمة","الحوسبة السحابية"}),
                ("نظم المعلومات", new[]{"تحليل وتصميم النظم","هندسة البرمجيات","ذكاء الأعمال","نظم دعم القرار","إدارة المشاريع"}),
            }),
            ("كلية الهندسة والبترول", "ENG", new[]
            {
                ("الهندسة المدنية", new[]{"الاستاتيكا","مقاومة المواد","الخرسانة المسلحة","ميكانيكا التربة","المساحة"}),
                ("الهندسة الكهربائية", new[]{"الدوائر الكهربائية","الإلكترونيات","الآلات الكهربائية","أنظمة التحكم","الطاقة المتجددة"}),
                ("هندسة النفط والغاز", new[]{"جيولوجيا البترول","هندسة الحفر","هندسة المكامن","إنتاج البترول","معالجة الغاز"}),
            }),
            ("كلية الطب والعلوم الصحية", "MED", new[]
            {
                ("الطب البشري", new[]{"التشريح","علم وظائف الأعضاء","الكيمياء الحيوية","علم الأمراض","علم الأدوية"}),
                ("الصيدلة", new[]{"الكيمياء الصيدلية","علم العقاقير","الصيدلانيات","علم السموم","الأحياء الدقيقة"}),
                ("التمريض", new[]{"أساسيات التمريض","تمريض الباطنة","تمريض الأطفال","صحة المجتمع","الإسعافات الأولية"}),
            }),
            ("كلية العلوم الإدارية", "BUS", new[]
            {
                ("إدارة الأعمال", new[]{"مبادئ الإدارة","السلوك التنظيمي","إدارة الموارد البشرية","الإدارة الاستراتيجية","ريادة الأعمال"}),
                ("المحاسبة", new[]{"مبادئ المحاسبة","المحاسبة المتوسطة","محاسبة التكاليف","المراجعة","المحاسبة الضريبية"}),
                ("التسويق", new[]{"مبادئ التسويق","سلوك المستهلك","التسويق الرقمي","إدارة المبيعات","بحوث التسويق"}),
            }),
            ("كلية التربية", "EDU", new[]
            {
                ("اللغة العربية", new[]{"النحو","الصرف","البلاغة","الأدب العربي","النقد الأدبي"}),
                ("اللغة الإنجليزية", new[]{"القواعد","المحادثة","الأدب الإنجليزي","الترجمة","علم اللغة"}),
                ("الرياضيات", new[]{"التفاضل والتكامل","الجبر الخطي","التحليل العددي","الإحصاء","الهندسة التحليلية"}),
            }),
            ("كلية العلوم", "SCI", new[]
            {
                ("الفيزياء", new[]{"الميكانيكا","الكهرومغناطيسية","الفيزياء الحرارية","فيزياء الكم","البصريات"}),
                ("الكيمياء", new[]{"الكيمياء العامة","الكيمياء العضوية","الكيمياء التحليلية","الكيمياء الفيزيائية","الكيمياء الحيوية"}),
                ("الأحياء", new[]{"علم الخلية","علم الوراثة","علم النبات","علم الحيوان","التقنية الحيوية"}),
            }),
        };

        private static readonly (string Key, string Name, string Desc)[] UniversityDefs =
        {
            ("had",    "جامعة حضرموت", "جامعة حكومية رائدة في محافظة حضرموت."),
            ("sanaa",  "جامعة صنعاء",  "أعرق الجامعات الحكومية في العاصمة صنعاء."),
            ("aden",   "جامعة عدن",    "جامعة حكومية عريقة في مدينة عدن الساحلية."),
            ("taiz",   "جامعة تعز",    "جامعة حكومية في محافظة تعز."),
            ("ibb",    "جامعة إب",     "جامعة حكومية في محافظة إب الخضراء."),
            ("dhamar", "جامعة ذمار",   "جامعة حكومية في محافظة ذمار."),
        };

        public static async Task SeedAsync(AppDbcontext db)
        {
            var now = DateTime.UtcNow;
            var roleIds = await EnsureRolesAsync(db);
            var statusIds = await EnsureStatusesAsync(db);
            int creatorId = await EnsureSuperAdminAsync(db, roleIds["Super Admin"]);

            // مراحل مستقلّة قابلة للاستئناف (كلٌّ يتخطّى إن كانت بياناته موجودة).
            await EnsureAcademicTreeAsync(db, roleIds, creatorId, now);
            await EnsureSectionSubjectsAsync(db, creatorId, now);
            await EnsureStudentsAsync(db, roleIds, statusIds, creatorId, now);
            await EnsureResultsAsync(db, creatorId, now);
            await EnsureExtrasAsync(db, roleIds, creatorId, now);
        }

        // ================= 1) الشجرة الأكاديمية + الفصول + الموظفون =================
        private static async Task EnsureAcademicTreeAsync(AppDbcontext db, Dictionary<string, int> roleIds, int creatorId, DateTime now)
        {
            if (await db.Universities.CountAsync() > 1) return; // موجودة مسبقًا

            var universities = new List<University>();
            for (int i = 0; i < UniversityDefs.Length; i++)
            {
                var def = UniversityDefs[i];
                University univ;
                if (i == 0)
                {
                    univ = await db.Universities.FirstOrDefaultAsync(u => u.Name == "Hadhramout University" || u.Name == def.Name)
                           ?? new University();
                    if (univ.UniversityId == 0) await db.Universities.AddAsync(univ);
                    univ.Name = def.Name; univ.Description = def.Desc; univ.CreatedAt = now; univ.CreatedByUserId = creatorId;
                }
                else
                {
                    univ = new University { Name = def.Name, Description = def.Desc, CreatedAt = now, CreatedByUserId = creatorId };
                    await db.Universities.AddAsync(univ);
                }
                BuildUniversityChildren(univ, i, creatorId, now);
                universities.Add(univ);
            }
            await db.SaveChangesAsync(); // يُدرِج الشجرة كاملة ويملأ المعرّفات

            // الموظفون: مسؤول جامعة/كلية/قسم.
            var adminSpecs = new List<(User User, int Univ, int? Col, int? Dep, int Role)>();
            for (int i = 0; i < universities.Count; i++)
            {
                var univ = universities[i];
                string key = UniversityDefs[i].Key;
                adminSpecs.Add((MakeUser($"univadmin.{key}", UserType.Employee, univ.UniversityId, creatorId, now),
                    univ.UniversityId, null, null, roleIds["UniversityAdmin"]));

                int ci = 0;
                foreach (var col in univ.Colleges)
                {
                    adminSpecs.Add((MakeUser($"colladmin.{key}.{ci}", UserType.Employee, univ.UniversityId, creatorId, now),
                        univ.UniversityId, col.CollegeId, null, roleIds["CollegeAdmin"]));
                    int di = 0;
                    foreach (var dep in col.Departments)
                    {
                        adminSpecs.Add((MakeUser($"deptadmin.{key}.{ci}{di}", UserType.Employee, univ.UniversityId, creatorId, now),
                            univ.UniversityId, col.CollegeId, dep.DepartmentId, roleIds["DepartmentAdmin"]));
                        di++;
                    }
                    ci++;
                }
            }
            await db.Users.AddRangeAsync(adminSpecs.Select(a => a.User));
            await db.SaveChangesAsync();
            foreach (var a in adminSpecs)
            {
                await db.Employees.AddAsync(new Employee { UserId = a.User.UserId, UniversityId = a.Univ, CollegeId = a.Col, DepartmentId = a.Dep });
                await db.UserRoles.AddAsync(new UserRole { UserId = a.User.UserId, RoleId = a.Role });
            }
            await db.SaveChangesAsync();
        }

        // ================= 2) ربط المواد بالشُّعَب (يُقرأ من القاعدة) =================
        private static async Task EnsureSectionSubjectsAsync(AppDbcontext db, int creatorId, DateTime now)
        {
            if (await db.SectionSubjects.AnyAsync()) return;

            var sections = await db.Sections
                .Select(s => new { s.SectionId, DepartmentId = s.Batch.DepartmentId, UniversityId = s.Batch.Department.College.UniversityId })
                .OrderBy(s => s.SectionId).ToListAsync();

            var subjectsByDept = (await db.Subjects.Select(su => new { su.SubjectId, su.DepartmentId }).ToListAsync())
                .GroupBy(x => x.DepartmentId).ToDictionary(g => g.Key, g => g.Select(x => x.SubjectId).ToList());

            var semestersByUniv = (await db.Semesters.Select(se => new { se.SemesterId, se.UniversityId, se.IsCurrent }).ToListAsync())
                .GroupBy(x => x.UniversityId).ToDictionary(g => g.Key, g => g.ToList());

            var links = new List<SectionSubject>();
            int lecIdx = 0;
            foreach (var sec in sections)
            {
                if (!subjectsByDept.TryGetValue(sec.DepartmentId, out var subs)) continue;
                if (!semestersByUniv.TryGetValue(sec.UniversityId, out var sems)) continue;
                var past = sems.FirstOrDefault(x => !x.IsCurrent)?.SemesterId ?? sems.First().SemesterId;
                var current = sems.FirstOrDefault(x => x.IsCurrent)?.SemesterId ?? sems.Last().SemesterId;

                int n = subs.Count;
                for (int i = 0; i < Math.Min(3, n); i++)
                    links.Add(NewLink(sec.SectionId, subs[i], past, ref lecIdx, creatorId, now));
                for (int i = Math.Max(0, n - 3); i < n; i++)
                    links.Add(NewLink(sec.SectionId, subs[i], current, ref lecIdx, creatorId, now));
            }
            await db.SectionSubjects.AddRangeAsync(links);
            await db.SaveChangesAsync();
        }

        private static SectionSubject NewLink(int sectionId, int subjectId, int semesterId, ref int lecIdx, int creatorId, DateTime now) =>
            new SectionSubject
            {
                SectionId = sectionId,
                SubjectId = subjectId,
                SemesterId = semesterId,
                LecturerName = Lecturers[lecIdx++ % Lecturers.Length],
                CreatedAt = now,
                CreatedByUserId = creatorId
            };

        // ================= 3) الطلاب (يُقرأ من القاعدة) =================
        private static async Task EnsureStudentsAsync(AppDbcontext db, Dictionary<string, int> roleIds, Dictionary<string, int> statusIds, int creatorId, DateTime now)
        {
            if (await db.Students.AnyAsync()) return;

            var sections = await db.Sections
                .Select(s => new { s.SectionId, s.BatchId, UniversityId = s.Batch.Department.College.UniversityId })
                .OrderBy(s => s.SectionId).ToListAsync();

            var studentUsers = new List<(User User, int SectionId, int BatchId, int StatusId, bool IsBatchAdmin)>();
            foreach (var sec in sections)
            {
                int count = 5 + Rnd.Next(0, 3); // 5..7 طلاب لكل شعبة
                for (int s = 0; s < count; s++)
                {
                    _studentCounter++;
                    var user = new User
                    {
                        FullName = PickName(),
                        UserName = $"student{_studentCounter}",
                        PasswordHash = Hash(),
                        Email = $"student{_studentCounter}@seed.test",
                        IsActive = true,
                        Type = UserType.Student,
                        UniversityId = sec.UniversityId,
                        CreatedAt = now,
                        CreatedByUserId = creatorId
                    };
                    bool isBatchAdmin = _studentCounter > 1 && _studentCounter % 45 == 0; // تغطية دور BatchAdmin
                    studentUsers.Add((user, sec.SectionId, sec.BatchId, PickStatus(statusIds, _studentCounter), isBatchAdmin));
                }
            }
            await db.Users.AddRangeAsync(studentUsers.Select(s => s.User));
            await db.SaveChangesAsync();

            int num = 0;
            foreach (var s in studentUsers)
            {
                num++;
                await db.Students.AddAsync(new Student
                {
                    UserId = s.User.UserId,
                    StudentNumber = $"2025{num:000000}",
                    BatchId = s.BatchId,
                    SectionId = s.SectionId,
                    StatusId = s.StatusId,
                    EnrollmentDate = new DateTime(2024, 9, 15)
                });
                await db.UserRoles.AddAsync(new UserRole { UserId = s.User.UserId, RoleId = s.IsBatchAdmin ? roleIds["BatchAdmin"] : roleIds["Student"] });
            }
            await db.SaveChangesAsync();
        }

        // ================= 4) النتائج (يُقرأ من القاعدة) =================
        private static async Task EnsureResultsAsync(AppDbcontext db, int creatorId, DateTime now)
        {
            if (await db.StudentResults.AnyAsync()) return;

            var students = await db.Students.Select(s => new { s.StudentId, s.SectionId }).ToListAsync();
            var linksBySection = (await db.SectionSubjects.Select(l => new { l.SectionId, l.SectionSubjectId }).ToListAsync())
                .GroupBy(x => x.SectionId).ToDictionary(g => g.Key, g => g.Select(x => x.SectionSubjectId).ToList());

            var results = new List<StudentResult>();
            foreach (var st in students)
            {
                if (st.SectionId is not int secId || !linksBySection.TryGetValue(secId, out var links)) continue;
                foreach (var linkId in links)
                {
                    var r = new StudentResult { StudentId = st.StudentId, SectionSubjectId = linkId, CreatedAt = now, CreatedByUserId = creatorId };
                    r.SetGrades(Rnd.Next(12, 31), Rnd.Next(8, 21), Rnd.Next(18, 51)); // ضمن المدى؛ Total يُحسب في SQL
                    results.Add(r);
                }
            }
            await db.StudentResults.AddRangeAsync(results);
            await db.SaveChangesAsync();
        }

        // ================= 5) محاضرون + محتوى + صور =================
        private static async Task EnsureExtrasAsync(AppDbcontext db, Dictionary<string, int> roleIds, int creatorId, DateTime now)
        {
            if (!await db.Users.AnyAsync(u => u.UserName == "lecturer1"))
            {
                int uniId = await db.Universities.Select(u => u.UniversityId).FirstAsync();
                var lecUsers = new List<User>();
                for (int i = 1; i <= 8; i++)
                    lecUsers.Add(new User
                    {
                        FullName = PickName(),
                        UserName = $"lecturer{i}",
                        PasswordHash = Hash(),
                        Email = $"lecturer{i}@seed.test",
                        IsActive = true,
                        Type = UserType.Employee,
                        UniversityId = uniId,
                        CreatedAt = now,
                        CreatedByUserId = creatorId
                    });
                await db.Users.AddRangeAsync(lecUsers);
                await db.SaveChangesAsync();
                foreach (var lu in lecUsers)
                    await db.UserRoles.AddAsync(new UserRole { UserId = lu.UserId, RoleId = roleIds["Lecturer"] });
                await db.SaveChangesAsync();
            }

            if (!await db.Images.AnyAsync())
                await db.Images.AddRangeAsync(
                    new Image { FileName = "welcome.jpg", FilePath = "/uploads/welcome.jpg", FileSize = 102400, UploadedAt = now, UploadedByUserId = creatorId, UpdatedAt = now },
                    new Image { FileName = "campus.jpg", FilePath = "/uploads/campus.jpg", FileSize = 204800, UploadedAt = now, UploadedByUserId = creatorId, UpdatedAt = now });

            if (!await db.Posts.AnyAsync())
                await db.Posts.AddRangeAsync(
                    new Post { Title = "مرحبًا بكم في العام الجامعي الجديد", Body = "نرحّب بجميع الطلاب في بداية العام الجامعي ونتمنى لهم التوفيق.", Type = EncontentType.Post, Scope = EnContentScope.Public, CreatedAt = now, CreatedByUserId = creatorId },
                    new Post { Title = "مواعيد المكتبة المركزية", Body = "المكتبة مفتوحة يوميًّا من الثامنة صباحًا حتى الثامنة مساءً.", Type = EncontentType.Post, Scope = EnContentScope.Public, CreatedAt = now, CreatedByUserId = creatorId });

            if (!await db.Announcements.AnyAsync())
                await db.Announcements.AddRangeAsync(
                    new Announcement { Title = "جدول الاختبارات النصفية", Body = "تبدأ الاختبارات النصفية مع بداية الأسبوع الثامن من الفصل الحالي.", Type = EncontentType.Announcement, Scope = EnContentScope.Public, CreatedAt = now, CreatedByUserId = creatorId },
                    new Announcement { Title = "إجازة العيد الوطني", Body = "تُعطّل الدراسة بمناسبة العيد الوطني ليوم واحد.", Type = EncontentType.Announcement, Scope = EnContentScope.Public, CreatedAt = now, CreatedByUserId = creatorId });

            await db.SaveChangesAsync();
        }

        // ================= بناء أبناء الجامعة =================
        private static void BuildUniversityChildren(University univ, int univIndex, int creatorId, DateTime now)
        {
            univ.Semesters.Add(new Semester { Name = "الفصل الدراسي الأول 2024/2025", UniversityId = univ.UniversityId, StartDate = new DateTime(2024, 9, 15), EndDate = new DateTime(2025, 1, 20), IsCurrent = false, CreatedAt = now, CreatedByUserId = creatorId });
            univ.Semesters.Add(new Semester { Name = "الفصل الدراسي الثاني 2024/2025", UniversityId = univ.UniversityId, StartDate = new DateTime(2025, 2, 15), EndDate = new DateTime(2025, 6, 20), IsCurrent = true, CreatedAt = now, CreatedByUserId = creatorId });

            univ.Colleges ??= new List<College>(); // لا تهيئة افتراضية على هذه الخاصية

            for (int k = 0; k < 3; k++)
            {
                var cat = Catalog[(univIndex + k) % Catalog.Length];
                var college = new College { Name = cat.College, Description = $"{cat.College} في {univ.Name}.", CreatedAt = now, CreatedByUserId = creatorId };

                for (int dIdx = 0; dIdx < cat.Depts.Length; dIdx++)
                {
                    var (deptName, subjectNames) = cat.Depts[dIdx];
                    var dept = new Department { Name = deptName, Description = $"قسم {deptName} — {cat.College}.", CreatedAt = now, CreatedByUserId = creatorId, Subjects = new List<Subject>() };

                    for (int si = 0; si < subjectNames.Length; si++)
                        dept.Subjects.Add(new Subject
                        {
                            Code = $"{cat.Prefix}{(dIdx + 1) * 100 + (si + 1)}",
                            Name = subjectNames[si],
                            Description = $"{subjectNames[si]} — {deptName}.",
                            CreditHours = 2 + (si % 3),
                            CreatedAt = now,
                            CreatedByUserId = creatorId
                        });

                    int year = 2023 + (dIdx % 3);
                    var batch = new Batch { Name = $"دفعة {year}", BatchYear = year, Description = $"دفعة {deptName} لعام {year}.", CreatedAt = now, CreatedByUserId = creatorId };
                    batch.Sections.Add(new Section { Name = "شعبة أ", CreatedAt = now, CreatedByUserId = creatorId });
                    batch.Sections.Add(new Section { Name = "شعبة ب", CreatedAt = now, CreatedByUserId = creatorId });
                    dept.Batches.Add(batch);

                    college.Departments.Add(dept);
                }
                univ.Colleges.Add(college);
            }
        }

        // ================= مساعدات =================
        private static User MakeUser(string userName, UserType type, int universityId, int creatorId, DateTime now) => new User
        {
            FullName = PickName(),
            UserName = userName,
            PasswordHash = Hash(),
            Email = $"{userName}@seed.test",
            PhoneNumber = "+9677" + Rnd.Next(10, 100) + Rnd.Next(100000, 1000000),
            IsActive = true,
            Type = type,
            UniversityId = universityId,
            CreatedAt = now,
            CreatedByUserId = creatorId
        };

        private static int PickStatus(Dictionary<string, int> statuses, int i)
        {
            if (i % 13 == 0 && statuses.TryGetValue("متخرّج", out var g)) return g;
            if (i % 17 == 0 && statuses.TryGetValue("موقوف", out var s)) return s;
            if (i % 23 == 0 && statuses.TryGetValue("مؤجّل", out var p)) return p;
            if (statuses.TryGetValue("منتظم", out var e)) return e;
            return statuses.Values.First();
        }

        private static async Task<Dictionary<string, int>> EnsureRolesAsync(AppDbcontext db)
        {
            var names = new[] { "Super Admin", "UniversityAdmin", "CollegeAdmin", "DepartmentAdmin", "Lecturer", "Student", "BatchAdmin" };
            foreach (var name in names)
                if (!await db.Roles.AnyAsync(r => r.Name == name))
                    await db.Roles.AddAsync(new Role { Name = name });
            await db.SaveChangesAsync();
            return await db.Roles.ToDictionaryAsync(r => r.Name, r => r.RoleId);
        }

        private static async Task<Dictionary<string, int>> EnsureStatusesAsync(AppDbcontext db)
        {
            var defs = new[]
            {
                ("منتظم", "طالب منتظم في دراسته."),
                ("متخرّج", "أكمل جميع متطلبات التخرّج."),
                ("موقوف", "موقوف مؤقتًا عن الدراسة."),
                ("مؤجّل", "أجّل دراسته لهذا الفصل."),
                ("منسحب", "انسحب من البرنامج."),
            };
            foreach (var (name, desc) in defs)
                if (!await db.StudentStatuses.AnyAsync(s => s.Name == name))
                    await db.StudentStatuses.AddAsync(new StudentStatus { Name = name, Description = desc });
            await db.SaveChangesAsync();
            return await db.StudentStatuses.ToDictionaryAsync(s => s.Name, s => s.StatusId);
        }

        private static async Task<int> EnsureSuperAdminAsync(AppDbcontext db, int superRoleId)
        {
            var admin = await db.Users.FirstOrDefaultAsync(u => u.UserName == "Ahmed.HU");
            if (admin is null)
            {
                admin = new User
                {
                    FullName = "د. أحمد الحضرمي",
                    UserName = "Ahmed.HU",
                    PasswordHash = Hash(),
                    Email = "ahmed@hu.edu.ye",
                    PhoneNumber = "+967777000111",
                    IsActive = true,
                    Type = UserType.SystemAdmin,
                    CreatedAt = DateTime.UtcNow,
                    CreatedByUserId = null
                };
                await db.Users.AddAsync(admin);
                await db.SaveChangesAsync();
                await db.UserRoles.AddAsync(new UserRole { UserId = admin.UserId, RoleId = superRoleId });
                await db.SaveChangesAsync();
            }
            return admin.UserId;
        }

        private static string PickName()
        {
            string first = FirstNames[_nameCounter % FirstNames.Length];
            string last = LastNames[(_nameCounter / FirstNames.Length) % LastNames.Length];
            _nameCounter++;
            return $"{first} {last}";
        }
    }
}
