

// =============================
// 📌 CONTROL DE MENÚ MÓVIL
// =============================
document.addEventListener('DOMContentLoaded', function () {
    const burger = document.getElementById('burgerBtn');
    const mobileMenu = document.getElementById('mobileMenu');
    const overlay = document.getElementById('menuOverlay');
    const closeMenuBtn = document.getElementById('closeMenuBtn');

    function openMenu() {
        mobileMenu?.classList.add('active');
        overlay?.classList.add('active');
        document.body.classList.add('menu-open');

        burger?.setAttribute('aria-expanded', 'true');
        mobileMenu?.setAttribute('aria-hidden', 'false');

        // Focus en el primer link
        setTimeout(() => mobileMenu?.querySelector('a')?.focus(), 200);
    }

    function closeMenu() {
        mobileMenu?.classList.remove('active');
        overlay?.classList.remove('active');
        document.body.classList.remove('menu-open');

        burger?.setAttribute('aria-expanded', 'false');
        mobileMenu?.setAttribute('aria-hidden', 'true');
    }

    if (burger && mobileMenu) {
        burger.addEventListener('click', () => {
            const opened = mobileMenu.classList.contains('active');
            opened ? closeMenu() : openMenu();
        });
    }

    closeMenuBtn?.addEventListener('click', closeMenu);
    overlay?.addEventListener('click', closeMenu);

    // Cerrar con ESC
    document.addEventListener('keydown', function (e) {
        if (e.key === 'Escape' && mobileMenu?.classList.contains('active')) {
            closeMenu();
        }
    });

});


// =============================
// 📌 ANIMACIÓN DE PULSO RADIAL
// =============================
window.restartRadialPulse = function (element) {
    if (!element) return;
    element.classList.remove('pulse');

    // Forzar reflow y reiniciar animación
    requestAnimationFrame(() => {
        void element.offsetWidth;
        element.classList.add('pulse');
    });
};


// =============================
// 📌 UTILIDADES GENERALES
// =============================
window.getWindowWidth = () => window.innerWidth;

window.downloadFileFromUrl = (filename, url) => {
    const link = document.createElement('a');
    link.href = url;
    link.download = filename;
    document.body.appendChild(link);
    link.click();
    document.body.removeChild(link);
};


// =============================
// 📌 DESCARGAS QR
// =============================

// 🔹 Descargar SVG nativo
window.downloadSvgNative = function (svgString, filename) {
    const blob = new Blob([svgString], { type: "image/svg+xml;charset=utf-8" });
    const url = URL.createObjectURL(blob);
    const a = document.createElement("a");
    a.href = url;
    a.download = filename;
    document.body.appendChild(a);
    a.click();
    setTimeout(() => {
        document.body.removeChild(a);
        URL.revokeObjectURL(url);
    }, 100);
};

// 🔹 Convertir SVG a PNG (con canvas)
window.svgStringToPng = function (svgString, width, height, filename) {
    const svgBlob = new Blob([svgString], { type: "image/svg+xml;charset=utf-8" });
    const url = URL.createObjectURL(svgBlob);
    const img = new Image();

    img.onload = function () {
        const canvas = document.createElement("canvas");
        canvas.width = width || img.naturalWidth || img.width;
        canvas.height = height || img.naturalHeight || img.height;
        const ctx = canvas.getContext("2d");
        ctx.drawImage(img, 0, 0, canvas.width, canvas.height);

        // Descarga el PNG
        if (canvas.toBlob) {
            canvas.toBlob(function (blob) {
                const url2 = URL.createObjectURL(blob);
                const a = document.createElement("a");
                a.href = url2;
                a.download = filename || "codigoQR.png";
                document.body.appendChild(a);
                a.click();
                setTimeout(() => {
                    document.body.removeChild(a);
                    URL.revokeObjectURL(url2);
                }, 100);
            }, "image/png");
        } else {
            alert("Tu navegador no soporta canvas.toBlob()");
        }

        URL.revokeObjectURL(url);
    };

    img.onerror = function () {
        alert("No se pudo convertir SVG a PNG");
        URL.revokeObjectURL(url);
    };

    img.src = url;
};

// 🔹 Convertir SVG a PNG DataURL (para PDF)
window.svgStringToPngDataUrl = function (svgString, width, height) {
    return new Promise(function (resolve, reject) {
        const svgBlob = new Blob([svgString], { type: "image/svg+xml;charset=utf-8" });
        const url = URL.createObjectURL(svgBlob);
        const img = new Image();

        img.onload = function () {
            const canvas = document.createElement("canvas");
            canvas.width = width || img.naturalWidth;
            canvas.height = height || img.naturalHeight;
            const ctx = canvas.getContext("2d");
            ctx.drawImage(img, 0, 0, canvas.width, canvas.height);
            resolve(canvas.toDataURL("image/png"));
            URL.revokeObjectURL(url);
        };

        img.onerror = function () {
            console.error("No se pudo convertir SVG a PNG");
            reject("No se pudo convertir SVG a PNG");
            URL.revokeObjectURL(url);
        };

        img.src = url;
    });
};

// 🔹 Exportar QR como PDF con jsPDF
window.exportQrAsPdf = async function (svgString, width, height, filename) {
    try {
        const pngDataUrl = await window.svgStringToPngDataUrl(svgString, width, height);
        const { jsPDF } = window.jspdf || {};
        if (!jsPDF) {
            console.error("jsPDF no está cargado");
            alert("jsPDF no está cargado");
            return;
        }

        const dpiFactor = 0.75; // 1 px ≈ 0.75 pt
        const pdfWidthPt = width * dpiFactor;
        const pdfHeightPt = height * dpiFactor;

        const pdf = new jsPDF({
            orientation: pdfWidthPt > pdfHeightPt ? "landscape" : "portrait",
            unit: "pt",
            format: [pdfWidthPt, pdfHeightPt]
        });

        pdf.addImage(pngDataUrl, "PNG", 0, 0, pdfWidthPt, pdfHeightPt);
        pdf.save(filename || "codigoQR.pdf");
    } catch (err) {
        console.error("Error al exportar PDF:", err);
        alert("No se pudo exportar el PDF");
    }
};


// =============================
// 📌 RESIZE PARA BLAZOR
// =============================
window.resizeListeners = [];

window.subscribeResize = (dotNetObjRef) => {
    function onResize() {
        dotNetObjRef.invokeMethodAsync('UpdateScreenWidth', window.innerWidth);
    }

    window.addEventListener('resize', onResize);
    window.resizeListeners.push({ ref: dotNetObjRef, handler: onResize });

    // Ejecutar una vez al suscribir (estado inicial)
    onResize();
};

window.unsubscribeResize = (dotNetObjRef) => {
    const entry = window.resizeListeners.find(x => x.ref === dotNetObjRef);
    if (entry) {
        window.removeEventListener('resize', entry.handler);
        window.resizeListeners = window.resizeListeners.filter(x => x !== entry);
    }
};

document.addEventListener("click", function (e) {
    if (e.target.closest(".accordion-toggle")) {
        const toggleBtn = e.target.closest(".accordion-toggle");
        const accordionContent = toggleBtn.nextElementSibling; // ✅ contenido justo después

        if (accordionContent && accordionContent.classList.contains("accordion-content")) {
            accordionContent.classList.toggle("active");
        }
    }
});
