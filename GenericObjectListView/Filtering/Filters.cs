/*
 * Filters - Filtering on ObjectListViews
 *
 * Author: Phillip Piper
 * Date: 03/03/2010 17:00 
 *
 * Change log:
 * 2011-03-01  JPP  Added CompositeAllFilter, CompositeAnyFilter and OneOfFilter
 * v2.4.1
 * 2010-06-23  JPP  Extended TextMatchFilter to handle regular expressions and string prefix matching.
 * v2.4
 * 2010-03-03  JPP  Initial version
 *
 * TO DO:
 *
 * Copyright (C) 2010-2014 Phillip Piper
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
using System.Linq;

namespace BrightIdeasSoftware
{
    /// <summary>
    /// Interface for model-by-model filtering
    /// </summary>
    public interface IModelFilter<T>
    {
        /// <summary>
        /// Should the given model be included when this filter is installed
        /// </summary>
        /// <param name="modelObject">The model object to consider</param>
        /// <returns>Returns true if the model will be included by the filter</returns>
        bool Filter(T modelObject);
    }

    /// <summary>
    /// Interface for whole list filtering
    /// </summary>
    public interface IListFilter<T>
    {
        /// <summary>
        /// Return a subset of the given list of model objects as the new
        /// contents of the ObjectListView
        /// </summary>
        /// <param name="modelObjects">The collection of model objects that the list will possibly display</param>
        /// <returns>The filtered collection that holds the model objects that will be displayed.</returns>
        IList<T> Filter(IList<T> modelObjects);
    }

    /// <summary>
    /// Base class for model-by-model filters
    /// </summary>
    public class AbstractModelFilter<T> : IModelFilter<T>
    {
        /// <summary>
        /// Should the given model be included when this filter is installed
        /// </summary>
        /// <param name="modelObject">The model object to consider</param>
        /// <returns>Returns true if the model will be included by the filter</returns>
        public virtual bool Filter(T modelObject) {
            return true;
        }
    }

    /// <summary>
    /// Instances of this class extract a value from the model object
    /// and compare that value to a list of fixed values. The model
    /// object is included if the extracted value is in the list
    /// </summary>
    /// <remarks>If there are no values to match, no model objects will be matched</remarks>
    public class OneOfFilter<T> : IModelFilter<T>
    {
        /// <summary>
        /// Create a filter that will extract values using the given delegate
        /// and compare them to the values in the given list.
        /// </summary>
        /// <param name="possibleValues"></param>
        public OneOfFilter(IList<T> possibleValues)
        {
            PossibleValues = possibleValues;
        }

        /// <summary>
        /// Gets or sets the list of values that the value extracted from
        /// the model object must match in order to be included.
        /// </summary>
        public IList<T> PossibleValues { get; set; }

        /// <summary>
        /// Should the given model object be included?
        /// </summary>
        /// <param name="modelObject"></param>
        /// <returns></returns>
        public virtual bool Filter(T modelObject)
        {
            if (PossibleValues == null || PossibleValues.Count == 0)
                return false;

            return PossibleValues.Any(possibleValue => DoesValueMatch(modelObject));
        }

        /// <summary>
        /// Decides if the given property is a match for the values in the PossibleValues collection
        /// </summary>
        /// <param name="result"></param>
        /// <returns></returns>
        protected virtual bool DoesValueMatch(T result)
        {
            return PossibleValues.Contains(result);
        }
    }

    /// <summary>
    /// Instances of this class extract a value from the model object
    /// and compare that value to a list of fixed values. The model
    /// object is included if the extracted value is in the list
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class OneOfLambdaFilter<T> : IModelFilter<T>
    {
        private IList<Func<T, bool>> _delegates;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="possibleValues"></param>
        public OneOfLambdaFilter(IEnumerable<Func<T, bool>> possibleValues)
        {
            _delegates = new List<Func<T, bool>>(possibleValues);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="possibleValues"></param>
        public OneOfLambdaFilter(params Func<T, bool>[] possibleValues)
        {
            _delegates = new List<Func<T, bool>>(possibleValues);
        }

        /// <summary>
        /// Should the given model be included when this filter is installed
        /// </summary>
        /// <param name="modelObject">The model object to consider</param>
        /// <returns>Returns true if the model will be included by the filter</returns>
        public bool Filter(T modelObject)
        {
            return _delegates.Any(del => del(modelObject));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="hndl"></param>
        public void Add(Func<T, bool> hndl) => _delegates.Add(hndl);
    }

    /// <summary>
    /// Instances of this class match a property of a model objects against
    /// a list of bit flags. The property should be an xor-ed collection
    /// of bits flags.
    /// </summary>
    /// <remarks>Both the property compared and the list of possible values 
    /// must be convertible to ulongs.</remarks>
    public class FlagBitSetFilter<T> : OneOfFilter<T>
    {

        /// <summary>
        /// Create an instance
        /// </summary>
        /// <param name="possibleValues"></param>
        public FlagBitSetFilter(IList<T> possibleValues) : base(possibleValues)
        {
        }

        /// <summary>
        /// Decides if the given property is a match for the values in the PossibleValues collection
        /// </summary>
        /// <param name="result"></param>
        /// <returns></returns>
        protected override bool DoesValueMatch(T result)
        {
            try
            {
                var value = Convert.ToUInt64(result);
                return PossibleValues.Cast<ulong>().Any(flag => (value & flag) == flag);
            }
            catch (InvalidCastException)
            {
                return false;
            }
            catch (FormatException)
            {
                return false;
            }
        }
    }

    /// <summary>
    /// This filter calls a given Predicate to decide if a model object should be included
    /// </summary>
    public sealed class ModelFilter<T> : IModelFilter<T>
    {
        /// <summary>
        /// Create a filter based on the given predicate
        /// </summary>
        /// <param name="predicate">The function that will filter objects</param>
        public ModelFilter(Predicate<T> predicate) {
            Predicate = predicate;
        }

        /// <summary>
        /// Gets or sets the predicate used to filter model objects
        /// </summary>
        private Predicate<T> Predicate { get; set; }

        /// <summary>
        /// Should the given model object be included?
        /// </summary>
        /// <param name="modelObject"></param>
        /// <returns></returns>
        public bool Filter(T modelObject) {
            return Predicate?.Invoke(modelObject) ?? true;
        }
    }

    /// <summary>
    /// A CompositeFilter joins several other filters together.
    /// If there are no filters, all model objects are included
    /// </summary>
    public abstract class CompositeFilter<T> : IModelFilter<T>
    {

        /// <summary>
        /// Create an empty filter
        /// </summary>
        public CompositeFilter() {
        }

        /// <summary>
        /// Create a composite filter from the given list of filters
        /// </summary>
        /// <param name="filters">A list of filters</param>
        public CompositeFilter(IList<IModelFilter<T>> filters) {
            foreach (var filter in filters) {
                if (filter != null)
                    Filters.Add(filter);
            }
        }

        /// <summary>
        /// Gets or sets the filters used by this composite
        /// </summary>
        public IList<IModelFilter<T>> Filters { get; set; } = new List<IModelFilter<T>>();

        /// <summary>
        /// Get the sub filters that are text match filters
        /// </summary>
        public IEnumerable<TextMatchFilter<T>> TextFilters => Filters.OfType<TextMatchFilter<T>>();

        /// <summary>
        /// Decide whether or not the given model should be included by the filter
        /// </summary>
        /// <param name="modelObject"></param>
        /// <returns>True if the object is included by the filter</returns>
        public virtual bool Filter(T modelObject)
        {
            return true;
        }
    }

    /// <summary>
    /// A CompositeAllFilter joins several other filters together.
    /// A model object must satisfy all filters to be included.
    /// If there are no filters, all model objects are included
    /// </summary>
    public class CompositeAllFilter<T> : CompositeFilter<T>
    {

        /// <summary>
        /// Create a filter
        /// </summary>
        /// <param name="filters"></param>
        public CompositeAllFilter(IList<IModelFilter<T>> filters)
            : base(filters) {
        }

        /// <summary>
        /// Decide whether or not the given model should be included by the filter
        /// </summary>
        /// <remarks>Filters is guaranteed to be non-empty when this method is called</remarks>
        /// <param name="modelObject">The model object under consideration</param>
        /// <returns>True if the object is included by the filter</returns>
        public override bool Filter(T modelObject)
        {
            return Filters.All(filter => filter.Filter(modelObject));
        }
    }

    /// <summary>
    /// A CompositeAllFilter joins several other filters together.
    /// A model object must only satisfy one of the filters to be included.
    /// If there are no filters, all model objects are included
    /// </summary>
    public class CompositeAnyFilter<T> : CompositeFilter<T>
    {

        /// <summary>
        /// Create a filter from the given filters
        /// </summary>
        /// <param name="filters"></param>
        public CompositeAnyFilter(IList<IModelFilter<T>> filters)
            : base(filters) {
        }

        /// <summary>
        /// Decide whether or not the given model should be included by the filter
        /// </summary>
        /// <remarks>Filters is guaranteed to be non-empty when this method is called</remarks>
        /// <param name="modelObject">The model object under consideration</param>
        /// <returns>True if the object is included by the filter</returns>
        public override bool Filter(T modelObject) {
            foreach (var filter in Filters)
                if (filter.Filter(modelObject))
                    return true;

            return false;
        }
    }

   
    /// <summary>
    /// Base class for whole list filters
    /// </summary>
    public class AbstractListFilter<T> : IListFilter<T>
    {
        /// <summary>
        /// Return a subset of the given list of model objects as the new
        /// contents of the ObjectListView
        /// </summary>
        /// <param name="modelObjects">The collection of model objects that the list will possibly display</param>
        /// <returns>The filtered collection that holds the model objects that will be displayed.</returns>
        public virtual IList<T> Filter(IList<T> modelObjects) {
            return modelObjects;
        }
    }

    /// <summary>
    /// Instance of this class implement delegate based whole list filtering
    /// </summary>
    public class ListFilter<T> : AbstractListFilter<T>
    {
        /// <summary>
        /// A delegate that filters on a whole list
        /// </summary>
        /// <param name="rowObjects"></param>
        /// <returns></returns>
        public delegate IList<T> ListFilterDelegate(IList<T> rowObjects);

        /// <summary>
        /// Create a ListFilter
        /// </summary>
        /// <param name="function"></param>
        public ListFilter(ListFilterDelegate function) {
            Function = function;
        }

        /// <summary>
        /// Gets or sets the delegate that will filter the list
        /// </summary>
        public ListFilterDelegate Function { get; set; }

        /// <summary>
        /// Do the actual work of filtering
        /// </summary>
        /// <param name="modelObjects"></param>
        /// <returns></returns>
        public override IList<T> Filter(IList<T> modelObjects) {
            if (Function == null)
                return modelObjects;

            return Function(modelObjects);
        }
    }

    /// <summary>
    /// Filter the list so only the last N entries are displayed
    /// </summary>
    public class TailFilter<T> : AbstractListFilter<T>
    {
        /// <summary>
        /// Create a no-op tail filter
        /// </summary>
        public TailFilter() {
        }

        /// <summary>
        /// Create a filter that includes on the last N model objects
        /// </summary>
        /// <param name="numberOfObjects"></param>
        public TailFilter(int numberOfObjects) {
            Count = numberOfObjects;
        }

        /// <summary>
        /// Gets or sets the number of model objects that will be 
        /// returned from the tail of the list
        /// </summary>
        public int Count { get; set; }

        /// <summary>
        /// Return the last N subset of the model objects
        /// </summary>
        /// <param name="modelObjects"></param>
        /// <returns></returns>
        public override IList<T> Filter(IList<T> modelObjects) {
            if (Count <= 0)
                return modelObjects;

            if (Count > modelObjects.Count)
                return modelObjects;

            var tail = new T[Count];
            ((List<T>)modelObjects).CopyTo(modelObjects.Count - Count, tail, 0, Count);
            return new List<T>(tail);
        }
    }
}