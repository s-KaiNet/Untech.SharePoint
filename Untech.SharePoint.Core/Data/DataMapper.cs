﻿using System;
using Microsoft.SharePoint;
using Untech.SharePoint.Core.Data.Converters;
using Untech.SharePoint.Core.Reflection;

namespace Untech.SharePoint.Core.Data
{
	internal class DataMapper
	{
		private readonly PropertyAccessor _propertyAccessor = new PropertyAccessor();
		private readonly PropertyMappings _propertyMappings = new PropertyMappings();

		public void Initialize(Type objectType)
		{
			_propertyAccessor.Initialize(objectType);
			_propertyMappings.Initialize(objectType);
		}

		public void Map(SPListItem sourceItem, object destItem)
		{
			var fields = sourceItem.Fields;
			foreach (var mappingInfo in _propertyMappings)
			{
				var field = fields.GetField(mappingInfo.SPFieldInternalName);
				MapProperty(sourceItem, destItem, mappingInfo, field);
			}
		}

		public void Map(object sourceItem, SPListItem destItem)
		{
			var fields = destItem.Fields;
			foreach (var mappingInfo in _propertyMappings)
			{
				var field = fields.GetField(mappingInfo.SPFieldInternalName);
				MapProperty(sourceItem, destItem, mappingInfo, field);
			}
		}

		private IFieldConverter GetConverter(PropertyMappingInfo mappingInfo, SPField field)
		{
			return mappingInfo.CustomConverterType != null ?
				FieldConverterResolver.Instance.Get(mappingInfo.CustomConverterType) :
				FieldConverterResolver.Instance.Get(field.TypeAsString);
		}

		private void MapProperty(SPListItem sourceItem, object destItem, PropertyMappingInfo mappingInfo, SPField field)
		{
			var converter = GetConverter(mappingInfo, field);

			var spValue = sourceItem[field.Id];
			var propValue = converter.FromSpValue(spValue, field, mappingInfo.PropertyOrFieldType);

			_propertyAccessor[destItem, mappingInfo.PropertyOrFieldName] = propValue;
		}

		private void MapProperty(object sourceItem, SPListItem destItem, PropertyMappingInfo mappingInfo, SPField field)
		{
			if (field.ReadOnlyField)
			{
				return;
			}

			var converter = GetConverter(mappingInfo, field);

			var propValue = _propertyAccessor[sourceItem, mappingInfo.PropertyOrFieldName];
			var spValue = converter.ToSpValue(propValue, field, mappingInfo.PropertyOrFieldType);

			destItem[field.Id] = spValue;
		}
	}
}
