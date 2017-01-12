using System;
using System.Drawing;
using System.Reflection;
using System.Windows.Forms;

namespace BrightIdeasSoftware
{
    public interface IOLVColumn
    {
        string AspectToStringFormat { get; set; }
        bool AutoCompleteEditor { get; set; }
        AutoCompleteMode AutoCompleteEditorMode { get; set; }
        int ButtonMaxWidth { get; set; }
        Size? ButtonPadding { get; set; }
        Size? ButtonSize { get; set; }
        ButtonSizingMode ButtonSizing { get; set; }
        bool CanBeHidden { get; }
        bool? CellEditUseWholeCell { get; set; }
        bool CellEditUseWholeCellEffective { get; }
        Rectangle? CellPadding { get; set; }
        StringAlignment? CellVerticalAlignment { get; set; }
        bool CheckBoxes { get; set; }
        bool EnableButtonWhenItemIsDisabled { get; set; }
        bool FillsFreeSpace { get; set; }
        int FreeSpaceProportion { get; set; }
        bool Groupable { get; set; }
        string GroupWithItemCountFormat { get; set; }
        string GroupWithItemCountFormatOrDefault { get; }
        string GroupWithItemCountSingularFormat { get; set; }
        string GroupWithItemCountSingularFormatOrDefault { get; }
        bool HasFilterIndicator { get; }
        bool HasHeaderImage { get; }
        bool HeaderCheckBox { get; set; }
        bool HeaderCheckBoxDisabled { get; set; }
        bool HeaderCheckBoxUpdatesRowCheckBoxes { get; set; }
        CheckState HeaderCheckState { get; set; }
        Font HeaderFont { get; set; }
        Color HeaderForeColor { get; set; }
        HeaderFormatStyle HeaderFormatStyle { get; set; }
        string HeaderImageKey { get; set; }
        HorizontalAlignment? HeaderTextAlign { get; set; }
        StringAlignment HeaderTextAlignAsStringAlignment { get; }
        HorizontalAlignment HeaderTextAlignOrDefault { get; }
        bool HeaderTriStateCheckBox { get; set; }
        bool Hideable { get; set; }
        bool Hyperlink { get; set; }
        string ImageAspectName { get; set; }
        bool IsButton { get; set; }
        bool IsEditable { get; set; }
        bool IsFixedWidth { get; }
        bool IsHeaderVertical { get; set; }
        bool IsTileViewColumn { get; set; }
        bool IsVisible { get; set; }
        int LastDisplayIndex { get; set; }
        int MaximumWidth { get; set; }
        int MinimumWidth { get; set; }
        MemberInfo PropertyInfo { get; set; }
        bool Searchable { get; set; }
        SearchValueGetterDelegate SearchValueGetter { get; set; }
        bool ShowTextInHeader { get; set; }
        bool Sortable { get; set; }
        HorizontalAlignment TextAlign { get; set; }
        StringAlignment TextStringAlign { get; }
        string ToolTipText { get; set; }
        bool TriStateCheckBoxes { get; set; }
        bool UseFiltering { get; set; }
        bool UseInitialLetterForGroup { get; set; }
        int Width { get; set; }
        bool WordWrap { get; set; }

        event EventHandler<EventArgs> VisibilityChanged;
    }
}