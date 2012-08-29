namespace Garlic
{
   public class AnalyticsSession
   {
      private readonly AnalyticsClient m_analyticsClient; 

      public AnalyticsSession(string domain, string trackingCode)
      {
         m_analyticsClient = new AnalyticsClient(domain, trackingCode);
      }

      public AnalyticsPageViewRequest CreatePageViewRequest(string page, string title)
      {
         return new AnalyticsPageViewRequest(m_analyticsClient, page, title);
      }

      public void SetCustomVariable(int position, string key, string value)
      {
         m_analyticsClient.SetCustomVariable(position, key, value);
      }
   }
}