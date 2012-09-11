namespace Garlic
{
   public class AnalyticsSession : IAnalyticsSession
   {
      private readonly AnalyticsClient m_analyticsClient; 

      public AnalyticsSession(string domain, string trackingCode)
      {
         m_analyticsClient = new AnalyticsClient(domain, trackingCode);
      }

      public IAnalyticsPageViewRequest CreatePageViewRequest(string page, string title)
      {
         return new AnalyticsPageViewRequest(m_analyticsClient, page, title);
      }

      public void SetCustomVariable(int position, string key, string value)
      {
         m_analyticsClient.SetCustomVariable(position, key, value);
      }

      public string UserAgent
      {
         get
         {
            return m_analyticsClient.UserAgent;
         }
         set
         {
            m_analyticsClient.UserAgent = value;
         }
      }
   }
}