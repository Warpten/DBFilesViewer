/*
 * FastObjectListView<T> - A listview that behaves like an ObjectListView<T> but has the speed of a virtual list
 *
 * Author: Phillip Piper
 * Date: 27/09/2008 9:15 AM
 *
 * Change log:
 * 2014-10-15   JPP  - Fire Filter event when applying filters
 * v2.8
 * 2012-06-11   JPP  - Added more efficient version of FilteredObjects
 * v2.5.1
 * 2011-04-25   JPP  - Fixed problem with removing objects from filtered or sorted list
 * v2.4
 * 2010-04-05   JPP  - Added filtering
 * v2.3
 * 2009-08-27   JPP  - Added GroupingStrategy
 *                   - Added optimized Objects property
 * v2.2.1
 * 2009-01-07   JPP  - Made all public and protected methods virtual
 * 2008-09-27   JPP  - Separated from ObjectListView.cs
 *
 * Copyright (C) 2006-2014 Phillip Piper
 *
 * This program is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with this program.  If not, see <http://www.gnu.org/licenses/>.
 *
 * If you wish to use this code in a closed source application, please contact phillip.piper@gmail.com.
 */

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Forms;

namespace BrightIdeasSoftware
{
    /// <summary>
    /// A FastObjectListView{T} trades function for speed.
    /// </summary>
    /// <remarks>
    /// <para>On my mid-range laptop, this view builds a list of 10,000 objects in 0.1 seconds,
    /// as opposed to a normal ObjectListView{T} which takes 10-15 seconds. Lists of up to 50,000 items should be
    /// able to be handled with sub-second response times even on low end machines.</para>
    /// <para>
    /// A FastObjectListView{T} is implemented as a virtual list with many of the virtual modes limits (e.g. no sorting)
    /// fixed through coding. There are some functions that simply cannot be provided. Specifically, a FastObjectListView{T} cannot:
    /// <list type="bullet">
    /// <item><description>use Tile view</description></item>
    /// <item><description>show groups on XP</description></item>
    /// </list>
    /// </para>
    /// </remarks>
    public class FastObjectListView<T> : VirtualObjectListView<T>
    {
        /// <summary>
        /// Make a FastObjectListView
        /// </summary>
        public FastObjectListView() {
            VirtualListDataSource = new FastObjectListDataSource<T>(this);
        }

        /// <summary>
        /// Gets the collection of objects that survive any filtering that may be in place.
        /// </summary>
        [Browsable(false),
            DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public override IEnumerable<T> FilteredObjects => ((FastObjectListDataSource<T>)VirtualListDataSource).FilteredObjectList;

        /// <summary>
        /// Get/set the collection of objects that this list will show
        /// </summary>
        /// <remarks>
        /// <para>
        /// The contents of the control will be updated immediately after setting this property.
        /// </para>
        /// <para>This method preserves selection, if possible. Use SetObjects() if
        /// you do not want to preserve the selection. Preserving selection is the slowest part of this
        /// code and performance is O(n) where n is the number of selected rows.</para>
        /// <para>This method is not thread safe.</para>
        /// </remarks>
        [Browsable(false),
         DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public override IList<T> Objects {
            get {
                // This is much faster than the base method
                return ((FastObjectListDataSource<T>)VirtualListDataSource).ObjectList;
            }
            set { base.Objects = value; }
        }

        /// <summary>
        /// Move the given collection of objects to the given index.
        /// </summary>
        /// <remarks>This operation only makes sense on non-grouped ObjectListViews.</remarks>
        /// <param name="index"></param>
        /// <param name="modelObjects"></param>
        public override void MoveObjects(int index, ICollection<T> modelObjects) {
            if (InvokeRequired) {
                Invoke((MethodInvoker)delegate { MoveObjects(index, modelObjects); });
                return;
            }

            // If any object that is going to be moved is before the point where the insertion 
            // will occur, then we have to reduce the location of our insertion point
            var displacedObjectCount = modelObjects.Select(IndexOf).Count(i => i >= 0 && i <= index);
            index -= displacedObjectCount;

            BeginUpdate();
            try {
                RemoveObjects(modelObjects);
                InsertObjects(index, modelObjects);
            }
            finally {
                EndUpdate();
            }
        }

        /// <summary>
        /// Remove any sorting and revert to the given order of the model objects
        /// </summary>
        /// <remarks>To be really honest, Unsort() doesn't work on FastObjectListViews since
        /// the original ordering of model objects is lost when Sort() is called. So this method
        /// effectively just turns off sorting.</remarks>
        public override void Unsort() {
            ShowGroups = false;
            PrimarySortColumn = null;
            PrimarySortOrder = SortOrder.None;
            SetObjects(Objects);
        }
    }

    /// <summary>
    /// Provide a data source for a FastObjectListView
    /// </summary>
    /// <remarks>
    /// This class isn't intended to be used directly, but it is left as a public
    /// class just in case someone wants to subclass it.
    /// </remarks>
    public class FastObjectListDataSource<T> : AbstractVirtualListDataSource<T>
    {
        /// <summary>
        /// Create a FastObjectListDataSource
        /// </summary>
        /// <param name="listView"></param>
        public FastObjectListDataSource(FastObjectListView<T> listView)
            : base(listView) {
        }

        #region IVirtualListDataSource Members

        /// <summary>
        /// Get n'th object
        /// </summary>
        /// <param name="n"></param>
        /// <returns></returns>
        public override T GetNthObject(int n) {
            if (n >= 0 && n < FilteredObjectList.Count)
                return FilteredObjectList[n];
            
            return default(T);
        }

        /// <summary>
        /// How many items are in the data source
        /// </summary>
        /// <returns></returns>
        public override int GetObjectCount() {
            return FilteredObjectList.Count;
        }

        /// <summary>
        /// Get the index of the given model
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public override int GetObjectIndex(T model) {
            int index;

            if (model != null && objectsToIndexMap.TryGetValue(model, out index))
                return index;
            
            return -1;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="text"></param>
        /// <param name="first"></param>
        /// <param name="last"></param>
        /// <param name="column"></param>
        /// <returns></returns>
        public override int SearchText(string text, int first, int last, OLVColumn<T> column) {
            if (first <= last) {
                for (var i = first; i <= last; i++) {
                    var data = column.GetStringValue(listView.GetNthItemInDisplayOrder(i).RowObject);
                    if (data.StartsWith(text, StringComparison.CurrentCultureIgnoreCase))
                        return i;
                }
            } else {
                for (var i = first; i >= last; i--) {
                    var data = column.GetStringValue(listView.GetNthItemInDisplayOrder(i).RowObject);
                    if (data.StartsWith(text, StringComparison.CurrentCultureIgnoreCase))
                        return i;
                }
            }

            return -1;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="column"></param>
        /// <param name="sortOrder"></param>
        public override void Sort(OLVColumn<T> column, SortOrder sortOrder) {
            if (sortOrder != SortOrder.None) {
                var comparer = new ModelObjectComparer<T>(column, sortOrder, listView.SecondarySortColumn, listView.SecondarySortOrder);
                ObjectList.Sort(comparer);
                FilteredObjectList.Sort(comparer);
            }
            RebuildIndexMap();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="modelObjects"></param>
        public override void AddObjects(ICollection<T> modelObjects) {
            foreach (var modelObject in modelObjects) {
                if (modelObject != null)
                    ObjectList.Add(modelObject);
            }
            FilterObjects();
            RebuildIndexMap();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <param name="modelObjects"></param>
        public override void InsertObjects(int index, ICollection<T> modelObjects) {
            ObjectList.InsertRange(index, modelObjects);
            FilterObjects();
            RebuildIndexMap();
        }

        /// <summary>
        /// Remove the given collection of models from this source.
        /// </summary>
        /// <param name="modelObjects"></param>
        public override void RemoveObjects(ICollection<T> modelObjects) {

            // We have to unselect any object that is about to be deleted
            var indicesToRemove = modelObjects.Select(GetObjectIndex).Where(i => i >= 0).ToList();

            // Sort the indices from highest to lowest so that we
            // remove latter ones before earlier ones. In this way, the
            // indices of the rows doesn't change after the deletes.
            indicesToRemove.Sort();
            indicesToRemove.Reverse();

            foreach (var i in indicesToRemove) 
                listView.SelectedIndices.Remove(i);

            // Remove the objects from the unfiltered list
            foreach (var modelObject in modelObjects)
                ObjectList.Remove(modelObject);

            FilterObjects();
            RebuildIndexMap();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="collection"></param>
        public override void SetObjects(IEnumerable<T> collection)
        {
            var newObjects = collection.ToList();

            ObjectList = newObjects;
            FilterObjects();
            RebuildIndexMap();
        }

        /// <summary>
        /// Update/replace the nth object with the given object
        /// </summary>
        /// <param name="index"></param>
        /// <param name="modelObject"></param>
        public override void UpdateObject(int index, T modelObject) {
            if (index < 0 || index >= FilteredObjectList.Count)
                return;

            var i = ObjectList.IndexOf(FilteredObjectList[index]);
            if (i < 0)
                return;

            if (ReferenceEquals(ObjectList[i], modelObject))
                return;

            ObjectList[i] = modelObject;
            FilteredObjectList[index] = modelObject;
            objectsToIndexMap[modelObject] = index;
        }

        private IModelFilter<T> modelFilter;
        private IListFilter<T> listFilter;

        #endregion

        #region IFilterableDataSource Members

        /// <summary>
        /// Apply the given filters to this data source. One or both may be null.
        /// </summary>
        /// <param name="iModelFilter"></param>
        /// <param name="iListFilter"></param>
        public override void ApplyFilters(IModelFilter<T> iModelFilter, IListFilter<T> iListFilter) {
            modelFilter = iModelFilter;
            listFilter = iListFilter;
            SetObjects(ObjectList);
        }

        #endregion

        #region Implementation

        /// <summary>
        /// Gets the full list of objects being used for this fast list. 
        /// This list is unfiltered.
        /// </summary>
        public List<T> ObjectList { get; private set; } = new List<T>();

        /// <summary>
        /// Gets the list of objects from ObjectList which survive any installed filters.
        /// </summary>
        public List<T> FilteredObjectList { get; private set; } = new List<T>();

        /// <summary>
        /// Rebuild the map that remembers which model object is displayed at which line
        /// </summary>
        protected void RebuildIndexMap() {
            objectsToIndexMap.Clear();
            for (var i = 0; i < FilteredObjectList.Count; i++)
                objectsToIndexMap[FilteredObjectList[i]] = i;
        }
        readonly Dictionary<T, int> objectsToIndexMap = new Dictionary<T, int>();

        /// <summary>
        /// Build our filtered list from our full list.
        /// </summary>
        protected void FilterObjects() {

            // If this list isn't filtered, we don't need to do anything else
            if (!listView.UseFiltering) {
                FilteredObjectList = new List<T>(ObjectList);
                return;
            }

            // Tell the world to filter the objects. If they do so, don't do anything else
            // ReSharper disable PossibleMultipleEnumeration
            var args = new FilterEventArgs<T>(ObjectList);
            listView.OnFilter(args);
            if (args.FilteredObjects != null)
            {
                FilteredObjectList = args.FilteredObjects.ToList();
                return;
            }

            IEnumerable<T> objects = (listFilter == null) ? ObjectList : listFilter.Filter(ObjectList);

            // Apply the object filter if there is one
            if (modelFilter == null) {
                FilteredObjectList = objects.ToList();
            } else
            {
                FilteredObjectList = new List<T>();
                foreach (var model in objects.Where(model => modelFilter.Filter(model)))
                {
                    FilteredObjectList.Add(model);
                }
            }
        }

        #endregion
    }

}
