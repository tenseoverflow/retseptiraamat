const API_BASE = 'http://localhost:5000/api';

export async function getFridgeItems() {
    const response = await fetch(`${API_BASE}/FridgeItem`);
    return await response.json();
}

export async function getRecipes() {
    const response = await fetch(`${API_BASE}/Recipe`);
    return await response.json();
}

export async function addFridgeItem(item: any) {
    const response = await fetch(`${API_BASE}/FridgeItem`, {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify(item)
    });
    return await response.json();
}

export async function deleteFridgeItem(id: number) {
    await fetch(`${API_BASE}/FridgeItem/${id}`, {
        method: 'DELETE'
    });
}