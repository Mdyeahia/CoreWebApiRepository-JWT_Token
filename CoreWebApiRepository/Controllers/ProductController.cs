using CoreWebApiRepository.IRepository;
using CoreWebApiRepository.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace CoreWebApiRepository.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class ProductController : ControllerBase
    {
        private IConfiguration _config;
        IProductRepository _productRepository = null;

        public ProductController(IConfiguration config, IProductRepository productRepository)
        {
            _config = config;
            _productRepository = productRepository;
        }   
        [HttpPost]
        [Route("Save")]
        public IActionResult Save([FromBody] Product product)
        {
            try
            {
                return Ok(_productRepository.DataSaveOrUpdate(product));
            }
            catch(Exception ex)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError, ex.Message);
            }
        }
        [HttpGet]
        [Route("AllProduct")]
        public async Task<IActionResult> AllProduct()
        {
            return Ok( await _productRepository.Gets());
           
        }
    }
}
