/*
 * IClusterStrategy - Encapsulates the ability to create a list of clusters from an ObjectListView
 *
 * Author: Phillip Piper
 * Date: 4-March-2011 11:59 pm
 *
 * Change log:
 * 2012-05-23  JPP  - Added CreateFilter() method to interface to allow the strategy
 *                    to control the actual model filter that is created.
 * v2.5
 * 2011-03-04  JPP  - First version
 * 
 * Copyright (C) 2011-2014 Phillip Piper
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

using System.Collections;
using System.Collections.Generic;

namespace BrightIdeasSoftware{

    /// <summary>
    /// Implementation of this interface control the selecting of cluster keys
    /// and how those clusters will be presented to the user
    /// </summary>
    public interface IClusteringStrategy<T>
    {

        /// <summary>
        /// Gets or sets the column upon which this strategy will operate
        /// </summary>
        OLVColumn<T> Column { get; set; }

        /// <summary>
        /// Create a cluster to hold the given cluster key
        /// </summary>
        /// <param name="clusterKey"></param>
        /// <returns></returns>
        ICluster<T> CreateCluster(T clusterKey);

        /// <summary>
        /// Gets the display label that the given cluster should use
        /// </summary>
        /// <param name="cluster"></param>
        /// <returns></returns>
        string GetClusterDisplayLabel(ICluster<T> cluster);

        /// <summary>
        /// Create a filter that will include only model objects that
        /// match one or more of the given values.
        /// </summary>
        /// <param name="valuesChosenForFiltering"></param>
        /// <returns></returns>
        IModelFilter<T> CreateFilter(IList<T> valuesChosenForFiltering);
    }
}
