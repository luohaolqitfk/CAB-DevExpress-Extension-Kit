using System;
using CABDevExpress.Adapters;
using DevExpress.XtraEditors;
using DevExpress.XtraNavBar;
using Microsoft.Practices.CompositeUI.UIElements;
using Microsoft.Practices.CompositeUI.Utility;

namespace CABDevExpress.Adapters
{
	/// <summary>
	/// An adapter that wraps a <see cref="NavItemCollection"/> for use as an <see cref="IUIElementAdapter"/>.
	/// </summary>
	public class NavigatorCustomButtonUIAdapter : UIElementAdapter<NavigatorCustomButton>
	{
		private readonly NavigatorCustomButtons buttonCollection;

		/// <summary>
		/// Initializes a new instance of the <see cref="NavBarItemCollectionUIAdapter"/> class.
		/// </summary>
		/// <param name="buttonCollection"></param>
		public NavigatorCustomButtonUIAdapter(NavigatorCustomButtons buttonCollection)
		{
			Guard.ArgumentNotNull(buttonCollection, "NavigatorCustomButtons");
			this.buttonCollection = buttonCollection;
		}

		/// <summary>
		/// See <see cref="UIElementAdapter{TUIElement}.Add(TUIElement)"/> for more information.
		/// </summary>
		protected override NavigatorCustomButton Add(NavigatorCustomButton uiElement)
		{
			buttonCollection.AddRange(new NavigatorCustomButton[] { uiElement });
			return uiElement;
		}

		/// <summary>
		/// See <see cref="UIElementAdapter{TUIElement}.Remove(TUIElement)"/> for more information.
		/// </summary>
		protected override void Remove(NavigatorCustomButton uiElement)
		{
			int index = -1;			//TODO I would like to test this
			foreach (object obj in buttonCollection)
			{
				index++;
				if (obj == uiElement)
					break;
			}

			if (index == -1)
				throw new InvalidOperationException("Cannot find NavigatorCustomButton to remove");

			buttonCollection.RemoveAt(index);
		}

		/// <summary>
		/// Returns the correct index for the item being added. By default,
		/// it will return the length of the buttonCollection.
		/// </summary>
		/// <param name="uiElement"></param>
		/// <returns></returns>
		protected virtual int GetInsertingIndex(object uiElement)
		{
			return buttonCollection.Count;
		}
	}
}