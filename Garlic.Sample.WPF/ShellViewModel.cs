using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Threading;
using Caliburn.Micro;

namespace Garlic.Sample.WPF
{
   [Export(typeof(IShell))]
   public class ShellViewModel : Conductor<object>.Collection.OneActive, IShell
   {
      [Import(typeof(AnalyticsSession))]
      public AnalyticsSession Session { get; set; }

      protected override void OnInitialize()
      {
         Session.SetCustomVariable(1, "SFTK_KS", "ascalonx");

         var page = Session.CreatePageViewRequest("/", "");
         page.SetCustomVariable(2, "CUSTOM", "variable");

         page.Track(this);

         Items.Add(new OneViewModel(Session));
         Items.Add(new TwoViewModel(Session));
         Items.Add(new ThreeViewModel(Session));
         Items.Add(new FourViewModel(Session));
      }
   }

   public static class CaliburnAnalyticsExtensions
   {
      private static readonly IDictionary<Screen, EventHandler<ActivationEventArgs>> m_screens =
         new Dictionary<Screen, EventHandler<ActivationEventArgs>>();

      public static void Track(this AnalyticsPageViewRequest request, Screen screen)
      {
         EventHandler<ActivationEventArgs> activated = (s, e) => ScreenActivated(request);
         m_screens.Add(screen, activated);
         screen.Activated += activated;
      }

      public static void Untrack(this AnalyticsPageViewRequest request, Screen screen)
      {
         screen.Activated -= m_screens[screen];
         m_screens.Remove(screen);
      }

      private static void ScreenActivated(AnalyticsPageViewRequest request)
      {
         request.Send();
      }
   }

   public class FourViewModel : Screen
   {
      private readonly AnalyticsPageViewRequest m_page;

      public FourViewModel(AnalyticsSession analytics)
      {
         m_page = analytics.CreatePageViewRequest("/four", "");
         m_page.Track(this);
      }

      public override string DisplayName
      {
         get { return "four"; }
         set { }
      }

      public void DoThing()
      {
         var timing = m_page.StartTiming("Do", "Thing", "");
         Thread.Sleep(10);
         timing.Finish();
      }
   }

   public class ThreeViewModel : Screen
   {
      public ThreeViewModel(AnalyticsSession analytics)
      {
         var page = analytics.CreatePageViewRequest("/three", "");
         page.Track(this);
      }

      public override string DisplayName
      {
         get { return "three"; } 
         set { }
      }
   }

   public class TwoViewModel : Screen
   {
      private readonly AnalyticsPageViewRequest m_page;

      public TwoViewModel(AnalyticsSession analytics)
      {
         m_page = analytics.CreatePageViewRequest("/two", "");
         m_page.Track(this);
      }

      public override string DisplayName
      {
         get { return "two"; }
         set { }
      }

      public void DoThing()
      {
         m_page.SendTiming("Do", "Thing", 1000, "");
      }
   }

   public class OneViewModel : Screen
   {
      private readonly AnalyticsPageViewRequest m_page; 

      public OneViewModel(AnalyticsSession analytics)
      {
         m_page = analytics.CreatePageViewRequest("/one", "");
         m_page.Track(this);
      }

      public override string DisplayName
      {
         get { return "one"; }
         set { }
      }

      public void DoThing()
      {
         m_page.SendEvent("Do", "thing", "", "");
      }
   }
}