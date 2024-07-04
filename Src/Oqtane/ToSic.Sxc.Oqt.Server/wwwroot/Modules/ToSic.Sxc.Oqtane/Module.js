/* Module Script */
var ToSic = ToSic || {};
ToSic.Sxc = ToSic.Sxc || {};
ToSic.Sxc.Oqtane = {
    // 2024-07-04 stv, looks that is not necesary anymore, we can probably remove it
    registeredModules: new Map(),
    // 2024-07-04 stv, looks that is not necesary anymore, we can probably remove it
    registerReloadModule: function (dotNetObjectReference, moduleId) {
        // console.log('stv: registerReloadModule', dotNetObjectReference, moduleId);
        this.registeredModules.set(moduleId, dotNetObjectReference);
    },
    // 2024-07-04 stv, looks that is not necesary anymore, we can probably remove it
    unregisterReloadModule: function (moduleId) {
        // console.log('stv: unregisterReloadModule', moduleId)
        // todo: avoid memory leaks
        if (this.registeredModules.has(moduleId)) {
            this.registeredModules.get(moduleId).dispose();
            this.registeredModules.delete(moduleId);
        }
    },
    // 2024-07-04 stv, looks that is not necesary anymore, we can probably remove it
    async reloadModule(moduleId) {
        console.log('oqt: reloadModule', moduleId);
        // 2024-07-04 stv, looks that is not necessary anymore, we can probably remove it
        // we had different strategy for Interactive and SSR to refresh module content on page after edit using 2sxc Edit UI
        // Interactive use our blazor 'ReloadModule' method to update DOM, but this breaks from Oqtane 5.1.2 because of blazor.web.js
        // so initial fix was just to reload page with window.location.reload()
        // but after more testing it looks that strategy we use for Static SSR is good enough for all cases
        // if this is true we can remove this code in total
        if (this.registeredModules.has(moduleId)) {
            console.log('oqt: interactive');
            window.location.reload(); // quick fix for Oqtane 5.1.2
            // await this.registeredModules.get(moduleId).invokeMethodAsync('ReloadModule'); // not working in Oqtane 5.1.2
            return Promise.resolve(true);
        } else {
            console.log('oqt: static SSR');
            return Promise.resolve(false);
        }
    },
    getTitleValue: function (title) {
        return document.title;
    },
    getMetaTagContentByName: function (name) {
        var elements = document.getElementsByName(name);
        if (elements.length) {
            return elements[0].content;
        } else {
            return "";
        }
    },
    includeInlineScripts: function (inlineScripts) {
        // 2024-06-12 2tv debug some more
        const debug = window?.$2sxc?.urlParams?.isDebug() ?? false;
        if (debug) console.log('includeInlineScripts:', inlineScripts);
        for (let i = 0; i < inlineScripts.length; i++) {
            Oqtane.Interop.includeScript(inlineScripts[i].id, inlineScripts[i].src, inlineScripts[i].integrity, inlineScripts[i].crossorigin, inlineScripts[i].type, inlineScripts[i].content, inlineScripts[i].location)
        }
    }
    // commented because it is moved to NativeModule.js
    // related to https://github.com/2sic/2sxc/issues/2844
    // /**
    // * copy of Oqtane.Interop.includeScripts from Oqtane v3.1.0
    // * https://github.com/oqtane/oqtane.framework/blob/v3.1.0/Oqtane.Server/wwwroot/js/interop.js#L195..L243
    // * with addition of httpAttributes support
    // * @param {any} scripts
    // */
    //includeScriptsWithAttributes: async function (scripts) {
    //  // 2022-08-20 2dm debug some more
    //  const debug = window?.$2sxc?.urlParams?.isDebug() ?? false;
    //  if (debug) console.log('includeScriptsWithAttributes', scripts);

    //  const bundles = [];
    //  for (let s = 0; s < scripts.length; s++) {
    //    if (scripts[s].bundle === '') {
    //      scripts[s].bundle = scripts[s].href;
    //    }
    //    if (!bundles.includes(scripts[s].bundle)) {
    //      bundles.push(scripts[s].bundle);
    //    }
    //  }
    //  const promises = [];
    //  for (let b = 0; b < bundles.length; b++) {
    //    const urls = [];
    //    for (let s = 0; s < scripts.length; s++) {
    //      if (scripts[s].bundle === bundles[b]) {
    //        urls.push(scripts[s].href);
    //      }
    //    }
    //    promises.push(new Promise((resolve, reject) => {
    //      if (loadjs.isDefined(bundles[b])) {
    //        resolve(true);
    //      }
    //      else {
    //        loadjs(urls, bundles[b], {
    //            async: false,
    //            returnPromise: true,
    //            before: function (path, element) {
    //              for (let s = 0; s < scripts.length; s++) {
    //                // 2022-03-10 start - httpAttributes support
    //                if (path === scripts[s].href && !!scripts[s].htmlAttributes) {
    //                  for (var key in scripts[s].htmlAttributes)
    //                    element.setAttribute(key, scripts[s].htmlAttributes[key]);
    //                }
    //                // 2022-03-10 end - httpAttributes support
    //                if (path === scripts[s].href && scripts[s].integrity !== '') {
    //                  element.integrity = scripts[s].integrity;
    //                }
    //                if (path === scripts[s].href && scripts[s].crossorigin !== '') {
    //                  element.crossOrigin = scripts[s].crossorigin;
    //                }
    //                if (path === scripts[s].href && scripts[s].es6module === true) {
    //                  element.type = "module";
    //                }
    //              }
    //            }
    //          })
    //          .then(function () { resolve(true) })
    //          .catch(function (pathsNotFound) { reject(false) });
    //      }
    //    }));
    //  }
    //  if (promises.length !== 0) {
    //    await Promise.all(promises);
    //  }
    //},
};

//function onLoad() {
//    console.log("ToSic.Sxc.Oqtane, onLoad");
//};

//function onUpdate() {
//    console.log("ToSic.Sxc.Oqtane, onUpdate");
//};

//function onDispose() {
//    console.log("ToSic.Sxc.Oqtane, onDispose");
//};

//export { ToSic, onLoad, onUpdate, onDispose };
