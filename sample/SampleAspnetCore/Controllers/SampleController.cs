using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace SampleAspnetCore.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class SampleController : ControllerBase
    {
        private readonly ILogger<SampleController> _logger;

        public SampleController(ILogger<SampleController> logger)
        {
            _logger = logger;
        }

        [HttpGet("info")]
        public IActionResult LogInfo()
        {
            _logger.LogInformation("Sample information:  Id: {id}, Name: {name}, LastName: {lastName}", 1, "Walther",
                "White");
            return Ok();
        }

        [HttpGet("warning")]
        public IActionResult LogWarning()
        {
            _logger.LogWarning("Sample warning");
            return Ok();
        }

        [HttpGet("exception")]
        public IActionResult LogException()
        {
            try
            {
                throw new ArithmeticException("Some Error");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Sample exception");
            }

            return Ok();
        }
    }
}