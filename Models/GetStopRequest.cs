using System.ComponentModel.DataAnnotations;

namespace WarsawCore.Models {
    public class GetStopRequest{
        public int? Page { get; set; } = 1;
        public string Search { get; set; }
    }
}