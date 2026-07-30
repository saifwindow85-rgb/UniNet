App.Api = (function () {
  // Thin fetch wrapper with JWT auth + refresh-on-401 + ProblemDetails parsing.
  let refreshing = false;

  function authHeaders() {
    const t = App.Auth.accessToken();
    return t ? { Authorization: `Bearer ${t}` } : {};
  }

  async function parseError(res) {
    let msg = `Request failed (${res.status})`;
    try {
      const ct = res.headers.get('content-type') || '';
      if (ct.includes('application/json')) {
        const body = await res.json();
        // ASP.NET ProblemDetails or custom envelopes
        if (body.title) msg = body.title;
        if (body.message) msg = body.message;
        if (body.Errors && Array.isArray(body.Errors) && body.Errors.length) msg = body.Errors.join(' • ');
        if (body.errors) {
          // validation problem details: { errors: { Field: [..] } }
          const flat = [];
          for (const k in body.errors) flat.push(...(body.errors[k] || []));
          if (flat.length) msg = flat.join(' • ');
        }
      } else {
        const txt = await res.text();
        if (txt) msg = txt;
      }
    } catch (_) { /* keep default */ }
    return msg;
  }

  async function doRefresh() {
    const rt = App.Auth.refreshToken();
    if (!rt) return false;
    if (refreshing) return false;
    refreshing = true;
    try {
      const res = await fetch(App.Config.url('api/Login/refresh'), {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({ RefreshToken: rt }),
      });
      if (!res.ok) { App.Auth.clear(); return false; }
      const data = await res.json();
      App.Auth.storeTokens(data.AccesseToken || data.accesseToken, data.RefreshToken || data.refreshToken);
      refreshing = false;
      return true;
    } catch (e) { App.Auth.clear(); refreshing = false; return false; }
  }

  async function request(method, path, { query, body, allowRetry = true } = {}) {
    let url = App.Config.url(path);
    if (query) url += queryString(query);

    const opt = { method, headers: { ...authHeaders() } };
    if (body !== undefined) {
      opt.headers['Content-Type'] = 'application/json';
      opt.body = JSON.stringify(body);
    }

    let res = await fetch(url, opt);

    // try one refresh on 401
    if (res.status === 401 && allowRetry) {
      const ok = await doRefresh();
      if (ok) {
        opt.headers = { ...authHeaders(), 'Content-Type': 'application/json' };
        res = await fetch(url, opt);
      } else {
        App.Auth.logout(false);
        throw new Error('Session expired. Please log in again.');
      }
    }

    if (!res.ok) {
      const msg = await parseError(res);
      const err = new Error(msg);
      err.status = res.status;
      throw err;
    }

    if (res.status === 204) return null;
    const ct = res.headers.get('content-type') || '';
    return ct.includes('application/json') ? res.json() : res.text();
  }

  function queryString(obj) {
    const parts = [];
    for (const k in obj) {
      const v = obj[k];
      if (v === undefined || v === null || v === '') continue;
      parts.push(`${encodeURIComponent(k)}=${encodeURIComponent(v)}`);
    }
    return parts.length ? `?${parts.join('&')}` : '';
  }

  return {
    get: (p, opt) => request('GET', p, opt),
    post: (p, body, opt) => request('POST', p, { ...opt, body }),
    put: (p, body, opt) => request('PUT', p, { ...opt, body }),
    del: (p, opt) => request('DELETE', p, opt),
  };
})();