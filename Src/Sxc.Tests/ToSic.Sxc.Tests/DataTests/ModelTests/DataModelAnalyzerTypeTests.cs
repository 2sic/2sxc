using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ToSic.Sxc.Data;
using ToSic.Sxc.Data.Models;

namespace ToSic.Sxc.Tests.DataTests.ModelTests;

[TestClass]
public class DataModelAnalyzerTypeTests : TestBaseSxcDb
{
    private void AssertType<TInspect, TExpected>()
        where TInspect : class, ICanWrapData =>
        Assert.AreEqual(typeof(TExpected), DataModelAnalyzerTestAccessors.GetTargetTypeTac<TInspect>());

    #region NotDecorated - should return itself as the type

    class NotDecorated: ICanWrapData;

    [TestMethod]
    public void NotDecoratedDataModelType() =>
        AssertType<NotDecorated, NotDecorated>();

    #endregion

    #region Interface not Decorated - should return itself as the type

    interface INotDecorated : ICanWrapData;

    [TestMethod]
    [ExpectedException(typeof(TypeInitializationException))]
    public void INotDecoratedType() =>
        AssertType<INotDecorated, INotDecorated>();

    #endregion

    #region Decorated - should return the decorated type

    [ModelCreation(Use = typeof(DecoratedEntity))]
    class Decorated: ModelFromEntity;

    class DecoratedEntity : Decorated;

    [TestMethod]
    public void DecoratedType() =>
        AssertType<Decorated, DecoratedEntity>();

    #endregion


    #region Inherit Decorated but not decorated - should return itself as the type

    class InheritDecorated : Decorated;

    [TestMethod]
    public void InheritDecoratedType() =>
        AssertType<InheritDecorated, InheritDecorated>();

    #endregion

    #region Inherit and redecorate, should return the newly decorated type

    [ModelCreation(Use = typeof(InheritReDecoratedEntity))]
    class InheritReDecorated : InheritDecorated;

    class InheritReDecoratedEntity : InheritReDecorated;

    [TestMethod]
    public void InheritReDecoratedType() =>
        AssertType<InheritReDecorated, InheritReDecoratedEntity>();

    #endregion

    #region Interface decorated - should return the decorated type

    [ModelCreation(Use = typeof(EntityOfIDecorated))]
    interface IDecorated : ICanWrapData;

    class EntityOfIDecorated : InheritReDecorated, IDecorated;

    [TestMethod]
    public void IDecoratedType() =>
        AssertType<IDecorated, EntityOfIDecorated>();

    #endregion
}