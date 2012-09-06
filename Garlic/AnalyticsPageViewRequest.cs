namespace Garlic
{
   public class AnalyticsPageViewRequest
   {
      private readonly AnalyticsClient m_analyticsClient;
      private readonly string m_page;
      private readonly string m_title;
      private readonly CustomVariableBag m_customVariables;

      internal AnalyticsPageViewRequest(AnalyticsClient analyticsClient, string page, string title)
      {
         m_analyticsClient = analyticsClient;
         m_page = page;
         m_title = title;
         m_customVariables = new CustomVariableBag();
      }

      public void Send()
      {
         m_analyticsClient.SubmitPageView(m_page, m_title, m_customVariables);
      }

      public void SendEvent(string category, string action, string label, string value)
      {
         m_analyticsClient.SubmitEvent(m_page, m_title, category, action, label, value, m_customVariables);
      }

      public ITiming StartTiming(string category, string variable, string label)
      {
         return new Timing(this, category, variable, label);
      }

      public void SendTiming(string category, string variable, int time, string label)
      {
         m_analyticsClient.SubmitTiming(m_page, m_title, category, variable, time, label, m_customVariables);
      }

      public void SetCustomVariable(int position, string key, string value)
      {
         m_customVariables.Set(position, key, value);
      }

      public void ClearCustomVariable(int position)
      {
         m_customVariables.Clear(position);
      }
   }
}