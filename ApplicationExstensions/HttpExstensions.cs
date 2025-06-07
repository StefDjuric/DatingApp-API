using DatingApp_API.Helpers;
using System.Text.Json;

namespace DatingApp_API.ApplicationExstensions
{
    public static class HttpExstensions
    {
        public static void AddPaginationHeader<T>(this HttpResponse response, PagedList<T> data)
        {
            var PaginationHeader = new PaginationHeader(data.CurrentPage, data.PageSize,
                data.TotalCount, data.TotalPages);

            var jsonOptions = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };

            response.Headers.Append("Pagination", JsonSerializer.Serialize(PaginationHeader, jsonOptions));
            response.Headers.Append("Access-Control-Expose-Headers", "Pagination");
        }
    }
}
