﻿using System.Windows.Input;
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
            return info.EnableIf(!string.IsNullOrEmpty(Test), "должно быть не пусто")
                   && info.EnableIf(Test.Length > 1, "должно быть Length > 1")
                   && info.EnableIf(Test.Length > 2, "должно быть Length > 2");
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
