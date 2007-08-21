//===============================================================================
// Microsoft patterns & practices
// CompositeUI Application Block
//===============================================================================
// Copyright � Microsoft Corporation.  All rights reserved.
// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY
// OF ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT
// LIMITED TO THE IMPLIED WARRANTIES OF MERCHANTABILITY AND
// FITNESS FOR A PARTICULAR PURPOSE.
//===============================================================================

using System;
using System.Drawing;
using System.Windows.Forms;
using Microsoft.Practices.CompositeUI;
using Microsoft.Practices.CompositeUI.Commands;
using Microsoft.Practices.CompositeUI.SmartParts;
using Microsoft.Practices.CompositeUI.UIElements;
using Microsoft.Practices.CompositeUI.Utility;
using Microsoft.Practices.CompositeUI.WinForms;
using BankTellerCommon;
using Microsoft.Practices.CompositeUI.EventBroker;
using DevExpress.XtraBars;
using DevExpress.CompositeUI.SmartPartInfos;

namespace BankTellerModule
{
	// The CustomerWorkItem represents the use case of editing a customer's data.
	// It contains the views necessary to edit a single customer. When it's time
	// to edit a customer, the work item's parent calls Run and passes the
	// workspace where the editing will take place.

	public class CustomerWorkItem : WorkItem
	{
		public static readonly string CUSTOMERDETAIL_TABWORKSPACE = "tabbedWorkspace1";

		private BarItem editCustomerMenuItem1;
		private CustomerSummaryView customerSummaryView;
		private CustomerCommentsView commentsView;
		private BarStaticItem addressLabel;

		// This event is published to indicate that the module would like to
		// "update status". The only subscriber to this event today is the shell
		// which updates the status bar. These two components don't know anything
		// about one another, because they communicate indirectly via the
		// EventBroker. In reality, you can have any number of publishers and
		// any number of subscribers; in fact, other modules will undoubtedly
		// also publish status update events.
		[EventPublication("topic://BankShell/statusupdate", PublicationScope.Global)]
		public event EventHandler<DataEventArgs<string>> UpdateStatusTextEvent;

		public void Show(IWorkspace parentWorkspace)
		{
			customerSummaryView = customerSummaryView ?? Items.AddNew<CustomerSummaryView>();

			AddMenuItems();

			Customer customer = (Customer)State[StateConstants.CUSTOMER];
			OnStatusTextUpdate(string.Format("Editing {0}, {1}", customer.LastName, customer.FirstName));
            WindowSmartPartInfo smartPartInfo = new WindowSmartPartInfo();
		    smartPartInfo.Title = customer.LastName + " " + customer.FirstName;
            customerSummaryView.Dock = DockStyle.Fill;
		    parentWorkspace.Show(customerSummaryView, smartPartInfo);

			UpdateUserAddressLabel(customer);

			this.Activate();

			// When activating, force focus on the first tab in the view.
			// Extensions may have added stuff at the end of the tab.
			customerSummaryView.FocusFirstTab();
		}

		private void UpdateUserAddressLabel(Customer customer)
		{
			if (addressLabel == null)
			{
				addressLabel = new BarStaticItem();
				UIExtensionSites[UIExtensionConstants.MAINSTATUS].Add(addressLabel);
				addressLabel.Caption = customer.Address1;
			}
		}

		private void AddMenuItems()
		{
			if (editCustomerMenuItem1 == null)
			{
                editCustomerMenuItem1 = new BarButtonItem();
                editCustomerMenuItem1.Caption = "Edit";
                UIExtensionSites[Properties.Resources.CustomerMenuExtensionSite].Add(editCustomerMenuItem1);
			}
		}

		private void SetUIElementVisibility(bool visible)
		{
			if (editCustomerMenuItem1 != null)
				editCustomerMenuItem1.Visibility = visible ? BarItemVisibility.Always : BarItemVisibility.Never;

			if (addressLabel != null)
				addressLabel.Visibility = visible ? BarItemVisibility.Always : BarItemVisibility.Never;
		}

		// We watch for when we are activated (i.e., shown to
		// be worked on), we want to fire a status update event and show ourselves
		// in the provided workspace.
		protected override void OnActivated()
		{
			base.OnActivated();

			SetUIElementVisibility(true);
		}

		protected override void OnDeactivated()
		{
			base.OnDeactivated();

			SetUIElementVisibility(false);
		}

		protected virtual void OnStatusTextUpdate(string newText)
		{
			if (UpdateStatusTextEvent != null)
			{
				UpdateStatusTextEvent(this, new DataEventArgs<string>(newText));
			}
		}

		// This is called by CustomerDetailController when the user has indicated
		// that they want to show the comments. We dynamically create the comments
		// view and add it to our tab workspace.
		public void ShowCustomerComments()
		{
			CreateCommentsView();

			IWorkspace ws = Workspaces[CUSTOMERDETAIL_TABWORKSPACE];
			if (ws != null)
			{
				ws.Show(commentsView);
			}
		}

		private void CreateCommentsView()
		{
			commentsView = commentsView ?? Items.AddNew<CustomerCommentsView>();
			XtraTabSmartPartInfo info = new XtraTabSmartPartInfo();
			info.Title = "Title";
			info.Text = "Comments";
			info.PageHeaderFont = new Font("Comic Sans MS", 11.25F, FontStyle.Regular); //CABDevExpress added property
			RegisterSmartPartInfo(commentsView, info);
		}

		[CommandHandler(CommandConstants.CUSTOMER_MOUSEOVER)]
		public void OnCustomerEdit(object sender, EventArgs args)
		{
			if (Status == WorkItemStatus.Active)
			{
				Form form = customerSummaryView.ParentForm;

				string tooltipText = "This is customer work item " + this.ID;
				ToolTip toolTip = new ToolTip();
				toolTip.IsBalloon = true;
				toolTip.Show(tooltipText, form, form.Size.Width - 30, 30, 3000);
			}
		}

	}
}