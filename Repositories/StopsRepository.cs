using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using WarsawCore.Models;

namespace WarsawCore.Repositories
{
    public class StopsRepository : IStopsRepository
    {
        private readonly WarsawDbContext _context;
        public StopsRepository(WarsawDbContext context)
        {
            _context = context;

        }

        public void Delete(int id)
        {
            Stop stopEntity = _context.Stops.Find(id);
            _context.Stops.Remove(stopEntity);
            _context.SaveChanges();
        }

        public Stop Create(Stop stop)
        {
            _context.Stops.Add(stop);
            _context.SaveChanges();
            return stop;
        }

        public Stop Update(Stop stop)
        {
            _context.Stops.Attach(stop);
            _context.Entry(stop).State = EntityState.Modified;
            _context.SaveChanges();
            return stop;
        }

        public (IEnumerable<Stop>, int) Get(string search, int skip)
        {
            var stopsFilteredByName = search != null ? _context.Stops
            .Where(x => x.Name.ToLower()
            .Contains(search)) : _context.Stops;
            var stopsCount = stopsFilteredByName.Count();

            var paginatedStop = stopsFilteredByName
            .OrderBy(x => x.Id)
            .Skip(skip)
            .Take(20);

            return (paginatedStop, stopsCount);
        }


        public Stop Get(int id)
        {
            return _context.Stops.Find(id);
        }

    }
}