using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace Fruityvice.WebApi.Controllers
{
    public class FruitController : ApiBaseController
    {
        private static readonly string[] Summaries = new[]
        {
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        /// <summary>
        /// Get Fruits with given nutrition between min and max parameter. At least one query parameter has to be given
        /// </summary>
        /// <param name="min">Optional minimum nutrition value, Default value: 0</param>
        /// <param name="max">Optional maximum nutrition value, Default value: 1000</param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> Nutrition(int min = 0, int max = 1000)
        {
            /// Validate : At least one query parameter has to be given
            if (!Request.QueryString.HasValue)
            {
                var errors = new Dictionary<string, string[]>() { { "Required", new string[] { "Required at least any one of minimum or maximum input parameter value!" } } };
                return await Task.FromResult(ValidationProblem(new ValidationProblemDetails(errors)
                {
                    Detail = "Missing input parameter value",
                    Status = (int)HttpStatusCode.BadRequest,
                    Instance = this.GetType().Name,
                    Title = "Required input parameter",
                }));
            }

            return await Task.FromResult(Ok(new { min = min, max = max }));
        }


    }
}
