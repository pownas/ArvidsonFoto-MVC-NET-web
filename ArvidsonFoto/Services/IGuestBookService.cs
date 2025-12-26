using ArvidsonFoto.Core.Models;

namespace ArvidsonFoto.Services;

/// <summary>
/// Legacy interface for guestbook services - migrated to use Core namespace
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