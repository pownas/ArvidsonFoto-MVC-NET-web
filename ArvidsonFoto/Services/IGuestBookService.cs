using ArvidsonFoto.Models;
using System.Collections.Generic;

namespace ArvidsonFoto.Services
{
    public interface IGuestBookService
    {
        bool CreateGBpost(TblGb gb);
        
        int GetLastGbId();
        
        List<TblGb> GetAll();
    }
}