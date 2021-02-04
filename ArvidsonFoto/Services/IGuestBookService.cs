using ArvidsonFoto.Models;
using System.Collections.Generic;

namespace ArvidsonFoto.Services
{
    public interface IGuestBookService
    {
        bool CreateGBpost(TblGb gb);

        bool DeleteGbPost(int gbId);

        int GetLastGbId();
        
        List<TblGb> GetAll();
    }
}