using System;

namespace SerializeReferenceEditor
{
    [AttributeUsage(AttributeTargets.Class)]
    public class SerializeReferenceNameAttribute : Attribute
    {
        public readonly string FullName;
        public readonly string Name;

        public SerializeReferenceNameAttribute(string fullName)
        {
            FullName = fullName;
            if (!fullName.Contains("/"))
            {
                Name = fullName;
                return;
            }

            var separateName = fullName.Split('/');
            Name = separateName[^1];
        }
    }
}