﻿using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.SharePoint;

namespace Untech.SharePoint.Core.Data.Converters.BuiltIn
{
	[SpFieldConverter("MultiChoice")]
	internal class MultiChoiceFieldConverter : IFieldConverter
	{
		public SPField Field { get; set; }
		public Type PropertyType { get; set; }

		public void Initialize(SPField field, Type propertyType)
		{
			Guard.ThrowIfArgumentNull(field, "field");
			Guard.ThrowIfArgumentNull(propertyType, "propertyType");

			Guard.ThrowIfArgumentNotIs<SPFieldMultiChoiceValue>(field.FieldValueType, "field.FieldValueType");

			Guard.ThrowIfArgumentNotArrayOrAssignableFromList<string>(propertyType, "propertType");

			
			PropertyType = propertyType;
		}

		public object FromSpValue(object value)
		{
			if (value == null)
				return null;

			var values = GetValues(new SPFieldMultiChoiceValue(value.ToString()));
			return PropertyType == typeof (string[]) ? values : values.ToList();
		}

		public object ToSpValue(object value)
		{
			if (value == null)
				return null;

			var strings = ((IEnumerable<string>) value).ToList();

			var fieldValues = new SPFieldMultiChoiceValue();
			
			strings.ForEach(s => fieldValues.Add(s));
			
			return fieldValues;
		}

		private IEnumerable<string> GetValues(SPFieldMultiChoiceValue value)
		{
			var strings = new string[value.Count];
			for (int i = 0; i < value.Count; i++)
			{
				strings[i] = value[i];
			}
			return strings;
		}

	}
}
