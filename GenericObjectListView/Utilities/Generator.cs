/*
 * Generator - Utility methods that generate columns or methods
 *
 * Author: Phillip Piper
 * Date: 15/08/2009 22:37
 *
 * Change log:
 * 2015-06-17  JPP  - Columns without [OLVColumn<T>] now auto size
 * 2012-08-16  JPP  - Generator now considers [OLVChildren] and [OLVIgnore] attributes.
 * 2012-06-14  JPP  - Allow columns to be generated even if they are not marked with [OLVColumn<T>]
 *                  - Converted class from static to instance to allow it to be subclassed.
 *                    Also, added IGenerator to allow it to be completely reimplemented.
 * v2.5.1
 * 2010-11-01  JPP  - DisplayIndex is now set correctly for columns that lack that attribute
 * v2.4.1
 * 2010-08-25  JPP  - Generator now also resets sort columns
 * v2.4
 * 2010-04-14  JPP  - Allow Name property to be set
 *                  - Don't double set the Text property
 * v2.3
 * 2009-08-15  JPP  - Initial version
 *
 * To do:
 * 
 * Copyright (C) 2009-2014 Phillip Piper
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
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace BrightIdeasSoftware
{
    /// <summary>
    /// An object that implements the IGenerator interface provides the ability 
    /// to dynamically create columns
    /// for an ObjectListView{T} based on the characteristics of a given collection
    /// of model objects.
    /// </summary>
    public interface IGenerator<T> {
        /// <summary>
        /// Generate columns into the given ObjectListView{T} that come from the given 
        /// model object type. 
        /// </summary>
        /// <param name="olv">The ObjectListView{T} to modify</param>
        /// <param name="type">The model type whose attributes will be considered.</param>
        /// <param name="allProperties">Will columns be generated for properties that are not marked with [OLVColumn{T}].</param>
        void GenerateAndReplaceColumns(ObjectListView<T> olv, bool allProperties);

        /// <summary>
        /// Generate a list of OLVColumn{T}s based on the attributes of the given type
        /// If allProperties to true, all public properties will have a matching column generated.
        /// If allProperties is false, only properties that have a OLVColumn{T} attribute will have a column generated.
        /// </summary>
        /// <param name="type"></param>
        /// <param name="allProperties">Will columns be generated for properties that are not marked with [OLVColumn{T}].</param>
        /// <returns>A collection of OLVColumn<T>s matching the attributes of Type that have OLVColumn{T}Attributes.</returns>
        IList<OLVColumn<T>> GenerateColumns(bool allProperties);
    }

    /// <summary>
    /// The Generator class provides methods to dynamically create columns
    /// for an ObjectListView{T} based on the characteristics of a given collection
    /// of model objects.
    /// </summary>
    /// <remarks>
    /// <para>For a given type, a Generator can create columns to match the public properties
    /// of that type. The generator can consider all public properties or only those public properties marked with
    /// [OLVColumn{T}] attribute.</para>
    /// </remarks>
    public class Generator<T> : IGenerator<T> {
        #region Static convenience methods

        /// <summary>
        /// Gets or sets the actual generator used by the static convenience methods.
        /// </summary>
        /// <remarks>If you subclass the standard generator or implement IGenerator yourself, 
        /// you should install an instance of your subclass/implementation here.</remarks>
        public static IGenerator<T> Instance {
            get { return instance ?? (instance = new Generator<T>()); }
            set { instance = value; }
        }
        private static IGenerator<T> instance;

        /// <summary>
        /// Replace all columns of the given ObjectListView{T} with columns generated
        /// from the first member of the given enumerable. If the enumerable is 
        /// empty or null, the ObjectListView{T} will be cleared.
        /// </summary>
        /// <param name="olv">The ObjectListView{T} to modify</param>
        /// <param name="enumerable">The collection whose first element will be used to generate columns.</param>
        /// <param name="allProperties">Will columns be generated for properties that are not marked with [OLVColumn{T}].</param>
        public static void GenerateColumns(ObjectListView<T> olv, IEnumerable<T> enumerable, bool allProperties = false) {
            // Generate columns based on the type of the first model in the collection and then quit
            if (enumerable != null) {
                if (enumerable.Any())
                {
                    Instance.GenerateAndReplaceColumns(olv, allProperties);
                    return;
                }
            }

            // If we reach here, the collection was empty, so we clear the list
            Instance.GenerateAndReplaceColumns(olv, allProperties);
        }

        /// <summary>
        /// Generate columns into the given ObjectListView{T} that come from the public properties of the given 
        /// model object type. 
        /// </summary>
        /// <param name="olv">The ObjectListView{T} to modify</param>
        /// <param name="type">The model type whose attributes will be considered.</param>
        public static void GenerateColumns(ObjectListView<T> olv) {
            Instance.GenerateAndReplaceColumns(olv, false);
        }

        /// <summary>
        /// Generate columns into the given ObjectListView that come from the public properties of the given 
        /// model object type. 
        /// </summary>
        /// <param name="olv">The ObjectListView to modify</param>
        /// <param name="type">The model type whose attributes will be considered.</param>
        /// <param name="allProperties">Will columns be generated for properties that are not marked with [OLVColumn<].</param>
        public static void GenerateColumns(ObjectListView<T> olv, bool allProperties) {
            Instance.GenerateAndReplaceColumns(olv, allProperties);
        }

        /// <summary>
        /// Generate a list of OLVColumns based on the public properties of the given type
        /// that have a OLVColumn attribute.
        /// </summary>
        /// <param name="type"></param>
        /// <returns>A collection of OLVColumn{T}s matching the attributes of Type that have OLVColumnAttributes.</returns>
        public static IList<OLVColumn<T>> GenerateColumns() {
            return Instance.GenerateColumns(false);
        }

        #endregion

        #region Public interface

        /// <summary>
        /// Generate columns into the given ObjectListView{T} that come from the given 
        /// model object type. 
        /// </summary>
        /// <param name="olv">The ObjectListView{T} to modify</param>
        /// <param name="type">The model type whose attributes will be considered.</param>
        /// <param name="allProperties">Will columns be generated for properties that are not marked with [OLVColumn{T}].</param>
        public virtual void GenerateAndReplaceColumns(ObjectListView<T> olv, bool allProperties) {
            var columns = GenerateColumns(allProperties);
            var tlv = olv as TreeListView<T>;
            if (tlv != null)
                this.TryGenerateChildrenDelegates(tlv);
            ReplaceColumns(olv, columns);
        }

        /// <summary>
        /// Generate a list of OLVColumns based on the attributes of the given type
        /// If allProperties to true, all public properties will have a matching column generated.
        /// If allProperties is false, only properties that have a OLVColumn attribute will have a column generated.
        /// </summary>
        /// <param name="type"></param>
        /// <param name="allProperties">Will columns be generated for properties that are not marked with [OLVColumn].</param>
        /// <returns>A collection of OLVColumn{T}s matching the attributes of Type that have OLVColumnAttributes.</returns>
        public virtual IList<OLVColumn<T>> GenerateColumns(bool allProperties) {
            var columns = new List<OLVColumn<T>>();

            // Iterate all public properties in the class and build columns from those that have
            // an OLVColumn{T} attribute and that are not ignored.
            foreach (var pinfo in typeof(T).GetProperties()) {
                if (Attribute.GetCustomAttribute(pinfo, typeof(OLVIgnoreAttribute)) != null)
                    continue;

                var attr = Attribute.GetCustomAttribute(pinfo, typeof(OLVColumnAttribute)) as OLVColumnAttribute;
                if (attr == null) {
                    if (allProperties)
                        columns.Add(MakeColumnFromPropertyInfo(pinfo));
                } else {
                    columns.Add(MakeColumnFromAttribute(pinfo, attr));
                }
            }

            // How many columns have DisplayIndex specifically set?
            var countPositiveDisplayIndex = columns.Count(col => col.DisplayIndex >= 0);

            // Give columns that don't have a DisplayIndex an incremental index
            var columnIndex = countPositiveDisplayIndex;
            foreach (var col in columns)
                if (col.DisplayIndex < 0)
                    col.DisplayIndex = (columnIndex++);

            columns.Sort((x, y) => x.DisplayIndex.CompareTo(y.DisplayIndex));

            return columns;
        }

        #endregion

        #region Implementation

        /// <summary>
        /// Replace all the columns in the given listview with the given list of columns.
        /// </summary>
        /// <param name="olv"></param>
        /// <param name="columns"></param>
        protected virtual void ReplaceColumns(ObjectListView<T> olv, IList<OLVColumn<T>> columns) {
            olv.Reset();

            // Are there new columns to add?
            if (columns == null || columns.Count == 0) 
                return;

            // Setup the columns
            olv.AllColumns.AddRange(columns);
            PostCreateColumns(olv);
        }

        /// <summary>
        /// Post process columns after creating them and adding them to the AllColumns collection.
        /// </summary>
        /// <param name="olv"></param>
        public virtual void PostCreateColumns(ObjectListView<T> olv) {
            if (olv.AllColumns.Exists(x => x.CheckBoxes))
                olv.UseSubItemCheckBoxes = true;
            if (olv.AllColumns.Cast<OLVColumn<T>>().Any(
                x => x.Index > 0 && (x.ImageGetter != null || !string.IsNullOrEmpty(x.ImageAspectName))))
                olv.ShowImagesOnSubItems = true;
            olv.RebuildColumns();
            olv.AutoSizeColumns();
        }

        /// <summary>
        /// Create a column from the given PropertyInfo and OLVColumn{T} attribute
        /// </summary>
        /// <param name="pinfo"></param>
        /// <param name="attr"></param>
        /// <returns></returns>
        protected virtual OLVColumn<T> MakeColumnFromAttribute(PropertyInfo pinfo, OLVColumnAttribute attr) {
            var column = MakeColumn(DisplayNameToColumnTitle(pinfo.Name), pinfo.CanWrite, pinfo.PropertyType, attr);
            column.PropertyInfo = pinfo;
            return column;
        }

        /// <summary>
        /// Make a column from the given PropertyInfo
        /// </summary>
        /// <param name="pinfo"></param>
        /// <returns></returns>
        protected virtual OLVColumn<T> MakeColumnFromPropertyInfo(PropertyInfo pinfo) {
            var column = MakeColumn(DisplayNameToColumnTitle(pinfo.Name), pinfo.CanWrite, pinfo.PropertyType, null);
            column.PropertyInfo = pinfo;
            return column;
        }

        /// <summary>
        /// Make a column from the given PropertyDescriptor
        /// </summary>
        /// <param name="pd"></param>
        /// <returns></returns>
        public virtual OLVColumn<T> MakeColumnFromPropertyDescriptor(PropertyDescriptor pd) {
            var attr = pd.Attributes[typeof(OLVColumnAttribute)] as OLVColumnAttribute;
            return MakeColumn(DisplayNameToColumnTitle(pd.DisplayName), !pd.IsReadOnly, pd.PropertyType, attr);
        }

        /// <summary>
        /// Create a column with all the given information
        /// </summary>
        /// <param name="aspectName"></param>
        /// <param name="title"></param>
        /// <param name="editable"></param>
        /// <param name="propertyType"></param>
        /// <param name="attr"></param>
        /// <returns></returns>
        protected virtual OLVColumn<T> MakeColumn(string title, bool editable, Type propertyType, OLVColumnAttribute attr) {

            var column = MakeColumn(title, attr);
            column.Name = attr?.Name ?? title;
            ConfigurePossibleBooleanColumn(column, propertyType);

            if (attr == null)
                return column;

            column.AspectToStringFormat = attr.AspectToStringFormat;
            if (attr.IsCheckBoxesSet)
                column.CheckBoxes = attr.CheckBoxes;
            column.DisplayIndex = attr.DisplayIndex;
            column.FillsFreeSpace = attr.FillsFreeSpace;
            if (attr.IsFreeSpaceProportionSet)
                column.FreeSpaceProportion = attr.FreeSpaceProportion;
            column.GroupWithItemCountFormat = attr.GroupWithItemCountFormat;
            column.GroupWithItemCountSingularFormat = attr.GroupWithItemCountSingularFormat;
            column.Hyperlink = attr.Hyperlink;
            column.ImageAspectName = attr.ImageAspectName;
            column.IsEditable = attr.IsEditableSet ? attr.IsEditable : editable;
            column.IsTileViewColumn = attr.IsTileViewColumn;
            column.IsVisible = attr.IsVisible;
            column.MaximumWidth = attr.MaximumWidth;
            column.MinimumWidth = attr.MinimumWidth;
            column.Tag = attr.Tag;
            if (attr.IsTextAlignSet)
                column.TextAlign = attr.TextAlign;
            column.ToolTipText = attr.ToolTipText;
            if (attr.IsTriStateCheckBoxesSet)
                column.TriStateCheckBoxes = attr.TriStateCheckBoxes;
            column.UseInitialLetterForGroup = attr.UseInitialLetterForGroup;
            column.Width = attr.Width;
            return column;
        }

        /// <summary>
        /// Create a column.
        /// </summary>
        /// <param name="title"></param>
        /// <param name="attr"></param>
        /// <returns></returns>
        protected virtual OLVColumn<T> MakeColumn(string title, OLVColumnAttribute attr) {
            var columnTitle = string.IsNullOrEmpty(attr?.Title) ? title : attr.Title;
            return new OLVColumn<T>(columnTitle);
        }

        /// <summary>
        /// Convert a property name to a displayable title.
        /// </summary>
        /// <param name="displayName"></param>
        /// <returns></returns>
        protected virtual string DisplayNameToColumnTitle(string displayName) {
            var title = displayName.Replace("_", " ");
            // Put a space between a lower-case letter that is followed immediately by an upper case letter
            title = Regex.Replace(title, @"(\p{Ll})(\p{Lu})", @"$1 $2");
            return CultureInfo.CurrentCulture.TextInfo.ToTitleCase(title);
        }

        /// <summary>
        /// Configure the given column to show a checkbox if appropriate
        /// </summary>
        /// <param name="column"></param>
        /// <param name="propertyType"></param>
        protected virtual void ConfigurePossibleBooleanColumn(OLVColumn<T> column, Type propertyType)
        {
            if (propertyType != typeof(bool) && propertyType != typeof(bool?) && propertyType != typeof(CheckState)) 
                return;

            column.CheckBoxes = true;
            column.TextAlign = HorizontalAlignment.Center;
            column.Width = 32;
            column.TriStateCheckBoxes = (propertyType == typeof(bool?) || propertyType == typeof(CheckState));
        }

        /// <summary>
        /// If this given type has an property marked with [OLVChildren], make delegates that will
        /// traverse that property as the children of an instance of the model
        /// </summary>
        /// <param name="tlv"></param>
        /// <param name="type"></param>
        protected virtual void TryGenerateChildrenDelegates(TreeListView<T> tlv) {
            foreach (var pinfo in typeof(T).GetProperties()) {
                var attr = Attribute.GetCustomAttribute(pinfo, typeof(OLVChildrenAttribute)) as OLVChildrenAttribute;
                if (attr != null) {
                    GenerateChildrenDelegates(tlv, pinfo);
                    return;
                }
            }
        }

        /// <summary>
        /// Generate CanExpand and ChildrenGetter delegates from the given property.
        /// </summary>
        /// <param name="tlv"></param>
        /// <param name="pinfo"></param>
        protected virtual void GenerateChildrenDelegates(TreeListView<T> tlv, PropertyInfo pinfo) {
            var childrenGetter = new Munger<T>(pinfo.Name);
            tlv.CanExpandGetter = delegate(T x) {
                try {
                    var result = childrenGetter.GetValueEx(x) as IEnumerable<T>;
                    return result.Any();
                }
                catch (MungerException ex) {
                    Debug.WriteLine(ex);
                    return false;
                }
            };
            tlv.ChildrenGetter = delegate(T x) {
                try {
                    return childrenGetter.GetValueEx(x) as IEnumerable<T>;
                }
                catch (MungerException ex) {
                    Debug.WriteLine(ex);
                    return null;
                }
            };
        }
        #endregion
    }
}
