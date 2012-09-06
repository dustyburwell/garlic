using System;
using System.Threading;

namespace Garlic
{
   public class Timing : IDisposable, ITiming
   {
      private readonly AnalyticsPageViewRequest m_request;
      private readonly string m_category;
      private readonly string m_variable;
      private readonly string m_label;
      private DateTime m_start;
      private bool m_stopped;
      private ReaderWriterLockSlim m_lockSlim = new ReaderWriterLockSlim();

      public Timing(AnalyticsPageViewRequest request, string category, string variable, string label)
      {
         m_request = request;
         m_category = category;
         m_variable = variable;
         m_label = label;
         m_start = DateTime.Now;
      }

      public void Finish()
      {
         try
         {
            m_lockSlim.EnterUpgradeableReadLock();

            if (m_stopped)
            {
               return;
            }

            m_lockSlim.EnterWriteLock();
            m_stopped = true;
         }
         finally
         {
            if (m_lockSlim.IsWriteLockHeld)
            {
               m_lockSlim.ExitWriteLock();
            }

            m_lockSlim.ExitUpgradeableReadLock();
         }

         m_request.SendTiming(m_category, m_variable, (int)(DateTime.Now - m_start).TotalMilliseconds, m_label);
      }

      void IDisposable.Dispose()
      {
         Finish();
      }
   }
}