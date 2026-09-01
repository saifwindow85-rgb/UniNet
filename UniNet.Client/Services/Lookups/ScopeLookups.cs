using Contracts.Enums;
using Contracts.Responses.AcademicResponses.BatchResponses;
using Contracts.Responses.AcademicResponses.CollegeResponses;
using Contracts.Responses.AcademicResponses.DepartmentResponses;
using Contracts.Responses.AcademicResponses.UniversityResponses;
using Contracts.Results;
using UniNet.Client.Services.Http;
using UniNet.Client.Components;
using UniNet.Client.Services.Academic;
using UniNet.Client.State;

namespace UniNet.Client.Services.Lookups;

// مساعد موحّد لبناء قوائم الاختيار المُدركة للنطاق.
// يعالج قيد endpoint الأقسام: "api/Department/collegeId" يتطلب CollegeId ويستخدمه كفلتر —
// لذا UniversityAdmin (بلا CollegeId في نطاقه) كان يُقصَر على كلية واحدة. الحلّ: تجميع أقسام كل كلياته.
public class ScopeLookups
{
    private readonly ScopeContext _scope;
    private readonly UniversityApiService _universities;
    private readonly CollegeApiService _colleges;
    private readonly DepartmentApiService _departments;
    private readonly BatchApiService _batches;

    // القوائم تُطلب مرارًا في شاشة واحدة (تغيير النطاق ذهابًا وإيابًا)، والنطاق ثابت
    // طوال الجلسة — فالتخزين المؤقّت هنا يقتل رحلات شبكة متكرّرة بلا فائدة.
    private readonly Dictionary<string, object> _cache = new();

    public ScopeLookups(ScopeContext scope, UniversityApiService universities, CollegeApiService colleges,
        DepartmentApiService departments, BatchApiService batches)
    {
        _scope = scope;
        _universities = universities;
        _colleges = colleges;
        _departments = departments;
        _batches = batches;
    }

    // سقف الخادم في PagedResultParameters هو 100 ([Range(1, 100)]).
    // كانت هذه الاستدعاءات تطلب 500 فترتدّ كلها بـ 400 وتفشل القوائم صامتةً:
    // لا رسالة، بل قائمة فارغة تُقرأ خطأً على أنها "لا يوجد ضمن نطاقك".
    // الترقيم هنا يجعل الجالب مستقلًّا عن السقف مهما تغيّر لاحقًا.
    private const int PageSize = 100;

    // حارس ضد حلقة لا تنتهي إن أعاد الخادم TotalPages خاطئًا — 2000 عنصر تكفي أي قائمة اختيار.
    private const int MaxPages = 20;

    /// <summary>آخر سبب فشل في التحميل، أو null. تقرؤه الواجهة لتُظهر خطأً بدل قائمة فارغة.</summary>
    public string? LastError { get; private set; }

    private async Task<List<T>> AllPagesAsync<T>(Func<int, Task<ApiResult<PagedResult<T>>>> fetch)
    {
        var all = new List<T>();

        for (var page = 1; page <= MaxPages; page++)
        {
            var res = await fetch(page);

            if (!res.Success)
            {
                // 403 متوقّع ومقصود: نقاط "الكل" مقصورة على مسؤول النظام، والمُقيَّد
                // يصل عبر النقاط المُدركة للنطاق. ما عداه فشل حقيقي يستحق الإبلاغ.
                if (res.Status != 403) LastError = res.Error;
                break;
            }

            if (res.Data?.Data is null) break;

            all.AddRange(res.Data.Data);
            if (page >= res.Data.TotalPages) break;
        }

        return all;
    }

    // يُستدعى عند تبديل الحساب حتى لا تبقى قوائم مستخدم سابق.
    public void ClearCache() { _cache.Clear(); LastError = null; }

    /// <summary>يمسح الخطأ وحده قبل محاولة تحميل جديدة — دون إسقاط المخزون المؤقّت.</summary>
    public void ClearError() => LastError = null;

    private async Task<List<T>> Cached<T>(string key, Func<Task<List<T>>> load)
    {
        if (_cache.TryGetValue(key, out var hit) && hit is List<T> list) return list;
        var loaded = await load();
        _cache[key] = loaded;
        return loaded;
    }

    // ------------------------------------------------------------------ الجامعات

    // مسؤول النظام يرى الجامعات كلها؛ وأي مستخدم آخر مُثبَّت على جامعته وحدها،
    // فلا معنى لعرض قائمة يستطيع الخادم رفض كل عناصرها إلا واحدًا.
    public Task<List<UniversityDTO>> UniversitiesAsync() => Cached("uni", async () =>
    {
        await _scope.EnsureAsync();

        if (_scope.IsSuperAdmin)
            return await AllPagesAsync(page => _universities.GetAllAsync(page, PageSize));

        if (_scope.UniversityId is int uid)
        {
            var one = await _universities.GetByIdAsync(uid);
            return one.Success && one.Data is not null ? new List<UniversityDTO> { one.Data } : new();
        }

        return new();
    });

    // ------------------------------------------------------------------ الكليات

    // الـ endpoint مُدرك للنطاق في الخادم: المُقيَّد يُطبَّق نطاقه تلقائيًا،
    // ومسؤول النظام وحده هو من يُمرَّر له universityId كفلتر اختياري.
    public Task<List<CollegeDTO>> CollegesAsync() => Cached("col", async () =>
    {
        await _scope.EnsureAsync();

        var list = await AllPagesAsync(page => _colleges.GetPerUniversityAsync(null, page, PageSize));

        // CollegeAdmin وما دونه: النطاق يحمل كلية واحدة بعينها — نضيّق القائمة إليها
        // حتى لو أعاد الـ endpoint أكثر، فالخادم سيرفض الباقي على أي حال.
        if (_scope.CollegeId is int cid)
            list = list.Where(c => c.CollegeId == cid).ToList();

        return list;
    });

    // ------------------------------------------------------------------ الأقسام

    // كل الأقسام ضمن نطاق المستخدم (بلا تكرار).
    public Task<List<DepartmentDTO>> DepartmentsAsync() => Cached("dept", async () =>
    {
        await _scope.EnsureAsync();
        var list = new List<DepartmentDTO>();

        if (_scope.IsSuperAdmin)
        {
            list = await AllPagesAsync(page => _departments.GetAllAsync(page, PageSize));
        }
        else if (_scope.CollegeId is int cid)
        {
            // CollegeAdmin/DepartmentAdmin: النطاق يفرض الكلية؛ الـ endpoint يتجاوز الفلتر.
            list = await AllPagesAsync(page => _departments.GetPerCollegeAsync(cid, page, PageSize));
        }
        else if (_scope.UniversityId is not null)
        {
            // UniversityAdmin: اجمع أقسام كل كلية في الجامعة (بدل تسرّب CollegeId وهمي إلى الفلتر).
            // الكليات تُقرأ من CollegesAsync لا بنداء مباشر: مخزَّنة مؤقتًا ومُرقَّمة أصلًا.
            foreach (var college in await CollegesAsync())
                list.AddRange(await AllPagesAsync(page => _departments.GetPerCollegeAsync(college.CollegeId, page, PageSize)));
        }

        // DepartmentAdmin: قسمه وحده — نفس منطق تضييق الكليات أعلاه.
        if (_scope.DepartmentId is int did)
            list = list.Where(d => d.DepartmentId == did).ToList();

        return list.GroupBy(d => d.DepartmentId).Select(g => g.First()).ToList();
    });

    // ------------------------------------------------------------------ الدفعات

    // الدفعات لا تُجلب إلا عبر القسم، فنبني عليها فوق قائمة الأقسام المُدركة للنطاق:
    // مسؤول الجامعة يحصل على دفعات كل أقسام كلياته، ومسؤول القسم على دفعات قسمه وحده.
    public Task<List<BatchDTO>> BatchesAsync() => Cached("batch", async () =>
    {
        await _scope.EnsureAsync();

        if (_scope.IsSuperAdmin)
            return await AllPagesAsync(page => _batches.GetAllAsync(page, PageSize));

        var list = new List<BatchDTO>();

        foreach (var department in await DepartmentsAsync())
            list.AddRange(await AllPagesAsync(page => _batches.GetPerDepartmentAsync(department.DepartmentId, page, PageSize)));

        // BatchAdmin: دفعته وحدها.
        if (_scope.BatchId is int bid)
            list = list.Where(b => b.BatchId == bid).ToList();

        return list.GroupBy(b => b.BatchId).Select(g => g.First()).ToList();
    });

    // ------------------------------------------------------------------ عناصر Combobox جاهزة

    public async Task<List<Combobox.ComboItem>> UniversityItemsAsync() =>
        (await UniversitiesAsync()).Select(u => new Combobox.ComboItem(u.UniversityId, u.UniversityName)).ToList();

    public async Task<List<Combobox.ComboItem>> CollegeItemsAsync() =>
        (await CollegesAsync()).Select(c => new Combobox.ComboItem(c.CollegeId, c.CollegeName, c.UniversityName)).ToList();

    public async Task<List<Combobox.ComboItem>> DepartmentItemsAsync() =>
        (await DepartmentsAsync()).Select(d => new Combobox.ComboItem(d.DepartmentId, d.DepartmentName)).ToList();

    public async Task<List<Combobox.ComboItem>> BatchItemsAsync() =>
        (await BatchesAsync()).Select(b => new Combobox.ComboItem(b.BatchId, b.BatchName, b.DepartmentName)).ToList();

    // ------------------------------------------------------------------ مستوى الفاعل نفسه

    /// <summary>أعمق مطالبة نطاق يحملها الفاعل. null لمسؤول النظام (بلا نطاق).</summary>
    public async Task<EnContentScope?> OwnScopeAsync()
    {
        await _scope.EnsureAsync();

        if (_scope.IsSuperAdmin) return null;
        if (_scope.BatchId is not null) return EnContentScope.Batch;
        if (_scope.DepartmentId is not null) return EnContentScope.Department;
        if (_scope.CollegeId is not null) return EnContentScope.College;
        if (_scope.UniversityId is not null) return EnContentScope.University;

        return null;
    }

    /// <summary>
    /// الكيان الوحيد عند مستوى الفاعل، مبنيًّا من مطالباته بلا أي نداء سرد.
    ///
    /// هذا ليس تحسين أداء بل الإصلاح نفسه: نقاط السرد الأكاديمية مقصورة كلٌّ منها على
    /// الأدوار الأعلى من مستواها — فهي تسرد "أبناءك" لا "مستواك". النتيجة أن كل مسؤول
    /// كان يتلقّى 403 عند طلب قائمة مستواه هو تحديدًا:
    ///   CollegeAdmin  → College/by-universityId  403
    ///   DepartmentAdmin → Department/collegeId    403
    ///   BatchAdmin    → Batch/by-departmentId     403
    /// وعند مستواه لا يوجد ما يُختار أصلًا: كيان واحد يحمل معرّفه في مطالبته.
    /// </summary>
    public async Task<Combobox.ComboItem?> OwnLevelItemAsync()
    {
        var own = await OwnScopeAsync();
        if (own is null) return null;

        int? id = own switch
        {
            EnContentScope.Batch => _scope.BatchId,
            EnContentScope.Department => _scope.DepartmentId,
            EnContentScope.College => _scope.CollegeId,
            EnContentScope.University => _scope.UniversityId,
            _ => null,
        };

        if (id is not int value) return null;

        return new Combobox.ComboItem(value, await OwnLevelNameAsync(own.Value, value));
    }

    // اسم كيان الفاعل. نقاط by-id هي الأخرى مقصورة على أدوار أعلى في بعض المستويات
    // (College/by-id لمسؤول الجامعة فأعلى)، فالفشل هنا قيد صلاحية مقصود لا عطل —
    // نسقط إلى تسمية مفهومة بدل ترك الحقل بلا اسم أو إظهار خطأ لا حيلة للمستخدم فيه.
    private async Task<string> OwnLevelNameAsync(EnContentScope scope, int id)
    {
        switch (scope)
        {
            case EnContentScope.University:
                var u = await _universities.GetByIdAsync(id);
                return u.Success && u.Data is not null ? u.Data.UniversityName : "جامعتك";

            case EnContentScope.College:
                var c = await _colleges.GetByIdAsync(id);
                return c.Success && c.Data is not null ? c.Data.CollegeName : "كليتك";

            case EnContentScope.Department:
                var d = await _departments.GetByIdAsync(id);
                return d.Success && d.Data is not null ? d.Data.DepartmentName : "قسمك";

            case EnContentScope.Batch:
                var b = await _batches.GetByIdAsync(id);
                return b.Success && b.Data is not null ? b.Data.BatchName : "دفعتك";

            default:
                return "—";
        }
    }

    // الموزّع: يُعيد عناصر المستوى المطلوب — نقطة واحدة يستدعيها منتقي النطاق.
    public async Task<List<Combobox.ComboItem>> TargetItemsAsync(EnContentScope scope)
    {
        // مستوى الفاعل نفسه: كيان واحد من مطالباته، بلا نداء سرد يرتدّ 403.
        if (await OwnScopeAsync() == scope)
        {
            var own = await OwnLevelItemAsync();
            return own is null ? new() : new List<Combobox.ComboItem> { own };
        }

        // المستويات الأعمق: نقاط السرد مسموحة له فيها، فتُجلب فعلًا.
        return scope switch
        {
            EnContentScope.University => await UniversityItemsAsync(),
            EnContentScope.College => await CollegeItemsAsync(),
            EnContentScope.Department => await DepartmentItemsAsync(),
            EnContentScope.Batch => await BatchItemsAsync(),
            _ => new List<Combobox.ComboItem>(),
        };
    }

    // ------------------------------------------------------------------ مستويات النطاق المسموحة

    // لا تُعرض للمستخدم مستوياتٌ يعرف مسبقًا أن الخادم سيرفضها:
    //   • Public لمسؤول النظام وحده (ResolveScopeTargetAsync يفرض ذلك).
    //   • ولا يُعرض مستوى أعلى من نطاق الفاعل — مسؤول القسم لا ينشر لكليته.
    public async Task<List<EnContentScope>> AllowedScopesAsync()
    {
        await _scope.EnsureAsync();

        if (_scope.IsSuperAdmin)
            return new() { EnContentScope.Public, EnContentScope.University, EnContentScope.College,
                           EnContentScope.Department, EnContentScope.Batch };

        if (_scope.BatchId is not null)
            return new() { EnContentScope.Batch };

        if (_scope.DepartmentId is not null)
            return new() { EnContentScope.Department, EnContentScope.Batch };

        if (_scope.CollegeId is not null)
            return new() { EnContentScope.College, EnContentScope.Department, EnContentScope.Batch };

        if (_scope.UniversityId is not null)
            return new() { EnContentScope.University, EnContentScope.College,
                           EnContentScope.Department, EnContentScope.Batch };

        return new();
    }

    public static string ScopeLabel(EnContentScope scope) => scope switch
    {
        EnContentScope.Public => "عام — كل المستخدمين",
        EnContentScope.University => "جامعة",
        EnContentScope.College => "كلية",
        EnContentScope.Department => "قسم",
        EnContentScope.Batch => "دفعة",
        _ => "—",
    };

    public static string TargetLabel(EnContentScope scope) => scope switch
    {
        EnContentScope.University => "الجامعة المستهدفة",
        EnContentScope.College => "الكلية المستهدفة",
        EnContentScope.Department => "القسم المستهدف",
        EnContentScope.Batch => "الدفعة المستهدفة",
        _ => "الهدف",
    };
}
