mergeInto(LibraryManager.library, {
  EntrarFullscreen: function() {
    var el = document.documentElement;
    var fn = el.requestFullscreen
           || el.webkitRequestFullscreen
           || el.mozRequestFullScreen
           || el.msRequestFullscreen;
    if (fn) fn.call(el);
  }
});