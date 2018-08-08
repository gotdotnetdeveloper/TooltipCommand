using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Messaging;
using ToolTipCommand;
using ToolTipCommand.GalaSoft.MvvmLight.CommandWpf;

namespace WpfApp
{
    public class FirstViewModel : ViewModelBase
    {
        private string _test;

        public FirstViewModel()
        {
            SaveEmployeesCommand = new AlertCommand(SaveEmployeesMethod, CanExecuteSaveEmployeesMethod);
        }

        private bool CanExecuteSaveEmployeesMethod(CanExecuteInfo arg)
        {
            if (Test!= null && Test.Length > 1)
            {
                arg.DisableReason = DisableReason.Error;
                arg.DisableReasonTip = "Error";
                return false;
            }
            else
            {
                return true;
            }
        }

        public ICommand SaveEmployeesCommand { get; private set; }


        public string Test
        {
            get => _test;
            set
            {
                _test = value;
                RaisePropertyChanged(() => Test);
            } }


        public void SaveEmployeesMethod()
        {
            Messenger.Default.Send<NotificationMessage>(new NotificationMessage("Employees Saved."));
        }

        private void LoadEmployeesMethod()
        {

        }
    }
}
