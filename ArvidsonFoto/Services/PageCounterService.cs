using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ArvidsonFoto.Data;
using ArvidsonFoto.Models;
using Microsoft.EntityFrameworkCore;
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

        /// <summary>
        /// En funktion som hämtar månadens sidvisningar. Exempel på input: "2021-02"
        /// </summary>
        /// <param name="YearMonth">Tar en DateTime likt "2021-02", exempel: DateTime.Now.ToString("yyyy-MM")</param>
        /// <returns></returns>
        public List<TblPageCounter> GetMonthCount(string yearMonth)
        {
            List<TblPageCounter> listToReturn = new List<TblPageCounter>();
            listToReturn = _entityContext.TblPageCounter
                                         .Where(p => p.MonthViewed.Equals(yearMonth))
                                         .OrderByDescending(p => p.PageViews)
                                         .ToList();
            return listToReturn;
        }

        public List<TblPageCounter> GetAllGroupedByPageCount()
        {
            List<TblPageCounter> listToReturn = new List<TblPageCounter>();
            var listOfPages = _entityContext.TblPageCounter
                                            .Select(p => p.PageName)
                                            .Distinct()
                                            .ToList();

            for (int i = 0; i < listOfPages.Count; i++)
            {
                TblPageCounter aCountedPage = new TblPageCounter()
                {
                    Id = i+1,
                    PageName = listOfPages[i],
                    PageViews = _entityContext.TblPageCounter.Where(p => p.PageName.Equals(listOfPages[i])).Sum(p => p.PageViews),
                    LastShowDate = _entityContext.TblPageCounter.Where(p => p.PageName.Equals(listOfPages[i])).Max(p => p.LastShowDate)
                };

                listToReturn.Add(aCountedPage);
            }

            //var SQLquery = "SELECT SUM(PageCounter_Views) AS PageCounter_Views, PageCounter_Name, MAX(PageCounter_LastShowDate) AS PageCounter_LastShowDate FROM tbl_PageCounter GROUP BY PageCounter_Name ORDER BY PageCounter_Views DESC";
            //var groupedList = _entityContext.TblPageCounter.FromSqlRaw(SQLquery).ToList();

            return listToReturn.OrderByDescending(p => p.PageViews).ToList();
        }
    }
}