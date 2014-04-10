using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text.RegularExpressions;
using DotNetNuke.Services.FileSystem;
using ToSic.Eav;

namespace ToSic.SexyContent
{
    public class DynamicEntity : DynamicObject
    {
        public ContentConfiguration Configuration = new ContentConfiguration();
        public IEntity Entity { get; set; }
        public string Toolbar { get; set; }
        private readonly string[] _dimensions;

        /// <summary>
        /// Contructor with EntityModel and DimensionIds
        /// </summary>
        /// <param name="entityModel"></param>
        /// <param name="dimensions"></param>
        public DynamicEntity(IEntity entityModel, string[] dimensions)
        {
            this.Entity = entityModel;
            this._dimensions = dimensions;
            this.Toolbar = "";
        }

        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            return TryGetMember(binder.Name, out result);
        }

        public bool TryGetMember(string memberName, out object result)
        {
            var propertyNotFound = false;
            result = GetEntityValue(memberName, out propertyNotFound);

            if (propertyNotFound)
                result = null; // String.Format(Configuration.ErrorKeyMissing, memberName);

            return true;
        }

        public object GetEntityValue(string attributeName, out bool propertyNotFound)
        {
            propertyNotFound = false;
            object result;
            
            if (Entity.Attributes.ContainsKey(attributeName))
            {
                var attribute = Entity.Attributes[attributeName];
                result = attribute[_dimensions];

                if (attribute.Type == "Hyperlink" && result is string)
                {
                    result = SexyContent.ResolveHyperlinkValues((string)result);
                }

                if (attribute.Type == "Entity")
                {
                    // Convert related entities to Dynamics
                    if (result != null && result is ToSic.Eav.EntityRelationshipModel)
                    {
                        string language = System.Threading.Thread.CurrentThread.CurrentCulture.Name;
                        result = ((ToSic.Eav.EntityRelationshipModel)result).Select(
                            p => new DynamicEntity(p, new string[] { language })
                            ).ToList();
                    }
                    else
                    {
                        result = new List<DynamicEntity>();
                    }
                }
            }
            else
            {
                switch (attributeName)
                {
                    case "EntityTitle":
                        result = EntityTitle;
                        break;
                    case "EntityId":
                        result = EntityId;
                        break;
                    case "Toolbar":
                        result = Toolbar;
                        break;
                    default:
                        result = null;
                        propertyNotFound = true;
                        break;
                }
            }

            return result;
        }

        /// <summary>
        /// Configuration class for this expando
        /// </summary>
        public class ContentConfiguration
        {
            public string ErrorKeyMissing {
                get { return null; }
                set
                {
                    throw new Exception("Obsolete: Do not use ErrorKeyMissing anymore. Check if the value is null instead.");
                }
            }
        }

        public int EntityId
        {
            get { return Entity.EntityId; }
        }

        public Guid EntityGuid
        {
            get { return Entity.EntityGuid; }
        }

        public object EntityTitle
        {
            get { return Entity.Title[_dimensions]; }
        }

    }
}