using System.Collections.Generic;
using WarsawCore.Models;

namespace WarsawCore.Repositories
{
    public interface IStopsRepository
    {
        (IEnumerable<Stop>, int) Get(string search, int skip);
        Stop Get(int Id);
        Stop Create(Stop stop);
        Stop Update(Stop stop);
        void Delete(int id);        
    }
}