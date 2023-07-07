using System.Collections.Generic;
using ToSic.Eav.Data;
using ToSic.Eav.DataFormats.EavLight;
using ToSic.Lib.Documentation;
using ToSic.Sxc.Blocks;
using ToSic.Sxc.Data.Decorators;

namespace ToSic.Sxc.Data
{
    /// <summary>
    /// Convert various types of entities (standalone, dynamic, in streams, etc.) to Dictionaries <br/>
    /// Mainly used for serialization scenarios, like in WebApis.
    /// </summary>
    [PrivateApi("Hide implementation; this was never public; the DataToDictionary was with empty constructor, but that's already polyfilled")]
    public class ConvertToEavLightWithCmsInfo : ConvertToEavLight
    {
        /// <summary>
        /// Determines if we should use edit-information
        /// </summary>
        [PrivateApi("Note: wasn't private till 2sxc 12.04, very low risk of it being published. Set was always internal")]
        public bool WithEdit { get; set; }

        /// <summary>
        /// Standard constructor, important for opening this class in dependency-injection
        /// </summary>
        [PrivateApi]
	    public ConvertToEavLightWithCmsInfo(MyServices services): base(services) { }

        [PrivateApi]
        protected override EavLightEntity GetDictionaryFromEntity(IEntity entity)
		{
            // Do groundwork
            var dictionary = base.GetDictionaryFromEntity(entity);

            AddPresentation(entity, dictionary);
            AddEditInfo(entity, dictionary);

            return dictionary;
		}

        #region to enhance serializable IEntities with 2sxc specific infos

        private void AddPresentation(IEntity entity, IDictionary<string, object> dictionary)
        {
            var decorator = entity.GetDecorator<EntityInBlockDecorator>();

            // Add full presentation object if it has one...because there we need more than just id/title
            if (decorator?.Presentation == null || dictionary.ContainsKey(ViewParts.Presentation)) return;

            // if (entityInGroup.Presentation != null)
                dictionary.Add(ViewParts.Presentation, GetDictionaryFromEntity(decorator.Presentation));
        }

        /// <summary>
        /// Add additional information in case we're in edit mode
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="dictionary"></param>
	    private void AddEditInfo(IEntity entity, IDictionary<string, object> dictionary)
	    {
	        if (!WithEdit) return;

	        var title = entity.GetBestTitle(Languages);
	        if (string.IsNullOrEmpty(title))
	            title = "(no title)";

            var editDecorator = entity.GetDecorator<EntityInBlockDecorator>();

            dictionary.Add(Constants.JsonEntityEditNodeName, editDecorator != null // entity is IHasEditingData entWithEditing
	            ? (object) new {
	                sortOrder = editDecorator.SortOrder,
	                isPublished = entity.IsPublished,
	            }
	            : new {
	                entityId = entity.EntityId,
	                title,
	                isPublished = entity.IsPublished,
	            });
	    }

        #endregion
    }



}