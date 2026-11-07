using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using Newtonsoft.Json;
using UnityEngine;
using Formatting = Newtonsoft.Json.Formatting;

namespace CodeBase.Data
{
  public static class DataExtensions
  {
    public static  Vector3Data AsVectorData(this Vector3 vector) => 
      new Vector3Data(vector.x,vector.y,vector.z);
    
    public static  Vector3 AsUnityVector(this  Vector3Data vector3Data) =>
     new Vector3(vector3Data.X,vector3Data.Y,vector3Data.Z);

    public static Vector3 AddY(this Vector3 vector, float y)
    {
      vector.y += y;
      return vector;
    }
    public static string ToJson(this object obj) 
      => JsonUtility.ToJson(obj);
    public static T ToDeserialized<T>(this string json) 
      => JsonUtility.FromJson<T>(json);

    public static T ToDeserialized<T>(this Dictionary<string, object> dict)
    {
      return JsonUtility.FromJson<T>(dict.ToJson());
    }

    //md5
    public static string ToMD5(this string input)
    {
      using (MD5 md5 = MD5.Create())
      {
        byte[] inputBytes = Encoding.UTF8.GetBytes(input);
        byte[] hashBytes = md5.ComputeHash(inputBytes);
        StringBuilder sb = new StringBuilder();
        foreach (var t in hashBytes)
        {
          sb.Append(t.ToString("X2"));
        }
        return sb.ToString();
      }
    }

    public static string ConvertToJson(this object obj) => JsonConvert.SerializeObject(obj);
        public static Dictionary<string, object> ToComplexDict(this string json) =>
              JsonConvert.DeserializeObject<Dictionary<string, object>>(json);
              
    public static T ToObj<T>(this Dictionary<string, object> dict)
    {
        var serializeObject = JsonConvert.SerializeObject(dict,  Formatting.Indented);
        return serializeObject.ToDeserialized<T>();
    }

    public static Dictionary<string, object> ParseJSON(this string json)
    {
        int end;
        return ParseJSON(json, 0, out end);
    }
    private static Dictionary<string, object> ParseJSON(this string json, int start, out int end)
    {
        Dictionary<string, object> dict = new Dictionary<string, object>();
        bool escbegin = false;
        bool escend = false;
        bool inquotes = false;
        string key = null;
        int cend;
        StringBuilder sb = new StringBuilder();
        Dictionary<string, object> child = null;
        List<object> arraylist = null;
        Regex regex = new Regex(@"\\u([0-9a-z]{4})", RegexOptions.IgnoreCase);
        int autoKey = 0;
        for (int i = start; i < json.Length; i++)
        {
            char c = json[i];
            if (c == '\\') escbegin = !escbegin;
            if (!escbegin)
            {
                if (c == '"')
                {
                    inquotes = !inquotes;
                    if (!inquotes && arraylist != null)
                    {
                        arraylist.Add(DecodeString(regex, sb.ToString()));
                        sb.Length = 0;
                    }
                    continue;
                }
                if (!inquotes)
                {
                    switch (c)
                    {
                        case '{':
                            if (i != start)
                            {
                                child = ParseJSON(json, i, out cend);
                                if (arraylist != null) arraylist.Add(child);
                                else
                                {
                                    dict.Add(key, child);
                                    key = null;
                                }
                                i = cend;
                            }
                            continue;
                        case '}':
                            end = i;
                            if (key != null)
                            {
                                if (arraylist != null) dict.Add(key, arraylist);
                                else dict.Add(key, DecodeString(regex, sb.ToString()));
                            }
                            return dict;
                        case '[':
                            arraylist = new List<object>();
                            continue;
                        case ']':
                            if (key == null)
                            {
                                key = "array" + autoKey.ToString();
                                autoKey++;
                            }
                            if (arraylist != null && sb.Length > 0)
                            {
                                arraylist.Add(sb.ToString());
                                sb.Length = 0;
                            }
                            dict.Add(key, arraylist);
                            arraylist = null;
                            key = null;
                            continue;
                        case ',':
                            if (arraylist == null && key != null)
                            {
                                dict.Add(key, DecodeString(regex, sb.ToString()));
                                key = null;
                                sb.Length = 0;
                            }
                            if (arraylist != null && sb.Length > 0)
                            {
                                arraylist.Add(sb.ToString());
                                sb.Length = 0;
                            }
                           continue;
                        case ':':
                            key = DecodeString(regex, sb.ToString());
                            sb.Length = 0;
                            continue;
                    }
                }
            }
            sb.Append(c);
            if (escend) escbegin = false;
            if (escbegin) escend = true;
            else escend = false;
        }
        end = json.Length - 1;
        return dict; //theoretically shouldn't ever get here
    }
    private static string DecodeString(Regex regex, string str)
    {
        return Regex.Unescape(regex.Replace(str, match => char.ConvertFromUtf32(Int32.Parse(match.Groups[1].Value, System.Globalization.NumberStyles.HexNumber))));
    }
  }
}