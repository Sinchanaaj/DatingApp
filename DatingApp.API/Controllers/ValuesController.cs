using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DatingApp.API.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace DatingApp.API.Controllers
{
    [Route("api/[controller]")]  //attribute type routing
    [ApiController]

    public class ValuesController : ControllerBase
    {
        private readonly DataContext _context;
        public ValuesController(DataContext context)   //contructer created (ctor)
        {
            _context = context;

        }

     //get api/values
        [HttpGet]
        public async Task<IActionResult> GetValues()                 //converted from syncronous to async code
        {
            //throw new Exception("test aj"); to check exception development page
          var values =await _context.Values.ToListAsync();           //awaiit is added to wait for req res
          return Ok(values);                                        //ok returns http 200 
       }
        
         [HttpGet("{id}")]
        public async Task<IActionResult> GetValue(int id)         //IactionResult allows to retuen http response
        {
            var value=await _context.Values.FirstOrDefaultAsync(x =>x.Id == id); //x is value we are returning ,id is the value that we pass
            return Ok(value);
        }


        [HttpPost]
        public void Post([FromBody] string value)
        {
        }
        
        [HttpPut("{id}")]
        public void Put(int id,[FromBody] string value)
        {
        }
        
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
        
        
}
}
