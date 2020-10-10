using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Keon.Util.Exceptions;
using NHibernate.Cfg;
using NHibernate.Tool.hbm2ddl;

namespace Keon.NHibernate.Helpers
{
    /// <summary>
    /// Make changes to database tables
    /// </summary>
    public class SchemaChanger
    {
        private readonly String scriptFileName;

	    /// <summary>
	    /// Construct class with the name of the script
	    /// </summary>
	    public SchemaChanger(String scriptFileName)
        {
            var addressPattern = new Regex(@"^(\\|[A-Z]\:).*");

            this.scriptFileName = addressPattern.IsMatch(scriptFileName)
                ? scriptFileName
				: Path.Combine(AppDomain.CurrentDomain.BaseDirectory, scriptFileName);

			var info = new FileInfo(this.scriptFileName);

			if (info.Directory != null && !info.Directory.Exists)
			{
				Directory.CreateDirectory(info.Directory.FullName);
			}
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

        private delegate void ScriptAction(Action<String> scriptAction, Boolean execute);
        private void scriptAction(ScriptAction schemaAction, Boolean execute)
        {
            if (scriptFileName == null)
            {
                schemaAction(null, execute);
            }
            else
            {
                var format = "yyyy-MM-dd hh:mm:ss ===========================";
                var time = DateTime.Now.ToString(format);
                write(scriptFileName, time);

                schemaAction(text => write(scriptFileName, text), execute);
            }
        }

        private void write(String path, String text)
        {
	        if (!File.Exists(path))
		        File.WriteAllLines(path, new[] { text });
	        else
				File.AppendAllLines(path, new[] { text });
        }
    }
}
