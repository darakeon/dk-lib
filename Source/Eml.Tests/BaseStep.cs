using System;
using System.Collections.Generic;
using TechTalk.SpecFlow;

namespace Eml.Tests
{
	public class BaseStep
	{
		protected readonly ScenarioContext context;

		public BaseStep(ScenarioContext context)
		{
			this.context = context;
		}

		protected Boolean exists(String key)
		{
			return context.ContainsKey(key);
		}

		protected T get<T>(String key)
		{
			return exists(key)
				? (T)context[key]
				: default;
		}

		protected void set(String key, Object value)
		{
			context[key] = value;
		}

		[StepArgumentTransformation(@"(\d{4}-\d{2}-\d{2} \d{2}:\d{2}:\d{2}|null)")]
		public DateTime? NullDateTime(String date)
		{
			if (date == "null")
				return null;
			else
				return DateTime.Parse(date);
		}
	}
}
