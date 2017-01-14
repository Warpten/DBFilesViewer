using System;
using System.Collections.Generic;
using System.Reflection;
using BrightIdeasSoftware;

namespace DBFilesViewer.UI.Controls
{
    /// <summary>
    /// A simple type-safe wrapper around OLVColumn that hides away AspectGetter under Getter.
    /// 
    /// Designed specifically for this usage - You will need to adapt for usage in other projects.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public sealed class TypeSafeColumn<T> : OLVColumn<KeyValuePair<uint, T>>
    {
        public TypeSafeColumn()
        {
            AspectGetter = rowObject => Getter.Invoke(rowObject.Value);
        } 

        /// <summary>
        /// The actual aspect getter.
        /// </summary>
        public Func<T, object> Getter { private get; set; }

        public FieldInfo FieldInfo { get; set; }
        public int ArrayIndex { get; set; }
    }
}
