using System.Collections;
using System.Collections.Generic;
using System.Web;
using ToSic.Eav;

namespace ToSic.SexyContent
{
    /// <summary>
    /// More or less just an overload of IList-of-DynamicEntity providing edit-context-commands
    /// </summary>
    public class DynamicEntityList : List<DynamicEntity>, IList<DynamicEntity>
    {
        private readonly string _jsonTemplate = "data-list-context='{{ `parent`: {0}, `field`: `{1}`, `type`: `{2}`}}'".Replace("`", "\"");

        private readonly IList<DynamicEntity> _list;
        private readonly IEntity _parent;
        private readonly string _field;
        public DynamicEntityList(IEntity parentEntity, string fieldName, IList<DynamicEntity> innerList, bool enableEdit)
        {
            _list = innerList;
            _parent = parentEntity;
            _field = fieldName;

            // if no edit, blank out the toolbar
            if (!enableEdit)
                _jsonTemplate = "";
        }

        public HtmlString EditContext() => new HtmlString(string.Format(
            _jsonTemplate, _parent.EntityId, _field, Settings.AttributeSetStaticNameContentBlockTypeName));


        public new IEnumerator<DynamicEntity> GetEnumerator()
        {
            return _list.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public new void Add(DynamicEntity item)
        {
            _list.Add(item);
        }

        public new void Clear()
        {
            _list.Clear();
        }

        public new bool Contains(DynamicEntity item)
        {
            return _list.Contains(item);
        }

        public new void CopyTo(DynamicEntity[] array, int arrayIndex)
        {
            _list.CopyTo(array, arrayIndex);
        }

        public new bool Remove(DynamicEntity item)
        {
            return _list.Remove(item);
        }

        public new int Count => _list.Count;

        public bool IsReadOnly => _list.IsReadOnly;

        public new int IndexOf(DynamicEntity item)
        {
            return _list.IndexOf(item);
        }

        public new void Insert(int index, DynamicEntity item)
        {
            _list.Insert(index, item);
        }

        public new void RemoveAt(int index)
        {
            _list.RemoveAt(index);
        }

        public new DynamicEntity this[int index]
        {
            get { return _list[index]; }
            set { _list[index] = value; }
        }
    }
}