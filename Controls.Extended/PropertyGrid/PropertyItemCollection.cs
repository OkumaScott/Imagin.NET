﻿using Imagin.Common.Attributes;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Media;
using Imagin.Common.Collections;

namespace Imagin.Controls.Extended
{
    public sealed class PropertyItemCollection : ConcurrentObservableCollection<PropertyItem>
    {
        /// <summary>
        /// The object to evaluate.
        /// </summary>
        public object Object = null;

        /// <summary>
        /// Creates a PropertyItem based on given arguments.
        /// </summary>
        PropertyItem GetPropertyItem(object Object, PropertyInfo Property, Dictionary<string, object> Attributes)
        {
            PropertyItem PropertyItem = null;
            string Name = Property.Name;
            object Value = Property.GetValue(Object);
            string Category = (string)Attributes["Category"];
            bool IsDisabled = (bool)Attributes["IsReadOnly"];
            bool IsPrimary = (bool)Attributes["IsPrimary"];
            if (Property.PropertyType == typeof(string))
            {
                if ((bool)Attributes["IsPassword"])
                    PropertyItem = new PasswordPropertyItem(Object, Name, Value, Category, IsDisabled, IsPrimary);
                else if ((bool)Attributes["IsFile"])
                    PropertyItem = new FileSystemObjectPropertyItem(Object, Name, Value, Category, IsDisabled, IsPrimary);
                else if ((bool)Attributes["IsMultiLine"])
                    PropertyItem = new MultiLinePropertyItem(Object, Name, Value, Category, IsDisabled, IsPrimary);
                else
                    PropertyItem = new StringPropertyItem(Object, Name, Value, Category, IsDisabled, IsPrimary);
            }
            else if (Property.PropertyType == typeof(int))
                PropertyItem = new IntPropertyItem(Object, Name, Value, Category, IsDisabled, IsPrimary);
            else if (Property.PropertyType == typeof(double))
                PropertyItem = new DoublePropertyItem(Object, Name, Value, Category, IsDisabled, IsPrimary);
            else if (Property.PropertyType.IsEnum)
                PropertyItem = new EnumPropertyItem(Object, Property, Name, Value, Category, IsDisabled, IsPrimary);
            else if (Property.PropertyType == typeof(bool))
                PropertyItem = new BoolPropertyItem(Object, Name, Value, Category, IsDisabled, IsPrimary);
            else if (Property.PropertyType == typeof(DateTime))
                PropertyItem = new DateTimePropertyItem(Object, Name, Value, Category, IsDisabled, IsPrimary);
            else if (Property.PropertyType == typeof(long))
                PropertyItem = new LongPropertyItem(Object, Name, Value, Category, IsDisabled, IsPrimary);
            return PropertyItem;
        }

        /// <summary>
        /// Sets this by enumerating a resource dictionary.
        /// </summary>
        /// <param name="Dictionary">The dictionary to enumerate.</param>
        /// <param name="Callback">What to do afterwards.</param>
        public void FromResourceDictionary(ResourceDictionary Dictionary, Action Callback = null)
        {
            BackgroundWorker Worker = new BackgroundWorker();
            Worker.DoWork += (s, e) =>
            {
            };
            Worker.RunWorkerCompleted += (s, e) =>
            {
                if (Callback != null)
                    Callback.Invoke();
            };
            Worker.RunWorkerAsync(Dictionary);
            this.Clear();
            ResourceDictionary r = Dictionary;
            foreach (DictionaryEntry Entry in r)
            {
                if (Entry.Value is LinearGradientBrush)
                    this.Add(new LinearGradientPropertyItem(null, Entry.Key.ToString(), Entry.Value, "Gradients", false));
                else if (Entry.Value is SolidColorBrush)
                    this.Add(new SolidColorPropertyItem(null, Entry.Key.ToString(), Entry.Value, "Brushes", false));
            }
        }

        /// <summary>
        /// Sets this by enumerating the properties of an object.
        /// </summary>
        /// <param name="Callback">What to do afterwards.</param>
        public void FromObject(Action<PropertyItem> Callback = null)
        {
            BackgroundWorker Worker = new BackgroundWorker();
            Worker.DoWork += (s, e) =>
            {
                this.Clear();

                PropertyInfo[] ObjectProperties = e.Argument.GetType().GetProperties();
                ObjectProperties = ObjectProperties.OrderBy(x => x.Name).ToArray();

                PropertyItem PrimaryItem = null;

                for (int i = 0, Length = ObjectProperties.Length; i < Length; i++)
                {
                    bool Skip = false;

                    PropertyInfo Property = ObjectProperties[i];
                    if (!(Property.CanWrite && Property.GetSetMethod(true).IsPublic))
                        continue;

                    Dictionary<string, object> FoundAttributes = new Dictionary<string, object>();
                    FoundAttributes.Add("IsHidden", false);
                    FoundAttributes.Add("Category", null);
                    FoundAttributes.Add("IsPassword", false);
                    FoundAttributes.Add("IsFile", false);
                    FoundAttributes.Add("IsPrimary", false);
                    FoundAttributes.Add("IsMultiLine", false);
                    FoundAttributes.Add("IsReadOnly", false);

                    object[] Attributes = Property.GetCustomAttributes(true);
                    foreach (object Attribute in Attributes)
                    {
                        if (Attribute is Hide)
                        {
                            bool IsHidden = (Attribute as Hide).Value;
                            if (IsHidden)
                            {
                                Skip = true;
                                break;
                            }
                            FoundAttributes["IsHidden"] = IsHidden;
                        }
                        if (Attribute is Category)
                            FoundAttributes["Category"] = (Attribute as Category).Name;
                        if (Attribute is Password)
                            FoundAttributes["IsPassword"] = (Attribute as Password).Value;
                        if (Attribute is File)
                            FoundAttributes["IsFile"] = (Attribute as File).Value;
                        if (Attribute is Primary)
                            FoundAttributes["IsPrimary"] = (Attribute as Primary).IsPrimary;
                        if (Attribute is MultiLine)
                            FoundAttributes["IsMultiLine"] = (Attribute as MultiLine).IsMultiLine;
                        if (Attribute is ReadOnlyAttribute)
                            FoundAttributes["IsReadOnly"] = (Attribute as ReadOnlyAttribute).IsReadOnly;
                    }
                    if (Skip)
                        continue;
                    PropertyItem PropertyItem = this.GetPropertyItem(e.Argument, Property, FoundAttributes);
                    if (PropertyItem != null)
                    {
                        if (PropertyItem.IsPrimary)
                            PrimaryItem = PropertyItem;
                        else
                            this.Add(PropertyItem);
                    }

                }
                e.Result = PrimaryItem;
            };
            Worker.RunWorkerCompleted += (s, e) =>
            {
                if (Callback != null)
                    Callback.Invoke(e.Result as PropertyItem);
            };
            Worker.RunWorkerAsync(this.Object);
        }

        public PropertyItemCollection() : base()
        {
        }
    }
}