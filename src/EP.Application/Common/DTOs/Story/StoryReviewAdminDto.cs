using EP.Application.Common.DTOs.Volume;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EP.Application.Common.DTOs.Story
{
    public class StoryReviewAdminDto
    {
        public double Tt_key { get; set; }
        public double Tt_parent { get; set; } = 0;
        public int StoryId { get; set; }
        public string? Title { get; set; }
        public string? CreateTime { get; set; }
        public int? Status { get; set; }
        public string? Author { get; set; }
        public List<VolumeReviewAdminDto> Volumes { get; set; }
    }
}
