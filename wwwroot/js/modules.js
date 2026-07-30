App.Modules = {};
(function () {
  const U = App.UI, A = App.Api, R = App.Config.ROLES;

  /* ---------------- Hierarchy loaders + cascading bar ---------------- */
  const PAGE_ALL = { PageNumber: 1, PageSize: 1000 };
  const H = {
    async universities(sel) {
      const u = App.Auth.current();
      sel.innerHTML = '';
      if (u && u.universityId) {
        try {
          const univ = await A.get('api/University/by-id', { query: { UniversityId: u.universityId } });
          const op = document.createElement('option'); op.value = univ.UniversityId; op.textContent = univ.UniversityName; sel.appendChild(op);
        } catch (e) { }
        sel.disabled = true; return u.universityId;
      }
      const p = await A.get('api/University', { query: PAGE_ALL });
      U.fillSelect(sel, p.Data || [], { valueKey: 'UniversityId', textKey: 'UniversityName', placeholder: '— select university —' });
      return null;
    },
    async colleges(sel, universityId) {
      sel.innerHTML = '';
      if (!universityId) { U.fillSelect(sel, [], { placeholder: '— select college —' }); return; }
      const u = App.Auth.current();
      if (u && u.collegeId) {
        try {
          const c = await A.get('api/College/by-id', { query: { CollegeId: u.collegeId } });
          const op = document.createElement('option'); op.value = c.CollegeId; op.textContent = c.CollegeName; sel.appendChild(op);
        } catch (e) { }
        sel.disabled = true; return u.collegeId;
      }
      const p = await A.get('api/College/by-universityId', { query: { UniversityId: universityId, ...PAGE_ALL } });
      U.fillSelect(sel, p.Data || [], { valueKey: 'CollegeId', textKey: 'CollegeName', placeholder: '— select college —' });
    },
    async departments(sel, collegeId) {
      sel.innerHTML = '';
      if (!collegeId) { U.fillSelect(sel, [], { placeholder: '— select department —' }); return; }
      const u = App.Auth.current();
      if (u && u.departmentId) {
        try {
          const d = await A.get('api/Department/by-id', { query: { DepartmentId: u.departmentId } });
          const op = document.createElement('option'); op.value = d.DepartmentId; op.textContent = d.DepartmentName; sel.appendChild(op);
        } catch (e) { }
        sel.disabled = true; return u.departmentId;
      }
      const p = await A.get('api/Department/collegeId', { query: { CollegeId: collegeId, ...PAGE_ALL } });
      U.fillSelect(sel, p.Data || [], { valueKey: 'DepartmentId', textKey: 'DepartmentName', placeholder: '— select department —' });
    },
    async batches(sel, departmentId) {
      sel.innerHTML = '';
      if (!departmentId) { U.fillSelect(sel, [], { placeholder: '— select batch —' }); return; }
      const p = await A.get('api/Batch/by-departmentId', { query: { DepartmentId: departmentId, ...PAGE_ALL } });
      U.fillSelect(sel, p.Data || [], { valueKey: 'BatchId', textKey: 'BatchName', placeholder: '— select batch —' });
    },
  };

  /* Build a cascading parent bar: levels = ['university','college','department','batch']
     Returns { node, get(), reset() } where get() returns {universityId, collegeId, departmentId, batchId} (deepest). */
  function hierarchyBar(levels, onChangeCb) {
    const wrap = document.createElement('div'); wrap.className = 'toolbar';
    const refs = {};
    levels.forEach(lv => {
      const sel = U.select('sel_' + lv, [], {});
      refs[lv] = sel;
      wrap.appendChild(U.field(lv, sel));
    });
    const out = { node: wrap, get: () => ({ universityId: +refs.university?.value || null, collegeId: +refs.college?.value || null, departmentId: +refs.department?.value || null, batchId: +refs.batch?.value || null }) };

    async function cascade(lv) {
      const v = +refs[lv].value || null;
      if (lv === 'university' && refs.college) { refs.college.disabled = false; await H.colleges(refs.college, v); if (refs.college.value) await cascade('college'); }
      if (lv === 'college' && refs.department) { refs.department.disabled = false; await H.departments(refs.department, v); if (refs.department.value) await cascade('department'); }
      if (lv === 'department' && refs.batch) { refs.batch.disabled = false; await H.batches(refs.batch, v); }
      if (onChangeCb) onChangeCb();
    }
    async function init() {
      if (refs.university) {
        await H.universities(refs.university);
        if (refs.university.value) await cascade('university');
        if (!refs.university.disabled) refs.university.onchange = () => cascade('university');
      }
      // wire change handlers for unlocked lower selects
      if (refs.college && !refs.college.disabled) refs.college.onchange = () => cascade('college');
      if (refs.department && !refs.department.disabled) refs.department.onchange = () => cascade('department');
      if (refs.batch && !refs.batch.disabled) refs.batch.onchange = () => cascade('batch');
    }
    init();
    return out;
  }

  /* ---------------- Form builder from spec ---------------- */
  // spec: [{ name, label, type:'text'|'number'|'checkbox'|'textarea'|'select', value, options, required, disabled, min, step, full, placeholder }]
  function formNode(spec, row) {
    const grid = document.createElement('div'); grid.className = 'form-grid';
    spec.forEach(f => {
      let input;
      if (f.type === 'select') input = U.select(f.name, f.options || [], { disabled: f.disabled });
      else if (f.type === 'textarea') input = U.textarea(f.name, f.value, { placeholder: f.placeholder, rows: f.rows });
      else if (f.type === 'checkbox') input = U.checkbox(f.name, !!f.value);
      else input = U.input(f.type || 'text', f.name, f.value, { placeholder: f.placeholder, required: f.required, disabled: f.disabled, min: f.min, step: f.step });
      const fld = U.field(f.label, input);
      if (f.full) fld.classList.add('full');
      grid.appendChild(fld);
    });
    return { node: grid, getData: () => U.collectForm(grid) };
  }

  /* ---------------- Generic CRUD module factory ---------------- */
  function crud(cfg) {
    return {
      title: cfg.title,
      render(container, app) {
        let page = 1; const size = cfg.pageSize || 10;
        let barCtx = {};
        const card = U.el('div', 'card');
        const top = U.el('div', 'toolbar');
        const view = U.el('div'); const pagerBox = U.el('div');
        card.append(top, view, pagerBox);
        container.appendChild(card);

        // optional filter bar (hierarchy)
        let bar = null;
        if (cfg.bar) {
          bar = cfg.bar(() => load(1));
          top.appendChild(bar.node);
        }
        // search box if enabled
        if (cfg.search) {
          const i = U.input('text', 'q', '', { placeholder: 'Search...' });
          i.oninput = U.debounce ? U.debounce(() => load(1), 300) : (() => load(1));
          top.appendChild(U.field('Search', i));
          barCtx._search = () => i.value;
        }
        if (cfg.canManage && cfg.canManage()) {
          const add = U.el('button', 'btn btn-primary btn-sm', '＋ Add');
          add.onclick = () => openForm(null); top.appendChild(add);
        }

        async function load(p) {
          page = p || 1;
          view.innerHTML = '<div class="empty">Loading...</div>';
          try {
            const q = Object.assign({ PageNumber: page, PageSize: size }, cfg.queryExtra ? cfg.queryExtra(bar) : {});
            const paged = await cfg.list(q);
            view.innerHTML = '';
            view.appendChild(U.table(cfg.columns, paged.Data, { empty: cfg.empty }));
            pagerBox.innerHTML = ''; pagerBox.appendChild(U.pager(page, paged, (n) => load(n)));
          } catch (e) { view.innerHTML = `<div class="empty">⚠ ${U.escapeHtml(e.message)}</div>`; }
        }

        function openForm(row) {
          const built = cfg.form(row);
          U.modal({
            title: row ? `Edit ${cfg.singular}` : `Add ${cfg.singular}`,
            bodyHtml: built.node,
            footHtml: `<button class="btn btn-ghost" data-act="cancel">Cancel</button>
                       <button class="btn btn-primary" data-act="save">Save</button>`,
            onClose: () => {},
          });
          $('#modal-foot').onclick = async (e) => {
            const a = e.target.dataset.act;
            if (a === 'cancel') U.closeModal();
            if (a === 'save') {
              const data = built.getData();
              try {
                if (row) await cfg.update(row[cfg.idKey], data);
                else await cfg.add(data);
                U.toast(`${cfg.singular} ${row ? 'updated' : 'created'} successfully.`, 'ok');
                U.closeModal(); load(page);
              } catch (err) { U.toast(err.message, 'err'); }
            }
          };
        }

        cfg._openForm = openForm; // allow columns to trigger edit
        cfg._reload = load;       // allow delete handler to refresh the list
        load(1);
      }
    };
  }

  function $id(id) { return document.getElementById(id); }
  function actionsCell(row, cfg) {
    const c = U.el('div', 'row-actions');
    if (cfg.canManage && cfg.canManage()) {
      const edit = U.el('button', 'btn btn-ghost btn-sm', '✎');
      edit.onclick = () => cfg._openForm(row); c.appendChild(edit);
      const del = U.el('button', 'btn btn-danger btn-sm', '🗑');
      del.onclick = () => U.confirmDialog(`Delete ${cfg.singular} #${row[cfg.idKey]}?`, async () => {
        try { await cfg.remove(row[cfg.idKey]); U.toast('Deleted.', 'ok'); cfg._reload && cfg._reload(); } catch (e) { U.toast(e.message, 'err'); }
      }, 'Delete', true);
      c.appendChild(del);
    }
    return c;
  }

  /* ---------------- helper: roles options for selects ---------------- */
  let _rolesCache = null;
  async function rolesOptions() {
    if (_rolesCache) return _rolesCache;
    const p = await A.get('api/Role', { query: PAGE_ALL });
    _rolesCache = (p.Data || []).map(r => ({ value: r.RoleId, text: r.RoleName }));
    return _rolesCache;
  }

  /* =====================================================================
     UNIVERSITIES
  ===================================================================== */
  (function () {
    const cfg = {
      key: 'universities', title: 'Universities', singular: 'University', idKey: 'UniversityId', pageSize: 10,
      canManage: () => App.Auth.hasAny(App.Config.PERMS.universities.manage),
      columns: [
        { key: 'UniversityId', label: 'ID', width: '60px' },
        { key: 'UniversityName', label: 'Name' },
        { key: 'Description', label: 'Description', render: r => U.escapeHtml(r.Description || '—') },
        { key: 'CreatedAt', label: 'Created', render: r => U.fmtDate(r.CreatedAt) },
        { key: '_a', label: 'Actions', render: r => actionsCell(r, cfg) },
      ],
      list: (q) => A.get('api/University', { query: q }),
      form: (row) => formNode([
        { name: 'UniversityName', label: 'Name', value: row?.UniversityName, required: true },
        { name: 'Description', label: 'Description', type: 'textarea', value: row?.Description, full: true },
      ], row),
      add: (d) => A.post('api/University', d),
      update: (id, d) => A.put('api/University', d, { query: { UniversityId: id } }),
      remove: (id) => A.del('api/University', { query: { UniversityId: id } }),
    };
    cfg._self = cfg;
    App.Modules.universities = crud(cfg);
  })();

  /* =====================================================================
     COLLEGES
  ===================================================================== */
  (function () {
    const cfg = {
      key: 'colleges', title: 'Colleges', singular: 'College', idKey: 'CollegeId', pageSize: 10,
      canManage: () => App.Auth.hasAny(App.Config.PERMS.colleges.manage),
      bar: (reload) => hierarchyBar(['university'], reload),
      queryExtra: (bar) => { const f = bar.get(); return f.universityId ? { UniversityId: f.universityId } : {}; },
      list: (q) => q.UniversityId ? A.get('api/College/by-universityId', { query: q }) : A.get('api/College', { query: q }),
      columns: [
        { key: 'CollegeId', label: 'ID', width: '60px' },
        { key: 'CollegeName', label: 'College' },
        { key: 'UniversityName', label: 'University' },
        { key: 'Description', label: 'Description', render: r => U.escapeHtml(r.Description || '—') },
        { key: '_a', label: 'Actions', render: r => actionsCell(r, cfg) },
      ],
      form: (row) => {
        // Add form needs University select; Update has no parent.
        if (row) return formNode([
          { name: 'CollegeName', label: 'College Name', value: row.CollegeName, required: true },
          { name: 'Description', label: 'Description', type: 'textarea', value: row.Description, full: true },
        ], row);
        const grid = document.createElement('div'); grid.className = 'form-grid';
        const usel = U.select('UniversityId', []); const nsel = U.input('text', 'CollegeName', '', { required: true });
        const desc = U.textarea('Description', '', { rows: 3 });
        grid.appendChild(U.field('University', usel)); grid.appendChild(U.field('College Name', nsel));
        const df = U.field('Description', desc); df.classList.add('full'); grid.appendChild(df);
        H.universities(usel);
        return { node: grid, getData: () => U.collectForm(grid) };
      },
      add: (d) => A.post('api/College', d),
      update: (id, d) => A.put('api/College', d, { query: { CollegeId: id } }),
      remove: (id) => A.del('api/College', { query: { CollegeId: id } }),
    };
    App.Modules.colleges = crud(cfg);
  })();

  /* =====================================================================
     DEPARTMENTS
  ===================================================================== */
  (function () {
    const cfg = {
      key: 'departments', title: 'Departments', singular: 'Department', idKey: 'DepartmentId', pageSize: 10,
      canManage: () => App.Auth.hasAny(App.Config.PERMS.departments.manage),
      bar: (reload) => hierarchyBar(['university', 'college'], reload),
      queryExtra: (bar) => { const f = bar.get(); return f.collegeId ? { CollegeId: f.collegeId } : {}; },
      list: (q) => q.CollegeId ? A.get('api/Department/collegeId', { query: q }) : A.get('api/Department', { query: q }),
      columns: [
        { key: 'DepartmentId', label: 'ID', width: '60px' },
        { key: 'DepartmentName', label: 'Department' },
        { key: 'Description', label: 'Description', render: r => U.escapeHtml(r.Description || '—') },
        { key: '_a', label: 'Actions', render: r => actionsCell(r, cfg) },
      ],
      form: (row) => {
        if (row) return formNode([
          { name: 'DepartmentName', label: 'Department Name', value: row.DepartmentName, required: true },
          { name: 'Description', label: 'Description', type: 'textarea', value: row.Description, full: true },
        ], row);
        const grid = document.createElement('div'); grid.className = 'form-grid';
        const usel = U.select('UniversityId', []); const csel = U.select('CollegeId', []);
        const nsel = U.input('text', 'DepartmentName', '', { required: true });
        const desc = U.textarea('Description', '', { rows: 3 });
        grid.appendChild(U.field('University', usel)); grid.appendChild(U.field('College', csel));
        grid.appendChild(U.field('Department Name', nsel));
        const df = U.field('Description', desc); df.classList.add('full'); grid.appendChild(df);
        H.universities(usel).then(() => { if (usel.value) H.colleges(csel, +usel.value); });
        usel.onchange = () => H.colleges(csel, +usel.value || null);
        return { node: grid, getData: () => U.collectForm(grid) };
      },
      add: (d) => A.post('api/Department', d),
      update: (id, d) => A.put('api/Department', d, { query: { DepartmentId: id } }),
      remove: (id) => A.del('api/Department', { query: { DepartmentId: id } }),
    };
    App.Modules.departments = crud(cfg);
  })();

  /* =====================================================================
     BATCHES
  ===================================================================== */
  (function () {
    const cfg = {
      key: 'batches', title: 'Batches', singular: 'Batch', idKey: 'BatchId', pageSize: 10,
      canManage: () => App.Auth.hasAny(App.Config.PERMS.batches.manage),
      bar: (reload) => hierarchyBar(['university', 'college', 'department'], reload),
      queryExtra: (bar) => { const f = bar.get(); return f.departmentId ? { DepartmentId: f.departmentId } : {}; },
      list: (q) => q.DepartmentId ? A.get('api/Batch/by-departmentId', { query: q }) : A.get('api/Batch', { query: q }),
      columns: [
        { key: 'BatchId', label: 'ID', width: '60px' },
        { key: 'BatchName', label: 'Batch' },
        { key: 'DepartmentName', label: 'Department' },
        { key: 'BatchYear', label: 'Year' },
        { key: 'Description', label: 'Description', render: r => U.escapeHtml(r.Description || '—') },
        { key: '_a', label: 'Actions', render: r => actionsCell(r, cfg) },
      ],
      form: (row) => {
        if (row) return formNode([
          { name: 'BatchName', label: 'Batch Name', value: row.BatchName, required: true },
          { name: 'BatchYear', label: 'Batch Year', type: 'number', value: row.BatchYear, required: true, min: 1990, step: 1 },
          { name: 'Description', label: 'Description', type: 'textarea', value: row.Description, full: true },
        ], row);
        const grid = document.createElement('div'); grid.className = 'form-grid';
        const usel = U.select('UniversityId', []); const csel = U.select('CollegeId', []);
        const dsel = U.select('DepartmentId', []);
        const nm = U.input('text', 'BatchName', '', { required: true });
        const yr = U.input('number', 'BatchYear', new Date().getFullYear(), { required: true, min: 1990, step: 1 });
        const desc = U.textarea('Description', '', { rows: 3 });
        grid.append(U.field('University', usel), U.field('College', csel), U.field('Department', dsel), U.field('Batch Name', nm), U.field('Batch Year', yr));
        const df = U.field('Description', desc); df.classList.add('full'); grid.appendChild(df);
        H.universities(usel).then(() => { if (usel.value) H.colleges(csel, +usel.value).then(() => { if (csel.value) H.departments(dsel, +csel.value); }); });
        usel.onchange = () => H.colleges(csel, +usel.value || null).then(() => { if (csel.value) H.departments(dsel, +csel.value); });
        csel.onchange = () => H.departments(dsel, +csel.value || null);
        return { node: grid, getData: () => U.collectForm(grid) };
      },
      add: (d) => A.post('api/Batch', d),
      update: (id, d) => A.put('api/Batch', d, { query: { BatchId: id } }),
      remove: (id) => A.del('api/Batch', { query: { BatchId: id } }),
    };
    App.Modules.batches = crud(cfg);
  })();

  /* =====================================================================
     SECTIONS
  ===================================================================== */
  (function () {
    const cfg = {
      key: 'sections', title: 'Sections', singular: 'Section', idKey: 'SectionId', pageSize: 10,
      canManage: () => App.Auth.hasAny(App.Config.PERMS.sections.manage),
      bar: (reload) => hierarchyBar(['university', 'college', 'department', 'batch'], reload),
      queryExtra: (bar) => { const f = bar.get(); return f.batchId ? { BatchId: f.batchId } : {}; },
      list: (q) => q.BatchId ? A.get('api/Section/batchId', { query: q }) : A.get('api/Section', { query: q }),
      columns: [
        { key: 'SectionId', label: 'ID', width: '60px' },
        { key: 'SectionName', label: 'Section' },
        { key: 'BatchName', label: 'Batch' },
        { key: '_a', label: 'Actions', render: r => actionsCell(r, cfg) },
      ],
      form: (row) => {
        if (row) return formNode([{ name: 'SectionName', label: 'Section Name', value: row.SectionName, required: true }], row);
        const grid = document.createElement('div'); grid.className = 'form-grid';
        const usel = U.select('UniversityId', []); const csel = U.select('CollegeId', []);
        const dsel = U.select('DepartmentId', []); const bsel = U.select('BatchId', []);
        const nm = U.input('text', 'SectionName', '', { required: true });
        grid.append(U.field('University', usel), U.field('College', csel), U.field('Department', dsel), U.field('Batch', bsel));
        const nf = U.field('Section Name', nm); nf.classList.add('full'); grid.appendChild(nf);
        H.universities(usel).then(() => { if (usel.value) H.colleges(csel, +usel.value).then(() => { if (csel.value) H.departments(dsel, +csel.value).then(() => { if (dsel.value) H.batches(bsel, +dsel.value); }); }); });
        usel.onchange = () => H.colleges(csel, +usel.value || null);
        csel.onchange = () => H.departments(dsel, +csel.value || null);
        dsel.onchange = () => H.batches(bsel, +dsel.value || null);
        return { node: grid, getData: () => U.collectForm(grid) };
      },
      add: (d) => A.post('api/Section', d),
      update: (id, d) => A.put('api/Section', d, { query: { SectionId: id } }),
      remove: (id) => A.del('api/Section', { query: { SectionId: id } }),
    };
    App.Modules.sections = crud(cfg);
  })();

  /* =====================================================================
     ROLES
  ===================================================================== */
  (function () {
    const cfg = {
      key: 'roles', title: 'Roles', singular: 'Role', idKey: 'RoleId', pageSize: 10,
      canManage: () => App.Auth.hasAny(App.Config.PERMS.roles.manage),
      list: (q) => A.get('api/Role', { query: q }),
      columns: [
        { key: 'RoleId', label: 'ID', width: '60px' },
        { key: 'RoleName', label: 'Role Name' },
        { key: '_a', label: 'Actions', render: r => actionsCell(r, cfg) },
      ],
      form: (row) => formNode([{ name: 'RoleName', label: 'Role Name', value: row?.RoleName, required: true, full: true }], row),
      add: (d) => A.post('api/Role', d),
      update: (id, d) => A.put('api/Role', d, { query: { RoleId: id } }),
      remove: (id) => A.del('api/Role', { query: { RoleId: id } }),
    };
    App.Modules.roles = crud(cfg);
  })();

  /* =====================================================================
     USER ROLES
  ===================================================================== */
  (function () {
    const cfg = {
      key: 'userroles', title: 'User Roles', singular: 'UserRole', idKey: '_ur', pageSize: 10,
      canManage: () => App.Auth.hasAny(App.Config.PERMS.userroles.manage),
      list: (q) => A.get('api/UserRole', { query: q }),
      columns: [
        { key: 'UserId', label: 'User ID', width: '70px' },
        { key: 'UserName', label: 'User' },
        { key: 'RoleId', label: 'Role ID', width: '70px' },
        { key: 'RoleName', label: 'Role' },
        { key: '_a', label: 'Actions', render: r => {
          const c = U.el('div', 'row-actions');
          if (cfg.canManage()) {
            const del = U.el('button', 'btn btn-danger btn-sm', '🗑');
            del.onclick = () => U.confirmDialog(`Remove role "${r.RoleName}" from "${r.UserName}"?`, async () => {
              try { await A.del('api/UserRole', { query: { UserId: r.UserId, RoleId: r.RoleId } }); U.toast('Removed.', 'ok'); } catch (e) { U.toast(e.message, 'err'); }
            }, 'Remove', true);
            c.appendChild(del);
          }
          return c;
        } },
      ],
      form: (row) => {
        const grid = document.createElement('div'); grid.className = 'form-grid';
        const uid = U.input('number', 'UserId', '', { required: true, min: 1, step: 1 });
        const rsel = U.select('RoleId', []);
        grid.append(U.field('User ID', uid), U.field('Role', rsel));
        rolesOptions().then(opts => U.fillSelect(rsel, opts, { valueKey: 'value', textKey: 'text', placeholder: '— select role —' }));
        return { node: grid, getData: () => U.collectForm(grid) };
      },
      add: (d) => A.post('api/UserRole', d),
      update: () => Promise.resolve(),  // no update endpoint for UserRole
      remove: () => Promise.resolve(),  // delete is handled inline in the actions cell
    };
    App.Modules.userroles = crud(cfg);
  })();

  /* =====================================================================
     EMPLOYEES  (custom: admin-type tabs, cascading scope in forms)
  ===================================================================== */
  App.Modules.employees = {
    title: 'Employees',
    render(container) {
      let page = 1; const size = 10;
      const card = U.el('div', 'card');
      const top = U.el('div', 'toolbar');
      const search = U.input('text', 'q', '', { placeholder: 'Search name...' });
      search.oninput = () => load(1);
      const activeSel = U.select('active', [{ value: '', text: 'All' }, { value: 'true', text: 'Active' }, { value: 'false', text: 'Inactive' }]);
      activeSel.onchange = () => load(1);
      top.append(U.field('Search', search), U.field('Status', activeSel));

      const perms = App.Config.PERMS.employees;
      const addBtns = [];
      if (App.Auth.hasAny(perms.addUniv)) addBtns.push(['university_admin', '＋ University Admin']);
      if (App.Auth.hasAny(perms.addCollege)) addBtns.push(['college_admin', '＋ College Admin']);
      if (App.Auth.hasAny(perms.addDept)) addBtns.push(['department_admin', '＋ Department Admin']);
      addBtns.forEach(([type, label]) => {
        const b = U.el('button', 'btn btn-primary btn-sm', label); b.onclick = () => openAdd(type); top.appendChild(b);
      });

      const view = U.el('div'); const pagerBox = U.el('div');
      card.append(top, view, pagerBox); container.appendChild(card);

      async function load(p) {
        page = p || 1;
        view.innerHTML = '<div class="empty">Loading...</div>';
        const q = { PageNumber: page, PageSize: size };
        if (search.value.trim()) q.Search = search.value.trim();
        if (activeSel.value) q.IsActive = activeSel.value;
        try {
          const paged = await A.get('api/Employee', { query: q });
          view.innerHTML = '';
          view.appendChild(U.table([
            { key: 'EmployeeId', label: 'ID', width: '60px' },
            { key: 'FullName', label: 'Full Name' },
            { key: 'UserName', label: 'Username' },
            { key: 'UniversityName', label: 'University' },
            { key: 'CollegeName', label: 'College', render: r => U.escapeHtml(r.CollegeName || '—') },
            { key: 'DepartmentName', label: 'Department', render: r => U.escapeHtml(r.DepartmentName || '—') },
            { key: 'IsActive', label: 'Status', render: r => U.pill(r.IsActive) },
            { key: '_a', label: 'Actions', render: r => {
              const c = U.el('div', 'row-actions');
              const canUpd = r.DepartmentId != null ? App.Auth.hasAny(perms.addDept)
                  : r.CollegeId != null ? App.Auth.hasAny(perms.addCollege)
                  : App.Auth.hasAny(perms.addUniv);
              if (canUpd) {
                const ed = U.el('button', 'btn btn-ghost btn-sm', '✎');
                ed.onclick = () => openUpdate(r); c.appendChild(ed);
              }
              return c;
            } },
          ], paged.Data));
          pagerBox.innerHTML = ''; pagerBox.appendChild(U.pager(page, paged, (n) => load(n)));
        } catch (e) { view.innerHTML = `<div class="empty">⚠ ${U.escapeHtml(e.message)}</div>`; }
      }

      function baseFields(row, isAdd) {
        return [
          { name: 'FullName', label: 'Full Name', value: row?.FullName, required: true },
          { name: 'UserName', label: 'Username', value: row?.UserName, required: true },
          ...(isAdd ? [{ name: 'Password', label: 'Password', type: 'text', value: '', required: true }] : []),
          { name: 'Email', label: 'Email', value: row?.Email },
          { name: 'PhoneNumber', label: 'Phone', value: row?.PhoneNumber },
          { name: 'IsActive', label: 'Active', type: 'checkbox', value: row ? row.IsActive : true },
        ];
      }

      function openAdd(type) {
        const grid = document.createElement('div'); grid.className = 'form-grid';
        const usel = U.select('UniversityId', []); const csel = U.select('CollegeId', []); const dsel = U.select('DepartmentId', []);
        baseFields(null, true).forEach(f => { const fld = U.field(f.label, f.type === 'checkbox' ? U.checkbox(f.name, f.value) : (f.type === 'textarea' ? U.textarea(f.name, f.value) : U.input(f.type || 'text', f.name, f.value, { required: f.required }))); if (f.full) fld.classList.add('full'); grid.appendChild(fld); });
        grid.appendChild(U.field('University', usel));
        if (type !== 'university_admin') grid.appendChild(U.field('College', csel));
        if (type === 'department_admin') grid.appendChild(U.field('Department', dsel));
        H.universities(usel).then(() => { if (usel.value && type !== 'university_admin') H.colleges(csel, +usel.value).then(() => { if (csel.value && type === 'department_admin') H.departments(dsel, +csel.value); }); });
        usel.onchange = () => { if (type !== 'university_admin') H.colleges(csel, +usel.value || null).then(() => { if (csel.value && type === 'department_admin') H.departments(dsel, +csel.value); }); };
        csel.onchange = () => { if (type === 'department_admin') H.departments(dsel, +csel.value || null); };
        U.modal({
          title: `Add ${type.replace('_', ' ')}`,
          bodyHtml: grid,
          footHtml: `<button class="btn btn-ghost" data-act="cancel">Cancel</button><button class="btn btn-primary" data-act="save">Save</button>`,
          onClose: () => {},
        });
        $id('modal-foot').onclick = async (e) => {
          if (e.target.dataset.act === 'cancel') return U.closeModal();
          if (e.target.dataset.act === 'save') {
            const d = U.collectForm(grid);
            try { await A.post(`api/Employee/${type}`, d); U.toast('Employee created.', 'ok'); U.closeModal(); load(page); }
            catch (err) { U.toast(err.message, 'err'); }
          }
        };
      }

      function openUpdate(row) {
        const type = row.DepartmentId != null ? 'department_admin' : row.CollegeId != null ? 'college_admin' : 'university_admin';
        const grid = document.createElement('div'); grid.className = 'form-grid';
        baseFields(row, false).forEach(f => { const fld = U.field(f.label, f.type === 'checkbox' ? U.checkbox(f.name, f.value) : U.input(f.type || 'text', f.name, f.value, { required: f.required })); if (f.full) fld.classList.add('full'); grid.appendChild(fld); });
        const csel = U.select('CollegeId', []); const dsel = U.select('DepartmentId', []);
        if (type !== 'university_admin') grid.appendChild(U.field('College', csel));
        if (type === 'department_admin') grid.appendChild(U.field('Department', dsel));
        // pre-fill parent selects from the employee's current university
        if (type !== 'university_admin') H.colleges(csel, row.UniversityId).then(() => { csel.value = row.CollegeId; if (type === 'department_admin') H.departments(dsel, row.CollegeId).then(() => { dsel.value = row.DepartmentId; }); });
        U.modal({
          title: `Update ${type.replace('_', ' ')}`,
          bodyHtml: grid,
          footHtml: `<button class="btn btn-ghost" data-act="cancel">Cancel</button><button class="btn btn-primary" data-act="save">Save</button>`,
          onClose: () => {},
        });
        $id('modal-foot').onclick = async (e) => {
          if (e.target.dataset.act === 'cancel') return U.closeModal();
          if (e.target.dataset.act === 'save') {
            const d = U.collectForm(grid);
            try { await A.put(`api/Employee/${type}`, d, { query: { EmployeeId: row.EmployeeId } }); U.toast('Employee updated.', 'ok'); U.closeModal(); load(page); }
            catch (err) { U.toast(err.message, 'err'); }
          }
        };
      }

      load(1);
    }
  };

  /* =====================================================================
     USERS  (custom: no list; lookup by id + add + update)
  ===================================================================== */
  App.Modules.users = {
    title: 'Users',
    render(container) {
      const canManage = App.Auth.hasAny(App.Config.PERMS.users.manage);
      const card = U.el('div', 'card');
      const top = U.el('div', 'toolbar');
      const idIn = U.input('number', 'UserId', '', { placeholder: 'User ID', min: 1, step: 1 });
      const find = U.el('button', 'btn btn-primary btn-sm', 'Find');
      find.onclick = async () => { if (idIn.value) loadUser(+idIn.value); };
      top.append(U.field('User ID', idIn), find);
      if (canManage) { const add = U.el('button', 'btn btn-primary btn-sm', '＋ Add User'); add.onclick = () => openForm(null); top.appendChild(add); }
      const out = U.el('div');
      card.append(top, out); container.appendChild(card);

      async function loadUser(userId) {
        out.innerHTML = '<div class="empty">Loading...</div>';
        try {
          const u = await A.get('api/User/Id', { query: { UserId: userId } });
          out.innerHTML = '';
          out.appendChild(U.table([
            { key: 'UserId', label: 'ID' }, { key: 'FullName', label: 'Name' }, { key: 'UserName', label: 'Username' },
            { key: 'Email', label: 'Email', render: r => U.escapeHtml(r.Email || '—') },
            { key: 'PhoneNumber', label: 'Phone', render: r => U.escapeHtml(r.PhoneNumber || '—') },
            { key: 'UniversityName', label: 'University', render: r => U.escapeHtml(r.UniversityName || '—') },
            { key: 'IsActive', label: 'Status', render: r => U.pill(r.IsActive) },
            { key: 'CreatedAt', label: 'Created', render: r => U.fmtDate(r.CreatedAt) },
            ...(canManage ? [{ key: '_a', label: 'Actions', render: r => { const c = U.el('div', 'row-actions'); const ed = U.el('button', 'btn btn-ghost btn-sm', '✎'); ed.onclick = () => openForm(r); c.appendChild(ed); return c; } }] : []),
          ], [u]));
        } catch (e) { out.innerHTML = `<div class="empty">⚠ ${U.escapeHtml(e.message)}</div>`; }
      }

      function openForm(row) {
        const grid = document.createElement('div'); grid.className = 'form-grid';
        const usel = U.select('UniversityId', []);
        [
          { name: 'FullName', label: 'Full Name', value: row?.FullName, required: true },
          { name: 'UserName', label: 'Username', value: row?.UserName, required: true },
          ...(row ? [] : [{ name: 'Password', label: 'Password', value: '', required: true }]),
          { name: 'Email', label: 'Email', value: row?.Email },
          { name: 'PhoneNumber', label: 'Phone', value: row?.PhoneNumber },
          { name: 'IsActive', label: 'Active', type: 'checkbox', value: row ? row.IsActive : true },
        ].forEach(f => { const inp = f.type === 'checkbox' ? U.checkbox(f.name, f.value) : U.input(f.type || 'text', f.name, f.value, { required: f.required }); grid.appendChild(U.field(f.label, inp)); });
        const uf = U.field('University (optional)', usel); uf.classList.add('full'); grid.appendChild(uf);
        H.universities(usel); if (row?.UniversityId) usel.value = row.UniversityId;
        U.modal({
          title: row ? 'Edit User' : 'Add User', bodyHtml: grid,
          footHtml: `<button class="btn btn-ghost" data-act="cancel">Cancel</button><button class="btn btn-primary" data-act="save">Save</button>`,
          onClose: () => {},
        });
        $id('modal-foot').onclick = async (e) => {
          if (e.target.dataset.act === 'cancel') return U.closeModal();
          if (e.target.dataset.act === 'save') {
            const d = U.collectForm(grid);
            try {
              if (row) await A.put('api/User', d, { query: { UserId: row.UserId } });
              else await A.post('api/User', d);
              U.toast('User saved.', 'ok'); U.closeModal(); if (idIn.value || row) loadUser(row ? row.UserId : +idIn.value);
            } catch (err) { U.toast(err.message, 'err'); }
          }
        };
      }
      // initial hint
      out.innerHTML = '<div class="empty">Enter a User ID and press Find, or add a new user.</div>';
    }
  };

  /* =====================================================================
     DASHBOARD
  ===================================================================== */
  App.Modules.dashboard = {
    title: 'Dashboard',
    render(container) {
      const u = App.Auth.current();
      const card = U.el('div', 'card');
      card.innerHTML = `<h3 style="margin-top:0">Welcome, ${U.escapeHtml(u?.userName || 'user')} 👋</h3>
        <p style="color:var(--muted)">Roles: <b>${U.escapeHtml((u?.roles || []).join(', ') || '—')}</b></p>
        <p style="color:var(--muted)">Scope: University=${(u && u.universityId != null) ? u.universityId : '—'}, College=${(u && u.collegeId != null) ? u.collegeId : '—'}, Department=${(u && u.departmentId != null) ? u.departmentId : '—'}</p>
        <hr style="border-color:var(--border);margin:18px 0">
        <p>Use the sidebar to manage academic structure, employees, users and roles.</p>`;
      container.appendChild(card);
    }
  };

  // expose hierarchy for app.js scope chip if needed
  App.Modules._hierarchy = H;
})();