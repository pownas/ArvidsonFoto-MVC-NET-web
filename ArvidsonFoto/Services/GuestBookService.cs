using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using ArvidsonFoto.Data;
using ArvidsonFoto.Models;
using Serilog;

namespace ArvidsonFoto.Services
{
    public class GuestBookService : IGuestBookService
    {
        // Databas koppling
        private readonly ArvidsonFotoDbContext _entityContext;

        public GuestBookService(ArvidsonFotoDbContext context)
        {
            _entityContext = context;
        }

        //public async Task<bool> CreateGBpost(TblGb gb)
        //{
        //    using (var conn = new SqlConnection(_configuration.Value))
        //    {
        //        var parameters = new DynamicParameters();
        //        parameters.Add("GB_ID", gb.GB_ID, DbType.Int32);
        //        parameters.Add("GB_name", gb.GB_text, DbType.String);
        //        parameters.Add("GB_email", gb.GB_email, DbType.String);
        //        parameters.Add("GB_homepage", gb.GB_homepage, DbType.String);
        //        parameters.Add("GB_text", gb.GB_text, DbType.String);
        //        parameters.Add("GB_date", gb.GB_date, DbType.DateTime);
        //        parameters.Add("gbSecret", gb.gbSecret, DbType.String);

        //        // Raw SQL method
        //        const string query = @"INSERT INTO tbl_gb (GB_ID, GB_name, GB_email, GB_homepage, GB_text, GB_date) VALUES (@GB_ID, @GB_name, @GB_email, @GB_homepage, @GB_text, @GB_date)";
        //        await conn.ExecuteAsync(query, new { gb.GB_ID, gb.GB_name, gb.GB_email, gb.GB_homepage, gb.GB_text, gb.GB_date }, commandType: CommandType.Text);
        //    }
        //    return true;
        //}

        //public async Task<int> GetLastGbId()
        //{
        //    int highestID = -1;
        //    using (var conn = new SqlConnection(_configuration.Value))
        //    {
        //        var sql = @"SELECT MAX(GB_ID) FROM tbl_gb";
        //        highestID = await conn.QuerySingleAsync<int>(sql);
        //    }
        //    return highestID;
        //}

        public List<TblGb> GetAll()
        {
            List<TblGb> gbPosts;
            gbPosts = _entityContext.TblGbs.OrderByDescending(g => g.GbId).ToList();
            return gbPosts;
        }

        public bool CreateGBpost(TblGb gb)
        {
            bool succeeded = false;
            try {
                _entityContext.TblGbs.Add(gb);
                _entityContext.SaveChanges();
                succeeded = true;
            } 
            catch (Exception ex)
            {
                succeeded = false;
                Log.Error("Error when creating GB-post. Error-message: " + ex.Message);
            }
            return succeeded;
        }

        public bool DeleteGbPost(int gbId)
        {
            bool succeeded = false;
            try
            {
                TblGb gb = _entityContext.TblGbs.FirstOrDefault(gb => gb.GbId == gbId);
                _entityContext.TblGbs.Remove(gb);
                _entityContext.SaveChanges();
                succeeded = true;
            }
            catch (Exception ex)
            {
                succeeded = false;
                Log.Error("Error when deleting GB-post. Error-message: " + ex.Message);
            }
            return succeeded;
        }

        public int GetLastGbId()
        {
            int idToReturn = _entityContext.TblGbs.Max(gb => gb.GbId);
            if (idToReturn is 0)
                idToReturn = -1;
            return idToReturn;
        }
    }
}
