using System;
using System.Collections.Generic;
using System.Linq;
using AgentHub.Entities;
using AgentHub.Entities.Models.Application;
using AgentHub.Entities.Repositories;
using AgentHub.Entities.Utilities;

namespace AgentHub.Service
{
    public class PageService: BaseService<PageItem>
    {
        public PageService(IRepositoryAsync<PageItem> pageItemRepository)
            : base(pageItemRepository)
        {
            _pages = null;
        }

        private static PageItem _currentPageItem = null;
        public static PageItem CurrentPageItem
        {
            get
            {
                return _currentPageItem;
            }
        }

        private static IList<PageItem> _pages = null;
        private static IEnumerable<PageItem> Pages
        {
            get
            {
                if (_pages == null || AppSettings.CachePageItems)
                {
                    var pageItemRepository = ObjectFactory.GetInstance<IRepositoryAsync<PageItem>>();
                    _pages = pageItemRepository.Queryable().ToList();
                }

                return _pages;
            }
        }

        public static PageItem GetPageByFriendlyUrl(string friendlyUrl)
        {
            if (string.IsNullOrEmpty(friendlyUrl) || friendlyUrl == "null")
                friendlyUrl = "Home";

            _currentPageItem = Pages.FirstOrDefault(_ => _.FriendlyUrl.Equals(friendlyUrl, StringComparison.CurrentCultureIgnoreCase)) ??
                       new PageItem() {ID = int.MaxValue, ControllerName = "Home", ActionName = "Index", FriendlyUrl = "Home", Title = StringTable.HomePageTitle, Description = StringTable.HomePageDescription};

            return _currentPageItem;
        }
    }
}
