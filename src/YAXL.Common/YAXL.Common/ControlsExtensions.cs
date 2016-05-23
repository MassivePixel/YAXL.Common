using System;
using Xamarin.Forms;

namespace YAXL.Common
{
	public static class ControlsExtensions
	{
		public static T Bind<T> (this T o, BindableProperty targetProperty, BindingBase binding)
			where T : BindableObject
		{
			if (o == null)
				throw new ArgumentNullException (nameof (o));

			o.SetBinding (targetProperty, binding);
			return o;
		}

		public static T Bind<T> (this T o, BindableProperty targetProperty, string binding)
			where T : BindableObject
		{
			if (o == null)
				throw new ArgumentNullException (nameof (o));

			o.SetBinding (targetProperty, binding);
			return o;
		}

		public static T WithGridRow<T> (this T o, int row)
			where T : BindableObject
		{
			if (o == null)
				throw new ArgumentNullException (nameof (o));

			Grid.SetRow (o, row);
			return o;
		}

		public static T WithGridColumn<T> (this T o, int column)
			where T : BindableObject
		{
			if (o == null)
				throw new ArgumentNullException (nameof (o));

			Grid.SetColumn (o, column);
			return o;
		}

		public static T WithGridRowSpan<T> (this T o, int rowSpan)
			where T : BindableObject
		{
			if (o == null)
				throw new ArgumentNullException (nameof (o));

			Grid.SetRowSpan (o, rowSpan);
			return o;
		}

		public static T WithGridColumnSpan<T> (this T o, int columnSpan)
			where T : BindableObject
		{
			if (o == null)
				throw new ArgumentNullException (nameof (o));

			Grid.SetColumnSpan (o, columnSpan);
			return o;
		}
	}
}

