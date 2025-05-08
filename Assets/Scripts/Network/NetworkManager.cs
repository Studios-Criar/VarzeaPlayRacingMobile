using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;
using Util;

namespace Network
{
    public class NetworkManager : MonoBehaviour
    {
        public Task<UnityWebRequest> Get(string uri, Dictionary<string, string> headers)
        {
            return Get(new Uri(uri), headers);
        }
    
        public Task<UnityWebRequest> Get(Uri uri, Dictionary<string, string> headers)
        {
            var tcs = new TaskCompletionSource<UnityWebRequest>();
            StartCoroutine(Network.Get(uri, headers, request => tcs.SetResult(request)));
            return tcs.Task;
        }
    
        public Task<UnityWebRequest> GetTexture(string uri, Dictionary<string, string> headers)
        {
            return GetTexture(new Uri(uri), headers);
        }

        public Task<UnityWebRequest> GetTexture(Uri uri, Dictionary<string, string> headers)
        {
            var tcs = new TaskCompletionSource<UnityWebRequest>();
            StartCoroutine(Network.GetTexture(uri, headers, request => tcs.SetResult(request)));
            return tcs.Task;
        }

        public static List<Uri> ParseApacheDirectoryIndex(Uri baseUrl, string htmlContent, string[] extensions = null)
        {
            var list = new List<Uri>();

            const string pattern = @"<a\s+href=""([^""]+)""[^>]*>(?:<img[^>]*>)?([^<]+)<\/a>";

            var matches = Regex.Matches(htmlContent, pattern, RegexOptions.IgnoreCase);

            foreach (Match match in matches)
            {
                if (!match.Success) continue;
            
                var href = match.Groups[1].Value;
                var linkText = match.Groups[2].Value.Trim();

                const StringComparison comparison = StringComparison.OrdinalIgnoreCase;
                var isNotFile = linkText.EqualsAny(comparison, true, "Parent Directory", "Name", "Last Modified", "Size");

                if (isNotFile || href.EndsWith("/")) continue;

                var filename = href.Split("/")[^1];

                if (extensions != null)
                {
                    var fileExtension = Path.GetExtension(filename);
                    if (string.IsNullOrEmpty(fileExtension) || !extensions.Contains(fileExtension)) continue;
                }

                // Debug.Log(filename);

                var uri = new Uri(baseUrl, Uri.EscapeDataString(filename));
                list.Add(uri);
            }
        
            return list;
        }
    }
}
