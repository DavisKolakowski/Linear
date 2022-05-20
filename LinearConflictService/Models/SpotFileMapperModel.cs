using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LinearConflictService.Models
{
    internal class SpotFileMapperModel
    {
        public string SpotCode { get; set; } = string.Empty;
        public string SpotTitle { get; set; } = string.Empty;
        public DateTime FirstAirDate { get; set; } = DateTime.MinValue;
    }
}
