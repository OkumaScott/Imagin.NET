﻿using Imagin.Common.Attributes;
using System;
using System.ComponentModel;
using System.Xml.Serialization;

namespace Imagin.Common
{
    [Serializable]
    ///<summary>
    /// A named object with a date that periodically notifies.
    /// </summary>
    public class NamedEntry : Entry, INamable
    {
        #region Properties

        protected string name = string.Empty;
        [Category("General")]
        [Description("The name of the object.")]
        [Featured(true)]
        public virtual string Name
        {
            get
            {
                return name;
            }
            set
            {
                name = value;
                OnPropertyChanged("Name");
            }
        }

        #endregion

        #region Methods

        public override string ToString()
        {
            return this.Name;
        }

        #endregion

        #region NamedEntry

        public NamedEntry() : base()
        {
        }

        public NamedEntry(string Name, int NotifyEvery = 1000) : base(NotifyEvery)
        {
            this.Name = Name;
        }

        public NamedEntry(string Name, DateTime Date, int NotifyEvery = 1000) : base(Date, NotifyEvery)
        {
            this.Name = Name;
        }

        public NamedEntry(DateTime Date, int NotifyEvery = 1000) : base(Date, NotifyEvery)
        {
        }
        
        #endregion
    }
}