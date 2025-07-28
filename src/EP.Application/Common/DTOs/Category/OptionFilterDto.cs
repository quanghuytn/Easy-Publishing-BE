using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EP.Application.Common.DTOs.Category
{
    public class OptionFilterDto
    {
        public decimal To { get; set; }
        public decimal From { get; set; }
        public required IEnumerable<CategoryDto> Categories { get; set; }
    }
}
