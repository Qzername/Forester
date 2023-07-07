using Avalonia.Collections;
using Avalonia.Media;
using Forester.Models.App;
using Forester.Services;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Forester.ViewModels
{
    public abstract class PageChanger : ViewModelBase
    {
        protected AvaloniaList<Page> pages { get; set; }
        protected ThemeService theme { get; set; }

        public PageChanger()
        {
            theme = GetService<ThemeService>(); 

            pages = new AvaloniaList<Page>();
        }

        public void SwitchPage(Page page)
        {
            int pageID = pages.IndexOf(page);
            SwitchPage(pageID);
        }

        public void SwitchPage(string name)
        {
            SwitchPage(pages.Single(x => x.Name == name));
        }

        public void SwitchPageInView(object obj)
        {
            SwitchPage((Page)obj);
        }

        void SwitchPage(int pageID)
        {
            //turn off all pages that are not the chosen one
            for (int i = 0; i < pages.Count; i++)
            {
                if (i == pageID)
                    continue;

                var copy = pages[i];
                copy.Color = theme.Orange;
                pages[i] = copy;
            }

            //turining one that one page
            var turnOnOne = pages[pageID];
            turnOnOne.Color = theme.FirstBrush;
            pages[pageID] = turnOnOne;

            SwitchedPage(turnOnOne);
        }

        public abstract void SwitchedPage(Page page);
    }
}
