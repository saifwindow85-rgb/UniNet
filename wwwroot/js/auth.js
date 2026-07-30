App.Auth = (function () {
  const K_ACC = 'uninet.access';
  const K_REF = 'uninet.refresh';

  function storeTokens(access, refresh) {
    localStorage.setItem(K_ACC, access);
    if (refresh) localStorage.setItem(K_REF, refresh);
  }
  function accessToken() { return localStorage.getItem(K_ACC); }
  function refreshToken() { return localStorage.getItem(K_REF); }
  function clear() { localStorage.removeItem(K_ACC); localStorage.removeItem(K_REF); }

  // Decode JWT payload without verifying (frontend cannot verify; server validates signature).
  function decode(token) {
    try {
      const p = token.split('.')[1];
      const b64 = p.replace(/-/g, '+').replace(/_/g, '/');
      const json = decodeURIComponent(atob(b64).split('').map(c =>
        '%' + ('00' + c.charCodeAt(0).toString(16)).slice(-2)).join(''));
      return JSON.parse(json);
    } catch (e) { return null; }
  }

  let _user = null; // cached decoded { userId, userName, roles[], universityId?, collegeId?, departmentId?, batchId? }

  function load() {
    const t = accessToken();
    if (!t) { _user = null; return null; }
    const c = decode(t);
    if (!c) { clear(); return null; }
    // Claims: nameidentifier=UserId, name=UserName, Role (array or single), UniversityId/CollegeId/DepartmentId/BatchId
    const roles = [];
    const raw = c.role || c['http://schemas.xmlsoap.org/ws/2005/05/identity/claims/role'];
    if (Array.isArray(raw)) roles.push(...raw); else if (raw) roles.push(raw);
    _user = {
      userId: parseInt((c['http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier'] != null ? c['http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier'] : (c.nameidentifier != null ? c.nameidentifier : c.sub)), 10),
      userName: c['http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name'] != null ? c['http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name'] : (c.name != null ? c.name : c.unique_name),
      roles,
      universityId: c.UniversityId ? parseInt(c.UniversityId, 10) : null,
      collegeId: c.CollegeId ? parseInt(c.CollegeId, 10) : null,
      departmentId: c.DepartmentId ? parseInt(c.DepartmentId, 10) : null,
      batchId: c.BatchId ? parseInt(c.BatchId, 10) : null,
    };
    return _user;
  }

  function current() { return _user || load(); }

  function hasRole(r) {
    const u = current();
    return !!u && u.roles.includes(r);
  }
  function hasAny(roles) { return roles.some(hasRole); }
  function isSuperAdmin() { return hasRole(App.Config.ROLES.SUPER); }

  // Scope info for "SmartDropDownLists" locking
  function scope() {
    const u = current();
    return {
      universityId: (u && u.universityId != null) ? u.universityId : null,
      collegeId: (u && u.collegeId != null) ? u.collegeId : null,
      departmentId: (u && u.departmentId != null) ? u.departmentId : null,
    };
  }

  async function login(userName, password) {
    const res = await fetch(App.Config.url('api/Login/login'), {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify({ UserName: userName, Password: password }),
    });
    if (res.status === 401) throw new Error('Invalid username or password.');
    if (res.status === 403) throw new Error('Your account is banned.');
    if (!res.ok) {
      let msg = 'Login failed.';
      try { const b = await res.json(); if (b && b.message) msg = b.message; } catch (_) {}
      throw new Error(msg);
    }
    const data = await res.json();
    const token = data.AccesseToken || data.accesseToken;
    if (!token) throw new Error('Unexpected login response.');
    storeTokens(token, data.RefreshToken || data.refreshToken);
    load();
    return current();
  }

  async function logout(callApi = true) {
    const rt = refreshToken();
    if (callApi && rt) {
      try { await fetch(App.Config.url('api/Login/logOut'), {
        method: 'POST', headers: { ...({}), 'Content-Type': 'application/json' }, body: JSON.stringify({ RefreshToken: rt }),
      }); } catch (_) {}
    }
    clear(); _user = null;
  }

  return { login, logout, storeTokens, accessToken, refreshToken, clear, load, current, hasRole, hasAny, isSuperAdmin, scope };
})();