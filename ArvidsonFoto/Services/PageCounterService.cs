using ArvidsonFoto.Data;
using ArvidsonFoto.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Serilog;

namespace ArvidsonFoto.Services
{
    public class PageCounterService : IPageCounterService
    {
        private readonly ArvidsonFotoDbContext _entityContext;
        public PageCounterService(ArvidsonFotoDbContext context)
        {
            _entityContext = context;
        }

        public void AddPageCount(string pageName)
        {
            string monthViewed = DateTime.Now.ToString("yyyy-MM");
            List<TblPageCounter> tblPageCounters = _entityContext.TblPageCounter.ToList();
            try
            {
                bool notExist = true;
                foreach (var item in tblPageCounters)
                {
                    if (item.PageName.Equals(pageName) && item.MonthViewed.Equals(monthViewed))
                    { 
                        notExist = false;
                        item.PageViews += 1;
                        item.LastShowDate = DateTime.Now;
                    }
                }

                if (notExist)
                {
                    TblPageCounter pageCounter = new TblPageCounter() { 
                        MonthViewed = monthViewed,
                        PageName = pageName,
                        PageViews = 1,
                        LastShowDate = DateTime.Now
                    };
                    _entityContext.TblPageCounter.Add(pageCounter);
                }
                
                _entityContext.SaveChanges();
            }
            catch (Exception ex)
            {
                Log.Error("Error while updating PageCounter. Error-message: " + ex.Message);
            }
        }
    }
}