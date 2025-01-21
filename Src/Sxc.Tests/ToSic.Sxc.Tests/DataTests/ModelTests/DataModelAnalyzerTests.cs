using Microsoft.VisualStudio.TestTools.UnitTesting;
using ToSic.Sxc.Data;
using ToSic.Sxc.Data.Models;

namespace ToSic.Sxc.Tests.DataTests.ModelTests;

[TestClass]
public class DataModelAnalyzerTests : TestBaseSxcDb
{
    private void AssertTypeName<T>(string name)
        where T : class, ICanWrapData =>
        Assert.AreEqual(name, DataModelAnalyzerTestAccessors.GetContentTypeNameTac<T>());
    private void AssertStreamName<T>(string name)
        where T : class, ICanWrapData =>
        Assert.AreEqual(name, DataModelAnalyzerTestAccessors.GetStreamNameTac<T>());

    class NotDecorated: ICanWrapData;

    [TestMethod]
    public void NotDecoratedDataModelType() =>
        AssertTypeName<NotDecorated>(nameof(NotDecorated));

    [TestMethod]
    public void NotDecoratedDataModelStream() =>
        AssertStreamName<NotDecorated>(nameof(NotDecorated));


    interface INotDecorated: ICanWrapData;

    [TestMethod]
    public void INotDecoratedType() =>
        AssertTypeName<INotDecorated>(nameof(INotDecorated).Substring(1));

    [TestMethod]
    public void INotDecoratedStream() =>
        AssertStreamName<INotDecorated>(nameof(INotDecorated));


    private const string ForContentType1 = "Abc";
    private const string StreamName1 = "AbcStream";
    [ModelSource(ContentTypes = ForContentType1, Streams = StreamName1)]
    class Decorated: ICanWrapData;

    [TestMethod]
    public void DecoratedType() =>
        AssertTypeName<Decorated>(ForContentType1);

    [TestMethod]
    public void DecoratedStream() =>
        AssertStreamName<Decorated>(StreamName1);


    class InheritDecorated : Decorated;

    [TestMethod]
    public void InheritDecoratedType() =>
        AssertTypeName<InheritDecorated>(nameof(InheritDecorated));

    [TestMethod]
    public void InheritDecoratedStream() =>
        AssertStreamName<InheritDecorated>(nameof(InheritDecorated));


    private const string ForContentTypeReDecorated = "ReDec";
    private const string StreamNameReDecorated = "ReDecStream";
    [ModelSource(ContentTypes = ForContentTypeReDecorated, Streams = StreamNameReDecorated + ",Abc")]
    class InheritReDecorated : InheritDecorated;

    [TestMethod]
    public void InheritReDecoratedType() =>
        AssertTypeName<InheritReDecorated>(ForContentTypeReDecorated);
    [TestMethod]
    public void InheritReDecoratedStream() =>
        AssertStreamName<InheritReDecorated>(StreamNameReDecorated);


    private const string ForContentTypeIDecorated = "IDec";
    private const string StreamNameIDecorated= "IRedecStream";
    [ModelSource(ContentTypes = ForContentTypeIDecorated, Streams = StreamNameIDecorated)]
    interface IDecorated: ICanWrapData;

    [TestMethod]
    public void IDecoratedType() =>
        AssertTypeName<IDecorated>(ForContentTypeIDecorated);

    [TestMethod]
    public void IDecoratedStream() =>
        AssertStreamName<IDecorated>(StreamNameIDecorated);

}