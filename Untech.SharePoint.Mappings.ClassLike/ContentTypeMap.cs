﻿using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Untech.SharePoint.Common.Collections;
using Untech.SharePoint.Common.MetaModels;
using Untech.SharePoint.Common.MetaModels.Providers;

namespace Untech.SharePoint.Mappings.ClassLike
{
	public class ContentTypeMap<TEntity> : IMetaContentTypeProvider
	{
		private readonly Container<MemberInfo, FieldPart> _fieldParts;

		public ContentTypeMap()
		{
			_fieldParts = new Container<MemberInfo, FieldPart>();
		} 

		public FieldPart Field<T>(Expression<Func<TEntity, T>> property)
		{
			var member = ((MemberExpression) property.Body).Member;

			_fieldParts.Register(member, new FieldPart(member));

			return _fieldParts.Resolve(member);
		}

		public MetaContentType GetMetaContentType(MetaList parent)
		{
			return new MetaContentType(parent, typeof(TEntity), _fieldParts.Select(n => n.Value).ToList());
		}
	}
}