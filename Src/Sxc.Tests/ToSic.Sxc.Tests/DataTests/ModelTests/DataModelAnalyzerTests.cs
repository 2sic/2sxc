using Microsoft.VisualStudio.TestTools.UnitTesting;
using ToSic.Sxc.Data.Model;
using ToSic.Sxc.Models;

namespace ToSic.Sxc.Tests.DataTests.ModelTests;

[TestClass]
public class DataModelAnalyzerTests : TestBaseSxcDb
{
    private void AssertTypeName<T>(string name)
        where T : class, IDataWrapper =>
        Assert.AreEqual(name, DataModelAnalyzerTestAccessors.GetContentTypeNameTac<T>());
    private void AssertStreamName<T>(string name)
        where T : class, IDataWrapper =>
        Assert.AreEqual(name, DataModelAnalyzerTestAccessors.GetStreamNameTac<T>());

    class NotDecorated: IDataWrapper;

    [TestMethod]
    public void NotDecoratedDataModelType() =>
        AssertTypeName<NotDecorated>(nameof(NotDecorated));

    [TestMethod]
    public void NotDecoratedDataModelStream() =>
        AssertStreamName<NotDecorated>(nameof(NotDecorated));


    interface INotDecorated: IDataWrapper;

    [TestMethod]
    public void INotDecoratedType() =>
        AssertTypeName<INotDecorated>(nameof(INotDecorated).Substring(1));

    [TestMethod]
    public void INotDecoratedStream() =>
        AssertStreamName<INotDecorated>(nameof(INotDecorated));


    private const string ForContentType1 = "Abc";
    private const string StreamName1 = "AbcStream";
    [DataModel(ForContentTypes = ForContentType1, StreamNames = StreamName1)]
    class Decorated: IDataWrapper;

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
    [DataModel(ForContentTypes = ForContentTypeReDecorated, StreamNames = StreamNameReDecorated + ",Abc")]
    class InheritReDecorated : InheritDecorated;

    [TestMethod]
    public void InheritReDecoratedType() =>
        AssertTypeName<InheritReDecorated>(ForContentTypeReDecorated);
    [TestMethod]
    public void InheritReDecoratedStream() =>
        AssertStreamName<InheritReDecorated>(StreamNameReDecorated);


    private const string ForContentTypeIDecorated = "IDec";
    private const string StreamNameIDecorated= "IRedecStream";
    [DataModel(ForContentTypes = ForContentTypeIDecorated, StreamNames = StreamNameIDecorated)]
    interface IDecorated: IDataWrapper;

    [TestMethod]
    public void IDecoratedType() =>
        AssertTypeName<IDecorated>(ForContentTypeIDecorated);

    [TestMethod]
    public void IDecoratedStream() =>
        AssertStreamName<IDecorated>(StreamNameIDecorated);

}