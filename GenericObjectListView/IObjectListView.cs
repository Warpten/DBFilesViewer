using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace BrightIdeasSoftware
{
    public interface IObjectListView
    {
        List<IOLVColumn> AllColumns { get; set; }
        Color AlternateRowBackColor { get; set; }
        Color AlternateRowBackColorOrDefault { get; }
        IOLVColumn AlwaysGroupByColumn { get; set; }
        SortOrder AlwaysGroupBySortOrder { get; set; }
        ImageList BaseSmallImageList { get; set; }
        bool CanShowGroups { get; }
        bool CanUseApplicationIdle { get; set; }
        CellEditActivateMode CellEditActivation { get; set; }
        bool CellEditEnterChangesRows { get; set; }
        Control CellEditor { get; }
        bool CellEditTabChangesRows { get; set; }
        bool CellEditUseWholeCell { get; set; }
        Rectangle? CellPadding { get; set; }
        StringAlignment CellVerticalAlignment { get; set; }
        bool CheckBoxes { get; set; }
        string CheckedAspectName { get; set; }
        ListView.ColumnHeaderCollection Columns { get; }
        Rectangle ContentRectangle { get; }
        bool CopySelectionOnControlC { get; set; }
        bool CopySelectionOnControlCUsesDragSource { get; set; }
        string EmptyListMsg { get; set; }
        Font EmptyListMsgFont { get; set; }
        Font EmptyListMsgFontOrDefault { get; }
        bool Frozen { get; set; }
        ImageList GroupImageList { get; set; }
        ListViewGroupCollection Groups { get; }
        string GroupWithItemCountFormat { get; set; }
        string GroupWithItemCountFormatOrDefault { get; }
        string GroupWithItemCountSingularFormat { get; set; }
        string GroupWithItemCountSingularFormatOrDefault { get; }
        bool HasCollapsibleGroups { get; set; }
        bool HasEmptyListMsg { get; }
        bool HasOverlays { get; }
        HeaderFormatStyle HeaderFormatStyle { get; set; }
        int HeaderMaximumHeight { get; set; }
        int HeaderMinimumHeight { get; set; }
        bool HeaderUsesThemes { get; set; }
        bool HeaderWordWrap { get; set; }
        HitTestLocation HotCellHitLocation { get; }
        HitTestLocationEx HotCellHitLocationEx { get; }
        int HotColumnIndex { get; }
        int HotRowIndex { get; }
        HyperlinkStyle HyperlinkStyle { get; set; }
        bool IgnoreMissingAspects { get; }
        bool IncludeColumnHeadersInCopy { get; set; }
        bool IncludeHiddenColumnsInDataTransfer { get; set; }
        bool IsCellEditing { get; }
        bool IsDesignMode { get; }
        bool IsFiltering { get; }
        bool IsLeftMouseDown { get; }
        bool IsSearchOnSortColumn { get; set; }
        bool IsSimpleDragSource { get; set; }
        bool IsSimpleDropSink { get; set; }
        ListView.ListViewItemCollection Items { get; }
        SortOrder LastSortOrder { get; set; }
        Point LowLevelScrollPosition { get; }
        string MenuLabelColumns { get; set; }
        string MenuLabelGroupBy { get; set; }
        string MenuLabelLockGroupingOn { get; set; }
        string MenuLabelSelectColumns { get; set; }
        string MenuLabelSortAscending { get; set; }
        string MenuLabelSortDescending { get; set; }
        string MenuLabelTurnOffGroups { get; set; }
        string MenuLabelUnlockGroupingOn { get; set; }
        string MenuLabelUnsort { get; set; }
        int OverlayTransparency { get; set; }
        bool OwnerDraw { get; set; }
        bool OwnerDrawnHeader { get; set; }
        bool PersistentCheckBoxes { get; set; }
        SortOrder PrimarySortOrder { get; set; }
        bool RenderNonEditableCheckboxesAsDisabled { get; set; }
        int RowHeight { get; set; }
        int RowHeightEffective { get; }
        int RowsPerPage { get; }
        SortOrder SecondarySortOrder { get; set; }
        bool SelectAllOnControlA { get; set; }
        bool SelectColumnsMenuStaysOpen { get; set; }
        bool SelectColumnsOnRightClick { get; set; }
        ColumnSelectBehaviour SelectColumnsOnRightClickBehaviour { get; set; }
        Color SelectedBackColor { get; set; }
        Color SelectedBackColorOrDefault { get; }
        Color SelectedColumnTint { get; set; }
        Color SelectedForeColor { get; set; }
        Color SelectedForeColorOrDefault { get; }
        int SelectedIndex { get; set; }
        bool ShowCellPaddingBounds { get; set; }
        bool ShowCommandMenuOnRightClick { get; set; }
        bool ShowFilterMenuOnRightClick { get; set; }
        bool ShowGroups { get; set; }
        bool ShowHeaderInAllViews { get; set; }
        bool ShowImagesOnSubItems { get; set; }
        bool ShowItemCountOnGroups { get; set; }
        bool ShowSortIndicators { get; set; }
        ImageList SmallImageList { get; set; }
        Size SmallImageSize { get; }
        SmoothingMode SmoothingMode { get; set; }
        bool SortGroupItemsByPrimaryColumn { get; set; }
        int SpaceBetweenGroups { get; set; }
        bool TintSortColumn { get; set; }
        int TopItemIndex { get; set; }
        bool TriggerCellOverEventsWhenOverHeader { get; set; }
        bool TriStateCheckBoxes { get; set; }
        Color UnfocusedSelectedBackColor { get; set; }
        Color UnfocusedSelectedBackColorOrDefault { get; }
        Color UnfocusedSelectedForeColor { get; set; }
        Color UnfocusedSelectedForeColorOrDefault { get; }
        bool UpdateSpaceFillingColumnsWhenDraggingColumnDivider { get; set; }
        bool UseAlternatingBackColors { get; set; }
        bool UseCellFormatEvents { get; set; }
        bool UseCustomSelectionColors { get; set; }
        bool UseExplorerTheme { get; set; }
        bool UseFilterIndicator { get; set; }
        bool UseFiltering { get; set; }
        bool UseHotControls { get; set; }
        bool UseHotItem { get; set; }
        bool UseHyperlinks { get; set; }
        bool UseNotifyPropertyChanged { get; set; }
        bool UseOverlays { get; set; }
        bool UseSubItemCheckBoxes { get; set; }
        bool UseTranslucentHotItem { get; set; }
        bool UseTranslucentSelection { get; set; }
        View View { get; set; }

        List<IOLVColumn> GetFilteredColumns(View view);
    }
}