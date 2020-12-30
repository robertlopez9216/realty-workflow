﻿using System;
using System.Threading.Tasks;
using Abp;
using Abp.Dependency;
using Abp.Events.Bus.Handlers;
using Abp.Localization;
using Abp.Localization.Sources;
using Abp.RealTime;
using Realty.Forms;
using Realty.Forms.Events;
using Realty.Libraries;
using Realty.Signings;
using Realty.Transactions;

namespace Realty.Documents.Handlers
{
    public class FormStatusChangedEventHandler: 
        IAsyncEventHandler<FormStatusChangedEventData>,
        ITransientDependency
    {
        private const string LocalizationSourceName = RealtyConsts.LocalizationSourceName;

        private readonly IOnlineClientManager<DocumentChannel> _onlineClientManager;
        private readonly IDocumentNotifier _communicator;
        private readonly DocumentNotificationFactory _notificationFactory;

        private readonly ILocalizationSource _localizationSource;

        public FormStatusChangedEventHandler(
            IDocumentNotifier communicator, 
            IOnlineClientManager<DocumentChannel> onlineClientManager, 
            DocumentNotificationFactory notificationFactory, 
            ILocalizationManager localizationManager)
        {
            _communicator = communicator;
            _onlineClientManager = onlineClientManager;
            _notificationFactory = notificationFactory;

            _localizationSource = localizationManager.GetSource(LocalizationSourceName);
        }

        public async Task HandleEventAsync(FormStatusChangedEventData eventData)
        {
            switch (eventData.Parent)
            {
                case Library library:
                    await HandleEventAsync(library, eventData.Entity);
                    break;
                case Transaction transaction:
                    HandleEventAsync(transaction, eventData.Entity);
                    break;
                case Signing signing:
                    HandleEventAsync(signing, eventData.Entity);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(eventData.Parent));
            }
        }
        
        private async Task HandleEventAsync(Library library, Form form)
        {
            var message = GetStatusChangeNotificationMessage(form);
            await DispatchNotification(library, form, message);
        }

        private void HandleEventAsync(Transaction transaction, Form form)
        {
            // Add custom functionality here
        }

        private void HandleEventAsync(Signing signing, Form form)
        {
            // Add custom functionality here
        }

        private async Task DispatchNotification(Library library, Form form, string message)
        {
            if (!form.CreatorUserId.HasValue) return;

            var userIdentifier = new UserIdentifier(form.TenantId, form.CreatorUserId.Value);
            var clients = _onlineClientManager.GetAllByUserId(userIdentifier);

            var notification = _notificationFactory.Create(library, form, message);
            await _communicator.SendDocumentStatusChangedToClient(clients, notification);
        }

        private string GetStatusChangeNotificationMessage(Form form)
        {
            return form.Status switch
            {
                FormStatus.Processing => L("Notification_LibraryForm_ProcessingStarted", form.Name),
                FormStatus.Ready => L("Notification_LibraryForm_ProcessingCompleted", form.Name),
                _ => throw new ArgumentOutOfRangeException()
            };
        }

        private string L(string name) => _localizationSource.GetString(name);

        private string L(string name, params object[] args) => _localizationSource.GetString(name, args);
    }
}
