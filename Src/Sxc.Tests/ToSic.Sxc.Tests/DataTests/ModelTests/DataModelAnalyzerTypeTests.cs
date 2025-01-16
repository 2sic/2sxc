using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ToSic.Eav.Data;
using ToSic.Sxc.Models;
using ToSic.Sxc.Models.Attributes;

namespace ToSic.Sxc.Tests.DataTests.ModelTests;

[TestClass]
public class DataModelAnalyzerTypeTests : TestBaseSxcDb
{
    private void AssertType<TInspect, TExpected>()
        where TInspect : class, IDataModel =>
        Assert.AreEqual(typeof(TExpected), DataModelAnalyzerTestAccessors.GetTargetTypeTac<TInspect>());

    #region NotDecorated - should return itself as the type

    class NotDecorated: IDataModel;

    [TestMethod]
    public void NotDecoratedDataModelType() =>
        AssertType<NotDecorated, NotDecorated>();

    #endregion

    #region Interface not Decorated - should return itself as the type

    interface INotDecorated : IDataModel;

    [TestMethod]
    [ExpectedException(typeof(TypeInitializationException))]
    public void INotDecoratedType() =>
        AssertType<INotDecorated, INotDecorated>();

    #endregion

    #region Decorated - should return the decorated type

    [DataModelConversion(Map = [typeof(DataModelFrom<IEntity, Decorated, DecoratedEntity>)])]
    class Decorated: DataModel;

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

    [DataModelConversion(Map = [typeof(DataModelFrom<IEntity, InheritReDecorated, InheritReDecoratedEntity>)])]
    class InheritReDecorated : InheritDecorated;

    class InheritReDecoratedEntity : InheritReDecorated;

    [TestMethod]
    public void InheritReDecoratedType() =>
        AssertType<InheritReDecorated, InheritReDecoratedEntity>();

    #endregion

    #region Interface decorated - should return the decorated type

    [DataModelConversion(Map = [typeof(DataModelFrom<IEntity, IDecorated, EntityOfIDecorated>)])]
    interface IDecorated : IDataModel;

    class EntityOfIDecorated : InheritReDecorated, IDecorated;

    [TestMethod]
    public void IDecoratedType() =>
        AssertType<IDecorated, EntityOfIDecorated>();

    #endregion
}