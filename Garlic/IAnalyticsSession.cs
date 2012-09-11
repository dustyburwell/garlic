namespace Garlic
{
   public interface IAnalyticsSession
   {
      IAnalyticsPageViewRequest CreatePageViewRequest(string page, string title);
      
      void SetCustomVariable(int position, string key, string value);
      
      string UserAgent { get; set; }
   }
}