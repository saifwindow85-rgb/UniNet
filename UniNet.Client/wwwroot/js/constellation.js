// ===== UniNet — خلفية "Constellation Mesh" الحيّة =====
// عُقَد متوهّجة تنجرف ببطء، خطوط ربط فيروزية، نبضات بيانات تسري بين العُقَد،
// توهّج نيون نابض، غبار ضوئي طافٍ، وتفاعل مع مؤشّر الفأرة — حلقة سلِسة 60fps.
// يُحترَم prefers-reduced-motion (إطار ساكن واحد بلا حركة).
(function () {
    const S = { raf: 0, canvas: null, onMove: null, onLeave: null, ro: null };

    const TEAL = '23,195,178';   // اللون الفيروزي الأساسي
    const BLUE = '150,200,240';  // أزرق فاتح ثانوي
    const LINK = 155;            // أقصى مسافة لرسم خط بين عُقدتين
    const MOUSE_R = 150;         // نطاق تأثير الفأرة

    const rand = (a, b) => a + Math.random() * (b - a);

    function start(target) {
        stop();
        const canvas = typeof target === 'string' ? document.getElementById(target) : target;
        if (!canvas) return;
        const ctx = canvas.getContext('2d');
        if (!ctx) return;
        S.canvas = canvas;

        const reduce = window.matchMedia && window.matchMedia('(prefers-reduced-motion: reduce)').matches;
        const dpr = Math.min(window.devicePixelRatio || 1, 2);

        let W = 0, H = 0;
        let nodes = [], dust = [], pulses = [];
        const mouse = { x: -9999, y: -9999, active: false };

        function build() {
            const area = W * H;
            const nCount = Math.max(16, Math.min(48, Math.round(area / 21000)));
            nodes = [];
            for (let i = 0; i < nCount; i++) {
                nodes.push({
                    x: rand(0, W), y: rand(0, H),
                    vx: rand(-7, 7), vy: rand(-7, 7),      // بكسل/ثانية — انجراف بطيء
                    r: rand(1.6, 3.6),
                    phase: rand(0, Math.PI * 2), freq: rand(0.5, 1.5)
                });
            }
            const dCount = Math.max(28, Math.min(80, Math.round(area / 12000)));
            dust = [];
            for (let i = 0; i < dCount; i++) {
                dust.push({ x: rand(0, W), y: rand(0, H), vx: rand(-4, 4), vy: rand(-9, -2), r: rand(0.4, 1.3), a: rand(0.04, 0.22) });
            }
            pulses = [];
        }

        function resize() {
            const rect = canvas.getBoundingClientRect();
            W = Math.max(1, rect.width); H = Math.max(1, rect.height);
            canvas.width = Math.round(W * dpr); canvas.height = Math.round(H * dpr);
            ctx.setTransform(dpr, 0, 0, dpr, 0, 0);
            build();
        }

        function spawnPulse() {
            if (pulses.length > 6) return;
            for (let k = 0; k < 14; k++) {
                const i = (Math.random() * nodes.length) | 0;
                const j = (Math.random() * nodes.length) | 0;
                if (i === j) continue;
                const a = nodes[i], b = nodes[j];
                const dx = a.x - b.x, dy = a.y - b.y;
                if (dx * dx + dy * dy < LINK * LINK) {
                    pulses.push({ a: i, b: j, t: 0, speed: rand(0.4, 0.85) });
                    return;
                }
            }
        }

        let last = performance.now(), pulseTimer = 0, gt = 0;

        function render(dt) {
            gt += dt;
            ctx.clearRect(0, 0, W, H);

            // غبار ضوئي طافٍ للأعلى
            for (const p of dust) {
                p.x += p.vx * dt; p.y += p.vy * dt;
                if (p.y < -6) { p.y = H + 6; p.x = rand(0, W); }
                if (p.x < -6) p.x = W + 6; else if (p.x > W + 6) p.x = -6;
                ctx.beginPath(); ctx.arc(p.x, p.y, p.r, 0, 6.283);
                ctx.fillStyle = `rgba(${BLUE},${p.a})`; ctx.fill();
            }

            // تحريك العُقَد + جذب نحو الفأرة
            for (const n of nodes) {
                n.x += n.vx * dt; n.y += n.vy * dt;
                if (n.x <= 0 || n.x >= W) n.vx *= -1;
                if (n.y <= 0 || n.y >= H) n.vy *= -1;
                n.x = Math.max(0, Math.min(W, n.x)); n.y = Math.max(0, Math.min(H, n.y));
                if (mouse.active) {
                    const dx = mouse.x - n.x, dy = mouse.y - n.y;
                    const d2 = dx * dx + dy * dy;
                    if (d2 > 1 && d2 < MOUSE_R * MOUSE_R) {
                        const d = Math.sqrt(d2);
                        const f = (1 - d / MOUSE_R) * 46 * dt;   // قوة الجذب (بكسل هذا الإطار)
                        n.x += (dx / d) * f; n.y += (dy / d) * f;
                    }
                }
            }

            // خطوط الربط
            for (let i = 0; i < nodes.length; i++) {
                const a = nodes[i];
                for (let j = i + 1; j < nodes.length; j++) {
                    const b = nodes[j];
                    const dx = a.x - b.x, dy = a.y - b.y;
                    const d2 = dx * dx + dy * dy;
                    if (d2 > LINK * LINK) continue;
                    const d = Math.sqrt(d2);
                    let al = (1 - d / LINK) * 0.5;
                    // إضاءة الخطوط القريبة من الفأرة
                    if (mouse.active) {
                        const mx = (a.x + b.x) / 2 - mouse.x, my = (a.y + b.y) / 2 - mouse.y;
                        if (mx * mx + my * my < MOUSE_R * MOUSE_R) al = Math.min(0.85, al + 0.3);
                    }
                    ctx.beginPath(); ctx.moveTo(a.x, a.y); ctx.lineTo(b.x, b.y);
                    ctx.strokeStyle = `rgba(${TEAL},${al})`; ctx.lineWidth = 1; ctx.stroke();
                }
            }

            // نبضات البيانات تسري على الخطوط
            for (let i = pulses.length - 1; i >= 0; i--) {
                const p = pulses[i];
                p.t += p.speed * dt;
                if (p.t >= 1 || !nodes[p.a] || !nodes[p.b]) { pulses.splice(i, 1); continue; }
                const a = nodes[p.a], b = nodes[p.b];
                const x = a.x + (b.x - a.x) * p.t, y = a.y + (b.y - a.y) * p.t;
                const g = ctx.createRadialGradient(x, y, 0, x, y, 7);
                g.addColorStop(0, `rgba(190,255,246,0.95)`);
                g.addColorStop(1, `rgba(${TEAL},0)`);
                ctx.beginPath(); ctx.arc(x, y, 7, 0, 6.283); ctx.fillStyle = g; ctx.fill();
                ctx.beginPath(); ctx.arc(x, y, 1.7, 0, 6.283); ctx.fillStyle = '#eafffb'; ctx.fill();
            }

            // العُقَد مع توهّج نيون نابض
            for (const n of nodes) {
                const breathe = 0.55 + 0.45 * Math.sin(gt * n.freq + n.phase);
                let boost = 0;
                if (mouse.active) {
                    const dx = mouse.x - n.x, dy = mouse.y - n.y;
                    const d2 = dx * dx + dy * dy;
                    if (d2 < MOUSE_R * MOUSE_R) boost = (1 - Math.sqrt(d2) / MOUSE_R) * 0.9;
                }
                const rr = n.r * (1 + boost * 0.6);
                const glowR = rr * 6;
                const gg = ctx.createRadialGradient(n.x, n.y, 0, n.x, n.y, glowR);
                gg.addColorStop(0, `rgba(${TEAL},${(0.28 + boost * 0.4) * breathe})`);
                gg.addColorStop(1, `rgba(${TEAL},0)`);
                ctx.beginPath(); ctx.arc(n.x, n.y, glowR, 0, 6.283); ctx.fillStyle = gg; ctx.fill();
                ctx.beginPath(); ctx.arc(n.x, n.y, rr, 0, 6.283);
                ctx.fillStyle = `rgba(200,255,248,${0.75 + boost * 0.25})`; ctx.fill();
            }
        }

        function loop(now) {
            const dt = Math.min(0.05, (now - last) / 1000); last = now;
            pulseTimer -= dt;
            if (pulseTimer <= 0) { spawnPulse(); pulseTimer = rand(0.5, 1.5); }
            render(dt);
            S.raf = requestAnimationFrame(loop);
        }

        S.onMove = (e) => {
            const r = canvas.getBoundingClientRect();
            const x = e.clientX - r.left, y = e.clientY - r.top;
            mouse.active = x >= 0 && y >= 0 && x <= r.width && y <= r.height;
            mouse.x = x; mouse.y = y;
        };
        S.onLeave = () => { mouse.active = false; };
        window.addEventListener('pointermove', S.onMove, { passive: true });
        window.addEventListener('pointerdown', S.onMove, { passive: true });
        document.addEventListener('pointerleave', S.onLeave);

        if (typeof ResizeObserver !== 'undefined') {
            S.ro = new ResizeObserver(() => resize());
            S.ro.observe(canvas);
        } else {
            window.addEventListener('resize', resize);
        }

        resize();
        if (reduce) { render(0); }        // إطار ساكن واحد فقط
        else { last = performance.now(); S.raf = requestAnimationFrame(loop); }
    }

    function stop() {
        if (S.raf) cancelAnimationFrame(S.raf);
        if (S.onMove) { window.removeEventListener('pointermove', S.onMove); window.removeEventListener('pointerdown', S.onMove); }
        if (S.onLeave) document.removeEventListener('pointerleave', S.onLeave);
        if (S.ro && S.canvas) { try { S.ro.unobserve(S.canvas); } catch { } S.ro = null; }
        S.raf = 0; S.onMove = null; S.onLeave = null; S.canvas = null;
    }

    window.uninetConstellation = { start, stop };
})();
