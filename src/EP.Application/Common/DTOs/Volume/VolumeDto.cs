using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EP.Application.Common.DTOs.Volume
{
    public class VolumeDto
    {
        public int VolumeId { get; set; }
        public string VolumeTitle { get; set; } = null!;
        public int VolumeNumber { get; set; }
    }
}
