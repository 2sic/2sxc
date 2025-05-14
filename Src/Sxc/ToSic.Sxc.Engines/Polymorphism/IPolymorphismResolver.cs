﻿using ToSic.Lib.DI;
using ToSic.Sxc.Polymorphism.Internal;

namespace ToSic.Sxc.Polymorphism;

/// <summary>
/// A polymorphism resolver - which can determine alternate editions for a view / template
/// </summary>
[ShowApiWhenReleased(ShowApiMode.Never)]
public interface IPolymorphismResolver: ISwitchableService
{
    string Edition(PolymorphismConfiguration config, string overrule, ILog log);
}