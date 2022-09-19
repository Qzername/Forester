using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Forester.ViewModels.AppPanelViewModel;

namespace Forester.ViewModels
{
    public abstract class PageChanger : ViewModelBase
    {
        ViewModelBase _content;
        public ViewModelBase Content
        {
            get => _content;
            set => this.RaiseAndSetIfChanged(ref _content, value);
        }

        ObservableCollection<Page> _pages;
        protected ObservableCollection<Page> Pages { get => _pages; set => this.RaiseAndSetIfChanged(ref _pages, value); }

        public PageChanger()
        {
            Pages = new ObservableCollection<Page>();
        }

        public void ChangePage(Page page)
        {
            int pageID = Pages.IndexOf(page);
            ChangePage(pageID);
        }

        public void ChangePage(int pageID)
        {
            //turn off all pages that are not the chosen one
            for (int i = 0; i < Pages.Count; i++)
            {
                if (i == pageID)
                    continue;

                var copy = Pages[i];
                copy.Color = Page.OffColor;
                Pages[i] = copy;

                Pages[i].Content.PageClosed();
            }

            //turining one that one page
            var turnOnOne = Pages[pageID];
            turnOnOne.Color = Page.OnColor;
            Pages[pageID] = turnOnOne;

            Pages[pageID].Content.PageOpened();
            Content = Pages[pageID].Content.ReceiveContent();
        }
    }
}
