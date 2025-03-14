using ToSic.Sxc.Data;
using ToSic.Sxc.Data.Models;

namespace ToSic.Sxc.Tests.DataTests.ModelTests;

[Startup(typeof(StartupSxcCoreOnly))]
public class DataModelAnalyzerTests //: TestBaseSxcDb
{
    private void AssertTypeName<T>(string name)
        where T : class, ICanWrapData =>
        Equal(name, DataModelAnalyzerTestAccessors.GetContentTypeNamesTac<T>().Flat);

    private void AssertStreamNames<T>(string namesCsv)
        where T : class, ICanWrapData =>
        Equal(namesCsv, DataModelAnalyzerTestAccessors.GetStreamNameListTac<T>().Flat);

    class NotDecorated: ICanWrapData;

    [Fact]
    public void NotDecoratedDataModelType() =>
        AssertTypeName<NotDecorated>(nameof(NotDecorated));

    [Fact]
    public void NotDecoratedDataModelStream() =>
        AssertStreamNames<NotDecorated>(nameof(NotDecorated));

    [Fact]
    public void NotDecoratedDataModelStreamList() =>
        AssertStreamNames<NotDecorated>(nameof(NotDecorated));

    class NotDecoratedModel : ICanWrapData;

    [Fact]
    public void NotDecoratedModelStreamList() =>
        AssertStreamNames<NotDecoratedModel>(nameof(NotDecoratedModel) + "," + nameof(NotDecorated));

    // Objects starting with an "I" won't have the "I" removed in the name checks
    class INotDecoratedModel : ICanWrapData;

    [Fact]
    public void INotDecoratedModelStreamList() =>
        AssertStreamNames<INotDecoratedModel>(nameof(INotDecoratedModel) + ",INotDecorated");

    interface INotDecorated: ICanWrapData;

    [Fact]
    public void INotDecoratedType() =>
        AssertTypeName<INotDecorated>(nameof(INotDecorated) + ',' + nameof(INotDecorated).Substring(1));

    [Fact]
    public void INotDecoratedStream() =>
        AssertStreamNames<INotDecorated>(nameof(INotDecorated) + ",NotDecorated");


    private const string ForContentType1 = "Abc";
    private const string StreamName1 = "AbcStream";
    [ModelSource(ContentTypes = ForContentType1, Streams = StreamName1)]
    class Decorated: ICanWrapData;

    [Fact]
    public void DecoratedType() =>
        AssertTypeName<Decorated>(ForContentType1);

    [Fact]
    public void DecoratedStream() =>
        AssertStreamNames<Decorated>(StreamName1);


    class InheritDecorated : Decorated;

    [Fact]
    public void InheritDecoratedType() =>
        AssertTypeName<InheritDecorated>(nameof(InheritDecorated));

    [Fact]
    public void InheritDecoratedStream() =>
        AssertStreamNames<InheritDecorated>(nameof(InheritDecorated));


    private const string ForContentTypeReDecorated = "ReDec";
    private const string StreamNameReDecorated = "ReDecStream";
    [ModelSource(ContentTypes = ForContentTypeReDecorated, Streams = StreamNameReDecorated + ",Abc")]
    class InheritReDecorated : InheritDecorated;

    [Fact]
    public void InheritReDecoratedType() =>
        AssertTypeName<InheritReDecorated>(ForContentTypeReDecorated);
    [Fact]
    public void InheritReDecoratedStream() =>
        AssertStreamNames<InheritReDecorated>(StreamNameReDecorated + ",Abc");


    private const string ForContentTypeIDecorated = "IDec";
    private const string StreamNameIDecorated= "IRedecStream";
    [ModelSource(ContentTypes = ForContentTypeIDecorated, Streams = StreamNameIDecorated)]
    interface IDecorated: ICanWrapData;

    [Fact]
    public void IDecoratedType() =>
        AssertTypeName<IDecorated>(ForContentTypeIDecorated);

    [Fact]
    public void IDecoratedStream() =>
        AssertStreamNames<IDecorated>(StreamNameIDecorated);

}