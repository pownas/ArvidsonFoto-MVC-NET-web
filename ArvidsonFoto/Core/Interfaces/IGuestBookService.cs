using ArvidsonFoto.Core.Models;

namespace ArvidsonFoto.Core.Interfaces;

/// <summary>
/// Interface for guestbook services
/// </summary>
public interface IGuestBookService
{
    bool CreateGBpost(TblGb gb);

    bool ReadGbPost(int gbId);

    bool DeleteGbPost(int gbId);

    int GetCountOfUnreadPosts();

    int GetLastGbId();

    List<TblGb> GetAll();
}
