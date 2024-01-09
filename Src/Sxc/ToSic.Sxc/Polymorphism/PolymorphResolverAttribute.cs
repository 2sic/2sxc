using System;

namespace ToSic.Sxc.Polymorphism;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface)]
[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
public class PolymorphResolverAttribute(string name) : Attribute
{
    public string Name { get; } = name;
}