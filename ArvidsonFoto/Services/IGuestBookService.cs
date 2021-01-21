using ArvidsonFoto.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ArvidsonFoto.Data
{
    public interface IGuestBookService
    {
        bool CreateGBpost(TblGb gb);
        
        int GetLastGbId();
        
        List<TblGb> GetAll();
    }
}