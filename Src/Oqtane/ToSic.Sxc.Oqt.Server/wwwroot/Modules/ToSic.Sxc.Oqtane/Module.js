var ToSic = window.ToSic = window.ToSic || {};

ToSic.Sxc = ToSic.Sxc || {};

ToSic.Sxc.Oqtane = ToSic.Sxc.Oqtane || {

    registeredModules: new Map(),

    registerReloadModule: function (dotNetObjectReference, moduleId) {
        const debug = window?.$2sxc?.urlParams?.isDebug() ?? false;
        if (debug) console.log('registerReloadModule', dotNetObjectReference, moduleId);
        this.registeredModules.set(moduleId, dotNetObjectReference);
    },

    unregisterReloadModule: function (moduleId) {
        const debug = window?.$2sxc?.urlParams?.isDebug() ?? false;
        if (debug) console.log('unregisterReloadModule', moduleId)
        if (this.registeredModules.has(moduleId)) {
            this.registeredModules.get(moduleId).dispose();
            this.registeredModules.delete(moduleId);
        }
    },

    reloadModule: async function (moduleId) {
        const debug = window?.$2sxc?.urlParams?.isDebug() ?? false;
        if (debug) console.log('oqt: reloadModule', moduleId);
        // 2024-07-04 stv, we had different strategy for Interactive and SSR to refresh module content on page after edit using 2sxc Edit UI
        // Interactive use our blazor 'ReloadModule' method to update DOM, but this breaks from Oqtane 5.1.2 because of blazor.web.js
        // so fix was just to reload page with window.location.reload()
        if (this.registeredModules.has(moduleId)) {
            if (debug) console.log('oqt: interactive');
            window.location.reload(); // quick fix for Oqtane 5.1.2
            // await this.registeredModules.get(moduleId).invokeMethodAsync('ReloadModule'); // not working in Oqtane 5.1.2
            return Promise.resolve(true);
        } else {
            if (debug) console.log('oqt: static SSR');
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
        const debug = window?.$2sxc?.urlParams?.isDebug() ?? false;
        if (debug) console.log('includeInlineScripts:', inlineScripts);
        for (let i = 0; i < inlineScripts.length; i++) {
          Oqtane.Interop.includeScript(inlineScripts[i].id, inlineScripts[i].src, inlineScripts[i].integrity, inlineScripts[i].crossorigin, inlineScripts[i].type, inlineScripts[i].content, inlineScripts[i].location, inlineScripts[i].dataAttributes)
        }
    }
};

