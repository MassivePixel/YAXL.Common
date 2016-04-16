using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Ioc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace YAXL.Common.Mvvm
{
    public class ViewModelBase : GalaSoft.MvvmLight.ViewModelBase
    {
        public override void Cleanup()
        {
            base.Cleanup();
            MessengerInstance.Unregister(this);
        }

        /// <summary>
        /// Helper method for retrieving instances from SimpleIoc.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        protected static T Resolve<T>() where T : class
            => SimpleIoc.Default.TryGetInstance<T>();

        #region Property helpers

        private readonly Dictionary<string, object> _values = new Dictionary<string, object>();

        protected TReturn Get<TReturn>(TReturn defaultValue = default(TReturn), [CallerMemberName] string propertyName = null)
        {
            if (propertyName == null)
                throw new ArgumentNullException(nameof(propertyName));

            object o;
            if (!_values.TryGetValue(propertyName, out o))
                return defaultValue;

            if (o is TReturn)
                return (TReturn)o;

            throw new InvalidOperationException(
                $"Invalid type for property {propertyName}. Requested {typeof(TReturn).FullName}, remembered {o.GetType().FullName}");
        }

        protected bool Set<TReturn>(TReturn value, [CallerMemberName] string propertyName = null)
        {
            if (propertyName == null)
                throw new ArgumentNullException(nameof(propertyName));

            object existing;
            if (!_values.TryGetValue(propertyName, out existing))
            {
                _values.Add(propertyName, value);
                // ReSharper disable once ExplicitCallerInfoArgument
                RaisePropertyChanged(propertyName);
                return true;
            }

            // existing value for this property is not null and types differ
            // in case of value types, check for nullable
            var newType = typeof(TReturn);
            var newTypeInfo = newType.GetTypeInfo();

            // ReSharper disable once CompareNonConstrainedGenericWithNull
            // it's ok, we checked if it was a value type before null comparison
            if (!newTypeInfo.IsValueType && value != null)
                newType = value.GetType();

            if (existing != null &&
                newType != existing.GetType() &&
                // Toni: fixed type comparison from silverlight to portable
                !newTypeInfo.IsAssignableFrom(typeof(TReturn).GetTypeInfo()) &&
                //!newType.IsAssignableFrom(typeof(TReturn)) &&
                (!newTypeInfo.IsValueType ||
                 newType != typeof(Nullable<>).MakeGenericType(existing.GetType())))
            {
                throw new InvalidOperationException(
                    $"Invalid type for property {propertyName}. Saving {newType.FullName}, previous {existing.GetType().FullName}. Property type is {typeof(TReturn).FullName}");
            }

            if (EqualityComparer<TReturn>.Default.Equals((TReturn)existing, value))
                return false;

            _values[propertyName] = value;
            // ReSharper disable once ExplicitCallerInfoArgument
            RaisePropertyChanged(propertyName);
            return true;
        }

        #endregion

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
