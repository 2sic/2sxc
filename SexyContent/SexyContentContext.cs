using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Objects.DataClasses;
using System.Linq;
using System.Reflection;
using System.Web;
using DotNetNuke.Common.Utilities;
using DotNetNuke.Entities.Portals;
using DotNetNuke.UI.Modules;
using System.Collections;
using ToSic.SexyContent.Engines;
using ToSic.Eav;
using System.Dynamic;
using ToSic.SexyContent;

namespace ToSic.SexyContent
{
    public class SexyContentContext : SexyContentContextBase
    {
        #region Constructors

        public SexyContentContext(string ConnectionString, bool EnableCaching = true) : base(ConnectionString)
        {
            this.CacheEnabled = EnableCaching;
        }

        #endregion

        #region Templates

        /// <summary>
        /// Fills the CachedTemplates list and returns them
        /// </summary>
        public new List<Template> Templates
        {
            get
            {
                List<Template> TemplateList;
                if (CacheEnabled && CacheExists(CacheKeys.Templates))
                    TemplateList = GetCache<List<Template>>(CacheKeys.Templates);
                else
                {
                    // No cache desired, or not cached yet...get from DB
                    TemplateList = base.Templates.Where(t => t.SysDeleted == null).OrderBy(t => t.Name).ToList();
                    if (CacheEnabled)
                    {
                        TemplateList.ForEach(this.Detach);
                        SetCache(TemplateList, CacheKeys.Templates);
                    }
                }
                return TemplateList;
            }
        }

        /// <summary>
        /// Clears Template Cache
        /// </summary>
        private void ClearTemplateCache()
        {
            DataCache.RemoveCache(CacheKeys.Templates);
        }

        /// <summary>
        /// Returns the template with the specified TemplateID
        /// </summary>
        /// <param name="TemplateID"></param>
        /// <returns></returns>
        public Template GetTemplate(int TemplateID)
        {
            return (from t in Templates where t.TemplateID == TemplateID select t).FirstOrDefault();
        }

        /// <summary>
        /// Gets a template for modifying
        /// </summary>
        /// <param name="TemplateID"></param>
        /// <returns></returns>
        public Template GetTemplateForModify(int TemplateID, int UserID)
        {
            Template TemplateForModify = GetTemplate(TemplateID);
            TemplateForModify.SysModified = DateTime.Now;
            TemplateForModify.SysModifiedBy = UserID;
            return TemplateForModify;
        }

        /// <summary>
        /// Returns a new template
        /// </summary>
        /// <returns></returns>
        public Template GetNewTemplate()
        {
            Template NewTemplate = new Template();
            NewTemplate.SysModified = DateTime.Now;
            NewTemplate.SysCreated = DateTime.Now;
            NewTemplate.IsFile = true;
            return NewTemplate;
        }

        /// <summary>
        /// Adds a template to the templates table
        /// </summary>
        /// <param name="NewTemplate"></param>
        /// <returns></returns>
        public Template AddTemplate(Template NewTemplate)
        {
            base.Templates.AddObject(NewTemplate);
            SaveChanges();
            return NewTemplate;
        }

        public Template UpdateTemplate(Template Template)
        {
            SaveChanges();
            return GetTemplate(Template.TemplateID);
        }

        /// <summary>
        /// Returns all templates from the specified DotNetNuke portal
        /// </summary>
        /// <param name="PortalID"></param>
        /// <returns></returns>
        public IEnumerable<Template> GetTemplates(int PortalID)
        {
            return (from t in Templates where t.PortalID == PortalID select t);
        }

        /// <summary>
        /// Returns all visible templates with the specified PortalID
        /// </summary>
        /// <param name="PortalID"></param>
        /// <returns></returns>
        public IEnumerable<Template> GetVisibleTemplates(int PortalID)
        {
            return GetTemplates(PortalID).Where(t => !t.IsHidden);
        }

        /// <summary>
        /// Returns all visible templates that belongs to the specified portal and use the given AttributeSet.
        /// </summary>
        /// <param name="PortalID">The id of the portal to get the templates from</param>
        /// <param name="AttributeSetID">The id of the AttributeSet</param>
        /// <returns></returns>
        public IEnumerable<Template> GetVisibleTemplates(int PortalID, int AttributeSetID)
        {
            return GetVisibleTemplates(PortalID).Where(t => t.AttributeSetID == AttributeSetID);
        }

        /// <summary>
        /// Returns all visible templates that belongs to the specified portal and use the given AttributeSet, and can be used for lists.
        /// </summary>
        /// <param name="PortalID"></param>
        /// <param name="AttributeSetID"></param>
        /// <returns></returns>
        public IEnumerable<Template> GetVisibleListTemplates(int PortalID, int AttributeSetID)
        {
            return GetVisibleTemplates(PortalID, AttributeSetID).Where(t => t.UseForList);
        }

        /// <summary>
        /// Returns all visible templates that belongs to the specified portal and use the given AttributeSet, and can be used for lists.
        /// </summary>
        /// <param name="PortalID"></param>
        /// <param name="AttributeSetID"></param>
        /// <returns></returns>
        public IEnumerable<Template> GetVisibleListTemplates(int PortalID)
        {
            return GetVisibleTemplates(PortalID).Where(t => t.UseForList);
        }

        /// <summary>
        /// Deletes the template with the given id
        /// </summary>
        /// <param name="TemplateID">The id of the template to delete</param>
        /// <param name="UserId">The user id that deletes the template</param>
        public void DeleteTemplate(int TemplateID, int UserId)
        {
            Template Template = GetTemplate(TemplateID);
            Template.SysDeleted = DateTime.Now;
            Template.SysDeletedBy = UserId;
            SaveChanges();
        }

        #endregion

        #region Content Group Items

        public new List<ContentGroupItem> ContentGroupItems
        {
            get
            {
                List<ContentGroupItem> ContentGroupItemList;
                if (CacheEnabled && CacheExists(CacheKeys.ContentGroupItems))
                    ContentGroupItemList = GetCache<List<ContentGroupItem>>(CacheKeys.ContentGroupItems);
                else
                {
                    // No cache desired, or not cached yet...get from DB
                    ContentGroupItemList = base.ContentGroupItems.Where(t => t.SysDeleted == null).OrderBy(t => t.SortOrder).ToList();
                    if (CacheEnabled)
                    {
                        ContentGroupItemList.ForEach(this.Detach);
                        SetCache(ContentGroupItemList, CacheKeys.ContentGroupItems);
                    }
                }
                return ContentGroupItemList;
            }
        }
        private void ClearContentGroupItemCache()
        {
            DataCache.RemoveCache(CacheKeys.ContentGroupItems);
        }

        /// <summary>
        /// Get all ContentGroupItems
        /// </summary>
        /// <returns></returns>
        public IEnumerable<ContentGroupItem> GetContentGroupItems()
        {
            return ContentGroupItems;
        }

        /// <summary>
        /// Get a List of ContentGroupItems by ContentGroupID
        /// </summary>
        /// <param name="ContentGroupID"></param>
        /// <param name="UserId"></param>
        /// <returns></returns>
        public IEnumerable<ContentGroupItem> GetContentGroupItems(int ContentGroupID)
        {
            return GetContentGroupItems().Where(p => p.ContentGroupID == ContentGroupID);
        }

        public IEnumerable<ContentGroupItem>  GetContentGroupItems(int ContentGroupID, ContentGroupItemType Type)
        {
            return GetContentGroupItems(ContentGroupID).Where(p => p.ItemType == Type);
        }

        /// <summary>
        /// Returns the ContentGroupItem with specified ID
        /// </summary>
        /// <param name="ContentGroupItemID"></param>
        /// <returns></returns>
        public ContentGroupItem GetContentGroupItem(int ContentGroupItemID)
        {
            return GetContentGroupItems().FirstOrDefault(c => c.ContentGroupItemID == ContentGroupItemID);
        }

        public ContentGroupItem GetListContentGroupItem(int ContentGroupID, int UserID)
        {
            string ListContentItemType = ContentGroupItemType.ListContent.ToString("F");
            return GetContentGroupItems(ContentGroupID).FirstOrDefault(p => p.Type == ListContentItemType);
        }

        /// <summary>
        /// Deletes ContentGroupItems with the same SortOrder
        /// </summary>
        /// <param name="ContentGroupItemID"></param>
        /// <param name="UserID"></param>
        /// <returns></returns>
        public void DeleteContentGroupItems(int ContentGroupID, int SortOrder, int UserID)
        {
            // Get ContentGroup Items to Delete
            var ItemsToDelete = GetContentGroupItems(ContentGroupID).Where(c => c.SortOrder == SortOrder);

            foreach (var Item in ItemsToDelete)
            {
                Item.SysModified = DateTime.Now;
                Item.SysModifiedBy = UserID;
                Item.SysDeleted = DateTime.Now;
                Item.SysDeletedBy = UserID;
            }

            if(SortOrder != -1)
                GetContentGroupItems(ContentGroupID).Where(c => c.SortOrder > SortOrder).ToList().ForEach(c => c.SortOrder--);

            SaveChanges();
        }

        public ContentGroupItem DeleteContentGroupItem(int ContentGroupItemID, int UserID)
        {
            var Item = GetContentGroupItem(ContentGroupItemID);

            Item.SysModified = DateTime.Now;
            Item.SysModifiedBy = UserID;
            Item.SysDeleted = DateTime.Now;
            Item.SysDeletedBy = UserID;

            SaveChanges();
            return Item;
        }

        public ContentGroupItem AddContentGroupItem(ContentGroupItem Item)
        {
            base.ContentGroupItems.AddObject(Item);

            SaveChanges();
            return Item;
        }

        public ContentGroupItem UpdateContentGroupItem(ContentGroupItem Item)
        {
            SaveChanges();
            return Item;
        }

        /// <summary>
        /// Reorders ContentGroupItems in the group
        /// </summary>
        /// <param name="Item"></param>
        /// <param name="DestinationSortOrder"></param>
        /// <param name="UserId"></param>
        /// <param name="AutoSave"></param>
        public void ReorderContentGroupItem(ContentGroupItem Item, int DestinationSortOrder, bool AutoSave)
        {
            // Return if the Item is already at that position
            if (Item.SortOrder == DestinationSortOrder)
                return;

            List<ContentGroupItem> Items;
            List<ContentGroupItem> RelatedItems = GetContentGroupItems(Item.ContentGroupID).Where(p => p.SortOrder == Item.SortOrder).ToList();

            // Item moved up
            if (Item.SortOrder > DestinationSortOrder)
            {
                Items = GetContentGroupItems(Item.ContentGroupID).Where(p => p.SortOrder >= DestinationSortOrder && p.SortOrder < Item.SortOrder).ToList();

                // Set new SortOrder on Items that have to move
                Items.ForEach(p => p.SortOrder = p.SortOrder + 1);
            }
            else // Item moved down
            {
                Items = GetContentGroupItems(Item.ContentGroupID).Where(p => p.SortOrder > Item.SortOrder && p.SortOrder <= DestinationSortOrder).ToList();

                // Set new SortOrder on Items that have to move
                Items.ForEach(p => p.SortOrder = p.SortOrder - 1);
            }

            // Set SortOrder of items with the same SortOrder (for example, presentation)
            RelatedItems.ForEach(p => p.SortOrder = DestinationSortOrder);

            if(AutoSave)
                SaveChanges();
        }

        #endregion

        #region Caching

        private readonly bool CacheEnabled = true;

        public static class CacheKeys
        {
            public static string Templates = "SexyContent-Templates";
            public static string ContentGroupItems = "SexyContent-ContentGroupItems";
        }

        public bool CacheExists(string Key)
        {
            return DataCache.GetCache(Key) != null;
        }

        public void SetCache<T>(T Object, string Key)
        {
            DataCache.SetCache(Key, Object);
        }

        public T GetCache<T>(string Key)
        {
            return (T) DataCache.GetCache(Key);
        }

        #endregion
        
        /// <summary>
        /// Clears caches and Saves Changes
        /// </summary>
        public new void SaveChanges()
        {
            ClearContentGroupItemCache();
            ClearTemplateCache();
            base.SaveChanges();
        }
    }
}
