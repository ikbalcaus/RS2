using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using eBooks.Models.Responses;
using eBooks.Interfaces;

namespace eBooks.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class RecommenderController : ControllerBase
    {
        protected IRecommenderService _service;

        public RecommenderController(IRecommenderService service)
        {
            _service = service;
        }

        [Authorize(Policy = "Admin")]
        [HttpGet("train-model")]
        public async Task TrainModel([FromQuery] int numberOfIterations = 200, [FromQuery] int approximationRank = 150, [FromQuery] float alpha = 0.02f, [FromQuery] float lambda = 0.04f, [FromQuery] int batchIterations = 10, [FromQuery] int patience = 3)
        {
            await _service.TrainModel(numberOfIterations, approximationRank, alpha, lambda);
        }

        [AllowAnonymous]
        [HttpGet("{bookId}/get-recommended-books")]
        public async Task<List<BooksRes>> GetRecommendedBooks(int bookId, [FromQuery] int numberOfRecommendations = 5)
        {
            return await _service.GetRecommendedBooks(bookId, numberOfRecommendations);
        }
    }
}
