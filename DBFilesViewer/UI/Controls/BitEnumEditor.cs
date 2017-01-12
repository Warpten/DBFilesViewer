using System;
using System.ComponentModel;
using System.Drawing.Design;
using System.Windows.Forms;
using System.Windows.Forms.Design;

namespace DBFilesViewer.UI.Controls
{
    public class FlagCheckedListBox : CheckedListBox
    {
        private Container components = null;
  
        public FlagCheckedListBox()
        {
            InitializeComponent();
        }
  
        protected override void Dispose(bool disposing)
        {
            if (disposing)
                components?.Dispose();

            base.Dispose(disposing);
        }
  
        #region Component Designer generated code
        private void InitializeComponent()
        {
            CheckOnClick = true;
        }
        #endregion
  
        public FlagCheckedListBoxItem Add(uint v, string c)
        {
            var listboxItem = new FlagCheckedListBoxItem(v, c);
            Items.Add(listboxItem);
            return listboxItem;
        }

        protected override void OnItemCheck(ItemCheckEventArgs e)
        {
            base.OnItemCheck(e);

            if (isUpdatingCheckStates)
                return;

            // Get the checked/unchecked item
            var item = Items[e.Index] as FlagCheckedListBoxItem;
            // Update other items
            UpdateCheckedItems(item, e.NewValue);
        }

        // Checks/Unchecks items depending on the give bitvalue
        protected void UpdateCheckedItems(uint value)
        {
            isUpdatingCheckStates = true;

            // Iterate over all items
            for (var i = 0; i < Items.Count; i++)
            {
                var item = Items[i] as FlagCheckedListBoxItem;
                if (item == null)
                    continue;

                if (item.Value == 0)
                    SetItemChecked(i, value == 0);
                else
                    SetItemChecked(i, (item.Value & value) == item.Value && item.Value != 0);
            }

            isUpdatingCheckStates = false;

        }

        // Updates items in the checklistbox
        // composite = The item that was checked/unchecked
        // cs = The check state of that item
        protected void UpdateCheckedItems(FlagCheckedListBoxItem composite, CheckState cs)
        {
            // If the value of the item is 0, call directly.
            if (composite.Value == 0)
                UpdateCheckedItems(0);

            // Get the total value of all checked items
            var sum = GetCurrentValue();

            // If the item has been unchecked, remove its bits from the sum
            if (cs == CheckState.Unchecked)
                sum &= ~composite.Value;
            else
                sum |= composite.Value;

            // Update all items in the checklistbox based on the final bit value
            UpdateCheckedItems(sum);

        }

        private bool isUpdatingCheckStates;
  
        // Gets the current bit value corresponding to all checked items
        public uint GetCurrentValue()
        {
            var sum = 0u;
            for (var i = 0; i < Items.Count; i++)
            {
                var item = Items[i] as FlagCheckedListBoxItem;
                if (item != null && GetItemChecked(i))
                    sum |= item.Value;
            }

            return sum;
        }

        private Type enumType;
        private Enum enumValue;

        // Adds items to the checklistbox based on the members of the enum
        private void FillEnumMembers()
        {
            foreach (var name in Enum.GetNames(enumType))
            {
                var intVal = (uint)Convert.ChangeType(Enum.Parse(enumType, name), typeof(uint));

                Add(intVal, name);
            }
        }

        // Checks/unchecks items based on the current value of the enum variable
        private void ApplyEnumValue()
        {
            UpdateCheckedItems((uint)Convert.ChangeType(enumValue, typeof(uint)));
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Enum EnumValue
        {
            get
            {
                return (Enum)Enum.ToObject(enumType, GetCurrentValue());
            }
            set
            {
                Items.Clear();
                enumValue = value;
                enumType = value.GetType();
                FillEnumMembers();
                ApplyEnumValue();
            }
        }
    }

    public class FlagCheckedListBoxItem
    {
        public FlagCheckedListBoxItem(uint v, string c)
        {
            Value = v;
            Caption = c;
        }

        public override string ToString() => Caption;

// Returns true if the value corresponds to a single bit being set
        public bool IsFlag => (Value & (Value - 1)) == 0;

// Returns true if this value is a member of the composite bit value
        public bool IsMemberFlag(FlagCheckedListBoxItem composite) => IsFlag && ((Value & composite.Value) == Value);

        public uint Value { get; }
        public string Caption { get; }
    }

    public class BitEnumEditor : UITypeEditor
    {
        private FlagCheckedListBox flagEnumCB;

        public BitEnumEditor()
        {
            flagEnumCB = new FlagCheckedListBox {BorderStyle = BorderStyle.None};
        }

        public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
        {
            if (context?.Instance == null || provider == null || context.PropertyDescriptor == null)
                return null;

            var edSvc = (IWindowsFormsEditorService) provider.GetService(typeof (IWindowsFormsEditorService));

            if (edSvc == null)
                return null;

            var e = (Enum) Convert.ChangeType(value, context.PropertyDescriptor.PropertyType);
            flagEnumCB.EnumValue = e;
            edSvc.DropDownControl(flagEnumCB);
            return flagEnumCB.EnumValue;
        }

        public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
            => UITypeEditorEditStyle.DropDown;
    }
}
