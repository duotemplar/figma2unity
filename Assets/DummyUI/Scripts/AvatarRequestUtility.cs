using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Security.Cryptography.X509Certificates;
using UnityEngine;
using UnityEngine.Networking;

namespace DummyUI
{
    /// <summary>
    /// Centralizes request configuration for downloading avatar images.
    /// Handles TLS configuration, domain-specific headers, and certificate validation.
    /// </summary>
    public static class AvatarRequestUtility
    {
        private const string DefaultUserAgent = "Mozilla/5.0 (Macintosh; Intel Mac OS X 10_15_7) AppleWebKit/537.36 (KHTML, like Gecko)";
        private static readonly HttpClient httpClient;

        static AvatarRequestUtility()
        {
            TryConfigureSecurityProtocols();
            httpClient = BuildHttpClient();
        }

        public static UnityWebRequest CreateTextureRequest(string url)
        {
            var request = new UnityWebRequest(url, UnityWebRequest.kHttpVerbGET)
            {
                downloadHandler = new DownloadHandlerBuffer(),
                timeout = 30
            };

            ApplyHeaders(request, url);
            AttachCertificateHandler(request, url);

            return request;
        }

        public static async Task<byte[]> DownloadWithHttpClientAsync(string url)
        {
            if (httpClient == null)
            {
                Debug.LogWarning("[AvatarRequestUtility] HttpClient is unavailable.");
                return null;
            }

            var requestMessage = new HttpRequestMessage(HttpMethod.Get, url);
            ApplyHeaders(requestMessage, url);

            try
            {
                using (var response = await httpClient.SendAsync(requestMessage, HttpCompletionOption.ResponseHeadersRead).ConfigureAwait(false))
                {
                    if (!response.IsSuccessStatusCode)
                    {
                        Debug.LogWarning($"[AvatarRequestUtility] HttpClient response {(int)response.StatusCode} for {url}");
                        return null;
                    }

                    return await response.Content.ReadAsByteArrayAsync().ConfigureAwait(false);
                }
            }
            catch (Exception ex)
            {
                Debug.LogWarning($"[AvatarRequestUtility] HttpClient download failed for {url}: {ex.Message}");
                return null;
            }
        }

        private static HttpClient BuildHttpClient()
        {
            try
            {
                var handler = new HttpClientHandler
                {
                    AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate
                };

#if !UNITY_WEBGL
                handler.ServerCertificateCustomValidationCallback = (message, certificate, chain, errors) =>
                {
                    if (certificate == null)
                    {
                        return false;
                    }

                    var host = message?.RequestUri?.Host?.ToLowerInvariant();
                    if (string.IsNullOrEmpty(host))
                    {
                        return false;
                    }

                    if (host.Contains("githubusercontent.com"))
                    {
                        return true;
                    }

                    return errors == System.Net.Security.SslPolicyErrors.None;
                };
#endif

                return new HttpClient(handler, disposeHandler: true)
                {
                    Timeout = TimeSpan.FromSeconds(30)
                };
            }
            catch (Exception ex)
            {
                Debug.LogWarning($"[AvatarRequestUtility] Failed to build HttpClient: {ex.Message}");
                return null;
            }
        }

        private static void ApplyHeaders(UnityWebRequest request, string url)
        {
            request.SetRequestHeader("User-Agent", DefaultUserAgent);

            try
            {
                var host = new Uri(url).Host.ToLowerInvariant();

                if (host.Contains("huaban.com"))
                {
                    request.SetRequestHeader("Referer", "https://huaban.com/");
                }
                else if (host.Contains("githubusercontent.com") || host.Contains("githubusercontent"))
                {
                    request.SetRequestHeader("Referer", "https://github.com/");
                    request.SetRequestHeader("Accept", "image/avif,image/webp,image/apng,image/*,*/*;q=0.8");
                }
            }
            catch (Exception ex)
            {
                Debug.LogWarning($"[AvatarRequestUtility] Unable to configure headers for URL '{url}': {ex.Message}");
            }
        }

        private static void ApplyHeaders(HttpRequestMessage message, string url)
        {
            if (!message.Headers.UserAgent.TryParseAdd(DefaultUserAgent))
            {
                message.Headers.TryAddWithoutValidation("User-Agent", DefaultUserAgent);
            }

            try
            {
                var host = new Uri(url).Host.ToLowerInvariant();

                if (host.Contains("huaban.com"))
                {
                    message.Headers.Referrer = new Uri("https://huaban.com/");
                }
                else if (host.Contains("githubusercontent.com") || host.Contains("githubusercontent"))
                {
                    message.Headers.Referrer = new Uri("https://github.com/");
                    message.Headers.TryAddWithoutValidation("Accept", "image/avif,image/webp,image/apng,image/*,*/*;q=0.8");
                }
            }
            catch (Exception ex)
            {
                Debug.LogWarning($"[AvatarRequestUtility] Unable to configure HttpClient headers for URL '{url}': {ex.Message}");
            }
        }

        private static void AttachCertificateHandler(UnityWebRequest request, string url)
        {
            try
            {
                var host = new Uri(url).Host.ToLowerInvariant();

                if (host.Contains("githubusercontent.com"))
                {
                    request.certificateHandler = new TrustedDomainCertificateHandler(host);
                }
            }
            catch (Exception ex)
            {
                Debug.LogWarning($"[AvatarRequestUtility] Unable to attach certificate handler for '{url}': {ex.Message}");
            }
        }

        private static void TryConfigureSecurityProtocols()
        {
            try
            {
                ServicePointManager.SecurityProtocol |= SecurityProtocolType.Tls12;

#if !UNITY_WEBGL
                // TLS 1.3 is not defined in older frameworks; guard via numeric value.
                const SecurityProtocolType tls13 = (SecurityProtocolType)12288;
                if (Enum.IsDefined(typeof(SecurityProtocolType), (int)tls13))
                {
                    ServicePointManager.SecurityProtocol |= tls13;
                }
#endif
            }
            catch (Exception ex)
            {
                Debug.LogWarning($"[AvatarRequestUtility] Unable to configure TLS settings: {ex.Message}");
            }
        }

        private sealed class TrustedDomainCertificateHandler : CertificateHandler
        {
            private readonly string host;
            private readonly string hostSuffix;

            public TrustedDomainCertificateHandler(string host)
            {
                this.host = host;
                hostSuffix = GetHostSuffix(host);
            }

            protected override bool ValidateCertificate(byte[] certificateData)
            {
                try
                {
                    var cert = new X509Certificate2(certificateData);

                    if (MatchesHost(cert.Subject) || MatchesSubjectAlternativeNames(cert))
                    {
                        return true;
                    }
                }
                catch (Exception ex)
                {
                    Debug.LogWarning($"[AvatarRequestUtility] Certificate validation failed for '{host}': {ex.Message}");
                }

                return false;
            }

            private bool MatchesHost(string target)
            {
                if (string.IsNullOrEmpty(target))
                {
                    return false;
                }

                var lowered = target.ToLowerInvariant();

                if (!string.IsNullOrEmpty(hostSuffix) && lowered.Contains(hostSuffix))
                {
                    return true;
                }

                if (lowered.Contains(host))
                {
                    return true;
                }

                return false;
            }

            private bool MatchesSubjectAlternativeNames(X509Certificate2 cert)
            {
                try
                {
                    foreach (var extension in cert.Extensions)
                    {
                        if (extension?.Oid?.Value != "2.5.29.17")
                        {
                            continue;
                        }

                        var formatted = extension.Format(true)?.ToLowerInvariant();
                        if (string.IsNullOrEmpty(formatted))
                        {
                            continue;
                        }

                        if (!string.IsNullOrEmpty(hostSuffix) && formatted.Contains(hostSuffix))
                        {
                            return true;
                        }

                        if (formatted.Contains(host))
                        {
                            return true;
                        }
                    }
                }
                catch (Exception ex)
                {
                    Debug.LogWarning($"[AvatarRequestUtility] Failed to inspect SAN for '{host}': {ex.Message}");
                }

                return false;
            }

            private static string GetHostSuffix(string fullHost)
            {
                if (string.IsNullOrEmpty(fullHost))
                {
                    return string.Empty;
                }

                var index = fullHost.IndexOf('.');
                return index >= 0 && index + 1 < fullHost.Length ? fullHost.Substring(index + 1) : fullHost;
            }
        }
    }
}
