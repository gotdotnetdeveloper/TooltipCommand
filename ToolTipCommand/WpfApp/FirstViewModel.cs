using System.Windows.Input;
using GalaSoft.MvvmLight;
using ToolTipCommand;

namespace WpfApp
{
    public class FirstViewModel : ViewModelBase
    {
        private string _test;

        public FirstViewModel()
        {
            SaveEmployeesCommand = new ViewModelCommand<string>(SaveEmployeesMethod, CanExecuteSaveEmployeesMethod);
        }

        private bool CanExecuteSaveEmployeesMethod(CanExecuteInfo info, string obj)
        {
            if (!string.IsNullOrEmpty(Test) &&  Test.Length > 1)
            {
                info.DisableReasonTip = $"{Test}!!!!!!";
                info.DisableReason = DisableReason.BusinessRule;
                return false;
            }
            return true;
        }

        private void SaveEmployeesMethod(string obj)
        {
            
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


        //public void SaveEmployeesMethod()
        //{
        //    Messenger.Default.Send<NotificationMessage>(new NotificationMessage("Employees Saved."));
        //}

        private void LoadEmployeesMethod()
        {

        }
    }
}
