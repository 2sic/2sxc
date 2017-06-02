(function(sxc) {
	var i, head, scripts = [
		'/DesktopModules/ToSIC_SexyContent/dist/sxc-edit/sxc-edit.min.js',
		'/DesktopModules/ToSIC_SexyContent/js/2sxc.api.min.js'
	]
	if (sxc) return;
	console.debug('lazy loading 2sxc');
	head = document.getElementsByTagName('head')[0];
	for (i=0; i<scripts.length; i++) head.appendChild(createScriptTag(scripts[i]));
	function createScriptTag(url) {
		var el = document.createElement('script');
		el.src = url;
		return el;
	}
})(window['$2sxc']);