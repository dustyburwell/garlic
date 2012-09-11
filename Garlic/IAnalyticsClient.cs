namespace Garlic
{
   internal interface IAnalyticsClient
   {
      string Domain { get; set; }
      
      string TrackingCode { get; set; }
      
      string UserAgent { get; set; }
      
      void SubmitPageView(string page, string title, CustomVariableBag pageVariables);
      
      void SubmitEvent(string page, string title, string category, string action, string label, string value, CustomVariableBag pageVariables);
      
      void SubmitTiming(string page, string title, string category, string action, int time, string label, CustomVariableBag pageVariables);
      
      void SetCustomVariable(int position, string key, string value);

      void ClearCustomVariable(int position);
   }
}