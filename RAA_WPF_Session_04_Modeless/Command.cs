#region Namespaces
using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;

#endregion

namespace RAA_WPF_Session_04_Modeless
{
    [Transaction(TransactionMode.Manual)]
    public class Command : IExternalCommand
    {
        private List<Level> levelList;

        public Result Execute(
          ExternalCommandData commandData,
          ref string message,
          ElementSet elements)
        {
            UIApplication uiapp = commandData.Application;
            UIDocument uidoc = uiapp.ActiveUIDocument;
            Application app = uiapp.Application;
            Document doc = uidoc.Document;

            // put any code needed for the form here

            //create and instance of the event action
            EventAction myAction = new EventAction();

            //create a variable for the external event, this is a Revit class. Then pass in that instance of the action class
            ExternalEvent myEvent = ExternalEvent.Create(myAction);

            //pass external event to form as arguments. create variables and assign arguments to them

            // open form
            MyForm currentForm = new MyForm(doc,levelList)
            {
                Width = 450,
                Height = 450,
                WindowStartupLocation = System.Windows.WindowStartupLocation.CenterScreen,
                Topmost = true,
            };

            currentForm.Show();

            // get form data and do something

            return Result.Succeeded;
        }

        public static String GetMethod()
        {
            var method = MethodBase.GetCurrentMethod().DeclaringType?.FullName;
            return method;
        }
    }

    public class EventAction : IExternalEventHandler
    {
        private Document doc;
        public void Execute(UIApplication uiapp)
        {
            Document Doc = uiapp.ActiveUIDocument.Document;

            //get form data and do something
            
            

            
        }

        public string GetName()
        {
            return "EventAction";
        }
    }
}
