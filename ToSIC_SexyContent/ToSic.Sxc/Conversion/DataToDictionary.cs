using System.Collections.Generic;
using System.Linq;
using ToSic.Eav.Data;
using ToSic.Sxc.Blocks;
using ToSic.Sxc.Conversion;
using ToSic.Sxc.Data;
using ToSic.Sxc.Interfaces;
using IDynamicEntity = ToSic.Sxc.Data.IDynamicEntity;

namespace ToSic.Sxc.Serializers
{
    public class Serializer: Eav.Serialization.EntitiesToDictionary, IDynamicEntityTo<Dictionary<string, object>>
    {
		public ICmsBlock Cms { get; internal set; }

        /// <summary>
        /// Standard constructor, important for opening this class in dependency-injection
        /// </summary>
	    public Serializer() { }

	    /// <summary>
	    /// Common constructor, directly preparing it with 2sxc
	    /// </summary>
	    /// <param name="cmsInstance"></param>
	    /// <param name="languages"></param>
	    public Serializer(ICmsBlock cmsInstance, string[] languages = null)
        {
            Cms = cmsInstance;
            Languages = languages;
        }


        #region Prepare statements expecting dynamic objects - extending the EAV Prepare variations

	    /// <inheritdoc />
	    public IEnumerable<Dictionary<string, object>> Prepare(IEnumerable<dynamic> dynamicList)
	        => dynamicList.Select(c => GetDictionaryFromEntity(c.Entity) as Dictionary<string, object>).ToList();

        /// <inheritdoc />
	    public Dictionary<string, object> Prepare(IDynamicEntity dynamicEntity)
	        => GetDictionaryFromEntity(dynamicEntity.Entity);
        
        #endregion



        public override Dictionary<string, object> GetDictionaryFromEntity(IEntity entity)
		{
            // Do groundwork
            var dictionary = base.GetDictionaryFromEntity(entity);

            AddPresentation(entity, dictionary);
            AddEditInfo(entity, dictionary);

            return dictionary;
		}

        #region to enhance serializable IEntities with 2sxc specific infos

        private void AddPresentation(IEntity entity, Dictionary<string, object> dictionary)
        {
            // Add full presentation object if it has one...because there we need more than just id/title
            if (!(entity is EntityInBlock entityInGroup) || dictionary.ContainsKey(ViewParts.Presentation)) return;

            if (entityInGroup.Presentation != null)
                dictionary.Add(ViewParts.Presentation, GetDictionaryFromEntity(entityInGroup.Presentation));
        }

	    private void AddEditInfo(IEntity entity, Dictionary<string, object> dictionary)
	    {
            // Add additional information in case we're in edit mode
	        var userMayEdit = Cms?.UserMayEdit ?? false;

	        if (!userMayEdit) return;

	        dictionary.Add(Constants.JsonModifiedNodeName, entity.Modified);
	        var title = entity.GetBestTitle(Languages);
	        if (string.IsNullOrEmpty(title))
	            title = "(no title)";
	        dictionary.Add(Constants.JsonEntityEditNodeName, entity is IHasEditingData entWithEditing
	            ? (object) new {
	                sortOrder = entWithEditing.SortOrder,
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