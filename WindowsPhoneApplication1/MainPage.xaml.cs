using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Microsoft.Phone.Controls;
using PhoneGap;

namespace WindowsPhoneApplication1
{
    public partial class MainPage : PhoneApplicationPage
    {
        public const string DEFAULT_INITIAL_URL = "/www/index.html";
        public const string DATA_PROTOCOL = "data://";

        private CommandManager manager = new CommandManager(); 

        /**
         * TODO List:
         * 1. Handle navigating between internal/external links.
         * 2. Add some freaking device functionality! we won't be able to test until we get a device... wonderful :|
         **/
        public MainPage()
        {
            InitializeComponent();
            SupportedOrientations = SupportedPageOrientation.Portrait;
        }
        private void browser_Loaded(object sender, RoutedEventArgs e)
        {
            browser.IsScriptEnabled = true;
            // Get the default asset to load initially and inject the HTML into the Web Browser control.
            loadLocalAsset(DEFAULT_INITIAL_URL);
        }
        private void loadLocalAsset(string url)
        {
            string content = "";
            try
            {
                content = this.readLocalAsset(url);
                if (url.EndsWith(".html") || url.EndsWith(".html"))
                {
                    content = parseScripts(content);
                }
            }
            catch
            {
                content = "PhoneGap fail :(";
            }
            browser.NavigateToString(content);
        }
        private string readLocalAsset(string url)
        {
            Assembly assembly = Assembly.GetExecutingAssembly();
            // Hacky, because assembly.GetName().Name raises an 'InvalidProgramException'. wtf
            string assName = assembly.FullName.Substring(0, assembly.FullName.IndexOf(','));
            string path = assName + ".www." + (url.StartsWith("/www/") ? url.Substring(5) : url).Replace("/", ".");
            Stream stream = assembly.GetManifestResourceStream(path);
            StreamReader reader = new StreamReader(stream, Encoding.GetEncoding("UTF-8"));
            return reader.ReadToEnd();
        }
        /// <summary>
        /// Parses script tags with data:// sources, since it looks like the WebBrowser object won't figure out to grab internal resources.
        /// TODO: likely need to do this for other things (like img tags)
        /// TODO: This is fuckin ugly, it'd be rad if the web browser did this for me :|
        /// </summary>
        /// <param name="html">HTML to parse</param>
        /// <returns></returns>
        private string parseScripts(string html)
        {
            string parsed = html;
            string toMatch = "<script src=\"" + DATA_PROTOCOL;
            int matchLength = toMatch.Length;
            int position = html.IndexOf(toMatch);
            if (position > 0)
            {
                string parsedText = parsed.Substring(position + matchLength);
                int endName = parsedText.IndexOf("\"");
                int endScript = parsed.IndexOf("</script>", position);
                int scriptTagLength = endScript + "</script>".Length - position;
                String jsName = parsedText.Substring(0, endName);
                parsedText = parsed.Remove(position, scriptTagLength);
                parsedText = parsedText.Insert(position, "<script type='text/javascript'>\n" + readLocalAsset(jsName) + "</script>");
                return parseScripts(parsedText);
            }
            return parsed;
        }
        /// <summary>
        /// Handles calls to window.external.Notify from within the web browser control. We'll hijack this for message passing from browser back into C#.
        /// TODO: Processing the instruction could be blocking....... bleh
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void browser_ScriptNotify(object sender, NotifyEventArgs e)
        {
            string command = "";
            string[] commandParams;
            int colPos = e.Value.IndexOf(':');
            if (colPos > 0)
            {
                command = e.Value.Substring(0, colPos);
                commandParams = e.Value.Substring(colPos).Split('/');
            }
            else
            {
                command = e.Value;
                commandParams = new string[0];
            }
            string returnValue = manager.processInstruction(command, commandParams);
            if (returnValue != null)
            {
                browser.InvokeScript("gap.evaluate", returnValue);
            }
        }
    }
}