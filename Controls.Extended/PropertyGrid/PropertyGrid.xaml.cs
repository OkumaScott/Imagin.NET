﻿using Imagin.Common.Events;
using Imagin.Common.Extensions;
using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;

namespace Imagin.Controls.Extended
{
    public partial class PropertyGrid : UserControl
    {
        #region Properties

        #region Events

        public event EventHandler<ObjectEventArgs> SelectedObjectChanged;

        #endregion

        #region Dependency

        public static DependencyProperty ShowPrimaryProperty = DependencyProperty.Register("ShowPrimary", typeof(bool), typeof(PropertyGrid), new FrameworkPropertyMetadata(true, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));
        public bool ShowPrimary
        {
            get
            {
                return (bool)GetValue(ShowPrimaryProperty);
            }
            set
            {
                SetValue(ShowPrimaryProperty, value);
            }
        }

        public static DependencyProperty PrimaryItemProperty = DependencyProperty.Register("PrimaryItem", typeof(PropertyItem), typeof(PropertyGrid), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));
        public PropertyItem PrimaryItem
        {
            get
            {
                return (PropertyItem)GetValue(PrimaryItemProperty);
            }
            private set
            {
                SetValue(PrimaryItemProperty, value);
            }
        }

        public static DependencyProperty SearchQueryProperty = DependencyProperty.Register("SearchQuery", typeof(string), typeof(PropertyGrid), new FrameworkPropertyMetadata(string.Empty, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));
        public string SearchQuery
        {
            get
            {
                return (string)GetValue(SearchQueryProperty);
            }
            set
            {
                SetValue(SearchQueryProperty, value);
            }
        }

        public static DependencyProperty PropertiesProperty = DependencyProperty.Register("Properties", typeof(PropertyItemCollection), typeof(PropertyGrid), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));
        public PropertyItemCollection Properties
        {
            get
            {
                return (PropertyItemCollection)GetValue(PropertiesProperty);
            }
            set
            {
                SetValue(PropertiesProperty, value);
            }
        }

        public static DependencyProperty ListCollectionViewProperty = DependencyProperty.Register("ListCollectionView", typeof(ListCollectionView), typeof(PropertyGrid), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));
        public ListCollectionView ListCollectionView
        {
            get
            {
                return (ListCollectionView)GetValue(ListCollectionViewProperty);
            }
            set
            {
                SetValue(ListCollectionViewProperty, value);
            }
        }

        public static DependencyProperty SelectedObjectProperty = DependencyProperty.Register("SelectedObject", typeof(object), typeof(PropertyGrid), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnSelectedObjectChanged));
        public object SelectedObject
        {
            get
            {
                return (object)GetValue(SelectedObjectProperty);
            }
            set
            {
                SetValue(SelectedObjectProperty, value);
            }
        }
        private static void OnSelectedObjectChanged(DependencyObject Object, DependencyPropertyChangedEventArgs e)
        {
            PropertyGrid PropertyGrid = (PropertyGrid)Object;
            PropertyGrid.OnSelectedObjectChanged();
        }

        public static DependencyProperty ShowHeaderProperty = DependencyProperty.Register("ShowHeader", typeof(bool), typeof(PropertyGrid), new FrameworkPropertyMetadata(true, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));
        public bool ShowHeader
        {
            get
            {
                return (bool)GetValue(ShowHeaderProperty);
            }
            set
            {
                SetValue(ShowHeaderProperty, value);
            }
        }

        public static DependencyProperty ExportTypesProperty = DependencyProperty.Register("ExportTypes", typeof(Type[]), typeof(PropertyGrid), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));
        public Type[] ExportTypes
        {
            get
            {
                return (Type[])GetValue(ExportTypesProperty);
            }
            set
            {
                SetValue(ExportTypesProperty, value);
            }
        }

        public static DependencyProperty IsSortAscendingProperty = DependencyProperty.Register("IsSortAscending", typeof(bool), typeof(PropertyGrid), new FrameworkPropertyMetadata(true, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnIsSortAscendingChanged));
        public bool IsSortAscending
        {
            get
            {
                return (bool)GetValue(IsSortAscendingProperty);
            }
            set
            {
                SetValue(IsSortAscendingProperty, value);
            }
        }
        private static void OnIsSortAscendingChanged(DependencyObject Object, DependencyPropertyChangedEventArgs e)
        {
            PropertyGrid PropertyGrid = (PropertyGrid)Object;
            if (PropertyGrid.IsSortAscending) PropertyGrid.Sort(ListSortDirection.Ascending);
        }

        public static DependencyProperty IsSortDescendingProperty = DependencyProperty.Register("IsSortDescending", typeof(bool), typeof(PropertyGrid), new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnIsSortDescendingChanged));
        public bool IsSortDescending
        {
            get
            {
                return (bool)GetValue(IsSortDescendingProperty);
            }
            set
            {
                SetValue(IsSortDescendingProperty, value);
            }
        }
        private static void OnIsSortDescendingChanged(DependencyObject Object, DependencyPropertyChangedEventArgs e)
        {
            PropertyGrid PropertyGrid = (PropertyGrid)Object;
            if (PropertyGrid.IsSortDescending)
                PropertyGrid.Sort(ListSortDirection.Descending);
        }

        public static DependencyProperty ShowCategoriesProperty = DependencyProperty.Register("ShowCategories", typeof(bool), typeof(PropertyGrid), new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnShowCategoriesChanged));
        public bool ShowCategories
        {
            get
            {
                return (bool)GetValue(ShowCategoriesProperty);
            }
            set
            {
                SetValue(ShowCategoriesProperty, value);
            }
        }
        private static void OnShowCategoriesChanged(DependencyObject Object, DependencyPropertyChangedEventArgs e)
        {
            PropertyGrid PropertyGrid = (PropertyGrid)Object;
            PropertyGrid.Group("Category");
        }

        #endregion

        #endregion

        #region PropertyGrid

        public PropertyGrid()
        {
            InitializeComponent();
            this.Properties = new PropertyItemCollection();
            this.ListCollectionView = new ListCollectionView(this.Properties);
        }

        #endregion

        #region Methods

        #region Private

        /// <summary>
        /// Performs non case-sensitive search. Camel-case version of property name is compared.
        /// </summary>
        void Search()
        {
            foreach (PropertyItem Item in this.Properties)
                Item.IsVisible = this.SearchQuery == string.Empty ? true : Item.Name.SplitCamelCase().ToLower().StartsWith(this.SearchQuery.ToLower()) ? true : false;
        }

        #endregion

        #region Virtual

        protected virtual void OnSelectedObjectChanged()
        {
            if (this.SelectedObjectChanged != null)
                this.SelectedObjectChanged(this, new ObjectEventArgs(this.SelectedObject));
            if (this.SelectedObject == null)
                return;
            this.Properties.Object = this.SelectedObject;
            this.Properties.Clear();
            this.SetObject();
        }

        /// <summary>
        /// Populate this.Properties with PropertyItems representing each member of the SelectedObject.
        /// </summary>
        protected virtual void SetObject()
        {
            this.Properties.FromObject(new Action<PropertyItem>((PrimaryItem) => this.PrimaryItem = PrimaryItem));
        }

        #endregion

        #region Public

        public void Group(string PropertyName)
        {
            if (this.ListCollectionView == null)
                return;
            this.ListCollectionView.GroupDescriptions.Clear();
            if (this.ShowCategories)
                this.ListCollectionView.GroupDescriptions.Add(new PropertyGroupDescription(PropertyName));
        }

        public void Sort(ListSortDirection Direction)
        {
            if (this.ListCollectionView == null)
                return;
            this.ListCollectionView.SortDescriptions.Clear();
            if (this.ShowCategories)
                this.ListCollectionView.SortDescriptions.Add(new SortDescription("Category", Direction));
            this.ListCollectionView.SortDescriptions.Add(new SortDescription("Name", Direction));
        }

        #endregion

        #region Events

        private void OnKeyUp(object sender, KeyEventArgs e)
        {
            this.Search();
        }

        private void OnGotKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            (sender as TextBox).SelectAll();
        }

        private void OnPreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            TextBox t = (sender as TextBox);
            if (t.IsKeyboardFocusWithin)
                return;
            e.Handled = true;
            t.Focus();
        }

        #endregion

        #region Commands

        private void Clear_Executed(object sender, RoutedEventArgs e)
        {
            this.SearchQuery = string.Empty;
            this.Search();
        }

        private void Reset_Executed(object sender, RoutedEventArgs e)
        {
            foreach (PropertyItem Item in this.Properties)
                Item.Value = null;
        }

        #endregion

        #endregion
    }
}