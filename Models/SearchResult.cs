using System.Collections.Generic;

namespace WarsawCore.Models
{
    public class SearchResult
    {
        public IEnumerable<StopResult> Items { get; set; }
        public PageInfo PageInfo { get; set; }
    }

    public class PageInfo
    {
        public int CurrentPage { get; set; }

        public int MaxPage { get; set; }
    }

    public class StopResult
    {
        public StopResult(Stop stop)
        {
            Id = stop.Id;
            Name = stop.Name;
            Number = stop.Number;
            Street = stop.Street;
            City = stop.City;
        }
        public int Id { get; set; }
        public string Name { get; set; }
        public string Number { get; set; }
        public string Street { get; set; }
        public string City { get; set; }
        
    }    
}