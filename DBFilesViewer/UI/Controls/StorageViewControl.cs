using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;
using BrightIdeasSoftware;
using DBFilesClient.NET;
using DBFilesViewer.Data.IO.Files;
using DBFilesViewer.Data.Structures.Types;
using DBFilesViewer.UI.Forms;

namespace DBFilesViewer.UI.Controls
{
    public sealed partial class StorageViewControl<T> : UserControl, IStorageViewControl where T : class, new()
    {
        public event Action<Type> OnClose;

        private Storage<T> Store { get; }
        public Type RecordType => Store.RecordType;
        private FieldInfo[] Fields => RecordType.GetFields();

        private List<Filter<T>> _filters = new List<Filter<T>>();
        public event Action<Type, uint> OnReferenceSelected;

        public StorageViewControl()
        {
            InitializeComponent();

            Dock = DockStyle.Fill;
            
            Store = DBC.Get<T>();

            #region Setup data view
            _fileSummaryListView.BeginUpdate();
            _fileSummaryListView.Reset();
            _fileSummaryListView.CellClick += (sender, args) => {
                if (args.ClickCount == 2)
                    InspectEntry(_fileSummaryListView.SelectedObject.Key);
            };

            _fileSummaryListView.AllColumns.Add(GenerateKeyColumn(Fields[Store.IndexField]));

            for (var i = 0; i < Fields.Length; ++i)
            {
                if (!Store.HasIndexTable && i == Store.IndexField)
                    continue;

                _fileSummaryListView.AllColumns.AddRange(GenerateColumn(Fields[i]));
            }

            _fileSummaryListView.UseFiltering = true;

            _fileSummaryListView.RebuildColumns();
            _fileSummaryListView.AutoSizeColumns();
            _fileSummaryListView.SetObjects(Store);
            _fileSummaryListView.EndUpdate();
            #endregion

            #region Inspector selector
            // TODO Move this to designer time
            _recordInspectListView.BeginUpdate();
            _recordInspectListView.Reset();
            _recordInspectListView.AllColumns.Add(new OLVColumn<uint>  {
                AspectGetter = rowObject => rowObject.ToString(),
                FillsFreeSpace = true
            });
            _recordInspectListView.RebuildColumns();
            _recordInspectListView.AutoSizeColumns();
            _recordInspectListView.SetObjects(Store.Keys);
            _recordInspectListView.EndUpdate();
            #endregion

            #region Setup filters
            foreach (var column in _fileSummaryListView.AllColumns)
                _columnComboBox.Items.Add(new FieldNameComboBox<T> { Column = column as OLVColumn<KeyValuePair<uint, T>> });

            Generator<Filter<T>>.GenerateColumns(_filterListView);
            #endregion

            #region Buttons to PropertyGrid
            foreach (var propControl in propertyGrid1.Controls)
            {
                if (!(propControl is ToolStrip))
                    continue;

                // -- ToolStrip control found.
                // Add every optional button.
                var buttonAttributes = typeof (T).GetCustomAttributes<OptionalButtonAttribute>();
                foreach (var buttonAttr in buttonAttributes)
                {
                    buttonAttr.TagGetter = () => _recordInspectListView.SelectedObject;
                    (propControl as ToolStrip).Items.Add(buttonAttr.Button);
                }

                // Add static button about references
                var referencesToolStripButton = new ToolStripButton("See references");
                referencesToolStripButton.Click += (o, _) =>
                {
                    var finderForm = new ReferenceFinderForm<T>();
                    finderForm.OnReferenceSelected += OnReferenceSelected;
                    finderForm.FindReferences(_recordInspectListView.SelectedObject);
                    finderForm.ShowDialog();
                };
                (propControl as ToolStrip).Items.Add(referencesToolStripButton);
                break;
            }
            #endregion
        }

        #region Creation helpers
        private OLVColumn<KeyValuePair<uint, T>> GenerateKeyColumn(FieldInfo indexField)
        {
            var argumentExpression = Expression.Parameter(typeof(KeyValuePair<uint, T>), "rowObject");
            var dictionaryValueExpression = Expression.MakeMemberAccess(
                argumentExpression, typeof(KeyValuePair<uint, T>).GetProperty("Key"));

            return new OLVColumn<KeyValuePair<uint, T>>
            {
                Text = !Store.HasIndexTable && indexField != null ? indexField.Name : "ID",
                AspectGetter = Expression.Lambda<Func<KeyValuePair<uint, T>, object>>(Expression.Convert(dictionaryValueExpression, typeof(object)),
                    argumentExpression).Compile(),
                Tag = !Store.HasIndexTable && indexField != null ? indexField : null,
                IsVisible = true
            };
        }

        private static IEnumerable<TypeSafeColumn<T>> GenerateColumn(FieldInfo fieldInfo)
        {
            var argumentExpression = Expression.Parameter(typeof (T), "rowObject.Value");
            var memberAccessExpr = Expression.MakeMemberAccess(argumentExpression, fieldInfo);

            if (!fieldInfo.FieldType.IsArray)
            {
                var exprBody = Expression.TypeAs(memberAccessExpr, typeof(object));

                return new[] { new TypeSafeColumn<T> {
                    Text = fieldInfo.Name,
                    Getter =
                        Expression.Lambda<Func<T, object>>(exprBody, argumentExpression).Compile(),
                    FieldInfo = fieldInfo,
                    ArrayIndex = -1,
                    IsVisible = fieldInfo.FieldType == typeof(string)
                } };
            }

            var marshalAttr = fieldInfo.GetCustomAttribute<MarshalAsAttribute>();
            if (marshalAttr == null)
                throw new InvalidOperationException(
                    "For performance reason, this project requires all array fields to be decorated with MarshalAsAttribute.");

            var columns = new TypeSafeColumn<T>[marshalAttr.SizeConst];
            for (var i = 0; i < columns.Length; ++i)
            {
                columns[i] = new TypeSafeColumn<T>
                {
                    Text = $"{fieldInfo.Name}[{i}]",
                    Getter =
                        Expression.Lambda<Func<T, object>>(Expression.Convert(Expression.ArrayIndex(memberAccessExpr,
                            Expression.Constant(i)), typeof(object)), argumentExpression).Compile(),
                    IsVisible = fieldInfo.FieldType.GetElementType() == typeof(string),
                    FieldInfo = fieldInfo,
                    ArrayIndex = i
                };
            }
            return columns;
        }
        #endregion

        private void CloseTab(object sender, EventArgs e) => OnClose?.Invoke(RecordType);

        /// <summary>
        /// Called when the selection changed (through arrow keys).
        /// Complementary to OnItemSelected.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnSelectionChanged(object sender, EventArgs e)
        {
            if (Store.ContainsKey(_recordInspectListView.SelectedObject))
                propertyGrid1.SelectedObject = new PropertyGridProxy<T>(Store[_recordInspectListView.SelectedObject]);
        }

        /// <summary>
        /// Switches to the inspection tab and displays the selected entry.
        /// </summary>
        /// <param name="key">Entry of the record to display in the property grid.</param>
        public void InspectEntry(uint key)
        {
            var instance = Store[key];
            if (instance == null)
                return;

            propertyGrid1.SelectedObject = new PropertyGridProxy<T>(instance);

            _tabControl.SelectedTab = _inspectorTabPage;
            _recordInspectListView.SelectedObject = key;
            _recordInspectListView.SelectedItem.EnsureVisible();
        }

        #region Export helpers
        /// <summary>
        /// Exports the model structure as C.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ExportStructure(object sender, EventArgs e) => new StructureExportForm<T>().ShowDialog(this);

        /// <summary>
        /// Exports the object store to CSV.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ExportCSV(object sender, EventArgs e)
        {
            if (Store == null)
                return;

            using (var writer = new StreamWriter(File.OpenWrite($"./{Store.RecordType.Name}.csv"), Encoding.UTF8))
            {
                var fields = Store.RecordType.GetFields();
                var fieldArraySizes = new Dictionary<FieldInfo, int>();

                var rows = new List<List<string>>();

                foreach (var keyValuePair in Store)
                {
                    var rowTokens = new List<string>
                    {
                        Store.HasIndexTable
                            ? keyValuePair.Key.ToString()
                            : fields[Store.IndexField].GetValue(keyValuePair.Value).ToString()
                    };

                    var currentCulture = CultureInfo.CurrentCulture;
                    CultureInfo.DefaultThreadCurrentCulture = CultureInfo.InvariantCulture;

                    for (var i = 0; i < fields.Length; ++i)
                    {
                        if (!Store.HasIndexTable && i == Store.IndexField)
                            continue;

                        var fieldInfo = fields[i];
                        var fieldValue = fieldInfo.GetValue(keyValuePair.Value);

                        if (!fieldInfo.FieldType.IsArray)
                        {
                            var elementValue = fieldValue.ToString().Replace("\"", "\"\"");
                            if (fieldInfo.FieldType == typeof(string) || fieldInfo.FieldType == typeof(float) ||
                                fieldInfo.FieldType == typeof(double))
                                rowTokens.Add($"\"{elementValue}\"");
                            else
                                rowTokens.Add(elementValue);
                        }
                        else
                        {
                            var arraySize = fieldInfo.GetCustomAttribute<MarshalAsAttribute>()?.SizeConst ??
                                            ((Array)fieldValue).Length;

                            fieldArraySizes[fieldInfo] = arraySize;
                            for (var j = 0; j < arraySize; ++j)
                            {
                                var elementValue = ((Array)fieldValue).GetValue(j).ToString().Replace("\"", "\"\"");
                                if (fieldInfo.FieldType == typeof(string) || fieldInfo.FieldType == typeof(float) ||
                                fieldInfo.FieldType == typeof(double))
                                    rowTokens.Add($"\"{elementValue}\"");
                                else
                                    rowTokens.Add(elementValue);
                            }
                        }
                    }

                    CultureInfo.DefaultThreadCurrentCulture = currentCulture;

                    rows.Add(rowTokens);
                }

                // Write index
                writer.Write(Store.HasIndexTable ? "ID" : fields[Store.IndexField].Name);

                for (var i = 0; i < fields.Length; ++i)
                {
                    if (!Store.HasIndexTable && i == Store.IndexField)
                        continue;

                    if (!fieldArraySizes.ContainsKey(fields[i]))
                        writer.Write($";{fields[i].Name}");
                    else
                    {
                        for (var j = 0; j < fieldArraySizes[fields[i]]; ++j)
                            writer.Write($";{fields[i].Name}[{j}]");
                    }
                }

                writer.WriteLine();

                foreach (var rowTokens in rows)
                    writer.WriteLine(string.Join(";", rowTokens));
            }
        }
        #endregion

        #region Result filtering
        private void CreateFilter(object sender, EventArgs e)
        {
            var targetField = (FieldNameComboBox<T>)_columnComboBox.SelectedItem;
            if (targetField == null)
                return;

            var filterOperation = (FilterOperation)Enum.Parse(typeof(FilterOperation), _filterOpComboBox.Text);
            var targetValueText = _filterValueComboBox.Text;
            object targetValue = targetValueText;
            if (targetField.FieldType.IsEnum)
                targetValue = Enum.Parse(targetField.FieldType, targetValueText, false);
            else
                targetValue = Convert.ChangeType(targetValue, targetField.FieldTypeCode);

            var targetColumnInfo = targetField.SafeColumn;

            var fieldGetters = new List<string> { targetColumnInfo == null ? "Key" : "Value" };
            if (targetField.FieldInfo != null && targetColumnInfo != null)
                fieldGetters.Add(targetField.FieldInfo.Name);

            // IObjectType<...> or IObectType<...>[]
            if (targetField.FieldInfo != null && targetField.FieldInfo.FieldType.IsClass)
            {
                if (targetField.FieldInfo.FieldType.IsArray && targetField.ArrayIndex != -1)
                    fieldGetters.Add($"#{targetField.ArrayIndex}");
                fieldGetters.Add("Key");
            }
            // T[]
            else if(targetField.FieldInfo != null && targetField.FieldInfo.FieldType.IsArray && targetField.ArrayIndex != -1)
                fieldGetters.Add($"#{targetField.ArrayIndex}");

            // Any column of the record.
            switch (targetField.FieldTypeCode)
            {
                case TypeCode.SByte:
                case TypeCode.Byte:
                case TypeCode.Int16:
                case TypeCode.UInt16:
                case TypeCode.Int32:
                case TypeCode.UInt32:
                case TypeCode.Int64:
                case TypeCode.UInt64:
                case TypeCode.Single:
                case TypeCode.Double:
                case TypeCode.String:
                    CreateFieldFilter(filterOperation, targetValue, targetField.FieldInfo?.Name ?? "ID", fieldGetters);
                    break;
            }
        }

        private void CreateFieldFilter(FilterOperation operation, object rightHandOperand, string fieldName, List<string> fieldGetters)
        {
            var argumentExpression = Expression.Parameter(typeof (KeyValuePair<int, T>));

            var memberType = typeof(KeyValuePair<uint, T>);
            Expression memberAccessExpr = argumentExpression;
            foreach (var fieldGetter in fieldGetters)
            {
                if (fieldGetter[0] == '#')
                {
                    memberAccessExpr = Expression.ArrayAccess(memberAccessExpr,
                        Expression.Constant(int.Parse(fieldGetter.Substring(1))));
                    memberType = memberType.GetElementType();
                    continue;
                }

                var fieldInfo = memberType.GetMember(fieldGetter).FirstOrDefault();
                if (fieldInfo == null)
                    break;

                memberAccessExpr = Expression.MakeMemberAccess(memberAccessExpr, fieldInfo);
                var fieldAsFieldInfo = fieldInfo as FieldInfo;
                if (fieldAsFieldInfo != null)
                    memberType = fieldAsFieldInfo.FieldType;
                else if (fieldInfo is PropertyInfo)
                    memberType = ((PropertyInfo)fieldInfo).PropertyType;
            }

            if (memberAccessExpr == null)
                throw new InvalidOperationException($@"Unable to find member path '{string.Join(".", fieldGetters)}' in KeyValuePair<int, {typeof (T).Name}>.");

            var lambdaExpression = Expression.Lambda<Func<KeyValuePair<uint, T>, bool>>(
                GetFilterExpression(operation, memberAccessExpr, rightHandOperand),
                argumentExpression);

            _filters.Add(new Filter<T> {
                Handler = lambdaExpression.Compile(),
                FieldName = fieldName,
                Operation = operation,
                RightOperand = rightHandOperand.ToString()
            });
            _fileSummaryListView.ModelFilter = new OneOfLambdaFilter<KeyValuePair<uint, T>>(_filters.Select(f => f.Handler));
            _filterListView.AddObject(_filters.Last());
        }

        private static Expression GetFilterExpression<U>(FilterOperation op, Expression leftHand, U rightHand)
        {
            switch (op)
            {
                case FilterOperation.Equal:
                    return Expression.Equal(leftHand, Expression.Constant(rightHand));
                case FilterOperation.Different:
                    return Expression.NotEqual(leftHand, Expression.Constant(rightHand));
                case FilterOperation.Greater:
                    return Expression.GreaterThan(leftHand, Expression.Constant(rightHand));
                case FilterOperation.Lesser:
                    return Expression.LessThan(leftHand, Expression.Constant(rightHand));
                case FilterOperation.GreaterOrEqual:
                    return Expression.GreaterThanOrEqual(leftHand, Expression.Constant(rightHand));
                case FilterOperation.LessOrEqual:
                    return Expression.LessThanOrEqual(leftHand, Expression.Constant(rightHand));
                case FilterOperation.And:
                    return Expression.And(leftHand, Expression.Constant(rightHand));
                case FilterOperation.Nand:
                    return Expression.Not(Expression.And(leftHand, Expression.Constant(rightHand)));
                case FilterOperation.Contains:
                {
                    var stringContainsMethod = typeof (string).GetMethod("IndexOf", new[] { typeof(string) });
                    return
                        Expression.NotEqual(
                            Expression.Call(leftHand, stringContainsMethod, Expression.Constant(rightHand)),
                                Expression.Constant(-1));
                }
                case FilterOperation.StartsWith:
                {
                    var stringContainsMethod = typeof (string).GetMethod("StartsWith", new[] { typeof(string) });
                    return
                        Expression.Equal(
                            Expression.Call(leftHand, stringContainsMethod, Expression.Constant(rightHand)),
                            Expression.Constant(0));
                }
                case FilterOperation.EndsWith:
                {
                    var stringContainsMethod = typeof (string).GetMethod("EndsWith", new[] { typeof(string) });
                    return
                        Expression.Equal(
                            Expression.Call(leftHand, stringContainsMethod, Expression.Constant(rightHand)),
                            Expression.Constant(0));
                }
                default:
                    throw new ArgumentOutOfRangeException(nameof(op), op, null);
            }
        }

        private void OnFilterFieldSelected(object sender, EventArgs e)
        {
            var comboBoxInfo = (FieldNameComboBox<T>) _columnComboBox.SelectedItem;
            if (comboBoxInfo == null)
                return;

            _filterOpComboBox.BeginUpdate();
            _filterOpComboBox.Items.Clear();
            _filterOpComboBox.Items.AddRange(comboBoxInfo.GetAvailableFilters().Cast<object>().ToArray());
            _filterValueComboBox.Items.Clear();
            _filterValueComboBox.Items.AddRange(comboBoxInfo.GetAvailableValues().ToArray());
            _filterOpComboBox.EndUpdate();
        }

        #endregion

        private void SelectSummaryColumns(object sender, EventArgs e)
        {
            new ColumnSelectionForm<KeyValuePair<uint, T>>().OpenOn(_fileSummaryListView);
        }
    }

    /// <summary>
    /// A helper class for creating filters.
    /// 
    /// Encapsulates OLV columns and provides meta information about possible filters.
    /// </summary>
    /// <typeparam name="U"></typeparam>
    public class FieldNameComboBox<U>
    {
        public OLVColumn<KeyValuePair<uint, U>> Column { private get; set; }
        public TypeSafeColumn<U> SafeColumn => Column as TypeSafeColumn<U>;

        public override string ToString() => Column.Text;

        public FieldInfo FieldInfo => SafeColumn?.FieldInfo ?? (FieldInfo) Column.Tag;
        public int ArrayIndex => SafeColumn?.ArrayIndex ?? -1;

        public Type FieldType
        {
            get
            {
                if (FieldInfo == null)
                    return typeof (int);

                var elementType = FieldInfo.FieldType;
                if (elementType.IsArray)
                    elementType = elementType.GetElementType();
                return elementType;
            }
        }

        public TypeCode FieldTypeCode
        {
            get
            {
                if (FieldInfo == null)
                    return TypeCode.Int32;

                var elementType = FieldInfo.FieldType;
                if (elementType.IsArray)
                    elementType = elementType.GetElementType();

                if (Type.GetTypeCode(elementType) == TypeCode.Object)
                {
                    while (elementType.BaseType.IsGenericType)
                        elementType = elementType.BaseType;
                    elementType = elementType.GetGenericArguments()[0];
                }

                return Type.GetTypeCode(elementType);
            }
        }

        public IEnumerable<FilterOperation> GetAvailableFilters()
        {
            switch (FieldTypeCode)
            {
                case TypeCode.SByte:
                case TypeCode.Byte:
                case TypeCode.Int16:
                case TypeCode.UInt16:
                case TypeCode.Int32:
                case TypeCode.UInt32:
                case TypeCode.Int64:
                case TypeCode.UInt64:
                    return new[] {
                        FilterOperation.Equal, FilterOperation.Different, FilterOperation.Greater, FilterOperation.Lesser,
                        FilterOperation.GreaterOrEqual, FilterOperation.LessOrEqual, FilterOperation.And, FilterOperation.Nand
                    };
                case TypeCode.Single:
                case TypeCode.Double:
                    return new[] {
                        FilterOperation.Equal, FilterOperation.Different, FilterOperation.GreaterOrEqual, FilterOperation.LessOrEqual,
                        FilterOperation.Greater, FilterOperation.Lesser
                    };
                case TypeCode.String:
                    return new[] {
                        FilterOperation.Equal, FilterOperation.Different, FilterOperation.Contains, FilterOperation.StartsWith,
                        FilterOperation.EndsWith
                    };
            }
            return null;
        }

        public IEnumerable<object> GetAvailableValues()
        {
            if (FieldInfo == null)
                return new object[0];

            var fieldType = FieldInfo.FieldType;
            if (fieldType.IsArray)
                fieldType = fieldType.GetElementType();
            
            if (fieldType.IsEnum)
                return Enum.GetValues(fieldType).Cast<object>();

            return new object[0];
        }
    }

    public enum FilterOperation
    {
        [Description("a == b")] Equal,
        [Description("a != b")] Different,
        [Description("a > b")] Greater,
        [Description("a < b")] Lesser,
        [Description("a >= b")] GreaterOrEqual,
        [Description("a <= b")] LessOrEqual,
        [Description("a & b")] And,
        [Description("!(a & b)")] Nand,
        [Description("a contains b")] Contains,
        [Description("a starts with b")] StartsWith,
        [Description("a ends with b")] EndsWith,
    }

    /// <summary>
    /// Model for listing active filters.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class Filter<T>
    {
        [OLVIgnore]
        public Func<KeyValuePair<uint, T>, bool> Handler { get; set; }

        [OLVColumn("Field")]
        public string FieldName { get; set; }

        [OLVColumn("Operator")]
        public FilterOperation Operation { get; set; }

        [OLVColumn("Value")]
        public string RightOperand { get; set; }
    }

    public interface IStorageViewControl
    {
        event Action<Type> OnClose;
        event Action<Type, uint> OnReferenceSelected;

        void InspectEntry(uint key);
    }
}
