﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using Keon.Util.Exceptions;

namespace Keon.XML
{
	/// <summary>
	/// Xml node structure
	/// </summary>
	public class Node
	{
		#region Contructors
		///<summary>
		/// Xml node structure
		///</summary>
		public Node()
		{
			Attributes = new Dictionary<String, String>();
			Children = new List<Node>();
			Value = String.Empty;
		}

		/// <inheritdoc />
		/// <summary>
		/// 	Xml node structure
		/// </summary>
		/// <param name="name">Name for the new Node</param>
		/// <param name="value">Text Content for the new Node</param>
		public Node(String name, String value) : this()
		{
			Name = name;
			Value = value;
		}

		/// <inheritdoc />
		/// <summary>
		///  Xml node structure
		/// </summary>
		/// <param name="xmlNode">Initial XMLNode</param>
		/// <param name="readChildren">Whether is to read all the children (recursive read)</param>
		private Node(XmlNode xmlNode, Boolean readChildren = true)
			: this()
		{
			if (xmlNode == null)
				throw new DKException("XmlNode needed. Use another initialization if won't have it.");


			Name = xmlNode.Name;


			if (xmlNode.Attributes != null)
			{
				for (var a = 0; a < xmlNode.Attributes.Count; a++)
				{
					var attr = xmlNode.Attributes[a];

					Attributes.Add(attr.Name, attr.Value);
				}
			}


			if (!readChildren) return;


			for (var n = 0; n < xmlNode.ChildNodes.Count; n++)
			{
				var node = xmlNode.ChildNodes[n];

				// ReSharper disable once SwitchStatementMissingSomeCases
				switch (node?.NodeType)
				{
					case XmlNodeType.Element:
						Children.Add(new Node(node));
						break;
					case XmlNodeType.Text:
						Value += node.Value;
						break;
				}
			}
		}

		/// <inheritdoc />
		///  <summary>
		///  Xml node structure
		///  </summary>
		/// <param name="path">Path of the XML file</param>
		/// <param name="readChildren">Whether is to read all the children (recursive read)</param>
		public Node(String path, Boolean readChildren = true) : this(createNodesFromFile(path), readChildren) { }

		private static XmlNode createNodesFromFile(String path)
		{
			var xml = new XmlDocument();

			xml.Load(path);
			file = path;

			return xml.LastChild;
		}
		#endregion



		#region Properties
		private static String file;

		///<summary>
		/// Name of the tag of the Node
		///</summary>
		public String Name { get; set; }

		///<summary>
		/// Text Content of the Node
		///</summary>
		public String Value { get; set; }

		///<summary>
		/// The Attributes of current Node
		///</summary>
		public IDictionary<String, String> Attributes { get; set; }

		///<summary>
		/// The Child Nodes
		///</summary>
		public IList<Node> Children { get; set; }


		/// <summary>
		/// Get attribute
		/// </summary>
		/// <param name="attribute">The attribute name</param>
		/// <returns>If found, the value of the attribute; Else, null</returns>
		public String this[String attribute]
		{
			get => Attributes.Keys.Contains(attribute) ? Attributes[attribute] : null;
			set => Add(attribute, value);
		}


		/// <summary>
		/// Get child node
		/// </summary>
		/// <param name="node">The node position</param>
		/// <returns>If found, the child node; Else, null</returns>
		public Node this[Int32 node]
		{
			get => node < Children.Count ? Children[node] : null;
			set => Children[node] = value;
		}
		#endregion



		#region Methods
		/// <summary>
		/// Add Child
		/// </summary>
		/// <param name="node">Child Node</param>
		public void Add(Node node)
		{
			Children.Add(node);
		}

		/// <summary>
		/// Add Attribute
		/// </summary>
		/// <param name="name">Attribute Name</param>
		/// <param name="value">Attribute Value</param>
		public void Add(String name, String value)
		{
			if (!Attributes.Keys.Contains(name))
				Attributes.Add(name, value);
			else
				Attributes[name] = value;
		}

		///<summary>
		/// Whether it has child nodes
		///</summary>
		public Boolean HasChildren()
		{
			return Children.Any();
		}


		/// <summary>
		/// Create a backup of the old file in a subfolder backup and override the original file
		/// </summary>
		public void BackUpAndSave()
		{
			BackUpAndSave(null);
		}

		/// <summary>
		/// Create a backup of the old file in a subfolder backup and override the original file
		/// </summary>
		public void BackUpAndSave(String backupPath)
		{
			backUp(backupPath);
			save();
		}


		/// <summary>
		/// Saves OVERWRITING the original file
		/// </summary>
		public void Overwrite()
		{
			save();
		}

		private void save()
		{
			var utf8WithoutBom = new UTF8Encoding(false);

			var textWriter = new XmlTextWriter(file, utf8WithoutBom)
				{ Formatting = Formatting.Indented, IndentChar = '\t', Indentation = 1 };

			textWriter.WriteStartDocument();

			var node = this;

			writeNodeAtFile(node, textWriter);

			textWriter.WriteEndDocument();

			textWriter.Close();
		}

		private static void writeNodeAtFile(Node node, XmlWriter textWriter)
		{
			if (textWriter == null)
				throw new ArgumentNullException(nameof(textWriter));


			textWriter.WriteStartElement(node.Name);

			foreach (var attr in node.Attributes)
			{
				textWriter.WriteAttributeString(attr.Key, attr.Value);
			}

			textWriter.WriteString(node.Value);

			foreach (var child in node.Children)
			{
				writeNodeAtFile(child, textWriter);
			}

			textWriter.WriteEndElement();
		}

		private static void backUp(String fileFullName)
		{
			var copy = fileFullName;

			if (fileFullName == null)
			{
				var barIndex = file.LastIndexOf(@"\", StringComparison.Ordinal);

				var path = file[..(barIndex + 1)] + "BackUp";

				// ReSharper disable once StringLiteralTypo
				const String dateFormat = "yyyyMMddHHmmssfff";

				var name = DateTime.Now.ToString(dateFormat) + "_" + file[(barIndex + 1)..];

				copy = Path.Combine(path, name);

				if (!Directory.Exists(path))
					Directory.CreateDirectory(path);
			}

			File.Copy(file, copy);
		}


		/// <summary>
		/// Returns a <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
		/// </summary>
		/// <returns>
		/// A <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
		/// </returns>
		/// <filterpriority>2</filterpriority>
		public override String ToString()
		{
			return Name;
		}
		#endregion




		#region IEnumerator Members
		///<summary>
		/// To make ForEach
		///</summary>
		public IEnumerator<Node> GetEnumerator()
		{
			return Children.GetEnumerator();
		}
		#endregion
	}
}
