using GalaSoft.MvvmLight.Command;

namespace YAXL.Common.Mvvm
{
    public class DependentRelayCommand : IDependentAction
    {
        public RelayCommand command;

        public DependentRelayCommand(RelayCommand command)
        {
            this.command = command;
        }

        public void Update() => command.RaiseCanExecuteChanged();
    }
}
