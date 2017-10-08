using Ploeh.AutoFixture;
using Ploeh.AutoFixture.AutoNSubstitute;
using Ploeh.AutoFixture.Kernel;
using Ploeh.AutoFixture.Xunit2;
using System.Reflection;


namespace Xunit
{
	/// <summary>
	/// Use this attribute when you want AutoFixture to populate your values and 
	/// NSubstitute to create mocks for you.  But AutoFixture does not populate values in the Mocks
	/// </summary>
	/// <example>
	/// In the code below, auditor will have values created,
	/// IAuditInfo will be an empty mock, so the only value that will be valid
	/// would be the LoggedInUser since it's being explicitly set.
	/// 
	/// [Theory, AutoData_NSubstitute]
	/// public void Sample(Auditor auditor, IAuditInfo info)
	/// {
	///	 //This is auto generated.
	///  Assert.NotNull(info);

	///	 //No value assigned to the members of mocked objects
	///	 Assert.True(info.ActingUser == "");
	/// }
	/// </example>
	internal class AutoData_NSubstitute_NoPropAttribute : AutoDataAttribute
	{
		public AutoData_NSubstitute_NoPropAttribute()
			: base(new Fixture().Customize(new AutoNSubstituteCustomization()))
		{
		}
	}

	/// <summary>
	/// Use this attribute when you want AutoFixture to populate values for both
	/// concrete classes and mocks created with NSubstitute
	/// 
	/// </summary>
	/// <example>
	/// In the code below, auditor will have values created,
	/// IAuditInfo will also have dummy values for every field, 
	/// LoggedinUser will have the empty string value.
	/// 
	/// [Theory, AutoData_NSubstitute_Plus]
	/// public void Sample2(Auditor auditor, IAuditInfo info)
	/// {
	/// 	//This is auto generated.
	/// 	Assert.NotNull(info);
	///
	/// 	//Members of mocked objects have values.
	/// 	Assert.NotNull(info.ActingUser);
	/// 	Assert.True(info.ActingUser != "");
	/// }
	/// </example>
	internal class AutoData_NSubstituteAttribute : AutoDataAttribute
	{
		public AutoData_NSubstituteAttribute()
			: base(new Fixture().Customize(new AutoPopulatedNSubstitutePropertiesCustomization()))
		{
		}
	}

	/// <summary>
	/// In-line version of AutoData_NSubstitute_NoProp
	/// </summary>
	internal class Inline_AutoData_NSubstitute_NoPropAttribute : CompositeDataAttribute
	{
		public Inline_AutoData_NSubstitute_NoPropAttribute(params object[] values)
			: base(
				new InlineDataAttribute(values), 
				new AutoData_NSubstitute_NoPropAttribute())
		{
		}
	}

	/// <summary>
	/// In-line version of AutoData_NSubstitute
	/// </summary>
	internal class Inline_AutoData_NSubstituteAttribute : CompositeDataAttribute
	{
		public Inline_AutoData_NSubstituteAttribute(params object[] values)
			: base(
				new InlineDataAttribute(values), 
				new AutoData_NSubstituteAttribute())
		{
		}
	}

	internal class AutoNSubstitutePropertyDataAttribute : CompositeDataAttribute
	{
		public AutoNSubstitutePropertyDataAttribute(string propertyName)
			: base(
				new MemberDataAttribute(propertyName),
				new AutoData_NSubstitute_NoPropAttribute())
		{
		}
	}

	 ////<summary>
	 ///Not for direct use.
	 ///</summary>
	internal class AutoPopulatedNSubstitutePropertiesCustomization : ICustomization
	{
		public void Customize(IFixture fixture)
		{
			fixture.ResidueCollectors.Add(
				new Postprocessor(
					new NSubstituteBuilder(
						new MethodInvoker(
							new NSubstituteMethodQuery())),
					new AutoPropertiesCommand(
						new PropertiesOnlySpecification())));
		}

		private class PropertiesOnlySpecification : IRequestSpecification
		{
			public bool IsSatisfiedBy(object request)
			{
				return request is PropertyInfo;
			}
		}
	}

}
