// Global namespace
window.App = window.App || {};

App.Config = (function () {
  // API base: empty = same origin (recommended when served from wwwroot by the API).
  // User can override via the login screen "API base URL" field (persisted in localStorage).
  const stored = localStorage.getItem('uninet.apiBase') || '';
  let apiBase = stored;

  function setBase(v) {
    apiBase = (v || '').trim();
    localStorage.setItem('uninet.apiBase', apiBase);
  }
  function url(path) {
    // path like 'api/University' -> joins with base (adds trailing slash handling)
    const b = apiBase.replace(/\/+$/, '');
    return `${b}/${path.replace(/^\/+/, '')}`;
  }

  // Role display names (matches JWT role claims exactly)
  const ROLES = {
    SUPER: 'Super Admin',
    UNIV: 'UniversityAdmin',
    COLLEGE: 'CollegeAdmin',
    DEPT: 'DepartmentAdmin',
    LECTURER: 'Lecturer',
    STUDENT: 'Student',
  };

  // Per-module role gates (mirrors backend [Authorize(Roles=...)] where sensible).
  // "view" = can open the page; "manage" = can Add/Edit/Delete.
  const PERMS = {
    universities:  { view: [ROLES.SUPER, ROLES.UNIV, ROLES.COLLEGE, ROLES.DEPT], manage: [ROLES.SUPER] },
    colleges:      { view: [ROLES.SUPER, ROLES.UNIV, ROLES.COLLEGE, ROLES.DEPT], manage: [ROLES.SUPER, ROLES.UNIV] },
    departments:   { view: [ROLES.SUPER, ROLES.UNIV, ROLES.COLLEGE, ROLES.DEPT], manage: [ROLES.SUPER, ROLES.UNIV, ROLES.COLLEGE] },
    batches:       { view: [ROLES.SUPER, ROLES.UNIV, ROLES.COLLEGE, ROLES.DEPT], manage: [ROLES.SUPER, ROLES.UNIV, ROLES.COLLEGE, ROLES.DEPT] },
    sections:      { view: [ROLES.SUPER, ROLES.UNIV, ROLES.COLLEGE, ROLES.DEPT], manage: [ROLES.SUPER, ROLES.UNIV, ROLES.COLLEGE, ROLES.DEPT] },
    employees:     { view: [ROLES.SUPER, ROLES.UNIV, ROLES.COLLEGE, ROLES.DEPT],
                     addUniv: [ROLES.SUPER], addCollege: [ROLES.SUPER, ROLES.UNIV], addDept: [ROLES.SUPER, ROLES.UNIV, ROLES.COLLEGE] },
    users:         { view: [ROLES.SUPER, ROLES.UNIV, ROLES.COLLEGE, ROLES.DEPT], manage: [ROLES.SUPER, ROLES.UNIV, ROLES.COLLEGE, ROLES.DEPT] },
    roles:         { view: [ROLES.SUPER, ROLES.UNIV, ROLES.COLLEGE, ROLES.DEPT], manage: [ROLES.SUPER, ROLES.UNIV, ROLES.COLLEGE, ROLES.DEPT] },
    userroles:     { view: [ROLES.SUPER, ROLES.UNIV, ROLES.COLLEGE, ROLES.DEPT], manage: [ROLES.SUPER, ROLES.UNIV, ROLES.COLLEGE, ROLES.DEPT] },
  };

  return { url, setBase, get base() { return apiBase; }, ROLES, PERMS };
})();