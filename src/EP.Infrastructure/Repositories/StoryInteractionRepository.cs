using EP.Application.Common.Interfaces.Repositories;
using EP.Domain.Models;
using EP.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EP.Infrastructure.Repositories
{
    public class StoryInteractionRepository : Repository<StoryInteraction>, IStoryInteractionRepository
    {
        public StoryInteractionRepository(Context context) : base(context)
        {
        }

    }
}
