using Fineo.DTOs;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;

namespace Fineo.Services.Common
{
    public abstract class ServiceBase : ControllerBase
    {
        [HttpGet("{message}")]
        public IActionResult Echo(string message)
        {
            IActionResult result = null;

            EchoRequest request = new EchoRequest()
            {
                Message = message
            };

            result = Echo(request);

            return result;

        }

        [HttpPost]
        public IActionResult Echo(EchoRequest request)
        {
            EchoResponse response = new EchoResponse()
            {
                Message = request.Message,
                SentDt = DateTime.UtcNow
            };
            IActionResult result = Ok(response);

            return result;

        }

        [HttpGet("health")]
        public virtual IActionResult Health()
        {
            HealthResponse response = new HealthResponse()
            {
                Data = new Dictionary<string, string>()
            };

            response.Data["healthy"] = "TRUE";

            IActionResult result = Ok(response);

            return result;
        }

        [HttpPost("health")]
        public virtual IActionResult Health(HealthRequest request)
        {
            HealthResponse response = new HealthResponse()
            {
                Data = new Dictionary<string, string>()
            };

            response.Data["healthy"] = "TRUE";

            IActionResult result = Ok(response);

            return result;
        }

    }
}
