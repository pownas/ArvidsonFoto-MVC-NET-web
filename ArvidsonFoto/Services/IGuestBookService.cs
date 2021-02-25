using ArvidsonFoto.Models;
using System.Collections.Generic;

namespace ArvidsonFoto.Services
{
    public interface IGuestBookService
    {
        bool CreateGBpost(TblGb gb);

        bool ReadGbPost(int gbId);

        bool DeleteGbPost(int gbId);

        int GetCountOfUnreadPosts();

        int GetLastGbId();
        
        List<TblGb> GetAll();
    }
}