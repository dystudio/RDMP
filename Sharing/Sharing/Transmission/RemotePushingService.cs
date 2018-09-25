﻿using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using CatalogueLibrary.Data;
using CatalogueLibrary.Data.ImportExport;
using CatalogueLibrary.Data.Remoting;
using CatalogueLibrary.Data.Serialization;
using CatalogueLibrary.Repositories;
using MapsDirectlyToDatabaseTable;
using ReusableLibraryCode.Progress;
using Sharing.Dependency.Gathering;

namespace Sharing.Transmission
{
    /// <summary>
    /// Serializes collections of RDMP objects into BINARY Json and streams to a RemoteRDMP endpoint.
    /// </summary>
    public class RemotePushingService
    {
        private readonly IRDMPPlatformRepositoryServiceLocator _repositoryLocator;
        private readonly IDataLoadEventListener listener;
        private readonly IEnumerable<RemoteRDMP> remotes;
        private Gatherer _gatherer;
        private ShareManager _shareManager;

        public RemotePushingService(IRDMPPlatformRepositoryServiceLocator repositoryLocator, IDataLoadEventListener listener)
        {
            _repositoryLocator = repositoryLocator;
            this.listener = listener;
            remotes = _repositoryLocator.CatalogueRepository.GetAllObjects<RemoteRDMP>();
            _gatherer = new Gatherer(_repositoryLocator);
            _shareManager = new ShareManager(_repositoryLocator);
        }

        public async void SendToAllRemotes<T>(T[] toSendAll, Action callback = null) where  T:IMapsDirectlyToDatabaseTable
        {
            listener.OnNotify(this, new NotifyEventArgs(ProgressEventType.Information, "Ready to send " + toSendAll.Length + " " + typeof(T).Name + " items to all remotes."));
            var done = new Dictionary<string, int>();

            foreach (var remoteRDMP in remotes)
            {
                listener.OnProgress(this, new ProgressEventArgs(remoteRDMP.Name, new ProgressMeasurement(0, ProgressType.Records, toSendAll.Length), new TimeSpan()));
            }

            var tasks = new List<Task>();

            foreach (var remote in remotes)
            {
                done.Add(remote.Name, 0);
                    
                foreach (var toSend in toSendAll)
                {
                    if(!_gatherer.CanGatherDependencies(toSend))
                        throw new Exception("Type " + typeof(T)  + " is not supported yet by Gatherer and therefore cannot be shared");

                    var share = _gatherer.GatherDependencies(toSend).ToShareDefinitionWithChildren(_shareManager);
                    var json = JsonConvertExtensions.SerializeObject(share, _repositoryLocator);

                    var handler = new HttpClientHandler()
                    {
                        Credentials = new NetworkCredential(remote.Username, remote.GetDecryptedPassword())
                    };

                    HttpResponseMessage result;

                    var apiUrl = remote.GetUrlFor<T>();

                    RemoteRDMP remote1 = remote;
                    T toSend1 = toSend;
                               
                    var sender = new Task(() =>
                    {
                        using (var client = new HttpClient(handler))
                        {
                            try
                            {
                                result = client.PostAsync(new Uri(apiUrl), new StringContent(json, Encoding.UTF8, "text/plain")).Result;
                                if (result.IsSuccessStatusCode)
                                    listener.OnNotify(this, new NotifyEventArgs(ProgressEventType.Information, "Sending " + toSend1 + " to " + remote1.Name + " completed."));
                                else
                                    listener.OnNotify(this, new NotifyEventArgs(ProgressEventType.Error, 
                                                                                    "Error sending " + toSend1 + " to " + remote1.Name + ": " + 
                                                                                    result.ReasonPhrase + " - " + 
                                                                                    result.Content.ReadAsStringAsync().Result));
                                lock (done)
                                {
                                    listener.OnProgress(this, new ProgressEventArgs(remote1.Name, new ProgressMeasurement(++done[remote1.Name], ProgressType.Records, toSendAll.Length), new TimeSpan()));
                                }
                            }
                            catch (Exception ex)
                            {
                                listener.OnNotify(this, new NotifyEventArgs(ProgressEventType.Error, "Error sending " + toSend1 + " to " + remote1.Name, ex));
                                listener.OnProgress(this, new ProgressEventArgs(remote1.Name, new ProgressMeasurement(1, ProgressType.Records, 1), new TimeSpan()));
                            }
                        }
                    });
                    sender.Start();
                    tasks.Add(sender);
                }
            }

            await Task.WhenAll(tasks);

            if (callback != null)
                callback();
        }
    }
}