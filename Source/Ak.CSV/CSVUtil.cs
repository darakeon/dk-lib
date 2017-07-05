using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using Kent.Boogaart.KBCsv;
using Oak.Contrato.IntegracaoAPI.Error;
using Oak.Contrato.IntegracaoAPI.Util.Models;
using Oak.WebApiCommon;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Globalization;

namespace Oak.Contrato.BusinessLogic.Util {
	public class CSVUtil {

		public static List<T> ReadFromCSV<T>(string arquivoId, string[] header, out ErrorModel error, char csvSeparator = ';') where T : CSVModel, new() {
			error = null;
			var filePath = Path.Combine(Configuracao.TempPath, arquivoId + ".csv");

			if (!File.Exists(filePath)) {
				error = ErrorModel.Create(ErrorCode.ArquivoNaoExiste);
				return null;
			}

			using (var csvReader = new CsvReader(new FileStream(filePath, FileMode.Open), Encoding.UTF8, true)) {
				csvReader.ValueSeparator = csvSeparator;
				var csvHeader = csvReader.ReadHeaderRecord();

				if (!csvHeader.ToArray().SequenceEqual(header)) {
					error = ErrorModel.Create(ErrorCode.CSVHeaderInvalido, String.Format("{0}: {1}.",
						ErrorCode.CSVHeaderInvalido.Message(), header.Aggregate((s1, s2) => s1 + csvSeparator.ToString() + s2)));
					return null;
				}

				var modelProperties = typeof(T).GetProperties();
				var result = header.Intersect(modelProperties.Select(p => p.Name));
				if (result.Count() != header.Length) {
					throw new Exception(String.Format("O header contém valores que não estão presentes na classe. Verifique os nomes dos atributos na classe: {0}.",
						typeof(T).FullName));
				}

				var modelList = new List<T>();
				while (csvReader.HasMoreRecords) {
					var data = csvReader.ReadDataRecord();

					if (data.Count == 0) { // Linha em branco
						continue;
					}

					var x = new T();
					var errors = new List<string>();
					foreach (var h in header) {
						var prop = modelProperties.First(p => p.Name == h);
						try {
							prop.SetValue(x, data[csvHeader[h]]);
						} catch {
							errors.Add(h);
						}
					}

					if (errors.Count > 0) {
						x.Error = String.Format("Os seguintes campos contém erros: {0}",
							errors.Aggregate((err1, err2) => err1 + csvSeparator.ToString() + err2) + ".");
					} else {
						var validationContext = new ValidationContext(x);
						var validationResults = new List<ValidationResult>();
						var isValid = Validator.TryValidateObject(x, validationContext, validationResults, true);
						if (!isValid) {
							x.Error = validationResults.ConvertAll(r => r.ErrorMessage.TrimEnd(new char[] { ' ', '.' }))
								.Aggregate((s1, s2) => String.Format("{0}, {1}", s1, s2)) + ".";
						}
					}

					modelList.Add(x);
				}

				return modelList;
			}
		}


		public static Stream ObjectToCSV(object obj) {
			return ObjectToCSV(new List<object>() { obj });
		}

		public static Stream ObjectToCSV(IEnumerable<object> objs, List<String> bypassProperties = null) {

			var memStream = new MemoryStream();
			var csvWriter = new CsvWriter(memStream, Encoding.GetEncoding(1252), true);

			csvWriter.ValueSeparator = ';';

			if (objs != null && objs.FirstOrDefault() != null) {
				Type sourceType = objs.FirstOrDefault().GetType();

				var objProps = new List<PropertyInfo>(sourceType.GetProperties());
				if (bypassProperties != null) {
					objProps = objProps.Where(p => !bypassProperties.Contains(p.Name)).ToList();
				}

				csvWriter.WriteRecord(objProps.Select(p => p.Name));

				CultureInfo cult = new CultureInfo("pt-BR");

				foreach (object obj in objs) {
				
					csvWriter.WriteRecord(objProps.Select(p => p.GetValue(obj) != null 
						? p.GetType() != typeof(DateTime?) 
							? p.GetValue(obj).ToString() 
							: ((p.GetValue(obj) as DateTime?).Value.ToString("dd/MM/yyyy HH:mm:ss", cult)) 
						: ""));
				}
			}

			csvWriter.Flush(); // This line is absolutely necessary. It seems that only closing the writer doesn't flush the data.
			csvWriter.Close();

			// The line below is absolutely necessary!!! It resets the stream so that we can read the data.
			memStream.Position = 0;

			return memStream;
		}



        public static HttpResponseMessage CreateResponse(Stream csv, string fileName)
        {
            var message = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent((new StreamReader(csv, Encoding.GetEncoding(1252)).ReadToEnd()),
                    Encoding.GetEncoding(1252),
                    "text/csv")
            };

            message.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment")
            {
                FileName = fileName
            };

            return message;
        }




	}
}
