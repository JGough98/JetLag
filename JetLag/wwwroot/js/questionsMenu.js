const DRAG_THRESHOLD = 5;
const DRAG_DAMPING   = 0.25;
const LERP_DAMPING   = 0.15;
const SNAP_EPSILON   = 0.5;

export function init(menuEl) {
    const titlebar = menuEl.querySelector('.questions-menu-titlebar');
    const cards    = menuEl.querySelector('.questions-menu-cards');
    const chevron  = menuEl.querySelector('.questions-menu-titlebar-chevron');

    let isOpen        = true;
    let minHeight     = 0;
    let maxHeight     = 0;
    let currentHeight = 0;
    let targetHeight  = 0;
    let rafId         = null;

    let isDragging    = false;
    let dragStartY    = 0;
    let dragStartHeight = 0;
    let dragActive    = false; // true once movement exceeds threshold

    // ── Measurement ────────────────────────────────────────────────────────────

    function measure() {
        // Ensure cards are visible for measurement
        cards.style.display = 'flex';
        cards.style.visibility = 'hidden';
        menuEl.style.height = 'auto';

        minHeight = titlebar.offsetHeight;
        maxHeight = menuEl.scrollHeight;

        cards.style.visibility = '';
        menuEl.style.overflow  = 'hidden';
    }

    // ── Chevron sync ───────────────────────────────────────────────────────────

    function syncChevron() {
        if (chevron) chevron.textContent = isOpen ? '▼' : '▲';
    }

    // ── Animation loop ─────────────────────────────────────────────────────────

    function tick() {
        const damping = dragActive ? DRAG_DAMPING : LERP_DAMPING;
        currentHeight += (targetHeight - currentHeight) * damping;

        if (Math.abs(currentHeight - targetHeight) < SNAP_EPSILON) {
            currentHeight = targetHeight;
            menuEl.style.height = currentHeight + 'px';
            menuEl.classList.remove('questions-menu--animating');
            rafId = null;
            return;
        }

        menuEl.style.height = currentHeight + 'px';
        rafId = requestAnimationFrame(tick);
    }

    function startLerp(to) {
        targetHeight = to;
        menuEl.classList.add('questions-menu--animating');
        if (!rafId) rafId = requestAnimationFrame(tick);
    }

    // ── Pointer helpers ────────────────────────────────────────────────────────

    function getClientY(e) {
        return e.touches ? e.touches[0].clientY : e.clientY;
    }

    // ── Event handlers ─────────────────────────────────────────────────────────

    function onPointerDown(e) {
        dragStartY      = getClientY(e);
        dragStartHeight = currentHeight;
        isDragging      = true;
        dragActive      = false;

        window.addEventListener('mousemove',  onPointerMove);
        window.addEventListener('touchmove',  onPointerMove, { passive: false });
        window.addEventListener('mouseup',    onPointerUp);
        window.addEventListener('touchend',   onPointerUp);
    }

    function onPointerMove(e) {
        if (!isDragging) return;
        if (e.cancelable) e.preventDefault();

        const deltaY = dragStartY - getClientY(e); // up = positive = taller

        if (!dragActive && Math.abs(deltaY) > DRAG_THRESHOLD) {
            dragActive = true;
            menuEl.classList.add('questions-menu--animating');
        }

        if (dragActive) {
            const clamped = Math.max(minHeight, Math.min(maxHeight, dragStartHeight + deltaY));
            targetHeight  = clamped;
            if (!rafId) rafId = requestAnimationFrame(tick);
        }
    }

    function onPointerUp() {
        window.removeEventListener('mousemove',  onPointerMove);
        window.removeEventListener('touchmove',  onPointerMove);
        window.removeEventListener('mouseup',    onPointerUp);
        window.removeEventListener('touchend',   onPointerUp);

        if (!isDragging) return;
        isDragging = false;

        if (dragActive) {
            dragActive = false;
            const midpoint = minHeight + (maxHeight - minHeight) * 0.5;
            isOpen = currentHeight > midpoint;
            syncChevron();
            startLerp(isOpen ? maxHeight : minHeight);
        } else {
            // Pure click — toggle
            isOpen = !isOpen;
            syncChevron();
            startLerp(isOpen ? maxHeight : minHeight);
        }
    }

    // ── Init ───────────────────────────────────────────────────────────────────

    measure();
    currentHeight = maxHeight;
    menuEl.style.height = currentHeight + 'px';
    syncChevron();

    titlebar.addEventListener('mousedown', onPointerDown);
    titlebar.addEventListener('touchstart', onPointerDown, { passive: true });
}
