using EP.Application.Common.DTOs.Chapter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EP.Application.Common.DTOs.Volume
{
    public class VolumeReviewAdminDto
    {
        public double Tt_key { get; set; }
        public double Tt_parent { get; set; }
        public int VolumeId { get; set; }
        public int VolumeNumber { get; set; }
        public string Title { get; set; }
        public string CreateTime { get; set; }
        public List<ChapterReviewAdminDto> Chapters { get; set; }
    }
}
