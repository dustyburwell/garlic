using System;
using System.Net;
using System.Text;

namespace Garlic
{
   internal class AnalyticsClient
   {
      private readonly CustomVariableBag m_sessionVariables;

      public AnalyticsClient(string domain, string trackingCode)
      {
         Random randomNumber = new Random();

         m_sessionVariables = new CustomVariableBag();
         Timestamp = ConvertToUnixTimestamp(DateTime.Now).ToString();
         Domain = domain;
         RandomNumber = randomNumber.Next(1000000000).ToString();
         TrackingCode = trackingCode;
      }

      public string Domain { get; set; }
      public string TrackingCode { get; set; }
      public string Timestamp { get; set; }
      public string ReferralSource = "(direct)";
      public string Medium = "(none)";
      public string Campaign = "(direct)";
      public string RandomNumber { get; set; }

      public int DomainHash
      {
         get
         {
            // converted from the google domain hash code listed here:
            // http://www.google.com/support/forum/p/Google+Analytics/thread?tid=626b0e277aaedc3c&hl=en
            int a = 1;
            int c = 0;
            int h;
            char chrCharacter;
            int intCharacter;

            a = 0;
            for (h = Domain.Length - 1; h >= 0; h--)
            {
               chrCharacter = char.Parse(Domain.Substring(h, 1));
               intCharacter = (int)chrCharacter;
               a = (a << 6 & 268435455) + intCharacter + (intCharacter << 14);
               c = a & 266338304;
               a = c != 0 ? a ^ c >> 21 : a;
            }

            return a;
         }
      }

      public string CookieString
      {
         get
         {
            string utma = String.Format("{0}.{1}.{2}.{3}.{4}.{5}",
                                        DomainHash,
                                        RandomNumber,
                                        Timestamp, // timestamp of first visit
                                        Timestamp, // timestamp of previous (most recent visit)
                                        Timestamp, // timestamp of current visit
                                        2);

            //referral informaiton
            string utmz = String.Format("{0}.{1}.{2}.{3}.utmcsr={4}|utmccn={5}|utmcmd={6}",
                                        DomainHash,
                                        Timestamp,
                                        "1",
                                        "1",
                                        ReferralSource,
                                        Campaign,
                                        Medium);

            //return String.Format("__utma%3D{0}.{1}.{2}.{3}.{4}.{5}",)))
            string utmcc = Uri.EscapeDataString(String.Format("__utma={0};+__utmz={1};",
                                                              utma,
                                                              utmz
                                                   ));

            return (utmcc);
         }
      }

      public void SubmitPageView(string page, string title, CustomVariableBag pageVariables)
      {
         var client = BuildBaseWebClient(page, title);

         var variables = m_sessionVariables.MergeWith(pageVariables);

         if (variables.Any())
            client.QueryString["utme"] = variables.ToUtme();

         client.DownloadDataAsync(new Uri("__utm.gif", UriKind.Relative));
      }

      public void SubmitEvent(string page, string title, string category, string action, string label, string value, CustomVariableBag pageVariables)
      {
         var client = BuildBaseWebClient(page, title);

         client.QueryString["utmt"]  = "event";
         client.QueryString["utme"]  = FormatUtme(category, action, label, value);

         var variables = m_sessionVariables.MergeWith(pageVariables);

         if (variables.Any())
            client.QueryString["utme"] += variables.ToUtme();

         client.DownloadDataAsync(new Uri("__utm.gif", UriKind.Relative));
      }

      public void SubmitTiming(string page, string title, string category, string action, int time, string label, CustomVariableBag pageVariables)
      {
         var client = BuildBaseWebClient(page, title);

         client.QueryString["utmt"] = "event";
         client.QueryString["utme"] = FormatTimingUtme(category, action, time, label);

         var variables = m_sessionVariables.MergeWith(pageVariables);

         if (variables.Any())
            client.QueryString["utme"] += variables.ToUtme();

         client.DownloadDataAsync(new Uri("__utm.gif", UriKind.Relative));
      }

      private static string FormatTimingUtme(string category, string variable, int time, string label)
      {
         var builder = new StringBuilder();

         builder.Append("14(90!");
         builder.AppendFormat("{1}*{0}*{2}", category, variable, time);

         if (!string.IsNullOrEmpty(label))
            builder.AppendFormat("*{0}", label);

         builder.AppendFormat(")(90!{0})", time);
         return builder.ToString();
      }

      private WebClient BuildBaseWebClient(string page, string title)
      {
         Random randomNumber = new Random();
         WebClient client = new WebClient();
         client.BaseAddress = "http://www.google-analytics.com/";

         client.QueryString["utmhn"] = Domain;
         client.QueryString["utmcs"] = "UTF-8";
         client.QueryString["utmsr"] = "1280x800";
         client.QueryString["utmvp"] = "1280x800";
         client.QueryString["utmsc"] = "24-bit";
         client.QueryString["utmul"] = "en-us";
         client.QueryString["utmdt"] = title;
         client.QueryString["utmhid"] = randomNumber.Next(1000000000).ToString();
         client.QueryString["utmac"] = TrackingCode;
         client.QueryString["utmn"] = randomNumber.Next(1000000000).ToString();
         client.QueryString["utmr"] = "-";
         client.QueryString["utmp"] = page;
         client.QueryString["utmwv"] = "5.3.5";
         client.QueryString["utmcc"] = CookieString;

         return client;
      }

      private static string FormatUtme(string category, string action, string label, string value)
      {
         StringBuilder builder = new StringBuilder();
         builder.AppendFormat("5({0}*{1}", category, action);

         if (!string.IsNullOrEmpty(label))
            builder.AppendFormat("*{0}", label);

         builder.Append(")");
         
         if (!string.IsNullOrEmpty(value))
            builder.AppendFormat("({0})", value);

         return builder.ToString();
      }

      private static int ConvertToUnixTimestamp(DateTime value)
      {
         TimeSpan span = (value - new DateTime(1970, 1, 1, 0, 0, 0, 0).ToLocalTime());
         return (int)span.TotalSeconds;
      }

      public void SetCustomVariable(int position, string key, string value)
      {
         m_sessionVariables.Set(position, key, value);
      }

      public void ClearCustomVariable(int position)
      {
         m_sessionVariables.Clear(position);
      }
   }
}
