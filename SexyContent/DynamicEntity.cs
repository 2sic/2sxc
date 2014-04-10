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
        //public IEntity AsEntity()
        //{
        //    return Entity;
        //}


        public string Toolbar { get; set; }

        private string[] DimensionIds;

        /// <summary>
        /// Contructor with EntityModel and DimensionIds
        /// </summary>
        /// <param name="Dict"></param>
        public DynamicEntity(IEntity entityModel, string[] DimensionIds)
        {
            this.Entity = entityModel;
            this.DimensionIds = DimensionIds;
            this.Toolbar = "";
        }

        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            return TryGetMember(binder.Name, out result);
            //var propertyNotFound = false;
            //result = GetEntityValue(binder.Name, out propertyNotFound);

            //if(propertyNotFound)
            //    result = String.Format(Configuration.ErrorKeyMissing, binder.Name);

            //return true;
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
                result = attribute[DimensionIds];

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

        // Uses the compiler's type inference mechanisms for generics to find out the type
        // 'self' was declared with in the current scope.
        private Type GetDeclaredType<TSelf>(TSelf self)
        {
            return typeof(TSelf);
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
            get { return Entity.Title[DimensionIds]; }
        }


    }

}