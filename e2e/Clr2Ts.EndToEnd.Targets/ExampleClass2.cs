﻿namespace Clr2Ts.EndToEnd.Targets
{
    /// <summary>
    /// Simple example class type.
    /// </summary>
    public class ExampleClass2
    {
        /// <summary>
        /// Simple string property.
        /// </summary>
        public string SomeProperty { get; set; }

        /// <summary>
        /// Example for a dependency on another type.
        /// </summary>
        public ExampleClass1 DependencyOtherType { get; set; }

        /// <summary>
        /// Example for a dependency on the type itself.
        /// </summary>
        public ExampleClass2 DependencySelfRecursive { get; set; }
    }
}