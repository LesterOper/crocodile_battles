using UnityEngine;

namespace CodeBase
{
  public static class Constants
  {
    public const float Epsilon = 0.001f;
    
    public const string INSTA = "https://www.instagram.com/alexplaynet_official/";
    public const string VK = "https://vk.com/alexplay_net";
    public const string FB = "https://www.facebook.com/groups/1592033661110248";
    public const string YOUTUBE = "https://www.youtube.com/channel/UC3T-TUKxos87BUdZMs1EUUw";
    public const string MAIL = "";

    public static string GetMail()
    {
      var mailToString = "mailto:support@alexplay.net" +
                         "?" +
                         "subject=" +
                         "Crocodile%20Battles" +
                         "&" +
                         "body=" +
                         "%0D%0A%0D%0A%0D%0A" +
                         "%0D%0A%20Device:%20" + SystemInfo.deviceModel + ",%20" +
                         "%0D%0A%20OS%20version:%20" + SystemInfo.operatingSystem + ",%20" +
                         "%0D%0A%20App%20version:%20" + Application.version + ",%20" +
                         "%0D%0A%20Country:%20" +   PlayerPrefs.GetString("country_code")+ ",%20" +
                         "%0D%0A%20" + ((Application.platform == RuntimePlatform.Android) ? "adid" : "idfa") +
                         ":%20" + IronSource.Agent.getAdvertiserId();
      var replace = mailToString.Replace(" ", "%20");
      //Application.OpenURL(replace);
      return replace;
    }

    public const string SOUND = "SOUND";
    public const string GDPR = "GDPR";
  }
}