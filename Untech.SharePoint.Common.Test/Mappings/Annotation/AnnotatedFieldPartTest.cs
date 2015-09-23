using Microsoft.VisualStudio.TestTools.UnitTesting;
using Untech.SharePoint.Common.Mappings.Annotation;
using Untech.SharePoint.Common.MetaModels;
using Untech.SharePoint.Common.Test.Mappings.Annotation.Models;

namespace Untech.SharePoint.Common.Test.Mappings.Annotation
{
	[TestClass]
	public class AnnotatedFieldPartTest 
	{
		[TestMethod]
		public void CanInheritFieldAnnotation()
		{
			var model = GetCtx<AnnotatedContext>();

			Assert.AreEqual("OldInternalName", GetField<DerivedAnnotatedEntityWithIheritedAnnotation>(model, "List1", "OverrideProperty").InternalName);
		}

		[TestMethod]
		public void CanOverwriteFieldAnnotation()
		{
			var model = new AnnotatedContextMapping<AnnotatedContext>().GetMetaContext();

			Assert.AreEqual("NewInternalName", GetField<DerivedAnnotatedEntityWithOverwrittenAnnotation>(model, "List2", "OverrideProperty").InternalName);
		}

		[TestMethod]
		public void ThrowErrorForReadOnlyEntityField()
		{
			CustomAssert.Throw<InvalidAnnotationException>(() =>
			{
				var model = GetCtx<ContextWithReadOnlyEntityField>();
			});
		}

		[TestMethod]
		public void ThrowErrorForEntityIndexer()
		{
			CustomAssert.Throw<InvalidAnnotationException>(() =>
			{
				var model = GetCtx<ContextWithEntityIndexer>();
			});
		}

		[TestMethod]
		public void ThrowErrorForReadOnlyEntityProperty()
		{
			CustomAssert.Throw<InvalidAnnotationException>(() =>
			{
				var model = GetCtx<ContextWithReadOnlyEntityProperty>();
			});
		}

		private MetaContext GetCtx<T>()
		{
			return new AnnotatedContextMapping<T>().GetMetaContext();
		}

		private MetaField GetField<T>(MetaContext context, string list, string field)
		{
			return context.Lists[list].ContentTypes[typeof(T)].Fields[field];
		}
	}
}