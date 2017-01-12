/*
 * Groups - Enhancements to the normal ListViewGroup
 *
 * Author: Phillip Piper
 * Date: 22/08/2009 6:03PM
 *
 * Change log:
 * v2.3
 * 2009-09-09   JPP  - Added Collapsed and Collapsible properties
 * 2009-09-01   JPP  - Cleaned up code, added more docs
 *                   - Works under VS2005 again
 * 2009-08-22   JPP  - Initial version
 *
 * To do:
 * - Implement subseting
 * - Implement footer items
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

namespace BrightIdeasSoftware
{
    /// <summary>
    /// These values indicate what is the state of the group. These values
    /// are taken directly from the SDK and many are not used by ObjectListView.
    /// </summary>
    [Flags]
    public enum GroupState
    {
        /// <summary>
        /// Normal
        /// </summary>
        LVGS_NORMAL = 0x0,

        /// <summary>
        /// Collapsed
        /// </summary>
        LVGS_COLLAPSED = 0x1,

        /// <summary>
        /// Hidden
        /// </summary>
        LVGS_HIDDEN = 0x2,

        /// <summary>
        /// NoHeader
        /// </summary>
        LVGS_NOHEADER = 0x4,

        /// <summary>
        /// Can be collapsed
        /// </summary>
        LVGS_COLLAPSIBLE = 0x8,

        /// <summary>
        /// Has focus
        /// </summary>
        LVGS_FOCUSED = 0x10,

        /// <summary>
        /// Is Selected
        /// </summary>
        LVGS_SELECTED = 0x20,

        /// <summary>
        /// Is subsetted
        /// </summary>
        LVGS_SUBSETED = 0x40,

        /// <summary>
        /// Subset link has focus
        /// </summary>
        LVGS_SUBSETLINKFOCUSED = 0x80,

        /// <summary>
        /// All styles
        /// </summary>
        LVGS_ALL = 0xFFFF
    }

    /// <summary>
    /// This mask indicates which members of a LVGROUP have valid data. These values
    /// are taken directly from the SDK and many are not used by ObjectListView.
    /// </summary>
    [Flags]
    public enum GroupMask
    {
        /// <summary>
        /// No mask
        /// </summary>
        LVGF_NONE = 0,

        /// <summary>
        /// Group has header
        /// </summary>
        LVGF_HEADER = 1,

        /// <summary>
        /// Group has footer
        /// </summary>
        LVGF_FOOTER = 2,

        /// <summary>
        /// Group has state
        /// </summary>
        LVGF_STATE = 4,

        /// <summary>
        /// 
        /// </summary>
        LVGF_ALIGN = 8,

        /// <summary>
        /// 
        /// </summary>
        LVGF_GROUPID = 0x10,

        /// <summary>
        /// pszSubtitle is valid
        /// </summary>
        LVGF_SUBTITLE = 0x00100,  

        /// <summary>
        /// pszTask is valid
        /// </summary>
        LVGF_TASK = 0x00200, 

        /// <summary>
        /// pszDescriptionTop is valid
        /// </summary>
        LVGF_DESCRIPTIONTOP = 0x00400,  

        /// <summary>
        /// pszDescriptionBottom is valid
        /// </summary>
        LVGF_DESCRIPTIONBOTTOM = 0x00800,  

        /// <summary>
        /// iTitleImage is valid
        /// </summary>
        LVGF_TITLEIMAGE = 0x01000,  

        /// <summary>
        /// iExtendedImage is valid
        /// </summary>
        LVGF_EXTENDEDIMAGE = 0x02000,  
        
        /// <summary>
        /// iFirstItem and cItems are valid
        /// </summary>
        LVGF_ITEMS = 0x04000,  
        
        /// <summary>
        /// pszSubsetTitle is valid
        /// </summary>
        LVGF_SUBSET = 0x08000,  
     
        /// <summary>
        /// readonly, cItems holds count of items in visible subset, iFirstItem is valid
        /// </summary>
        LVGF_SUBSETITEMS = 0x10000  
    }

    /// <summary>
    /// This mask indicates which members of a GROUPMETRICS structure are valid
    /// </summary>
    [Flags]
    public enum GroupMetricsMask
    {
        /// <summary>
        /// 
        /// </summary>
        LVGMF_NONE = 0,

        /// <summary>
        /// 
        /// </summary>
        LVGMF_BORDERSIZE = 1,

        /// <summary>
        /// 
        /// </summary>
        LVGMF_BORDERCOLOR = 2,

        /// <summary>
        /// 
        /// </summary>
        LVGMF_TEXTCOLOR = 4
    }
}
