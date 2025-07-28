using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EP.Application.Common.DTOs.Interaction
{
    public class MinimalInteractionDto
    {
        public int Like { get; set; }
        public int Follow { get; set; }
        public int View { get; set; }
        public int Read { get; set; }
    }
}
