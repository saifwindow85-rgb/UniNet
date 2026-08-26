using Contracts.Responses.AcademicResponses.DepartmentResponses;
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
    private readonly DepartmentApiService _departments;
    private readonly CollegeApiService _colleges;

    public ScopeLookups(ScopeContext scope, DepartmentApiService departments, CollegeApiService colleges)
    {
        _scope = scope;
        _departments = departments;
        _colleges = colleges;
    }

    // كل الأقسام ضمن نطاق المستخدم (بلا تكرار).
    public async Task<List<DepartmentDTO>> DepartmentsAsync()
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

        return list.GroupBy(d => d.DepartmentId).Select(g => g.First()).ToList();
    }

    // نفس السابق كعناصر Combobox جاهزة.
    public async Task<List<Combobox.ComboItem>> DepartmentItemsAsync()
    {
        var list = await DepartmentsAsync();
        return list.Select(d => new Combobox.ComboItem(d.DepartmentId, d.DepartmentName)).ToList();
    }
}
