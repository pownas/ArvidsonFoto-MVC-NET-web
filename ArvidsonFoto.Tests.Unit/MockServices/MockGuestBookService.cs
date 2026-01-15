using ArvidsonFoto.Core.Models;
using ArvidsonFoto.Core.Interfaces;

namespace ArvidsonFoto.Tests.Unit.MockServices;

/// <summary>
/// Mock implementation of IGuestBookService for unit testing using Core.Models
/// </summary>
public class MockGuestBookService : IGuestBookService
{
    private readonly List<TblGb> _mockGuestbookEntries;
    private int _nextId;

    public MockGuestBookService()
    {
        _mockGuestbookEntries = new List<TblGb>
        {
            new TblGb
            {
                Id = 1,
                GbId = 1,
                GbName = "Test User 1",
                GbEmail = "test1@example.com",
                GbHomepage = "example.com",
                GbText = "Test message 1",
                GbDate = DateTime.Now.AddDays(-2),
                GbReadPost = true
            },
            new TblGb
            {
                Id = 2,
                GbId = 2,
                GbName = "Test User 2",
                GbEmail = "test2@example.com",
                GbHomepage = "example2.com",
                GbText = "Test message 2",
                GbDate = DateTime.Now.AddDays(-1),
                GbReadPost = false
            }
        };
        _nextId = 3;
    }

    public bool CreateGBpost(TblGb gb)
    {
        if (gb == null)
            return false;

        gb.Id = _nextId++;
        gb.GbDate = DateTime.Now;
        _mockGuestbookEntries.Add(gb);
        return true;
    }

    public bool ReadGbPost(int gbId)
    {
        var post = _mockGuestbookEntries.FirstOrDefault(g => g.GbId == gbId);
        if (post == null)
            return false;

        post.GbReadPost = true;
        return true;
    }

    public bool DeleteGbPost(int gbId)
    {
        var post = _mockGuestbookEntries.FirstOrDefault(g => g.GbId == gbId);
        if (post == null)
            return false;

        _mockGuestbookEntries.Remove(post);
        return true;
    }

    public int GetCountOfUnreadPosts()
    {
        return _mockGuestbookEntries.Count(g => g.GbReadPost == null || g.GbReadPost == false);
    }

    public int GetLastGbId()
    {
        if (!_mockGuestbookEntries.Any())
            return 0;

        return _mockGuestbookEntries.Max(g => g.GbId);
    }

    public List<TblGb> GetAll()
    {
        return _mockGuestbookEntries.OrderByDescending(g => g.GbId).ToList();
    }

    public int GetAllGuestbookEntriesCounted()
    {
        return _mockGuestbookEntries.Count;
    }

    // Async methods
    public async Task<IEnumerable<TblGb>> GetAllGuestbookEntriesAsync()
    {
        return await Task.FromResult(GetAll());
    }

    public async Task<TblGb> GetOneGbEntryAsync(int id)
    {
        var entry = _mockGuestbookEntries.FirstOrDefault(gb => gb.GbId == id);
        return await Task.FromResult(entry ?? new TblGb { GbId = -1 });
    }

    public async Task<bool> CreateGbEntryAsync(TblGb gb)
    {
        return await Task.FromResult(CreateGBpost(gb));
    }

    public async Task<bool> DeleteGbEntryAsync(int id)
    {
        return await Task.FromResult(DeleteGbPost(id));
    }
}
