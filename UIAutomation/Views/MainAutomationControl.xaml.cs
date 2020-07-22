using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using Prism.Events;
using Prism.Ioc;
using UIAutomation.Events;
using UIAutomation.Models;

namespace UIAutomation.Views
{
    /// <summary>
    /// Interaction logic for MainAutomationControl
    /// </summary>
    public partial class MainAutomationControl : UserControl
    {
        private readonly IContainerExtension IContainer;
        private readonly IEventAggregator IEvents;
        public delegate Point GetDragDropPosition(IInputElement theElement);
        public MainAutomationControl(IContainerExtension container)
        {
            InitializeComponent();
            IContainer = container;
            IEvents = IContainer.Resolve<IEventAggregator>();
            //The Event on DataGrid for selecting the Row
            this.DataGrid1.PreviewMouseLeftButtonDown +=
                new MouseButtonEventHandler(dgEmployee_PreviewMouseLeftButtonDown);
            //The Drop Event
            this.DataGrid1.Drop += new DragEventHandler(dgEmployee_Drop);
        }
        int prevRowIndex = -1;
        /// <summary>
        /// Defines the Drop Position based upon the index.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void dgEmployee_Drop(object sender, DragEventArgs e)
        {
            if (prevRowIndex < 0)
                return;

            int index = this.GetDataGridItemCurrentRowIndex(e.GetPosition);

            //The current Rowindex is -1 (No selected)
            if (index < 0)
                return;
            //If Drag-Drop Location are same
            if (index == prevRowIndex)
                return;
            //If the Drop Index is the last Row of DataGrid(
            // Note: This Row is typically used for performing Insert operation)
            if (index == DataGrid1.Items.Count - 1)
            {
                MessageBox.Show("This row-index cannot be used for Drop Operations");
                return;
            }
             
            Dictionary<string, int> dics = new Dictionary<string, int>();
            dics["prevRowIndex"] = prevRowIndex;
            dics["index"] = index;
            IEvents.GetEvent<ChangeRowOrderEvent>().Publish(dics);
        }

        void dgEmployee_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            prevRowIndex = GetDataGridItemCurrentRowIndex(e.GetPosition);

            if (prevRowIndex < 0)
                return;
            DataGrid1.SelectedIndex = prevRowIndex;

            AutomationModel selectedEmp = DataGrid1.Items[prevRowIndex] as AutomationModel;

            if (selectedEmp == null)
                return;

            //Now Create a Drag Rectangle with Mouse Drag-Effect
            //Here you can select the Effect as per your choice

            DragDropEffects dragdropeffects = DragDropEffects.Move;

            if (DragDrop.DoDragDrop(DataGrid1, selectedEmp, dragdropeffects)
                                != DragDropEffects.None)
            {
                //Now This Item will be dropped at new location and so the new Selected Item
                DataGrid1.SelectedItem = selectedEmp;
            }
        }

        /// <summary>
        /// Method checks whether the mouse is on the required Target
        /// Input Parameter (1) "Visual" -> Used to provide Rendering support to WPF
        /// Input Paraneter (2) "User Defined Delegate" positioning for Operation
        /// </summary>
        /// <param name="theTarget"></param>
        /// <param name="pos"></param>
        /// <returns>The "Rect" Information for specific Position</returns>
        private bool IsTheMouseOnTargetRow(Visual theTarget, GetDragDropPosition pos)
        {
            if (theTarget == null)
                return false;
            Rect posBounds = VisualTreeHelper.GetDescendantBounds(theTarget);
            Point theMousePos = pos((IInputElement)theTarget);
            return posBounds.Contains(theMousePos);
        }



        /// <summary>
        /// Returns the selected DataGridRow
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        private DataGridRow GetDataGridRowItem(int index)
        {
            if (DataGrid1.ItemContainerGenerator.Status
                    != GeneratorStatus.ContainersGenerated)
                return null;

            return DataGrid1.ItemContainerGenerator.ContainerFromIndex(index)
                                                            as DataGridRow;
        }

        /// <summary>
        /// Returns the Index of the Current Row.
        /// </summary>
        /// <param name="pos"></param>
        /// <returns></returns>
        private int GetDataGridItemCurrentRowIndex(GetDragDropPosition pos)
        {
            int curIndex = -1;
            for (int i = 0; i < DataGrid1.Items.Count; i++)
            {
                DataGridRow itm = GetDataGridRowItem(i);
                if (IsTheMouseOnTargetRow(itm, pos))
                {
                    curIndex = i;
                    break;
                }
            }
            return curIndex;
        }
    }
}
