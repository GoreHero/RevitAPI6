using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Plumbing;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using Prism.Commands;
using RevitAPITrainingLibrary;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace RevitAPI_WPF1
{
    public class MainViewViewModel
    {
        private ExternalCommandData _commandData;

        public List<FamilySymbol> Tags { get; }
        public Pipe Pipe { get; }
        public FamilySymbol SelectedTagType { get; set; }
        public DelegateCommand SaveCommand { get; }


        public MainViewViewModel(ExternalCommandData commandData)
        {
            _commandData = commandData;
            Tags = TagsUtils.GetPipeTagTypes(commandData);
            Pipe = SelectionUtils.GetObject<Pipe>(_commandData, "Выберите трубу");
            SaveCommand = new DelegateCommand(OnSaveCommand);
        }

        private void OnSaveCommand()
        {
            UIApplication uiapp = _commandData.Application;
            UIDocument uidoc = uiapp.ActiveUIDocument;
            Document doc = uidoc.Document;

            var pipeLocCurve = Pipe.Location as LocationCurve;
            var pipeCurve = pipeLocCurve.Curve;
            var pipeMidPoint = (pipeCurve.GetEndPoint(0) + pipeCurve.GetEndPoint(1)) / 2;
            using (var ts = new Transaction(doc, "Create tag"))
            {
                ts.Start();
                IndependentTag.Create(doc, SelectedTagType.Id, doc.ActiveView.Id, new Reference(Pipe), false, TagOrientation.Horizontal, pipeMidPoint);
                ts.Commit();
            }

            RaiseCloseRequest();
        }

        public event EventHandler CloseRequest;
        private void RaiseCloseRequest()
        {
            CloseRequest?.Invoke(this, EventArgs.Empty);
        }
    }
}
