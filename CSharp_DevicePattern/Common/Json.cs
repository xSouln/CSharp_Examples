using System;
using System.IO;
using System.Text.Json;
using xLib;
using xLib.Common;

namespace Device.Common
{
    public class Json
    {
        public static xAction<string> Tracer = xTracer.Message;

        private static void trace(string str)
        {
            Tracer?.Invoke(str);
        }

        public static string FileExtensionClear(string file_name)
        {
            string[] str = file_name.Split('.');
            int i = str.Length;

            while (i > 0) { if (str[i - 1] != "json") { break; } i--; }

            file_name = "";
            for (int j = 0; j < i; j++) { file_name += str[j]; }

            return file_name + ".json";
        }

        public static bool Save<TObject>(string file_name, in TObject arg)
        {
            if (file_name == null) { trace("json file save error: file_name = null"); return false; }
            if (arg == null) { trace("json file save error: arg = null"); return false; }

            try
            {
                using (FileStream Stream = new FileStream(file_name, FileMode.Create))
                {
                    JsonSerializer.SerializeAsync(Stream, arg);
                    trace("json file is save:\r" + file_name);
                    Stream.Close();
                    return true;
                }
            }
            catch (Exception e) { trace("json file save error:\r" + e); }
            return false;
        }

        public static bool Open<TObject>(string file_name, out TObject arg)
        {
            arg = default(TObject);
            if (file_name == null) { trace("json file save error: file_name = null"); return false; }
            try
            {
                using (FileStream Stream = new FileStream(file_name, FileMode.Open))
                {
                    var result = JsonSerializer.DeserializeAsync<TObject>(Stream);
                    arg = result.Result;
                    trace("json file is save:\r" + file_name);
                    return true;
                }
            }
            catch (Exception e) { trace("json file save error:\r" + e); }
            return false;
        }
    }
}
