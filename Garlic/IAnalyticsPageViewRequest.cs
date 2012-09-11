namespace Garlic
{
   public interface IAnalyticsPageViewRequest
   {
      void Send();

      void SendEvent(string category, string action, string label, string value);
      
      ITiming StartTiming(string category, string variable, string label);
      
      void SendTiming(string category, string variable, int time, string label);
      
      void SetCustomVariable(int position, string key, string value);
      
      void ClearCustomVariable(int position);
   }
}