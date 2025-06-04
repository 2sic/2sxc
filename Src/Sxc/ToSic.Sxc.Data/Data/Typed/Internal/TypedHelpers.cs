using System.Runtime.CompilerServices;
using ToSic.Sys.Code.Help;
using ToSic.Sys.Exceptions;

namespace ToSic.Sxc.Data.Internal.Typed;

// TODO: MAKE INTERNAL AGAIN AFTER MOVING TO ToSic.Sxc.Custom

[ShowApiWhenReleased(ShowApiMode.Never)]
public static class TypedHelpers
{
    public static bool ContainsKey<TNode>(string name, TNode start, Func<TNode, string, bool> checkNode, Func<TNode, string, TNode> dig) where TNode: class
    {
        var parts = PropertyStack.SplitPathIntoParts(name);
        if (!parts.Any()) return false;

        var current = start;
        var max = parts.Length - 1;
        for (var i = 0; i < parts.Length; i++)
        {
            var key = parts[i];
            var has = checkNode(current, key); // current.Attributes.ContainsKey(key);
            if (i == max || !has) return has;

            // has = true, and we have more nodes, so we must check the children
            //var children = current.Children(key);
            //if (!children.Any()) return false;
            current = dig(current, key); // children[0];
            if (current == null) return false;
        }

        return false;

    }

    public static IEnumerable<string> FilterKeysIfPossible(NoParamOrder noParamOrder, IEnumerable<string> only, IEnumerable<string> result)
    {
        if (result == null) return Array.Empty<string>();

        if (only == default || !only.Any()) return result;
        var filtered = result.Where(r => only.Any(k => k.EqualsInsensitive(r))).ToArray();
        return filtered;
    }

    public static bool IsErrStrict(bool found, bool? required, bool requiredDefault)
        => !found && (required ?? requiredDefault);


    public static bool IsErrStrict(ITyped parent, string name, bool? required, bool requiredDefault)
        => !parent.ContainsKey(name) && (required ?? requiredDefault);

    private const int MaxKeysToUseList = 20;

    public static Exception ErrStrictForTyped(ITyped parent, string name, [CallerMemberName] string cName = default)
    {
        // Note that the parent may not fully implement som APIs, so we must try/catch everything
        const string unknown = "unknown";
        string typeName = null;
        var keys = new[] { "keys can't be determined" };
        string id = null;
        //string guid = null;
        var title = "title can't be determined";
        if (parent != null)
        {
            try
            {
                keys = parent.Keys()?.ToArray() ?? keys;
            }
            catch { /* ignore */ }
            try
            {
                var item = parent as ITypedItem;
                typeName = item?.Type?.Name;
                id = item?.Id.ToString();
                //guid = item?.Guid.ToString();
                title = item?.Title;
            }
            catch { /* ignore */ }
        }

        var help = new CodeHelp
        {
            Name = "get-help",
            Detect = name,
            UiMessage = null,
            DetailsHtml = $@"<div>
<em>You tried to use {cName}(""{name}"") which failed. Here is more info about the object:</em>
<br>
Content-Type: <strong>{typeName ?? unknown}</strong>
<br>
Item Title: <strong>{title ?? unknown}</strong>
<br>
Item Id: <strong>{id ?? unknown}</strong>
<br>
<em>Fields of type {typeName}:</em>
{(keys.Length > MaxKeysToUseList
    ? "<br>" + string.Join(", ", keys) + "<br>"
    : $@"<ol>
    <li>
    {string.Join("</li><li>", keys)}
    </li>
</ol>")}
</div>",
        };
        return ErrStrict(name, help, cName);
    }


    public static Exception ErrStrict(string name, CodeHelp codeHelp = default, [CallerMemberName] string cName = default)
    {
        var info = $"Either a) correct the name '{name}'; b) use {cName}(\"{name}\", required: false); or c) or use AsItem(..., propsRequired: false) or similar";
        var msg = cName == "."
            ? $".{name} not found and 'strict' is true, meaning that an error is thrown. {info}"
            : $"{cName}('{name}', ...) not found and 'strict' is true, meaning that an error is thrown. {info}";
        var argEx = new ArgumentException(msg, nameof(name));
        if (codeHelp != default)
            return new ExceptionWithHelp(codeHelp, inner: argEx);
        return argEx;
    }

}