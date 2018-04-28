using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Threading.Tasks;
using Insta.Common;
using Insta.Processing.Domain;

namespace Insta.Processing
{
    public class PhotoRepository : IPhotoRepository
    {
        private const string ParamId = nameof(Photo.Id);
        private const string ParamName = nameof(Photo.Name);
        private const string ParamVisionAnalysis = nameof(Photo.VisionAnalysis);
        private const string ParamOriginalContent = nameof(Photo.OriginalContent);
        private const string ParamThumbnailContent = nameof(Photo.ThumbnailContent);

        private readonly IWebConfiguration _webConfiguration;

        // Add db execution logging
        public PhotoRepository(IWebConfiguration webConfiguration)
        {
            _webConfiguration = webConfiguration;
        }

        public async Task<IEnumerable<Photo>> GetAll()
        {
            var commandText = $@"SELECT [{ParamId}], [{ParamName}] FROM [dbo].[T_Photo]";
            var result = new List<Photo>();

            using (var connection = new SqlConnection(_webConfiguration.ConfigurationString))
            {
                using (var command = new SqlCommand(commandText, connection))
                {
                    connection.Open();

                    var reader = await command.ExecuteReaderAsync();

                    while (reader.Read())
                    {
                        result.Add(new Photo
                        {
                            Id = reader.GetInt32(0),
                            Name = reader.GetString(1)
                        });
                    }
                }
            }

            return result;
        }

        public async Task<Photo> Get(int id)
        {
            var commandText = $@"SELECT [{ParamName}], [{ParamVisionAnalysis}] 
                                 FROM [dbo].[T_Photo]
                                 WHERE [{ParamId}] = @{ParamId}";

            using (var connection = new SqlConnection(_webConfiguration.ConfigurationString))
            {
                using (var command = new SqlCommand(commandText, connection))
                {
                    connection.Open();

                    command.Parameters.AddWithValue($"@{ParamId}", id);

                    var reader = await command.ExecuteReaderAsync();

                    return reader.Read()
                        ? new Photo
                        {
                            Id = id,
                            Name = reader.GetString(0),
                            VisionAnalysis = await reader.IsDBNullAsync(1)
                                ? null
                                : reader.GetString(1)
                        }
                        : null;
                }
            }
        }

        public async Task<byte[]> GetGetOriginal(int id) => await GetBinaryValue(id, ParamOriginalContent);

        public async Task<byte[]> GetGetThumbnail(int id) => await GetBinaryValue(id, ParamThumbnailContent);

        public async Task Add(Photo photo)
        {
            var commandText = $@"INSERT INTO [dbo].[T_Photo] (
                                    [{ParamName}],
                                    [{ParamVisionAnalysis}], 
                                    [{ParamOriginalContent}], 
                                    [{ParamThumbnailContent}])
	                             VALUES(
                                    @{ParamName}, 
                                    @{ParamVisionAnalysis},
                                    CAST(@{ParamOriginalContent} as varbinary(max)), 
                                    CAST(@{ParamThumbnailContent} as varbinary(max)))";

            using (var connection = new SqlConnection(_webConfiguration.ConfigurationString))
            {
                using (var command = new SqlCommand(commandText, connection))
                {
                    connection.Open();

                    command.Parameters.AddWithValue($"@{ParamName}", photo.Name);
                    command.Parameters.AddWithValue($"@{ParamVisionAnalysis}", photo.VisionAnalysis);
                    command.Parameters.AddWithValue($"@{ParamOriginalContent}", photo.OriginalContent);
                    command.Parameters.AddWithValue($"@{ParamThumbnailContent}", 
                        (object)photo.ThumbnailContent ?? DBNull.Value);

                    await command.ExecuteNonQueryAsync();
                }
            }
        }

        private async Task<byte[]> GetBinaryValue(int id, string columnName)
        {
            var commandText = $@"SELECT [{columnName}] 
                                 FROM [dbo].[T_Photo]
                                 WHERE [{ParamId}] = @{ParamId}";

            using (var connection = new SqlConnection(_webConfiguration.ConfigurationString))
            {
                using (var command = new SqlCommand(commandText, connection))
                {
                    connection.Open();

                    command.Parameters.AddWithValue($"@{ParamId}", id);

                    var reader = await command.ExecuteReaderAsync();

                    return reader.Read() && !await reader.IsDBNullAsync(0)
                        ? reader.GetSqlBinary(0).Value
                        : null;
                }
            }
        }
    }
}
