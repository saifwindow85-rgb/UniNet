App.UI = (function () {
  const $ = (s, el = document) => el.querySelector(s);
  const el = (tag, cls, html) => { const e = document.createElement(tag); if (cls) e.className = cls; if (html != null) e.innerHTML = html; return e; };

  function escapeHtml(s) {
    if (s === null || s === undefined) return '';
    return String(s).replace(/[&<>"']/g, c => ({ '&': '&amp;', '<': '&lt;', '>': '&gt;', '"': '&quot;', "'": '&#39;' }[c]));
  }
  function fmtDate(d) {
    if (!d) return '—';
    const dt = new Date(d);
    return isNaN(dt) ? '—' : dt.toLocaleString();
  }
  function pill(bool, yes = 'Active', no = 'Inactive') {
    return `<span class="pill ${bool ? 'ok' : 'off'}">${bool ? yes : no}</span>`;
  }

  /* ---------- Toasts ---------- */
  function toast(msg, type = 'info', ms = 3200) {
    const c = $('#toast-container');
    const t = el('div', `toast ${type}`, escapeHtml(msg));
    c.appendChild(t);
    setTimeout(() => { t.style.opacity = '0'; t.style.transition = '.3s'; setTimeout(() => t.remove(), 300); }, ms);
  }

  /* ---------- Modal ---------- */
  function closeModal() { $('#modal-overlay').classList.add('hidden'); $('#modal-body').innerHTML = ''; $('#modal-foot').innerHTML = ''; }
  function modal({ title, bodyHtml, footHtml, onClose }) {
    $('#modal-title').textContent = title;
    const body = $('#modal-body'); body.innerHTML = '';
    if (typeof bodyHtml === 'string') body.innerHTML = bodyHtml; else if (bodyHtml) body.appendChild(bodyHtml);
    $('#modal-foot').innerHTML = footHtml || '';
    const overlay = $('#modal-overlay');
    overlay.classList.remove('hidden');
    const closeBtn = $('#modal-close');
    closeBtn.onclick = () => { closeModal(); onClose && onClose(); };
    overlay.onclick = (e) => { if (e.target === overlay) { closeModal(); onClose && onClose(); } };
  }

  function confirmDialog(text, onYes, yesLabel = 'Confirm', danger = false) {
    modal({
      title: 'Confirm',
      bodyHtml: `<p style="margin:6px 0 14px">${escapeHtml(text)}</p>`,
      footHtml: `<button class="btn btn-ghost" data-act="cancel">Cancel</button>
                 <button class="btn ${danger ? 'btn-danger' : 'btn-primary'}" data-act="yes">${escapeHtml(yesLabel)}</button>`,
      onClose: () => {},
    });
    $('#modal-foot').onclick = (e) => {
      const a = e.target.dataset.act;
      if (a === 'yes') { closeModal(); onYes && onYes(); }
      else if (a === 'cancel') closeModal();
    };
  }

  /* ---------- Table ---------- */
  // columns: [{ key, label, render?(row) -> html, width? }]
  function table(columns, rows, { empty = 'No records found.' } = {}) {
    const wrap = el('div', 'table-wrap');
    if (!rows || !rows.length) { wrap.innerHTML = `<div class="empty">${empty}</div>`; return wrap; }
    const t = el('table');
    const thead = el('thead'); const trh = el('tr');
    columns.forEach(c => { const th = el('th', '', c.label); if (c.width) th.style.width = c.width; trh.appendChild(th); });
    thead.appendChild(trh); t.appendChild(thead);
    const tbody = el('tbody');
    rows.forEach(r => {
      const tr = el('tr');
      columns.forEach(c => {
        const td = el('td');
        td.innerHTML = c.render ? c.render(r) : escapeHtml(r[c.key]);
        tr.appendChild(td);
      });
      tbody.appendChild(tr);
    });
    t.appendChild(tbody); wrap.appendChild(t);
    return wrap;
  }

  /* ---------- Pagination ---------- */
  // PagedResult: { Data, TotalRecords, TotalPages, CurrentPage }
  function pager(page, paged, onPage) {
    const box = el('div', 'pager');
    const cur = paged.CurrentPage || page;
    const total = paged.TotalPages || 1;
    box.appendChild(el('div', 'info', `${paged.TotalRecords != null ? paged.TotalRecords : (paged.Data ? paged.Data.length : 0)} records • page ${cur}/${Math.max(total,1)}`));
    const btns = el('div', 'pager-btns');
    const bPrev = el('button', 'btn btn-ghost btn-sm', '◀ Prev');
    bPrev.disabled = cur <= 1;
    bPrev.onclick = () => onPage(cur - 1);
    const bNext = el('button', 'btn btn-ghost btn-sm', 'Next ▶');
    bNext.disabled = cur >= total;
    bNext.onclick = () => onPage(cur + 1);
    btns.append(bPrev, bNext); box.appendChild(btns);
    return box;
  }

  /* ---------- Field builders ---------- */
  function field(labelText, input) {
    const f = el('label', 'field');
    f.appendChild(el('span', '', labelText));
    f.appendChild(input);
    return f;
  }
  function input(type, name, value, opts = {}) {
    const i = el('input'); i.type = type; i.name = name; i.id = name; i.value = value != null ? value : '';
    if (opts.placeholder) i.placeholder = opts.placeholder;
    if (opts.required) i.required = true;
    if (opts.disabled) i.disabled = true;
    if (opts.min != null) i.min = opts.min;
    if (opts.step) i.step = opts.step;
    return i;
  }
  function select(name, opts, { disabled = false } = {}) {
    const s = el('select'); s.name = name; s.id = name; if (disabled) s.disabled = true;
    opts.forEach(o => { const op = el('option'); op.value = o.value; op.textContent = o.text; if (o.disabled) op.disabled = true; s.appendChild(op); });
    return s;
  }
  function textarea(name, value, opts = {}) {
    const t = el('textarea'); t.name = name; t.id = name; t.rows = opts.rows || 3; t.value = value != null ? value : ''; if (opts.placeholder) t.placeholder = opts.placeholder; return t;
  }
  function checkbox(name, checked) {
    const c = el('input'); c.type = 'checkbox'; c.name = name; c.id = name; c.checked = !!checked; return c;
  }

  // Fill a <select> with options; first option is the placeholder.
  function fillSelect(sel, items, { valueKey, textKey, placeholder = '— select —', keepFirst = false } = {}) {
    if (!keepFirst) sel.innerHTML = '';
    if (placeholder) {
      const op = el('option'); op.value = ''; op.textContent = placeholder; op.disabled = true; sel.appendChild(op);
    }
    items.forEach(it => { const op = el('option'); op.value = it[valueKey]; op.textContent = it[textKey]; sel.appendChild(op); });
  }

  // Collect a form's named inputs into an object (using input.name as key, type-aware).
  function collectForm(container) {
    const out = {};
    container.querySelectorAll('[name]').forEach(n => {
      let v;
      if (n.type === 'checkbox') v = n.checked;
      else if (n.type === 'number') v = n.value === '' ? null : (n.step ? parseFloat(n.value) : parseInt(n.value, 10));
      else v = n.value;
      out[n.name] = v;
    });
    return out;
  }

  return { $, el, escapeHtml, fmtDate, pill, toast, modal, closeModal, confirmDialog, table, pager, field, input, select, textarea, checkbox, fillSelect, collectForm };
})();