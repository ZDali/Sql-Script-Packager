using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace SqlScriptPackager.Core.Packaging
{
    public class PackageScriptResource : ScriptContentResource
    {
        private ScriptPackage _package = null;
        private string _scriptLocation = null;
        private string _content = null;

        public override string Location
        {
            get
            {
                return string.Concat("package://", _package.Location, "/", _scriptLocation);
            }
            protected set
            {
                throw new System.Data.ReadOnlyException();
            }
        }

        public string UnpackagedLocation
        {
            get { return _scriptLocation; }
        }

        public override string GetContent()
        {
            return _content;
        }

        public PackageScriptResource(ScriptPackage package, string scriptLocation, string content)
        {
            this._package = package;
            this._scriptLocation = scriptLocation;
            this._content = content;
        }
    }
}
