using System;

namespace ToSic.Sxc.Polymorphism;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface)]
[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
public class PolymorphResolverAttribute : Attribute
{
    public string Name { get; }

    public PolymorphResolverAttribute(string name) => Name = name;
}