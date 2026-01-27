using ToSic.Eav.Model;
using ToSic.Sxc.Data;
using ToSic.Sxc.Data.Models;

namespace ToSic.Sxc.DataTests.ModelTests;

[Startup(typeof(StartupSxcCoreOnly))]
public class DataModelAnalyzerTypeTests //: TestBaseSxcDb
{
    private void AssertType<TInspect, TExpected>()
        where TInspect : class, ICanWrapData =>
        Equal(typeof(TExpected), DataModelAnalyzerTestAccessors.GetTargetTypeTac<TInspect>());

    #region NotDecorated - should return itself as the type

    class NotDecorated: ICanWrapData;

    [Fact]
    public void NotDecoratedDataModelType() =>
        AssertType<NotDecorated, NotDecorated>();

    #endregion

    #region Interface not Decorated - should return itself as the type

    interface INotDecorated : ICanWrapData;

    [Fact]
    //[ExpectedException(typeof(TypeInitializationException))]
    public void INotDecoratedType() =>
        Throws<TypeInitializationException>(AssertType<INotDecorated, INotDecorated>);

    #endregion

    #region Decorated - should return the decorated type

    [ModelCreation(Use = typeof(DecoratedEntity))]
    class Decorated: ModelFromEntity;

    class DecoratedEntity : Decorated;

    [Fact]
    public void DecoratedType() =>
        AssertType<Decorated, DecoratedEntity>();

    #endregion


    #region Inherit Decorated but not decorated - should return itself as the type

    class InheritDecorated : Decorated;

    [Fact]
    public void InheritDecoratedType() =>
        AssertType<InheritDecorated, InheritDecorated>();

    #endregion

    #region Inherit and redecorate, should return the newly decorated type

    [ModelCreation(Use = typeof(InheritReDecoratedEntity))]
    class InheritReDecorated : InheritDecorated;

    class InheritReDecoratedEntity : InheritReDecorated;

    [Fact]
    public void InheritReDecoratedType() =>
        AssertType<InheritReDecorated, InheritReDecoratedEntity>();

    #endregion

    #region Interface decorated - should return the decorated type

    [ModelCreation(Use = typeof(EntityOfIDecorated))]
    interface IDecorated : ICanWrapData;

    class EntityOfIDecorated : InheritReDecorated, IDecorated;

    [Fact]
    public void IDecoratedType() =>
        AssertType<IDecorated, EntityOfIDecorated>();

    #endregion
}