using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.Xml.Schema;
using System.Xml;
using System.IO;

namespace SqlScriptPackager.Core.Packaging
{
    public class ScriptPackage : IXmlSerializable
    {
        #region Constants
        public const string ELEMENT_PACKAGE = "package";
        public const string ELEMENT_SCRIPT = "script";
        public const string ELEMENT_DATA = "data";
        public const string ELEMENT_CONNECTION = "connection";

        public const string ATTRIBUTE_TYPE = "type";
        public const string ATTRIBUTE_LOCATION = "location";
        public string ATTRIBUTE_CONNECTION_STRING = "connectionstring";
        public string ATTRIBUTE_NAME = "name";
        #endregion

        public string Location
        {
            get;
            protected set;
        }

        public ScriptCollection Scripts
        {
            get;
            protected set;
        }

        public ScriptPackage()
        {
            this.Location = "temporary";
            this.Scripts = new ScriptCollection();
        }

        public void Save(string path)
        {
            this.Location = path;
            using(XmlWriter writer = XmlWriter.Create(this.Location))
            {
                this.WriteXml(writer);
            }
        }

        public void Load(string path)
        {
            this.Location = path;
            this.ReadXml(XmlReader.Create(this.Location));
        }

        public System.Xml.Schema.XmlSchema GetSchema()
        {
            return null;
        }

        public void ReadXml(XmlReader reader)
        {
            reader.ReadStartElement(ELEMENT_PACKAGE);

            while (reader.Name == ELEMENT_SCRIPT)
                this.Scripts.Add(ReadScriptXml(reader));

            reader.ReadEndElement();
        }

        public void WriteXml(XmlWriter writer)
        {
            writer.WriteStartElement(ELEMENT_PACKAGE);

            foreach (Script script in this.Scripts)
                WriteScriptXml(writer, script);

            writer.WriteEndElement();
        }

        protected Script ReadScriptXml(XmlReader reader)
        {
            Type scriptType = Type.GetType(reader[ATTRIBUTE_TYPE]);
            Script script = (Script)Activator.CreateInstance(scriptType);
            
            reader.Read();
            PackageScriptResource resource = null;
            DatabaseConnection connection = null;
            while (reader.Name != ELEMENT_SCRIPT)
            {
                if (reader.Name == ELEMENT_CONNECTION)
                    connection = ReadScriptConnectionXml(reader);
                else if (reader.Name == ELEMENT_DATA)
                    resource = ReadScriptResourceXml(reader);
                else
                    throw new XmlException("Unrecognized node: " + reader.Name);
            }
            reader.Read();
            script.InitializeScript(resource, connection);

            return script;
        }

        protected PackageScriptResource ReadScriptResourceXml(XmlReader reader)
        {
            return new PackageScriptResource(this, reader[ATTRIBUTE_LOCATION], reader.ReadElementContentAsString());
        }

        protected DatabaseConnection ReadScriptConnectionXml(XmlReader reader)
        {
            var connection = new DatabaseConnection(reader[ATTRIBUTE_NAME], reader[ATTRIBUTE_CONNECTION_STRING]);
            reader.Read();
            return connection;
        }

        protected void WriteScriptXml(XmlWriter writer, Script script)
        {
            writer.WriteStartElement(ELEMENT_SCRIPT);
            writer.WriteAttributeString(ATTRIBUTE_TYPE, script.GetType().AssemblyQualifiedName);

            if (script.ContentResource is PackageScriptResource)
                writer.WriteAttributeString(ATTRIBUTE_LOCATION, ((PackageScriptResource)script.ContentResource).UnpackagedLocation);
            else
                writer.WriteAttributeString(ATTRIBUTE_LOCATION, script.ContentResource.Location);

            writer.WriteStartElement(ELEMENT_CONNECTION);
            writer.WriteAttributeString(ATTRIBUTE_NAME, script.Connection.ConnectionName);
            writer.WriteAttributeString(ATTRIBUTE_CONNECTION_STRING, script.Connection.ConnectionString);
            writer.WriteEndElement();

            writer.WriteStartElement(ELEMENT_DATA);
            writer.WriteValue(script.ScriptContents);
            writer.WriteEndElement();

            writer.WriteEndElement();
        }
    }
}
