using Microsoft.VisualStudio.TestTools.UnitTesting;
using ToSic.Sxc.Data;
using ToSic.Sxc.Data.Models;

namespace ToSic.Sxc.Tests.DataTests.ModelTests;

[TestClass]
public class DataModelAnalyzerTests : TestBaseSxcDb
{
    private void AssertTypeName<T>(string name)
        where T : class, ICanWrapData =>
        Assert.AreEqual(name, DataModelAnalyzerTestAccessors.GetContentTypeNamesTac<T>().Flat);

    private void AssertStreamNames<T>(string namesCsv)
        where T : class, ICanWrapData =>
        Assert.AreEqual(namesCsv, DataModelAnalyzerTestAccessors.GetStreamNameListTac<T>().Flat);

    class NotDecorated: ICanWrapData;

    [TestMethod]
    public void NotDecoratedDataModelType() =>
        AssertTypeName<NotDecorated>(nameof(NotDecorated));

    [TestMethod]
    public void NotDecoratedDataModelStream() =>
        AssertStreamNames<NotDecorated>(nameof(NotDecorated));

    [TestMethod]
    public void NotDecoratedDataModelStreamList() =>
        AssertStreamNames<NotDecorated>(nameof(NotDecorated));

    class NotDecoratedModel : ICanWrapData;

    [TestMethod]
    public void NotDecoratedModelStreamList() =>
        AssertStreamNames<NotDecoratedModel>(nameof(NotDecoratedModel) + "," + nameof(NotDecorated));

    // Objects starting with an "I" won't have the "I" removed in the name checks
    class INotDecoratedModel : ICanWrapData;

    [TestMethod]
    public void INotDecoratedModelStreamList() =>
        AssertStreamNames<INotDecoratedModel>(nameof(INotDecoratedModel) + ",INotDecorated");

    interface INotDecorated: ICanWrapData;

    [TestMethod]
    public void INotDecoratedType() =>
        AssertTypeName<INotDecorated>(nameof(INotDecorated) + ',' + nameof(INotDecorated).Substring(1));

    [TestMethod]
    public void INotDecoratedStream() =>
        AssertStreamNames<INotDecorated>(nameof(INotDecorated) + ",NotDecorated");


    private const string ForContentType1 = "Abc";
    private const string StreamName1 = "AbcStream";
    [ModelSource(ContentTypes = ForContentType1, Streams = StreamName1)]
    class Decorated: ICanWrapData;

    [TestMethod]
    public void DecoratedType() =>
        AssertTypeName<Decorated>(ForContentType1);

    [TestMethod]
    public void DecoratedStream() =>
        AssertStreamNames<Decorated>(StreamName1);


    class InheritDecorated : Decorated;

    [TestMethod]
    public void InheritDecoratedType() =>
        AssertTypeName<InheritDecorated>(nameof(InheritDecorated));

    [TestMethod]
    public void InheritDecoratedStream() =>
        AssertStreamNames<InheritDecorated>(nameof(InheritDecorated));


    private const string ForContentTypeReDecorated = "ReDec";
    private const string StreamNameReDecorated = "ReDecStream";
    [ModelSource(ContentTypes = ForContentTypeReDecorated, Streams = StreamNameReDecorated + ",Abc")]
    class InheritReDecorated : InheritDecorated;

    [TestMethod]
    public void InheritReDecoratedType() =>
        AssertTypeName<InheritReDecorated>(ForContentTypeReDecorated);
    [TestMethod]
    public void InheritReDecoratedStream() =>
        AssertStreamNames<InheritReDecorated>(StreamNameReDecorated + ",Abc");


    private const string ForContentTypeIDecorated = "IDec";
    private const string StreamNameIDecorated= "IRedecStream";
    [ModelSource(ContentTypes = ForContentTypeIDecorated, Streams = StreamNameIDecorated)]
    interface IDecorated: ICanWrapData;

    [TestMethod]
    public void IDecoratedType() =>
        AssertTypeName<IDecorated>(ForContentTypeIDecorated);

    [TestMethod]
    public void IDecoratedStream() =>
        AssertStreamNames<IDecorated>(StreamNameIDecorated);

}