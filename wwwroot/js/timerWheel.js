window.timerWheel.onScrollStop = function(el, dotNetRef, callbackMethod) {
    let timeout = null;
    el.addEventListener('scroll', function() {
        if (timeout) clearTimeout(timeout);
        timeout = setTimeout(function() {
            dotNetRef.invokeMethodAsync(callbackMethod, el.scrollTop);
        }, 150);
    }, { passive: true });
};
