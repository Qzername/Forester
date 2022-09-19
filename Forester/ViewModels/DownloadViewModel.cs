using Forester.Code;
using Forester.Models.API;
using Forester.ViewModels.Download;
using ReactiveUI;
using System;
using System.Collections.Generic;

namespace Forester.ViewModels
{
    internal class DownloadViewModel : ViewModelBase, IPage
    {
        bool _isEmpty;

        DownloadQueueElement? CurrentDownloading;
        List<DownloadQueueElement> Queue;

        public DownloadViewModel()
        {
            Queue = new List<DownloadQueueElement>();
        }

        public bool IsEmpty
        {
            get => _isEmpty;
            private set => this.RaiseAndSetIfChanged(ref _isEmpty, value);
        }

        public void AddToQueue(Application app, DownloadType type) => AddToQueue(app, type, new Action(() => { }));

        public void AddToQueue(Application app, DownloadType type, Action finished)
        {
            DownloadQueueElement Element = new DownloadQueueElement()
            {
                Application = app,
                DownloadFinished = finished,
                DownloadType = type,
                Size = 0
            };



        }

        //debug
        public void SampleToQueueAdd()
        {
           
        }

        //IPage
        public void PageClosed()
        {
        }

        public void PageOpened()
        {
        }

        public ViewModelBase ReceiveContent()
        {
            return this;
        }
    }

}
