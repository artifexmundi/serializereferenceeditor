﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace SerializeReferenceEditor
{
	[AttributeUsage(AttributeTargets.Field, Inherited = true, AllowMultiple = false)]
	public class SerializeReferenceEditorAttribute : PropertyAttribute
	{
		private static Dictionary<Type, Type[]> _typeCache = new Dictionary<Type, Type[]>();

		public class TypeInfo
		{
			public Type Type;
			public string Path;
		}

		public TypeInfo[] Types { get; private set; }

		public SerializeReferenceEditorAttribute()
		{
			Types = null;
		}

		public SerializeReferenceEditorAttribute(Type baseType)
		{
			if(baseType == null)
			{
				Debug.LogError("[SerializeReferenceEditorAttribute] Incorrect type.");
			}

			Types = GetTypeInfos(GetChildTypes(baseType));
		}

		public SerializeReferenceEditorAttribute(params Type[] types)
		{
			if(types == null || types.Length <= 0)
			{
				Debug.LogError("[SerializeReferenceEditorAttribute] Incorrect types.");
			}

			Types = GetTypeInfos(types);
		}

		public void SetTypeByName(string typeName)
		{
			if(string.IsNullOrEmpty(typeName))
			{
				Debug.LogError("[SerializeReferenceEditorAttribute] Incorrect type name.");
			}
			var type = GetTypeByName(typeName);
			if(type == null)
			{
				Debug.LogError("[SerializeReferenceEditorAttribute] Incorrect type.");
			}

			Types = GetTypeInfos(GetChildTypes(type));
		}

		public TypeInfo TypeInfoByPath(string path)
		{
			return Types != null ? Array.Find(Types, p => p.Path == path) : null;
		}

		public static TypeInfo[] GetTypeInfos(Type[] types)
		{
			if(types == null)
				return null;

			TypeInfo[] result = new TypeInfo[types.Length];
			for(int i = 0; i < types.Length; ++i)
			{
				var typeName = types[i].FullName;
				var nameAttribute = types[i].GetCustomAttributes(typeof(SerializeReferenceNameAttribute), false)
					.Select(attr=> attr as SerializeReferenceNameAttribute)
					.FirstOrDefault();
			
				if (nameAttribute != null) 
					typeName = nameAttribute.FullName;
			
				result[i] = new TypeInfo { 
					Type = types[i], 
					Path = typeName };
			}

			return result;
		}

		public static Type[] GetChildTypes(Type type)
		{
			Type[] result;
			if(_typeCache.TryGetValue(type, out result))
				return result;

			if (type.IsInterface)
				result = AppDomain.CurrentDomain.GetAssemblies().SelectMany(s => s.GetTypes())
					.Where(p => p != type && type.IsAssignableFrom(p)).ToArray();
			else
				result = Assembly.GetAssembly(type).GetTypes().Where(t => t.IsSubclassOf(type)).ToArray();

			if(result != null)
				_typeCache[type] = result;

			return result;
		}

		public static Type GetTypeByName(string typeName)
		{
			if(string.IsNullOrEmpty(typeName))
				return null;

			var typeSplit = typeName.Split(char.Parse(" "));
			var typeAssembly = typeSplit[0];
			var typeClass = typeSplit[1];

			return Type.GetType(typeClass + ", " + typeAssembly);
		}

		public virtual void OnCreate(object instance)
		{

		}
	}
}
