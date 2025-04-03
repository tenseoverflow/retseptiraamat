using System;
using System.Collections.Generic;
using backend.Model;
using Microsoft.AspNetCore.Mvc;

namespace backend.Controller {
    [Route("api/[controller]")]
    [ApiController]
    
    public class FridgeController : ControllerBase
    {
        private readonly IFridgeService _fridgeService;

        public FridgeController(IFridgeService fridgeService)
        {
            _fridgeService = fridgeService;
        }

        [HttpGet]
        public ActionResult<IEnumerable<FridgeItem>> GetFridgeItems()
        {
            var fridgeItems = _fridgeService.GetFridgeItems();
            return Ok(fridgeItems);
        }

        [HttpPost]
        public ActionResult AddFridgeItem([FromBody] FridgeItem fridgeItem)
        {
            if (fridgeItem == null)
            {
                return BadRequest("Invalid fridge item.");
            }

            _fridgeService.AddFridgeItem(fridgeItem);
            return CreatedAtAction(nameof(GetFridgeItems), new { id = fridgeItem.Id }, fridgeItem);
        }
    }
}