using NRedisStack.RedisStackCommands;
using NRedisStack.Search;
using NRedisStack.Search.Literals.Enums;
using StackExchange.Redis;

namespace app.Service.Caching
{
    public class IndexCreationService : IHostedService
    {
        private readonly RedisCacheService _redisService;

        public IndexCreationService(RedisCacheService redisService)
        {
            _redisService = redisService;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            try
            {
               
                bool indexExists = await IndexExistsAsync("story-idx");
                if (!indexExists)
                {
                    var schema = new Schema()
                        .AddNumericField(new FieldName("$.StoryId", "StoryId"))
                        .AddTextField(new FieldName("$.StoryTitle", "StoryTitle"), 1.0, sortable: true)
                        .AddTextField(new FieldName("$.StoryDescription", "StoryDescription"), 0.5)
                        .AddTagField(new FieldName("$.StoryCategories[*].CategoryId", "CategoryId"))
                        .AddNumericField(new FieldName("$.StoryAuthor.UserId", "StoryAuthor.UserId"))
                        .AddTextField(new FieldName("$.StoryAuthor.UserFullname", "StoryAuthor.UserFullname"))
                        .AddNumericField(new FieldName("$.StoryPrice", "StoryPrice"))
                        .AddNumericField(new FieldName("$.Status", "Status"));

                    var options = new FTCreateParams()
                        .On(IndexDataType.JSON)
                        .Prefix("story:");

                    await _redisService.RedisDb.FT().CreateAsync("story-idx", options, schema);
                }
            }
            catch (RedisServerException ex) when (ex.Message.Contains("Index already exists"))
            {
            }
        }

        private async Task<bool> IndexExistsAsync(string indexName)
        {
            try
            {
                await _redisService.RedisDb.FT().InfoAsync(indexName);
                return true;
            }
            catch (RedisServerException ex) when (ex.Message.Contains("no such index"))
            {
                return false;
            }
        }

        public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
    }
}
