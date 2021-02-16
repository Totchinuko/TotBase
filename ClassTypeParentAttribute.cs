using System;
using System.Collections.Generic;


namespace TotBase
{
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
    public class ClassTypeParentAttribute : Attribute
    {
        private Type type;

        public Type Type => type;

        public ClassTypeParentAttribute(Type parent)
        {
            type = parent;
        }
    }

}
