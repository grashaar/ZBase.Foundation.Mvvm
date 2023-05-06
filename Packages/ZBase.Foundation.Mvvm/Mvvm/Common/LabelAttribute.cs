﻿using System;

namespace ZBase.Foundation.Mvvm
{
    /// <summary>
    /// Provides label text that will be shown on the Unity Inspector.
    /// </summary>
    [AttributeUsage(AttributeTargets.All, AllowMultiple = false, Inherited = false)]
    public sealed class LabelAttribute : Attribute
    {
        public string Label { get; }

        public string Directory { get; }

        public LabelAttribute(string label)
        {
            this.Label = label;
            this.Directory = "";
        }

        public LabelAttribute(string label, string directory)
        {
            this.Label = label;
            this.Directory = directory;
        }
    }
}
