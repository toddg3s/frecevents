using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Security.Cryptography;
using Newtonsoft.Json;
using System.Text;
using Newtonsoft.Json.Linq;
using System.Net.Http;
using System.Net;
using System.IO;
using System.Collections.Specialized;
using System.Web.Script.Serialization;
using System.Security.Cryptography.X509Certificates;

namespace frecevents.web
{
  //public enum JwtHashAlgorithm
  //{
  //  RS256,
  //  HS384,
  //  HS512
  //}

  public class Utilities
  {
    public const string SCOPE_ANALYTICS_READONLY = "https://www.googleapis.com/auth/analytics.readonly";

    public static string keypath { get; set; }
    private static DateTime tokenExp;
    private static string tokenSave;

    public static string GetAccessToken()
    {
      if (DateTime.Now < tokenExp) return tokenSave;

      // certificate
      var certificate = new X509Certificate2( keypath, "notasecret");

      // header
      var header = new { typ = "JWT", alg = "RS256" };

      // claimset
      var times = GetExpiryAndIssueDate();
      var claimset = new
      {
        iss = "calapi@frecevents.iam.gserviceaccount.com",
        scope = "https://www.googleapis.com/auth/calendar.readonly",
        aud = "https://accounts.google.com/o/oauth2/token",
        iat = times[0],
        exp = times[1],
      };

      JavaScriptSerializer ser = new JavaScriptSerializer();

      // encoded header
      var headerSerialized = ser.Serialize(header);
      var headerBytes = Encoding.UTF8.GetBytes(headerSerialized);
      var headerEncoded = Convert.ToBase64String(headerBytes);

      // encoded claimset
      var claimsetSerialized = ser.Serialize(claimset);
      var claimsetBytes = Encoding.UTF8.GetBytes(claimsetSerialized);
      var claimsetEncoded = Convert.ToBase64String(claimsetBytes);

      // input
      var input = headerEncoded + "." + claimsetEncoded;
      var inputBytes = Encoding.UTF8.GetBytes(input);

      // signiture
      var rsa = certificate.PrivateKey as RSACryptoServiceProvider;
      var cspParam = new CspParameters
      {
        KeyContainerName = rsa.CspKeyContainerInfo.KeyContainerName,
        KeyNumber = rsa.CspKeyContainerInfo.KeyNumber == KeyNumber.Exchange ? 1 : 2
      };
      var aescsp = new RSACryptoServiceProvider(cspParam) { PersistKeyInCsp = false };
      var signatureBytes = aescsp.SignData(inputBytes, "SHA256");
      var signatureEncoded = Convert.ToBase64String(signatureBytes);

      // jwt
      var jwt = headerEncoded + "." + claimsetEncoded + "." + signatureEncoded;

      var client = new WebClient();
      client.Encoding = Encoding.UTF8;
      var uri = "https://accounts.google.com/o/oauth2/token";
      var content = new NameValueCollection();

      content["assertion"] = jwt;
      content["grant_type"] = "urn:ietf:params:oauth:grant-type:jwt-bearer";

      string response = Encoding.UTF8.GetString(client.UploadValues(uri, "POST", content));

      var responseobj = JObject.Parse(response);
      tokenExp = DateTime.Now.AddSeconds(responseobj["expires_in"].Value<int>());
      tokenSave = responseobj["access_token"].Value<string>();

      return tokenSave;
    }

    private static int[] GetExpiryAndIssueDate()
    {
      var utc0 = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
      var issueTime = DateTime.UtcNow;

      var iat = (int)issueTime.Subtract(utc0).TotalSeconds;
      var exp = (int)issueTime.AddMinutes(55).Subtract(utc0).TotalSeconds;

      return new[] { iat, exp };
    }

    /*
    #region original_try
    private static Dictionary<JwtHashAlgorithm, Func<byte[], byte[], byte[]>> HashAlgorithms;
    private static DateTime jwtExpDate;
    private static string jwtSave;
    private static DateTime tokenExpDate;
    private static string tokenSave;

    static Utilities()
    {
      HashAlgorithms = new Dictionary<JwtHashAlgorithm, Func<byte[], byte[], byte[]>>
            {
                { JwtHashAlgorithm.RS256, (key, value) => { using (var sha = new HMACSHA256(key)) { return sha.ComputeHash(value); } } },
                { JwtHashAlgorithm.HS384, (key, value) => { using (var sha = new HMACSHA384(key)) { return sha.ComputeHash(value); } } },
                { JwtHashAlgorithm.HS512, (key, value) => { using (var sha = new HMACSHA512(key)) { return sha.ComputeHash(value); } } }
            };
      jwtExpDate = DateTime.MinValue;
    }

    public static string GetAuthToken()
    {
      if (DateTime.Now < tokenExpDate) return tokenSave;

      var jwt = GetJWT();
      HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create("https://www.googleapis.com/oauth2/v4/token");
      request.Method = "POST";
      request.ContentType = "application/x-www-form-urlencoded";
      var body = Encoding.UTF8.GetBytes("grant_type=urn%3Aietf%3Aparams%3Aoauth%3Agrant-type%3Ajwt-bearer&assertion=" + jwt);
      request.ContentLength = body.Length;
      var s = request.GetRequestStream();
      s.Write(body, 0, body.Length);
      s.Close();
      string authresponse = "";
      try
      {
        var response = request.GetResponse();
        var rs = response.GetResponseStream();
        var rdr = new StreamReader(rs);
        authresponse = rdr.ReadToEnd();
        rdr.Close();
        rs.Close();
        response.Close();
      }
      catch (HttpException hex)
      {
        throw new Exception("Http Exception from Google Auth API: (" + hex.GetHttpCode() + "): " + hex.Message);
      }
      JObject auth = JObject.Parse(authresponse);
      tokenExpDate = DateTime.Now.AddSeconds(auth["expires_in"].Value<double>());
      tokenSave = auth["access_token"].Value<string>();

      return tokenSave;
    }

    public static string GetJWT()
    {
      if (DateTime.Now < jwtExpDate) return jwtSave;

      var utc0 = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
      var issueTime = DateTime.Now;

      var iat = (int)issueTime.Subtract(utc0).TotalSeconds;
      var exp = (int)issueTime.AddMinutes(55).Subtract(utc0).TotalSeconds;
      jwtExpDate = DateTime.Now.AddMinutes(55);

      var payload = new
      {
        iss = "calapi@frecevents.iam.gserviceaccount.com",
        scope = "https://www.googleapis.com/auth/calendar.readonly",
        aud = "https://accounts.google.com/o/oauth2/token",
        exp = exp,
        iat = iat
      };

      var privateKey = "-----BEGIN PRIVATE KEY-----\nMIIEvAIBADANBgkqhkiG9w0BAQEFAASCBKYwggSiAgEAAoIBAQDIygl3RlyUniPd\nBOuFzKEdUh5tT51hCFEDzDUSaGN1kqpWvsW9U+GnCxQ9vXtUvf1SCipcRULgSlC1\niuxayem2C6+xxcnzSSC0KmjNxXyzTQJDYGz/8CL0Xg/Jc8XEapHLxbRpTgLe4EqM\nqHfGCgUdcJsulTnzWPZ0lYoeQrQJYqBtiXwfW2+7OrFZIlpGlV1i9AGHc9waMo1K\nnje25F3FxpMLM5tUXZhVgy58M0BrgDRSFW/wMh9aWVUJ1oDHuLoxflND21aHBBlK\nD8ViUbk/6sJ+5o0BB5wDZ5x+0qCZP6l+8zfU74/iUtCnmTIYvvM/hG74NxN5ASmN\nPewWvH87AgMBAAECggEALi1IwAWOhR9ZYBshj0PgSb35AWqt5wLintz316PqO7/f\nLNPy5ffEjYYewZQyKOsItzVfSJklKC/vH9EzSi3lcdh+L4QtTaYjv0uBjtE8PIAZ\nZSVn6/RuFYUMXGgjs2hO3244b39haaBrhK253fGv+3VKOwxWp4BuLaG2gf8VKJL9\nOz1g3/Bz75gn16TiHaVzUCNqCYMBAg7wtit1+QFjhPAXQu5085FMefhBoYlli983\nox0vGWbJn61BK/TOL0IX414j87cP91nHlHi2tVdbhyL+VVFpHK202JozGgA0pEGw\nQc6z2mkwLeOiCucv7SXBUxGe5kVeJ8OuYZYfBbm4AQKBgQD7ZtkOtH9QLCwNP/Cj\nmfB35yipKUX8y5aCzx1NiB0/K6HXonjmZ9aNUzWhyqu2KS4ZRmaxTD24d8d8s5V/\nNXXCeuMLderdkG031HqH2YpwT0Hk4LaQ/lBwC4zXqYUPeAtaUuMffwWF98AShSgy\ngU2qqyJGN/uyP/zzYOlGL+l6WwKBgQDMdjQGvKNCV/SygVWxMjj9k1LIHUc9QDT+\n+0v8ljG1mC4JWoAk2wdQY4Nu57HQjlVOpA8uxDHEzhIaN/yfH9hQwjCgGHyjqCgF\nFwE2OtDIJn5gvOaioh/GONJqzv8M+jwdG8ZZ8n/fISH2agekekR3/PY8dJwNTAI0\nc//ZEEFkoQKBgCPIT7y4FCblIqAAKdAfaQqn7DGnnj6M+69Cq3kNlpwXKcH3bh3d\nSbxzy35rymTzF4yhaJxzrZVD9zDYnr6sbUZxFz/aWMOQevsnUwli/UFfBpH9Kf8Z\nM3m/KxzHFBlPjM4eXhVtjuuyh7QbH82Qee2AzjAQZ9LHKMm7UXib+S9hAoGAeP0V\nakskHLUpkpvgFnwOp1cPbGWO61rcQWp5G53RCpFj5JsOK0EFAffxJaarXStspZ2F\ncnocrUX4BlXNAmh4u8k9tu7min2OOPzU9b82HspQjHQb0m83eDfVo+ibmpVVDzCJ\nXl6WdnYHv+YcYaMMcyhYmYuzcFbjEyD1bAAngiECgYAmeX5FnDQq+qyzBJwCgJKf\nQZu75y62JTVj/cfyHjoalKSA5YDw9oZprkgANyuZjuJcYEh5rqqgrUuGfQX8VmDR\nrIeOjnisN8eyw+SctlhTpaJp4ftxBB0kdaW0sMlBDNcMrqvt7hMhjDw7YsSlDU9e\nwH1LZx2xyKWrGJTsS7QVIA==\n-----END PRIVATE KEY-----\n";

      jwtSave = Encode(payload, privateKey, JwtHashAlgorithm.RS256);
      return jwtSave;
    }

    public static string Encode(object payload, string key, JwtHashAlgorithm algorithm)
    {
      return Encode(payload, Encoding.UTF8.GetBytes(key), algorithm);
    }

    public static string Encode(object payload, byte[] keyBytes, JwtHashAlgorithm algorithm)
    {
      var segments = new List<string>();
      var header = new { alg = algorithm.ToString(), typ = "JWT" };

      byte[] headerBytes = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(header, Formatting.None));
      byte[] payloadBytes = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(payload, Formatting.None));

      segments.Add(Base64UrlEncode(headerBytes));
      segments.Add(Base64UrlEncode(payloadBytes));

      var stringToSign = string.Join(".", segments.ToArray());

      var bytesToSign = Encoding.UTF8.GetBytes(stringToSign);

      
      
      byte[] signature = HashAlgorithms[algorithm](keyBytes, bytesToSign);
      segments.Add(Base64UrlEncode(signature));

      return string.Join(".", segments.ToArray());
    }

    public static string Decode(string token, string key)
    {
      return Decode(token, key, true);
    }

    public static string Decode(string token, string key, bool verify)
    {
      var parts = token.Split('.');
      var header = parts[0];
      var payload = parts[1];
      byte[] crypto = Base64UrlDecode(parts[2]);

      var headerJson = Encoding.UTF8.GetString(Base64UrlDecode(header));
      var headerData = JObject.Parse(headerJson);
      var payloadJson = Encoding.UTF8.GetString(Base64UrlDecode(payload));
      var payloadData = JObject.Parse(payloadJson);

      if (verify)
      {
        var bytesToSign = Encoding.UTF8.GetBytes(string.Concat(header, ".", payload));
        var keyBytes = Encoding.UTF8.GetBytes(key);
        var algorithm = (string)headerData["alg"];

        var signature = HashAlgorithms[GetHashAlgorithm(algorithm)](keyBytes, bytesToSign);
        var decodedCrypto = Convert.ToBase64String(crypto);
        var decodedSignature = Convert.ToBase64String(signature);

        if (decodedCrypto != decodedSignature)
        {
          throw new ApplicationException(string.Format("Invalid signature. Expected {0} got {1}", decodedCrypto, decodedSignature));
        }
      }

      return payloadData.ToString();
    }

    private static JwtHashAlgorithm GetHashAlgorithm(string algorithm)
    {
      switch (algorithm)
      {
        case "RS256": return JwtHashAlgorithm.RS256;
        case "HS384": return JwtHashAlgorithm.HS384;
        case "HS512": return JwtHashAlgorithm.HS512;
        default: throw new InvalidOperationException("Algorithm not supported.");
      }
    }

    // from JWT spec
    private static string Base64UrlEncode(byte[] input)
    {
      var output = Convert.ToBase64String(input);
      output = output.Split('=')[0]; // Remove any trailing '='s
      output = output.Replace('+', '-'); // 62nd char of encoding
      output = output.Replace('/', '_'); // 63rd char of encoding
      return output;
    }

    // from JWT spec
    private static byte[] Base64UrlDecode(string input)
    {
      var output = input;
      output = output.Replace('-', '+'); // 62nd char of encoding
      output = output.Replace('_', '/'); // 63rd char of encoding
      switch (output.Length % 4) // Pad with trailing '='s
      {
        case 0: break; // No pad chars in this case
        case 2: output += "=="; break; // Two pad chars
        case 3: output += "="; break; // One pad char
        default: throw new System.Exception("Illegal base64url string!");
      }
      var converted = Convert.FromBase64String(output); // Standard base64 decoder
      return converted;
    }
    #endregion
     */
  }
}