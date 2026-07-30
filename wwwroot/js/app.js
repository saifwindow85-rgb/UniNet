App.App = (function () {
  const U = App.UI;
  const $ = (s) => document.querySelector(s);

  const MENU = [
    { key: 'dashboard', label: 'Dashboard', icon: '🏠' },
    { group: 'Academic' },
    { key: 'universities', label: 'Universities', icon: '🏛️' },
    { key: 'colleges', label: 'Colleges', icon: '🎓' },
    { key: 'departments', label: 'Departments', icon: '📂' },
    { key: 'batches', label: 'Batches', icon: '📦' },
    { key: 'sections', label: 'Sections', icon: '🏫' },
    { group: 'People' },
    { key: 'employees', label: 'Employees', icon: '👥' },
    { key: 'users', label: 'Users', icon: '👤' },
    { key: 'roles', label: 'Roles', icon: '🔑' },
    { key: 'userroles', label: 'User Roles', icon: '🔗' },
  ];

  function canView(key) {
    if (key === 'dashboard') return true;
    const p = App.Config.PERMS[key];
    return p && p.view ? App.Auth.hasAny(p.view) : false;
  }

  function renderNav() {
    const nav = $('#nav'); nav.innerHTML = '';
    MENU.forEach(m => {
      if (m.group) { nav.appendChild(U.el('div', 'nav-group', m.group)); return; }
      if (!canView(m.key)) return;
      const item = U.el('div', 'nav-item', '<span class="ic">' + m.icon + '</span><span>' + m.label + '</span>');
      item.dataset.key = m.key;
      item.onclick = () => navigate(m.key, item);
      nav.appendChild(item);
    });
  }

  function navigate(key, itemEl) {
    document.querySelectorAll('.nav-item').forEach(n => n.classList.remove('active'));
    if (itemEl) itemEl.classList.add('active');
    const mod = App.Modules[key];
    const view = $('#view'); view.innerHTML = '';
    $('#page-title').textContent = mod ? mod.title : key;
    if (mod && mod.render) {
      try { mod.render(view); } catch (e) { view.innerHTML = '<div class="empty">⚠ ' + U.escapeHtml(e.message) + '</div>'; }
    }
  }

  function renderScopeChip() {
    const u = App.Auth.current();
    const chip = $('#scope-chip');
    const parts = [];
    if (u && u.universityId) parts.push('U:' + u.universityId);
    if (u && u.collegeId) parts.push('C:' + u.collegeId);
    if (u && u.departmentId) parts.push('D:' + u.departmentId);
    chip.textContent = parts.length ? 'Scope: ' + parts.join(' · ') : ((u && u.roles && u.roles.indexOf(App.Config.ROLES.SUPER) >= 0) ? 'Super Admin (all)' : '');
  }

  function showApp() {
    $('#login-view').classList.add('hidden');
    $('#app-view').classList.remove('hidden');
    const u = App.Auth.current();
    $('#user-chip').innerHTML = '<b>' + U.escapeHtml((u && u.userName) || '') + '</b>' + U.escapeHtml(((u && u.roles) || []).join(', '));
    renderNav(); renderScopeChip();
    // default to dashboard
    const first = document.querySelector('.nav-item');
    if (first) { first.classList.add('active'); navigate('dashboard'); }
  }

  function showLogin() {
    $('#app-view').classList.add('hidden');
    $('#login-view').classList.remove('hidden');
    $('#login-username').value = ''; $('#login-password').value = '';
  }

  function bindLogin() {
    const form = $('#login-form');
    // prefill api base
    const baseInput = $('#api-base-input'); baseInput.value = App.Config.base || '';
    baseInput.onchange = () => App.Config.setBase(baseInput.value);

    form.onsubmit = async (e) => {
      e.preventDefault();
      const err = $('#login-error'); err.textContent = '';
      const btn = $('#login-btn'); btn.disabled = true; btn.textContent = 'Logging in...';
      try {
        await App.Auth.login($('#login-username').value.trim(), $('#login-password').value);
        showApp();
      } catch (ex) {
        err.textContent = ex.message;
      } finally { btn.disabled = false; btn.textContent = 'Login'; }
    };

    $('#logout-btn').onclick = async () => {
      await App.Auth.logout(true);
      showLogin();
    };

    $('#sidebar-toggle').onclick = () => $('#sidebar').classList.toggle('open');
  }

  function init() {
    bindLogin();
    // auto-login if a valid token exists
    const u = App.Auth.load();
    if (u && App.Auth.accessToken()) {
      // quick sanity: token may be expired; first API call will refresh or logout.
      showApp();
    } else {
      showLogin();
    }
  }

  document.addEventListener('DOMContentLoaded', init);
})();