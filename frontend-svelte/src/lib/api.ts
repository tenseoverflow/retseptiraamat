// Get API URL from environment or use fallback
const API_URL = import.meta.env.VITE_API_URL || 'http://localhost:5036';
const API_BASE = `${API_URL}/api`;

// Recipe API functions
export async function getRecipes() {
    const response = await fetch(`${API_BASE}/Recipe`);
    return await response.json();
}

export async function getRecipe(id: number) {
    const response = await fetch(`${API_BASE}/Recipe/${id}`);
    if (!response.ok) {
        throw new Error(`Recipe not found (${response.status})`);
    }
    return await response.json();
}

export async function createRecipe(recipe: any) {
    const response = await fetch(`${API_BASE}/Recipe`, {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify(recipe)
    });
    return await response.json();
}

export async function updateRecipe(id: number, recipe: any) {
    const response = await fetch(`${API_BASE}/Recipe/${id}`, {
        method: 'PUT',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify(recipe)
    });
    return await response.json();
}

export async function deleteRecipe(id: number) {
    await fetch(`${API_BASE}/Recipe/${id}`, {
        method: 'DELETE'
    });
}

// Fridge API functions
export async function getFridgeItems() {
    const response = await fetch(`${API_BASE}/Fridge`);
    return await response.json();
}

export async function getAggregatedFridgeIngredients() {
    const response = await fetch(`${API_BASE}/Fridge/aggregated`);
    return await response.json();
}

export async function createFridgeItem(item: any) {
    const response = await fetch(`${API_BASE}/Fridge`, {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify(item)
    });
    
    if (!response.ok) {
        const errorData = await response.json().catch(() => null);
        throw new Error(errorData || response.statusText || 'Failed to add ingredient');
    }
    
    return await response.json();
}

export async function updateFridgeItem(id: number, item: any) {
    const response = await fetch(`${API_BASE}/Fridge/${id}`, {
        method: 'PUT',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify(item)
    });
    
    if (!response.ok) {
        const errorData = await response.json().catch(() => null);
        throw new Error(errorData || response.statusText || 'Failed to update ingredient');
    }
    
    return true;
}

export async function deleteFridgeItem(id: number) {
    await fetch(`${API_BASE}/Fridge/${id}`, {
        method: 'DELETE'
    });
}

// Helper function to remove all recipe ingredients from fridge
export async function removeRecipeIngredientsFromFridge(recipeIngredients: Record<string, number>) {
    // Get all fridge items
    const fridgeItems = await getFridgeItems();
    
    // Find matching ingredients (case-insensitive)
    const matchingItems = fridgeItems.filter(item => {
        // Check if this fridge item's ingredient name exists in the recipe ingredients (case insensitive)
        return Object.keys(recipeIngredients).some(
            ingredient => ingredient.toLowerCase() === item.ingredient.toLowerCase()
        );
    });
    
    // Delete all matching items
    const deletePromises = matchingItems.map(item => deleteFridgeItem(item.id));
    await Promise.all(deletePromises);
    
    // Return the deleted items for confirmation message
    return matchingItems;
}