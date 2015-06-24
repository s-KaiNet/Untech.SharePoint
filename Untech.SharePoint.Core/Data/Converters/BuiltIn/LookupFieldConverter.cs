﻿using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.SharePoint;
using Untech.SharePoint.Core.Models;

namespace Untech.SharePoint.Core.Data.Converters.BuiltIn
{
	[SPFieldConverter("Lookup")]
	[SPFieldConverter("LookupMulti")]
	internal class LookupFieldConverter : IFieldConverter
	{
		public SPFieldLookup Field { get; set; }
		public Type PropertyType { get; set; }

		public void Initialize(SPField field, Type propertyType)
		{
			if (field == null) throw new ArgumentNullException("field");
			if (propertyType == null) throw new ArgumentNullException("propertyType");

			Field = field as SPFieldLookup;
			if (Field == null)
				throw new ArgumentException("SPFieldLookup only supported");

			if (Field.AllowMultipleValues)
			{
				if (!propertyType.IsAssignableFrom(typeof(List<ObjectReference>)) && propertyType != typeof(ObjectReference[]))
					throw new ArgumentException(
						"This converter can be used only with ObjectReference[] or with types assignable from List<ObjectReference>");
			}
			else
			{
				if (propertyType != typeof(ObjectReference))
					throw new ArgumentException("This converter can be used only with ObjectReference");
			}

			PropertyType = propertyType;
		}

	    public object FromSpValue(object value)
		{
			if (value == null||string.IsNullOrEmpty(Field.LookupList))
				return null;

			if (!Field.AllowMultipleValues)
			{
				var fieldValue = new SPFieldLookupValue(value.ToString());

				return new ObjectReference(new Guid(Field.LookupList), fieldValue.LookupId, fieldValue.LookupValue);
			}

			var fieldValues = new SPFieldLookupValueCollection(value.ToString());

			return fieldValues.Select(fieldValue => new ObjectReference(new Guid(Field.LookupList), fieldValue.LookupId, fieldValue.LookupValue)).ToList();
		}

		public object ToSpValue(object value)
		{
			if (value == null)
				return null;
			
			if (!Field.AllowMultipleValues)
			{
				var reference = (ObjectReference)value;

				return new SPFieldLookupValue(reference.Id, reference.Value);
			}

			var references = (IEnumerable<ObjectReference>)value;

			var fieldValues = new SPFieldLookupValueCollection();
			fieldValues.AddRange(references.Select(referenceInfo => new SPFieldLookupValue(referenceInfo.Id, referenceInfo.Value)));

			return fieldValues;
		}
	}
}