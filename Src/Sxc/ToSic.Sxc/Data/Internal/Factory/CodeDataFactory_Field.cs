using System;
using ToSic.Eav.Data;
using ToSic.Lib.Coding;
using ToSic.Razor.Blade;
using static ToSic.Sxc.Data.Typed.TypedHelpers;

namespace ToSic.Sxc.Data;

partial class CodeDataFactory
{
    public IField Field(ITypedItem parent, string name, bool propsRequired, NoParamOrder noParamOrder = default, bool? required = default)
    {
        //Protect(noParamOrder, nameof(required));
        // TODO: make sure that if we use a path, the field is from the correct parent
        var dot = PropertyStack.PathSeparator.ToString();
        if (name.Contains(dot))
        {
            var parentPath = name.BeforeLast(dot);
            var field = name.AfterLast(dot);
            var newParent = parent.Child(parentPath);
            if (newParent == null)
                throw new NullReferenceException(
                    $"Tried to get the child object the path '{name}' (would be '{parentPath}') but got null. Can't return the field '{field}' of this null object.");
            return newParent.Field(field, required: propsRequired);

            // throw new NotImplementedException("Path support on this method is not yet supported. Ask iJungleboy");
        }

        return IsErrStrict(parent, name, required, propsRequired)
            ? throw ErrStrictForTyped(parent, name)
            : new Field(parent, name, this);
    }

}