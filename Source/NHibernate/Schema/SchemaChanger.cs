﻿using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Keon.Util.Exceptions;
using NHibernate.Cfg;
using NHibernate.Tool.hbm2ddl;

namespace Keon.NHibernate.Schema
{
	/// <summary>
	/// Make changes to database tables
	/// </summary>
	public class SchemaChanger
	{
		private readonly String scriptFileName;
		private readonly Action<String> logQueries;

		/// <summary>
		/// Construct class with the name of the script
		/// </summary>
		public SchemaChanger(String scriptFileName, Action<String> logQueries)
		{
			if (scriptFileName != null)
			{
				var addressPattern = new Regex(@"^(\\|[A-Z]\:).*");

				this.scriptFileName = addressPattern.IsMatch(scriptFileName)
					? scriptFileName
					: Path.Combine(AppDomain.CurrentDomain.BaseDirectory, scriptFileName);

				var info = new FileInfo(this.scriptFileName);

				if (info.Directory is { Exists: false })
				{
					Directory.CreateDirectory(info.Directory.FullName);
				}
			}

			this.logQueries = logQueries;
		}

		internal void Build(Configuration config)
		{
			var schema = new SchemaExport(config);

			schema.Drop(false, true);

			scriptAction(schema.Create, true);
		}

		internal void Update(Configuration config)
		{
			var schema = new SchemaUpdate(config);

			scriptAction(schema.Execute, true);

			if (!schema.Exceptions.Any())
				return;

			var message = new StringBuilder();

			schema.Exceptions
				.ToList()
				.ForEach(
					e => message.AppendLine(e.Message)
				);

			throw new DKException(message.ToString());
		}

		internal void Validate(Configuration config)
		{
			new SchemaValidator(config).Validate();
		}

		private delegate void scriptActionDelegate(Action<String> scriptAction, Boolean execute);
		private void scriptAction(scriptActionDelegate schemaActionDelegate, Boolean execute)
		{
			if (scriptFileName != null)
			{
				var format = "yyyy-MM-dd hh:mm:ss ===========================";
				var time = DateTime.Now.ToString(format);
				writeToScriptFile(time);
			}

			schemaActionDelegate(writeAll, execute);
		}

		private void writeToScriptFile(String text)
		{
			if (!File.Exists(scriptFileName))
				File.WriteAllLines(scriptFileName, new[] { text });
			else
				File.AppendAllLines(scriptFileName, new[] { text });
		}

		private void writeAll(String text)
		{
			if (scriptFileName != null)
				writeToScriptFile(text);

			logQueries?.Invoke(text);
		}
	}
}
