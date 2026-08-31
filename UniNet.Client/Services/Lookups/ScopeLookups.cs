using Contracts.Enums;
using Contracts.Responses.AcademicResponses.BatchResponses;
using Contracts.Responses.AcademicResponses.CollegeResponses;
using Contracts.Responses.AcademicResponses.DepartmentResponses;
using Contracts.Responses.AcademicResponses.UniversityResponses;
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

    // يُستدعى عند تبديل الحساب حتى لا تبقى قوائم مستخدم سابق.
    public void ClearCache() => _cache.Clear();

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
        {
            var all = await _universities.GetAllAsync(1, 500);
            return all.Success && all.Data?.Data is not null ? all.Data.Data : new();
        }

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

        var res = await _colleges.GetPerUniversityAsync(null, 1, 500);
        var list = res.Success && res.Data?.Data is not null ? res.Data.Data : new();

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
            var r = await _departments.GetAllAsync(1, 500);
            if (r.Success && r.Data?.Data is not null) list = r.Data.Data;
        }
        else if (_scope.CollegeId is int cid)
        {
            // CollegeAdmin/DepartmentAdmin: النطاق يفرض الكلية؛ الـ endpoint يتجاوز الفلتر.
            var r = await _departments.GetPerCollegeAsync(cid, 1, 500);
            if (r.Success && r.Data?.Data is not null) list = r.Data.Data;
        }
        else if (_scope.UniversityId is not null)
        {
            // UniversityAdmin: اجمع أقسام كل كلية في الجامعة (بدل تسرّب CollegeId وهمي إلى الفلتر) — بالتوازي.
            var cols = await _colleges.GetPerUniversityAsync(null, 1, 500);
            if (cols.Success && cols.Data?.Data is not null)
            {
                var tasks = cols.Data.Data.Select(c => _departments.GetPerCollegeAsync(c.CollegeId, 1, 500)).ToList();
                foreach (var r in await Task.WhenAll(tasks))
                    if (r.Success && r.Data?.Data is not null) list.AddRange(r.Data.Data);
            }
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
        {
            var all = await _batches.GetAllAsync(1, 500);
            return all.Success && all.Data?.Data is not null ? all.Data.Data : new();
        }

        var list = new List<BatchDTO>();
        var depts = await DepartmentsAsync();

        var tasks = depts.Select(d => _batches.GetPerDepartmentAsync(d.DepartmentId, 1, 500)).ToList();
        foreach (var r in await Task.WhenAll(tasks))
            if (r.Success && r.Data?.Data is not null) list.AddRange(r.Data.Data);

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

    // الموزّع: يُعيد عناصر المستوى المطلوب — نقطة واحدة يستدعيها منتقي النطاق.
    public Task<List<Combobox.ComboItem>> TargetItemsAsync(EnContentScope scope) => scope switch
    {
        EnContentScope.University => UniversityItemsAsync(),
        EnContentScope.College => CollegeItemsAsync(),
        EnContentScope.Department => DepartmentItemsAsync(),
        EnContentScope.Batch => BatchItemsAsync(),
        _ => Task.FromResult(new List<Combobox.ComboItem>()),
    };

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
