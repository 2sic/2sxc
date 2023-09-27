//using Microsoft.AspNetCore.Mvc.ApplicationParts;
//using Microsoft.AspNetCore.Mvc.Razor;
//using Microsoft.CodeAnalysis;
//using Microsoft.Extensions.Options;
//using System;
//using System.Collections.Generic;
//using System.IO;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using TypeInfo = System.Reflection.TypeInfo;

//namespace ToSic.Sxc.Oqt.Server.Code
//{

//    public class MetadataReferenceFeature
//    {
//        public ICollection<MetadataReference> References { get; } = new List<MetadataReference>();
//    }

//    //public class MetadataReferenceApplicationPart : ApplicationPart, IApplicationPartTypeProvider
//    //{
//    //    public MetadataReferenceApplicationPart() { }

//    //    public override string Name => "MetadataReferencePart";

//    //    public IEnumerable<TypeInfo> Types => new List<TypeInfo>();
//    //}

//    public class MetadataReferenceFeatureProvider : IApplicationFeatureProvider<MetadataReferenceFeature>
//    {
//        private readonly MetadataReference _metadataReference;

//        public MetadataReferenceFeatureProvider(MetadataReference metadataReference)
//        {
//            _metadataReference = metadataReference;
//        }
        
//        public void PopulateFeature(IEnumerable<ApplicationPart> parts, MetadataReferenceFeature feature)
//        {
//            // Logic to create your MetadataReference from in-memory assembly
//            feature.References.Add(_metadataReference);
//        }
//    }





//}
