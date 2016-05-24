namespace YAXL.Common.Mvvm
{
    public class DependentProperty : IDependentAction
    {
        public string DependentPropertyName { get; }
        public ViewModelBase Owner { get; }

        public DependentProperty(string dependentPropertyName, ViewModelBase owner)
        {
            Owner = owner;
            DependentPropertyName = dependentPropertyName;
        }

		public void Update() => Owner.RaisePropertyChanged(DependentPropertyName);
    }
}
