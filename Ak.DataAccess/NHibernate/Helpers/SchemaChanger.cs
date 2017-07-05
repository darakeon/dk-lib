using System;
using System.IO;
using System.Linq;
using System.Text;
using Ak.Generic.Exceptions;
using NHibernate.Cfg;
using NHibernate.Linq;
using NHibernate.Tool.hbm2ddl;

namespace Ak.DataAccess.NHibernate.Helpers
{
    internal class SchemaChanger
    {
        private readonly String scriptFileName;

        public SchemaChanger(String scriptFileName)
        {
            this.scriptFileName = scriptFileName;
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


            if (schema.Exceptions.Any())
            {
                var message = new StringBuilder();

                schema.Exceptions.ForEach(e => message.AppendLine(e.Message));

                throw new AkException(message.ToString());
            }
        }

        internal void Validate(Configuration config)
        {
            new SchemaValidator(config).Validate();
        }



        private delegate void ScriptAction(Action<String> scriptAction, Boolean execute);

        private void scriptAction(ScriptAction method, Boolean execute)
        {
            if (scriptFileName == null)
            {
                method(null, execute);
            }
            else
            {
                if (!File.Exists(scriptFileName))
                    File.Create(scriptFileName).Close();

                using (var file = new FileStream(scriptFileName, FileMode.Append, FileAccess.Write))
                using (var sw = new StreamWriter(file))
                {
                    sw.WriteLine(
                            DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss ===========================")
                        );

                    Action<String> updateExport = x => sw.WriteLine(x);

                    method(updateExport, execute);

                    sw.Close();
                }
            }
        }
    }
}
