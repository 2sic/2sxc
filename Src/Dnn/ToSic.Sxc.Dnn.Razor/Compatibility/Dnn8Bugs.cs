/*
    This is a special file to handle DNN 8 bugs

    Background: DNN 8.0.0 delivers a special web.config in the /Portals/ folder, which
    expects the namespace Dnn.Modules.DynamicContentViewer.Helpers

    But this namespace doesn't exist in that version of DNN - it's a leftover of previous
    experiments trying to create a DCC (Dynamic Content Creator)

    So the razor engine complains. Because of this, I'm adding stuff to these namespaces,
    just to make sure the namespace actually exists

*/

// ReSharper disable once CheckNamespace
namespace Dnn.Modules.DynamicContentViewer.Helpers;

[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
class Dnn8Bugs
{
}