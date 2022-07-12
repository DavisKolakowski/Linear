using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LinearWebService.Models
{
    public class SpotFileMapperModel
    {
        public string SpotCode { get; set; } = string.Empty;
        public string SpotTitle { get; set; } = string.Empty;
        public DateTime FirstAirDate { get; set; } = DateTime.MinValue;
    }
}
