using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Web.Script.Serialization;

namespace WeddingApp
{
    /// <summary>
    /// Server calls
    /// </summary>
    internal class ServiceAccess
    {
        static string serverURL = null;

        const string PROPFILE = "wedding.properties";
        const string LOGFILE = "wedding.log";

        internal static void Init()
        {
            InitServer();
        }

        /// <summary>
        /// GET /
        /// </summary>
        /// <returns>Album list</returns>
        internal static string[] GetFolders()
        {
            if (serverURL == null)
                return new string[0];
            HttpWebRequest request = WebRequest.CreateHttp(serverURL);
            WebResponse response = SendRequest(request);
            if (response == null)
                return new string[0];
            string text = slurpInputStreamAsString(response.GetResponseStream());
            if (text == null)
                return new string[0];
            JavaScriptSerializer jss = new JavaScriptSerializer();
            return jss.Deserialize<string[]>(text);
        }

        /// <summary>
        /// GET /album
        /// </summary>
        /// <param name="folder">album key</param>
        /// <returns>List of images</returns>
        internal static ImageInfo[] GetImageList(string folder)
        {
            if (serverURL == null)
                return new ImageInfo[0];
            string uri = string.Format("{0}/{1}", serverURL, 
                Uri.EscapeDataString(folder));
            HttpWebRequest request = WebRequest.CreateHttp(uri);
            WebResponse response = SendRequest(request);
            if (response == null)
                return new ImageInfo[0];
            string text = slurpInputStreamAsString(response.GetResponseStream());
            if (text == null)
                return new ImageInfo[0];
            JavaScriptSerializer jss = new JavaScriptSerializer();
            return jss.Deserialize<ImageInfo[]>(text);
        }

        /// <summary>
        /// GET /album/key
        /// </summary>
        /// <param name="folder">album name</param>
        /// <param name="filename">image key</param>
        /// <param name="path">where to save image</param>
        internal static void DownloadFile(string folder, string filename, string path)
        {
            if (serverURL == null)
                return; 
            string uri = string.Format("{0}/{1}/{2}", serverURL,
                Uri.EscapeDataString(folder), Uri.EscapeDataString(filename));
            HttpWebRequest request = WebRequest.CreateHttp(uri);
            WebResponse response = SendRequest(request);
            if (response != null)
                WriteFileFromContent(response, path);
        }

        /// <summary>
        /// PUT /album/key?alias=x
        /// </summary>
        /// <param name="album"></param>
        /// <param name="image key"></param>
        /// <param name="alias"></param>
        internal static void SetAlias (string folder, string filename, string alias)
        {
            if (serverURL == null)
                return;
            string uri = string.Format("{0}/{1}/{2}?alias={3}", serverURL,
                Uri.EscapeDataString(folder), Uri.EscapeDataString(filename),
                Uri.EscapeDataString(alias));
            HttpWebRequest request = WebRequest.CreateHttp(uri);
            request.Method = "PUT";
            WebResponse response = SendRequest(request);
        }

        private static void WriteFileFromContent(WebResponse response, string path)
        {
            try
            {
                Stream webstream = response.GetResponseStream();
                Stream filestream = File.OpenWrite(path);
                webstream.CopyTo(filestream);
                filestream.Close();
                webstream.Close();
            }
            catch (Exception e)
            {
                Control.ShowError(string.Format("File saving error: {0}", e.Message));
            }
        }

        private static byte[] slurpInputStream(Stream stream)
        {
            try
            {
                using (MemoryStream ms = new MemoryStream())
                {
                    byte[] buffer = new byte[32 * 1024];
                    while (true)
                    {
                        int nRead = stream.Read(buffer, 0, buffer.Length);
                        if (nRead <= 0)
                        {
                            return ms.ToArray();
                        }
                        ms.Write(buffer, 0, nRead);
                    }
                }
            }
            catch (Exception e)
            {
                Control.ShowError(string.Format("File reading error: {0}", e.Message));
                return null;
            }
        }

        private static string slurpInputStreamAsString(Stream stream)
        {
            ASCIIEncoding encoding = new ASCIIEncoding();
            return encoding.GetString(slurpInputStream(stream));
        }

        /// <summary>
        /// Load communication properties
        /// </summary>
        private static void InitServer()
        {
            try
            {
                string[] props = File.ReadAllLines(PROPFILE);
                foreach (string prop in props)
                {
                    int eq = prop.IndexOf('=');
                    if (eq > 0)
                    {
                        if (prop.Substring(0, eq).Equals("Server"))
                            serverURL = prop.Substring(eq + 1).Trim();
                    }
                }
            }
            catch
            {
            }

            if (serverURL == null)
            {
                Control.ShowError("Specify your Server in wedding.properties");
            }
        }

        /// <summary>
        /// Send/Receive with logging
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        private static WebResponse SendRequest(HttpWebRequest request)
        {
            try
            {
                LogRequest(request);
                WebResponse response = request.GetResponse();
                LogResponse(response);
                return response;
            }
            catch (Exception e)
            {
                Control.ShowError(string.Format("Communication error: {0}", e.Message));
                return null;
            }
        }

        private static void LogRequest(HttpWebRequest request)
        {
            string logpath = Path.Combine(Path.GetTempPath(), LOGFILE);
            List<string> fields = new List<string>();
            fields.Add(string.Format("Logged {0}", DateTime.Now));
            fields.Add(string.Format("{0} {1} HTTP/{2}",
                request.Method, request.Address.PathAndQuery, request.ProtocolVersion));
            fields.Add("Host: " + request.Host);
            WebHeaderCollection headers = request.Headers;
            for (int i = 0; i < headers.Count; i++)
            {
                string key = headers.GetKey(i);
                if (!key.Equals("Host"))
                    fields.Add(key + ": " + headers.Get(i));
            }
            fields.Add("");
            File.AppendAllLines(logpath, fields);
        }

        private static void LogResponse(WebResponse response1)
        {
            HttpWebResponse response = response1 as HttpWebResponse;
            if (response == null)
                return;
            string logpath = Path.Combine(Path.GetTempPath(), LOGFILE);
            List<string> fields = new List<string>();
            fields.Add(string.Format("Logged {0}", DateTime.Now));
            fields.Add(string.Format("HTTP/{0} {1} {2}",
                response.ProtocolVersion, response.StatusCode, response.StatusDescription));
            WebHeaderCollection headers = response.Headers;
            for (int i = 0; i < headers.Count; i++)
            {
                fields.Add(headers.GetKey(i) + ": " + headers.Get(i));
            }
            fields.Add("");
            File.AppendAllLines(logpath, fields);
        }
    }
}
