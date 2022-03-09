/* Module Script */
var ToSic = ToSic || {};

ToSic.Sxc = {
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
  includeClientScripts: async function (sxcResources) {
    const bundles = [];
    for (let s = 0; s < sxcResources.length; s++) {
      if (sxcResources[s].bundle === null) {
        sxcResources[s].bundle = sxcResources[s].url;
      }
      if (!bundles.includes(sxcResources[s].bundle)) {
        bundles.push(sxcResources[s].bundle);
      }
    }
    const promises = [];
    for (let b = 0; b < bundles.length; b++) {
      const urls = [];
      for (let s = 0; s < sxcResources.length; s++) {
        if (sxcResources[s].bundle === bundles[b]) {
          urls.push(sxcResources[s].url);
        }
      }
      promises.push(new Promise((resolve, reject) => {
        if (loadjs.isDefined(bundles[b])) {
          resolve(true);
        } else {
          loadjs(urls, bundles[b], {
            async: false,
            returnPromise: true,
            before: function (path, element) {
              for (let s = 0; s < sxcResources.length; s++) {
                if (path === sxcResources[s].url) {
                  for (var key in sxcResources[s].htmlAttributes)
                    element.setAttribute(key, sxcResources[s].htmlAttributes[key]);
                  if (sxcResources[s].integrity !== null) {
                    element.integrity = sxcResources[s].integrity;
                  }
                  if (sxcResources[s].crossOrigin !== null) {
                    element.crossOrigin = sxcResources[s].crossOrigin;
                  }
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
      await Promise.all(promises);
    }
  }
};