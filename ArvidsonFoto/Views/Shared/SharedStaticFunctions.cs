using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using System;
using System.Globalization;

namespace ArvidsonFoto
{
    public static class SharedStaticFunctions
    {
        public static int GetWeekNumber(DateTime date)
        {
            //Code from: https://docs.microsoft.com/en-us/dotnet/api/system.globalization.calendar.getweekofyear?view=net-5.0

            // Gets the Calendar instance associated with a CultureInfo.
            CultureInfo myCI = new CultureInfo("sv-SE");
            Calendar myCal = myCI.Calendar;

            // Gets the DTFI properties required by GetWeekOfYear.
            CalendarWeekRule myCWR = myCI.DateTimeFormat.CalendarWeekRule;
            DayOfWeek myFirstDOW = myCI.DateTimeFormat.FirstDayOfWeek;

            // Displays the number of the current week relative to the beginning of the year.
            //Console.WriteLine("The CalendarWeekRule used for the en-US culture is {0}.", myCWR);
            //Console.WriteLine("The FirstDayOfWeek used for the en-US culture is {0}.", myFirstDOW);
            //Console.WriteLine("Therefore, the current week is Week {0} of the current year.", myCal.GetWeekOfYear(DateTime.Now, myCWR, myFirstDOW));

            // Displays the total number of weeks in the current year.
            //DateTime LastDay = new System.DateTime(DateTime.Now.Year, 12, 31);
            //Console.WriteLine("There are {0} weeks in the current year ({1}).", myCal.GetWeekOfYear(LastDay, myCWR, myFirstDOW), LastDay.Year);

            return myCal.GetWeekOfYear(date, myCWR, myFirstDOW);
        }

        public static string ReplaceAAO(string replaceAAO)
        {
            replaceAAO = replaceAAO.Replace("Ã¥", "å");
            replaceAAO = replaceAAO.Replace("Ã¤", "ä");
            replaceAAO = replaceAAO.Replace("Ã¶", "ö");
            replaceAAO = replaceAAO.Replace("%C3%83%C2%B6", "ö");
            replaceAAO = replaceAAO.Replace("Ã…", "Å");
            replaceAAO = replaceAAO.Replace("Ã„", "Ä");
            replaceAAO = replaceAAO.Replace("Ã–", "Ö");
            
            return replaceAAO;
        }
    }

    public static class HttpRequestExtensions
    {
        public static string GetRawUrl(this HttpContext httpContext)
        {
            var requestFeature = httpContext.Features.Get<IHttpRequestFeature>();
            return requestFeature.RawTarget.ToString();
        }
    }

    ////Original-Code above from: https://stackoverflow.com/a/38747631/14036841
    //public static class HttpRequestExtensions
    //{
    //    public static Uri GetRawUrl(this HttpRequest request)
    //    {
    //        var httpContext = request.HttpContext;

    //        var requestFeature = httpContext.Features.Get<IHttpRequestFeature>();

    //        return new Uri(requestFeature.RawTarget);
    //    }
    //}
}