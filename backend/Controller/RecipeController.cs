using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using backend.Model;
using Microsoft.AspNetCore.Mvc;
using backend.Interfaces;

namespace backend.Controller
{
    [Route("api/[controller]")]
    [ApiController]
    public class RecipeController : ControllerBase
    {
        private readonly IRecipeService _recipeService;

        public RecipeController(IRecipeService recipeService)
        {
            _recipeService = recipeService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Recipe>>> GetRecipes()
        {
            var recipes = await _recipeService.GetAllAsync();
            return Ok(recipes);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Recipe>> GetRecipe(int id)
        {
            var recipe = await _recipeService.GetByIdAsync(id);
            if (recipe == null)
            {
                return NotFound();
            }
            return Ok(recipe);
        }

        [HttpPost]
        public async Task<ActionResult> CreateRecipe([FromBody] Recipe recipe)
        {
            if (recipe == null)
            {
                return BadRequest("Invalid recipe data.");
            }

            recipe.CreatedAt = DateTime.UtcNow;
            await _recipeService.CreateAsync(recipe);
            return CreatedAtAction(nameof(GetRecipe), new { id = recipe.Id }, recipe);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateRecipe(int id, [FromBody] Recipe recipe)
        {
            if (id != recipe.Id)
            {
                return BadRequest("Recipe ID mismatch.");
            }

            var existingRecipe = await _recipeService.GetByIdAsync(id);
            if (existingRecipe == null)
            {
                return NotFound("Recipe not found.");
            }

            recipe.UpdatedAt = DateTime.UtcNow;
            await _recipeService.UpdateAsync(recipe);
            return NoContent();
        }
    }
}