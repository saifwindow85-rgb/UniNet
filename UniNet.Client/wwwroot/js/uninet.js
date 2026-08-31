// مساعدات صغيرة يحتاجها Blazor ولا يستطيع تنفيذها بنفسه.
window.uninet = {

    // نقطة الصورة في الخادم محميّة بـ Bearer، والمتصفح لا يُرفق ترويسة Authorization
    // مع <img src="...">، فتظهر الصورة مكسورة دائمًا. الحل: يجلب Blazor البايتات
    // بالرمز عبر HttpClient ثم يُمرّرها هنا لتصير عنوان Blob محلّيًا يصلح لـ src.
    // نستعمل Object URL لا data:base64 — الأخير يضخّم الحجم 33% ويُبقيه في الذاكرة كنصّ.
    createObjectUrl: function (bytes, contentType) {
        const blob = new Blob([new Uint8Array(bytes)], { type: contentType || 'image/*' });
        return URL.createObjectURL(blob);
    },

    // إطلاق الذاكرة عند تفكيك المكوّن — بدونه تتراكم الـ Blobs مع كل تصفّح للخلاصة.
    revokeObjectUrl: function (url) {
        if (url) { URL.revokeObjectURL(url); }
    },

    // يُعيد جسم النافذة إلى أعلاه. الحاجة إليه ظهرت بعد جعل الجسم قابلًا للتمرير:
    // زرّا الحفظ والإلغاء مثبَّتان في التذييل، بينما رسالة الخطأ في أعلى الجسم —
    // فيضغط المستخدم «نشر» وهو أسفل النموذج، ويعود الخطأ إلى منطقة خارج مجال رؤيته.
    scrollModalToTop: function () {
        const body = document.querySelector('.modal-body');
        if (body) { body.scrollTo({ top: 0, behavior: 'smooth' }); }
    },

    // قفل تمرير الصفحة خلف عارض الصورة المكبَّر.
    lockScroll: function (locked) {
        document.body.style.overflow = locked ? 'hidden' : '';
    }
};
