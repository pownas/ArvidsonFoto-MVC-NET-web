using ArvidsonFoto.Models;

namespace ArvidsonFoto.Services
{
    public interface IPageCounterService
    {
        void AddPageCount(string pageName);
    }
}