namespace WarsawCore.Models
{
    public class CreateStopRequest
    {
        public string Name { get; set; }
        public string Number { get; set; }
        public string Direction { get; set; }
        public string Street { get; set; }
        public string City { get; set; }
        public double Lat { get; set; }
        public double Long { get; set; }

        public Stop GetStop()
        {
            var stop = new Stop
            {
                Name = this.Name,
                Number = this.Number,
                Direction = this.Direction,
                Street = this.Street,
                City = this.City,
                Lat = this.Lat,
                Long = this.Long
            };

            return stop;
        }
    }


}