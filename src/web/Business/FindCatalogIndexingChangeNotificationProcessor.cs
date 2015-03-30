using System;
using System.Collections.Generic;
using System.Threading;
using EPiServer.Events.ChangeNotification;
using EPiServer.Logging;
using Mediachase.Commerce.Catalog;

namespace OxxCommerceStarterKit.Web.Business
{
    public class FindCatalogIndexingChangeNotificationProcessor : IChangeProcessor<string>
    {
        public FindCatalogIndexingChangeNotificationProcessor()
        {
            _log.Debug("FindCatalogIndexingChangeNotificationProcessor initialized.");
        }

        private class CatalogEntryChangeListener : IChangeListener<CatalogEntryChange, string>
        {
            protected static ILogger _log = LogManager.GetLogger();

            public IEnumerable<string> NotifyChange(CatalogEntryChange before, CatalogEntryChange after)
            {
                if (before != null)
                {
                    _log.Debug("NotifyChange before: {0}", before.GetQueueableString());
                    yield return before.GetQueueableString();
                    if (after != null && !before.Equals(after))
                    {
                        _log.Debug("NotifyChange after: {0}", after.GetQueueableString());
                        yield return after.GetQueueableString();
                    }
                }
                else if (after != null)
                {
                    _log.Debug("NotifyChange after: {0}", after.GetQueueableString());
                    yield return after.GetQueueableString();
                }
            }
        }

        protected static ILogger _log = LogManager.GetLogger();
        private const string _displayName = "Find Catalog Indexer";
        private static readonly Guid _processorId = new Guid("ea403de2-c91d-4675-a82e-ac087ea368de");
        private static readonly IChangeListener _changeListener = new CatalogEntryChangeListener();
        private static readonly TimeSpan _retryInterval = TimeSpan.FromSeconds(30);
        private const int _maxBatchSize = 30;
        private DateTime _lastLogTime = DateTime.Now;

        public static Guid ProcessorId
        {
            get { return _processorId; }
        }

        Guid IChangeProcessor.ProcessorId
        {
            get { return _processorId; }
        }

        public string Name
        {
            get { return _displayName; }
        }

        public int MaxBatchSize
        {
            get
            {
                // By default, it logs every 5 sec. We'll do this
                // once a minute while in debug mode, just to keep
                // the logs smaller.
                if(_log.IsDebugEnabled())
                {
                    TimeSpan span = DateTime.Now - _lastLogTime;
                    if(span.TotalSeconds > 60)
                    {
                        _log.Debug("MaxBatchSize: {0}", _maxBatchSize);
                        _lastLogTime = DateTime.Now;
                    }
                }
                return _maxBatchSize;
            }
        }

        public int MaxRetryCount
        {
            get
            {
                _log.Debug("MaxRetryCount: 10");
                return 10;
            }
        }

        public TimeSpan RetryInterval
        {
            get
            {
                _log.Debug("RetryInterval: 30 sec");
                return _retryInterval;
            }
        }

        public IEnumerable<IChangeListener> Listeners
        {
            get { yield return _changeListener; }
        }


        public bool RecoverConsistency(IRecoveryContext recoveryContext)
        {
            _log.Debug("RecoverConsistency called");
            // recoveryContext.SetActivity();
            recoveryContext.SetProgress(1, 1);

            return true;
        }


        public bool ProcessItems(IEnumerable<string> queuedStrings, CancellationToken cancellationToken)
        {
            _log.Debug("ProcessItems called");

            foreach (string queuedString in queuedStrings)
            {
                CatalogEntryChange change = new CatalogEntryChange(queuedString);
                _log.Debug("Process Items - Code: {0}, Entry ID: {1}", change.CatalogEntryCode, change.CatalogEntryId);
            }

            var result = true;

            return result;
        }
    }
}
