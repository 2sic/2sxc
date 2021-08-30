using System;
using System.Collections.Generic;
using System.Linq;
using ToSic.Eav.Data;
using ToSic.Eav.DataSources;
using ToSic.Eav.Documentation;
using ToSic.Sxc.Blocks;
using ToSic.Sxc.Data;
using ToSic.Sxc.Interfaces;
using IDynamicEntity = ToSic.Sxc.Data.IDynamicEntity;

namespace ToSic.Sxc.Conversion
{
    /// <summary>
    /// Convert various types of entities (standalone, dynamic, in streams, etc.) to Dictionaries <br/>
    /// Mainly used for serialization scenarios, like in WebApis.
    /// 
    /// </summary>
    [PublicApi_Stable_ForUseInYourCode]
    public class DataToDictionary: Eav.Conversion.EntitiesToDictionary, IDataToDictionary // IDynamicEntityTo<IDictionary<string, object>>
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
	    public DataToDictionary(Dependencies dependencies): base(dependencies) { }

#if NETFRAMEWORK
        /// <summary>
        /// Standard constructor, important for opening this class in dependency-injection
        /// </summary>
        [PrivateApi]
        [Obsolete("only keep in case external code was using this in apps ca. 2sxc 11. v12+ should use CreateService")]
	    public DataToDictionary() { }

        /// <summary>
        /// Common constructor, directly preparing it with 2sxc
        /// </summary>
        /// <param name="withEdit">Include editing information in serialized result</param>
        ///// <param name="languages"></param>
        [Obsolete]
        public DataToDictionary(bool withEdit)
        {
            WithEdit = withEdit;
        }
#endif


        #region Convert statements expecting dynamic objects - extending the EAV Prepare variations

        /// <inheritdoc />
        public IEnumerable<IDictionary<string, object>> Convert(IEnumerable<dynamic> dynamicList)
        {
            if (dynamicList is IDataStream stream) return base.Convert(stream);

            return dynamicList
                .Select(c =>
                {
                    IEntity entity = null;
                    if (c is IEntity) entity = c;
                    else if (c is IDynamicEntity dynEnt) entity = dynEnt.Entity;
                    if (entity == null)
                        throw new Exception("tried to convert an item, but it was not a known Entity-type");
                    return GetDictionaryFromEntity(entity);
                })
                .ToList();
        }

        /// <inheritdoc />
	    public IDictionary<string, object> Convert(IDynamicEntity dynamicEntity)
	        => GetDictionaryFromEntity(dynamicEntity.Entity);
        
        #endregion


        [PrivateApi]
        protected override IDictionary<string, object> GetDictionaryFromEntity(IEntity entity)
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
            // Add full presentation object if it has one...because there we need more than just id/title
            if (!(entity is EntityInBlock entityInGroup) || dictionary.ContainsKey(ViewParts.Presentation)) return;

            if (entityInGroup.Presentation != null)
                dictionary.Add(ViewParts.Presentation, GetDictionaryFromEntity(entityInGroup.Presentation));
        }

	    private void AddEditInfo(IEntity entity, IDictionary<string, object> dictionary)
	    {
            // Add additional information in case we're in edit mode
            var userMayEdit = WithEdit;

	        if (!userMayEdit) return;

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