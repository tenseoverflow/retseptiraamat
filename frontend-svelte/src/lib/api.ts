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
    
    if (!response.ok) {
        const errorText = await response.text();
        let errorMessage = 'Failed to update recipe';
        
        try {
            // Try to parse as JSON if possible
            const errorData = JSON.parse(errorText);
            errorMessage = errorData.message || errorData.title || errorMessage;
        } catch (e) {
            // If parsing fails, use the raw text if available
            if (errorText) errorMessage = errorText;
        }
        
        throw new Error(errorMessage);
    }
    
    // Handle empty responses or content-type issues
    const contentType = response.headers.get('content-type');
    if (contentType && contentType.includes('application/json')) {
        // If response has JSON content, parse and return it
        const text = await response.text();
        return text ? JSON.parse(text) : {}; // Safe parsing with empty check
    }
    
    // For non-JSON or empty responses, just return success indicator
    return { success: true };
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
    
    // Track updated and removed items
    const updatedItems: any[] = [];
    const removedItems: any[] = [];
    
    // Process each fridge item
    for (const item of fridgeItems) {
        // Check if this fridge item's ingredient exists in the recipe (case insensitive)
        const recipeIngredientKey = Object.keys(recipeIngredients).find(
            ingredient => ingredient.toLowerCase() === item.ingredient.toLowerCase()
        );
        
        if (recipeIngredientKey) {
            const amountToRemove = recipeIngredients[recipeIngredientKey];
            
            // If recipe requires more or equal to what we have, remove the item
            if (amountToRemove >= item.amount) {
                await deleteFridgeItem(item.id);
                removedItems.push(item);
            } else {
                // Otherwise, just update with reduced amount
                const updatedItem = {
                    ...item,
                    amount: item.amount - amountToRemove,
                    lastUpdated: new Date().toISOString()
                };
                await updateFridgeItem(item.id, updatedItem);
                updatedItems.push(updatedItem);
            }
        }
    }
    
    // Return both updated and removed items for confirmation message
    return {
        updated: updatedItems,
        removed: removedItems
    };
}