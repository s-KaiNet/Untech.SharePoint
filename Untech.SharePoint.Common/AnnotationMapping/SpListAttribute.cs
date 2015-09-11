﻿using System;

namespace Untech.SharePoint.Common.AnnotationMapping
{
	[AttributeUsage(AttributeTargets.Property)]
	public sealed class SpListAttribute : Attribute
	{
		public string Title { get; set; }
	}
}
