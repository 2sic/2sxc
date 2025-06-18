(function (sxc) {
    if (sxc || window['2sxc-loading']) return;
    window['2sxc-loading'] = true;
    var scripts = [
        '/DesktopModules/ToSic.Sxc/js/2sxc.api.min.js',
        '/DesktopModules/ToSic.Sxc/dist/inpage/inpage.min.js',
    ];
    console.debug('lazy loading 2sxc');
    var head = document.getElementsByTagName('head')[0];
    for (var i = 0; i < scripts.length; i++) head.appendChild(createScriptTag(scripts[i]));
    function createScriptTag(url) {
        var el = document.createElement('script');
        el.setAttribute('defer', 'defer');
        el.src = url;
        return el;
    }
})(window['$2sxc']);