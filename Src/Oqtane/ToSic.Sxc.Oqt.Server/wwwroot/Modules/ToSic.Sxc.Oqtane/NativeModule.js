/* native JavaScript module */

/**
 * copy of Oqtane.Interop.includeScripts from Oqtane v3.1.0
 * https://github.com/oqtane/oqtane.framework/blob/v3.1.0/Oqtane.Server/wwwroot/js/interop.js#L195..L243
 * with addition of httpAttributes support
 * @param {any} scripts
 */
async function includeScriptsWithAttributes(scripts) {
  // 2022-08-20 2dm debug some more
  const debug = window?.$2sxc?.urlParams?.isDebug() ?? false;
  if (debug) console.log('includeScriptsWithAttributes', scripts);

  //// remove existing scripts
  //for (let s = 0; s < scripts.length; s++) {
  //  const src = scripts[s].href;
  //  if (src !== "") {
  //    let script = document.querySelector("script[src=\"" + CSS.escape(src) + "\"]");
  //    if (script !== null) script.remove();
  //  }
  //}

  const bundles = [];
  for (let s = 0; s < scripts.length; s++) {
    if (scripts[s].bundle === '') {
      scripts[s].bundle = scripts[s].href;
    }
    if (!bundles.includes(scripts[s].bundle)) {
      bundles.push(scripts[s].bundle);
    }
  }
  const promises = [];
  for (let b = 0; b < bundles.length; b++) {
    const urls = [];
    for (let s = 0; s < scripts.length; s++) {
      if (scripts[s].bundle === bundles[b]) {
        urls.push(scripts[s].href);
      }
    }
    promises.push(new window.Promise((resolve, reject) => {
      if (loadjs.isDefined(bundles[b])) {
        resolve(true);
      }
      else {
        loadjs(urls, bundles[b], {
          async: false,
          returnPromise: true,
          before: function (path, element) {
            for (let s = 0; s < scripts.length; s++) {
              // 2022-03-10 start - httpAttributes support
              if (path === scripts[s].href && !!scripts[s].htmlAttributes) {
                for (var key in scripts[s].htmlAttributes)
                  element.setAttribute(key, scripts[s].htmlAttributes[key]);
              }
              // 2022-03-10 end - httpAttributes support
              if (path === scripts[s].href && scripts[s].integrity !== '') {
                element.integrity = scripts[s].integrity;
              }
              if (path === scripts[s].href && scripts[s].crossorigin !== '') {
                element.crossOrigin = scripts[s].crossorigin;
              }
              if (path === scripts[s].href && scripts[s].es6module === true) {
                element.type = "module";
              }
            }
          }
        })
          .then(function () { resolve(true) })
          .catch(function (pathsNotFound) { reject(false) });
      }
    }));
  }
  if (promises.length !== 0) {
    await window.Promise.all(promises);
  }
}

export { includeScriptsWithAttributes };