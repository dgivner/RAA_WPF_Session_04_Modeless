using Autodesk.Revit.DB;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Autodesk.Revit.UI;
using Color = Autodesk.Revit.DB.Color;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.Creation;
using Document = Autodesk.Revit.Creation.Document;

//using Document = Autodesk.Revit.Creation.Document;

namespace RAA_WPF_Session_04_Modeless
{
    /// <summary>
    /// Interaction logic for Window.xaml
    /// </summary>
    public partial class MyForm : Window
    {
        
        private readonly Autodesk.Revit.DB.Document doc;

        private ObservableCollection<Level> LevelList { get; set; }
        //private ExternalEvent myEvent;
        public MyForm(Autodesk.Revit.DB.Document doc, List<Level> levelList)
        {
            InitializeComponent();
            this.doc = doc;

            //myEvent = _event;
            // Check if levelList is null or not
            LevelList = new ObservableCollection<Level>(levelList);
            if (levelList == null)
            {
                MessageBox.Show("levelList is null!");
                return;
            }

            // Set the ItemsSource property of the ComboBox
            cmbLevels.ItemsSource = levelList;

            // Check if the ItemsSource property was set properly
            if (cmbLevels.ItemsSource == null)
            {
                MessageBox.Show("cmbLevels.ItemsSource is null!");
                return;
            }

            // Set a breakpoint here to verify that levelList has items and that cmbLevels.ItemsSource has been set properly
            cmbLevels.SelectionChanged += ComboBox_SelectionChanged;
        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Get all levels in the current project
            List<Level> levels = new FilteredElementCollector(doc)
                .OfClass(typeof(Level))
                .Cast<Level>()
                .OrderBy(level => level.Elevation)
                .ToList();

            // Debugging code - check if levels list is populated correctly
            if (levels.Count == 0)
            {
                MessageBox.Show("No levels found in the current project.");
            }
            else
            {
                foreach (Level level in levels)
                {
                    Debug.Print("Level: " + level.Name);
                }
            }

            // Add the level names to the combo box
            cmbLevels.Items.Add(levels);

            // Debugging code - check if ItemsSource is set correctly
            Debug.Print("ItemsSource: " + cmbLevels.ItemsSource);

        }

        private void btnReset_Click(object sender, RoutedEventArgs e)
        {
            // Get the active view
            View activeView = doc.ActiveView;

            // Get the original category selections
            bool lightFixturesSelected = LightFixturesCheckBox.IsChecked != null && (bool)LightFixturesCheckBox.IsChecked;
            bool communicationDevicesSelected = CommunicationDevicesCheckBox.IsChecked != null && (bool)CommunicationDevicesCheckBox.IsChecked;
            bool mechanicalEquipmentSelected = MechanicalEquipmentCheckBox.IsChecked != null && (bool)MechanicalEquipmentCheckBox.IsChecked;

            // Clear graphic overrides for each selected category
            if (lightFixturesSelected)
            {
                ClearElementOverrides(doc, activeView, BuiltInCategory.OST_LightingFixtures);
            }
            if (communicationDevicesSelected)
            {
                ClearElementOverrides(doc, activeView, BuiltInCategory.OST_CommunicationDevices);
            }
            if (mechanicalEquipmentSelected)
            {
                ClearElementOverrides(doc, activeView, BuiltInCategory.OST_MechanicalEquipment);
            }

        }

        private void btnApply_Click(object sender, RoutedEventArgs e)
        {
            //myEvent.Raise();
            // Get the selected color option
            string colorOption = "";
            if (RedRadioButton.IsChecked == true) colorOption = "Red";
            else if (BlueRadioButton.IsChecked == true) colorOption = "Blue";
            else if (GreenRadioButton.IsChecked == true) colorOption = "Green";
            else if (YellowRadioButton.IsChecked == true) colorOption = "Yellow";

            // Get the selected categories
            List<BuiltInCategory> selectedCategories = new List<BuiltInCategory>();
            if (LightFixturesCheckBox.IsChecked == true) selectedCategories.Add(BuiltInCategory.OST_LightingFixtures);
            if (CommunicationDevicesCheckBox.IsChecked == true) selectedCategories.Add(BuiltInCategory.OST_CommunicationDevices);
            if (MechanicalEquipmentCheckBox.IsChecked == true) selectedCategories.Add(BuiltInCategory.OST_MechanicalEquipment);

            // Get the selected level
            Element selectedLevel = cmbLevels.SelectedItem as Element;

            // Begin a Revit transaction
            using (Transaction t = new Transaction(doc))
            {
                t.Start("Change element color");

                // Loop through the selected categories and change their color to the selected option
                foreach (BuiltInCategory category in selectedCategories)
                {
                    // Get elements in the selected category on the selected level
                    //var elements = new FilteredElementCollector(doc, doc.ActiveView.Id).OfCategory(category).OnLevel(selectedLevel.Id).ToElements();
                    var elements = new FilteredElementCollector(doc, doc.ActiveView.Id).OfCategory(category)
                        .OfClass(typeof(Level)).ToElements();

                    // Change their color to the selected option
                    foreach (var element in elements)
                    {
                        OverrideGraphicSettings overrideSettings = new OverrideGraphicSettings();
                        overrideSettings.SetProjectionLineColor(GetColor(colorOption));
                        doc.ActiveView.SetElementOverrides(element.Id, overrideSettings);
                    }
                }

                // Commit the transaction
                t.Commit();
            }
        }
        private Color GetColor(string colorOption)
        {
            switch (colorOption)
            {
                case "Red":
                    return new Color(255, 0, 0);
                case "Blue":
                    return new Color(0, 0, 255);
                case "Green":
                    return new Color(0, 255, 0);
                case "Yellow":
                    return new Color(255, 255, 0);
                default:
                    return null;
            }
        }
        private void ClearElementOverrides(Autodesk.Revit.DB.Document doc, View view, BuiltInCategory category)
        {
            // Get a filtered element collector to retrieve elements of the specified category
            FilteredElementCollector collector = new FilteredElementCollector(doc, view.Id);
            collector.OfCategory(category);

            // Create a list of element IDs to clear overrides for
            List<ElementId> elementIds = new List<ElementId>();
            foreach (Element elem in collector)
            {
                elementIds.Add(elem.Id);
            }

            // Clear overrides for the selected elements in the view
            //view.ClearElementOverrides(elementIds);
            
        }
    }
    
}
