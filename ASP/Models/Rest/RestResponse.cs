namespace ASP.Models.Rest
{
    // Layered system - system must be valid for insertion of proxy
    // [client] ---- [server]                   [client] ---- [proxy] ---- [server]
    // GET /x  <---> 404 Not found                GET /x <--->  500 IE <---> 404 Not found
    // Conclusion: divide statuses of passing the request and executing the request
    // [client] ---- [proxy] ---- [server]
    // GET /x  <--->  200 OK <---> 200 OK (Server was found, it understood the request, but the result of execution is 404) 
    //           (404 Not found)    (404 Not found) (literally status within status)
    //
    // (1) Practically introduce different types to represent status
    //      - logical: true/false
    //      - digital: code
    //      - text:    phrase
    //
    // (2) Metadata - data that accompanies the resource but is not a part of it.
    // Define necessary ones:
    //      - cache duration
    //      - resource manipulation
    //      - sub-resources
    //      + additional
    //      + resource identification confirmation
    //      + timestamp
    //      ~ variative
    //      ~ pagination (number of page, number of total pages)
    //      ~ accepted request parameters (e.g. seach request)
    //
    // Response example
    // {
    //      status: {...},
    //      meta: {
    //          serverTimestamp: 161844684846,
    //          resource: "Shop API: 'product'",
    //          resourceUrl: "/api/product/christmas-tree",
    //          cache: 3600,
    //          manipulations: [GET, POST, PATCH, DELETE],
    //          dataType: "json/object",
    //          links: {                        // HATEOAS
    //              "all": "/api/product",
    //              "id": "/api/product/id",
    //              "slug": "/api/product/slug"
    //              "img": "/api/product/slug?img={num}",
    //          },
    //          pagination:
    //          {
    //              "page": 2,
    //              "perPage": 20,
    //              "lastPage": 10,
    //              "totalItems": 195,
    //              "prevPage": "/api/product?page=1,
    //              "nextPage": "/api/product?page=3,
    //          }
    //      },
    //      data:{
    //          name: "Christmas tree"
    //          ...
    //      }
    // }
    public class RestResponse
    {
        public RestStatus Status { get; set; } = new();
        public RestMeta Meta { get; set; } = new();
        public object? Data { get; set; }
    }
}
