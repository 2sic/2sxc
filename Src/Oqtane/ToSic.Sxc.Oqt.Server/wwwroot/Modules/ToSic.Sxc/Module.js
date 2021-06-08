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
};