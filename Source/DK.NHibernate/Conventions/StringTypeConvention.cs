using System;
using FluentNHibernate.Conventions;
using FluentNHibernate.Conventions.Instances;

namespace DK.NHibernate.Conventions
{
    /// <summary>
    /// Convention for String Types
    /// </summary>
    public class StringTypeConvention : IPropertyConvention
    {
	    /// <summary>
	    /// Apply changes to the target
	    /// </summary>
	    public void Apply(IPropertyInstance instance)
        {
            if (instance.Type.GetUnderlyingSystemType() == typeof(String))
                instance.CustomType("AnsiString");
        }
    }
}
