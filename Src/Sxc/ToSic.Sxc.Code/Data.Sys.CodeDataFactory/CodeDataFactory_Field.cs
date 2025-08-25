﻿using ToSic.Eav.Data.Sys.PropertyStack;
using ToSic.Razor.Blade;
using ToSic.Sxc.Data.Sys.Fields;
using static ToSic.Sxc.Data.Sys.Typed.TypedHelpers;

namespace ToSic.Sxc.Data.Sys.CodeDataFactory;

partial class CodeDataFactory
{
    public IField? Field(ITypedItem parent, bool supportOldMetadata, string? name, Factory.ConvertItemSettings settings)
    {
        if (name.IsEmptyOrWs())
        {
            if (!settings.EntryPropIsRequired)
                return null; // name is optional, so no error
            throw new ArgumentNullException(nameof(name), @"Field name must not be null or empty.");
        }

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
            return newParent.Field(field, required: settings.ItemIsStrict);

            // throw new NotImplementedException("Path support on this method is not yet supported. Ask iJungleboy");
        }

        return IsErrStrictNameRequired(parent, name, settings.EntryPropIsRequired, settings.ItemIsStrict)
            ? throw ErrStrictForTyped(parent, name)
            : supportOldMetadata
                ? new FieldForDynamic(parent, name, this)
                : new Fields.Field(parent, name, this);
    }

}