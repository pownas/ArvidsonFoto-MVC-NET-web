using Microsoft.AspNetCore.Mvc.Rendering;

namespace ArvidsonFoto.Views.Shared;

public static class UploadAdminNavPages
{
    public static string Index => "Index";
    public static string IndexNavClass(ViewContext viewContext) => UploadPageNavClass(viewContext, Index);

    public static string NyBild => "NyBild";
    public static string NyBildNavClass(ViewContext viewContext) => UploadPageNavClass(viewContext, NyBild);

    public static string NyKategori => "NyKategori";
    public static string NyKategoriNavClass(ViewContext viewContext) => UploadPageNavClass(viewContext, NyKategori);

    public static string RedigeraBilder => "RedigeraBilder";
    public static string RedigeraBilderNavClass(ViewContext viewContext) => UploadPageNavClass(viewContext, RedigeraBilder);

    public static string HanteraGB => "HanteraGB";
    public static string HanteraGBNavClass(ViewContext viewContext) => UploadPageNavClass(viewContext, HanteraGB);

    public static string Statistik => "Statistik";
    public static string StatistikNavClass(ViewContext viewContext) => UploadPageNavClass(viewContext, Statistik);

    public static string VisaLoggboken => "VisaLoggboken";
    public static string VisaLoggbokenNavClass(ViewContext viewContext) => UploadPageNavClass(viewContext, VisaLoggboken);

    public static string HanteraKontot => "HanteraKontot";
    public static string HanteraKontotNavClass(ViewContext viewContext) => UploadPageNavClass(viewContext, HanteraKontot);

    public static string LoggaUt => "LoggaUt";
    public static string LoggaUtNavClass(ViewContext viewContext) => UploadPageNavClass(viewContext, LoggaUt);



    private static string UploadPageNavClass(ViewContext viewContext, string page)
    {
        var activePage = viewContext.ViewData["ActiveUploadPage"] as string
            ?? System.IO.Path.GetFileNameWithoutExtension(viewContext.ActionDescriptor.DisplayName);
        return string.Equals(activePage, page, StringComparison.OrdinalIgnoreCase) ? "active" : null;
    }
}