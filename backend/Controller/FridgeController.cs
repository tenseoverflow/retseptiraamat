using System;
using System.Collections.Generic;
using backend.Model;
using Microsoft.AspNetCore.Mvc;
using backend.Services;
using backend.Interfaces;

namespace backend.Controller
{
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
        public async Task<ActionResult<IEnumerable<FridgeItem>>> GetFridgeItems()
        {
            var fridgeItems = await _fridgeService.GetAllAsync();
            return Ok(fridgeItems);
        }

        [HttpGet("aggregated")]
        public async Task<ActionResult<Dictionary<string, int>>> GetAggregatedIngredients()
        {
            var aggregatedIngredients = await _fridgeService.GetAggregatedIngredientsAsync();
            return Ok(aggregatedIngredients);
        }

        [HttpPost]
        public async Task<ActionResult> AddFridgeItem([FromBody] FridgeItem fridgeItem)
        {
            if (fridgeItem == null)
            {
                return BadRequest("Invalid fridge item.");
            }

            // Check if an ingredient with the same name already exists
            if (await _fridgeService.IngredientExistsAsync(fridgeItem.Ingredient))
            {
                return BadRequest("An ingredient with this name already exists. Please update the existing ingredient instead of adding a duplicate.");
            }

            await _fridgeService.CreateAsync(fridgeItem);
            return CreatedAtAction(nameof(GetFridgeItems), new { id = fridgeItem.Id }, fridgeItem);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateFridgeItem(int id, [FromBody] FridgeItem fridgeItem)
        {
            if (id != fridgeItem.Id)
            {
                return BadRequest("Fridge item ID mismatch.");
            }

            var existingItem = await _fridgeService.GetByIdAsync(id);
            if (existingItem == null)
            {
                return NotFound("Fridge item not found.");
            }

            await _fridgeService.UpdateAsync(fridgeItem);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteFridgeItem(int id)
        {
            var fridgeItem = await _fridgeService.GetByIdAsync(id);
            if (fridgeItem == null)
            {
                return NotFound("Fridge item not found.");
            }

            await _fridgeService.DeleteAsync(id);
            return NoContent();
        }
    }
}