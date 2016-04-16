using GalaSoft.MvvmLight.Command;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace YAXL.Common.Mvvm
{
    public class ViewModelBase : GalaSoft.MvvmLight.ViewModelBase
    {
        #region Bound properties

        private Dictionary<string, List<IDependentAction>> boundProperties = new Dictionary<string, List<IDependentAction>>();

        protected void Bind(string sourcePropertyName, params string[] dependentProperties)
            => Bind(sourcePropertyName, dependentProperties.Select(x => new DependentProperty(x, this)));

        protected void Bind(string sourcePropertyName, params RelayCommand[] dependentCommand)
            => Bind(sourcePropertyName, dependentCommand.Select(x => new DependentRelayCommand(x)));

        protected void Bind(string sourcePropertyName, IEnumerable<IDependentAction> actions)
            => GetDependencies(sourcePropertyName)
                .AddRange(actions);

        protected void Bind(string sourcePropertyName, params IDependentAction[] actions)
            => GetDependencies(sourcePropertyName)
                .AddRange(actions);

        private List<IDependentAction> GetDependencies(string name)
        {
            List<IDependentAction> bound;
            if (!boundProperties.TryGetValue(name, out bound))
            {
                bound = new List<IDependentAction>();
                boundProperties.Add(name, bound);
            }

            return bound;
        }

        #endregion

        protected override void RaisePropertyChanged([CallerMemberName] string propertyName = null)
        {
            base.RaisePropertyChanged(propertyName);

            List<IDependentAction> bound;
            if (boundProperties.TryGetValue(propertyName, out bound))
            {
                foreach (var action in bound)
                    action.Update();
            }
        }

        internal void InternalRaisePropertyChanged(string propertyName)
            => RaisePropertyChanged(propertyName);
    }
}
