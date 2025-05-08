using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;

namespace Network
{
    public static class Network
    {
        public static IEnumerator Get(Uri uri, Dictionary<string, string> headers, Action<UnityWebRequest> onComplete)
        {
            using var request = UnityWebRequest.Get(uri);

            if (headers != null)
            {
                foreach (var (k, v) in headers)
                {
                    request.SetRequestHeader(k, v);
                }
            }

            yield return request.SendWebRequest();
            onComplete?.Invoke(request);
        }

        public static IEnumerator GetTexture(Uri uri, Dictionary<string, string> headers, Action<UnityWebRequest> onComplete)
        {
            using var request = UnityWebRequestTexture.GetTexture(uri);

            if (headers != null)
            {
                foreach (var (k, v) in headers)
                {
                    request.SetRequestHeader(k, v);
                }
            }

            yield return request.SendWebRequest();
            onComplete?.Invoke(request);
        }
    }
}