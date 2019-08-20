using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml.Linq;

namespace LightLog
{
    public class AppConfigHelper
    {
        static AppConfigHelper()
        { }
        public AppConfigHelper()
        {
            //其中configuration为 xe 的根节点
            XElement xe = XElement.Load(Utils.PathCombine(Directory.GetCurrentDirectory(), "applog.config"));
            var els = xe?.Element("appSettings");
            if (els == null)
            {
                return;
            }
            var nextnode = els.FirstNode;
            while (nextnode != null)
            {
                XElement xel = nextnode as XElement;
                if (xel != null)
                {
                    string key = xel.FirstAttribute.Value;
                    string val = xel.LastAttribute.Value;
                    if (key != null && val != null)
                    {
                        _settingsDic.Add(key, val);
                    }
                }
                nextnode = nextnode.NextNode;
            }
        }
        private static Dictionary<string, string> _settingsDic = new Dictionary<string, string>();
        private static AppConfigHelper _Instance;
        public static AppConfigHelper Instance
        {
            get
            {
                if (_Instance == null)
                {
                    _Instance = new AppConfigHelper();
                }
                return _Instance;
            }
        }
        public bool SetSettingsKeyValue(string key, string value)
        {
            //其中configuration为 xe 的根节点
            XElement xe = XElement.Load(Path.Combine(Directory.GetCurrentDirectory(), "applog.config"));
            var els = xe?.Element("appSettings");
            var nextnode = els.FirstNode;
            while (nextnode != null)
            {
                XElement xel = nextnode as XElement;
                if (xel != null)
                {
                    string ky = xel.FirstAttribute.Value;
                    if (ky == key)
                    {
                        if (xel?.LastAttribute?.Name == "value")
                        {
                            xel?.LastAttribute?.SetValue(value);
                            if (_settingsDic.ContainsKey(key))
                                _settingsDic[key] = value;
                            return true;
                        }
                    }
                }
                nextnode = nextnode.NextNode;
            }
            return false;
        }
        public string GetSettingsKeyValue(string key)
        {
            if (_settingsDic.ContainsKey(key))
            {
                return _settingsDic[key];
            }
            return null;
        }

    }
}
