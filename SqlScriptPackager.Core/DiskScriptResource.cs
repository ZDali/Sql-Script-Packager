using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace SqlScriptPackager.Core
{
    public class DiskScriptResource : ScriptContentResource
    {
        public override string Location
        {
            get;
            protected set;
        }

        public override string GetContent()
        {
            return File.ReadAllText(this.Location);
        }

        public DiskScriptResource(string file)
        {
            this.Location = file;
        }
    }
}
