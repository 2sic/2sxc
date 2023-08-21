//using System;
//using ToSic.Sxc.Code;

//namespace ToSic.Sxc.Dnn.Code
//{
//    /// <summary>
//    /// Special helper which will create the code-root based on the parent class requesting it.
//    /// If the parent is generic supporting IDynamicModel[Model, Kit] it will create the generic root
//    /// </summary>
//    internal class DnnCodeRootFactory: CodeRootFactory
//    {
//        public DnnCodeRootFactory(IServiceProvider serviceProvider): base(serviceProvider)
//        {
//        }

//        protected override Type UntypedRoot() => typeof(DnnDynamicCodeRoot);

//        protected override Type GetTypedRoot(Type kitType)
//        {
//            var genType = typeof(DnnDynamicCodeRoot<,>);
//            var newFirstParam = typeof(object);
//            var finalTypesArgs = new[] { newFirstParam, kitType };
//            return genType.MakeGenericType(finalTypesArgs);
//        }

//    }
//}
