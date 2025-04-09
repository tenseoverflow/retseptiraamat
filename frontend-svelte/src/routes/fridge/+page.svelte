<script lang="ts">
	import { onMount } from 'svelte';

	interface FridgeItem {
		id: number;
		ingredient: string;
		amount: number;
		lastUpdated: string;
	}

	let fridgeItems: FridgeItem[] = [];
	let newItem = { ingredient: '', amount: 0 };
	let loading = true;
	let error = '';

	onMount(async () => {
		try {
			const response = await fetch('http://localhost:5000/api/FridgeItem');
			fridgeItems = await response.json();
		} catch (e) {
			error = 'Failed to load fridge items';
		} finally {
			loading = false;
		}
	});

	async function addItem() {
		try {
			const response = await fetch('http://localhost:5000/api/FridgeItem', {
				method: 'POST',
				headers: { 'Content-Type': 'application/json' },
				body: JSON.stringify({ ...newItem, lastUpdated: new Date().toISOString() })
			});

			if (response.ok) {
				const addedItem = await response.json();
				fridgeItems = [...fridgeItems, addedItem];
				newItem = { ingredient: '', amount: 0 };
			}
		} catch (e) {
			error = 'Failed to add item';
		}
	}

	async function deleteItem(id: number) {
		try {
			await fetch(`http://localhost:5000/api/FridgeItem/${id}`, {
				method: 'DELETE'
			});
			fridgeItems = fridgeItems.filter((item) => item.id !== id);
		} catch (e) {
			error = 'Failed to delete item';
		}
	}
</script>

<div class="container mx-auto p-4">
	<h1 class="text-2xl font-bold mb-6">My Fridge</h1>

	{#if error}
		<div class="bg-red-100 border border-red-400 text-red-700 px-4 py-3 rounded mb-4">
			{error}
		</div>
	{/if}

	<div class="mb-8">
		<h2 class="text-xl font-semibold mb-4">Add New Item</h2>
		<form class="flex gap-4 items-end" on:submit|preventDefault={addItem}>
			<div class="flex-1">
				<label class="block text-sm font-medium text-gray-700">Ingredient</label>
				<input
					type="text"
					bind:value={newItem.ingredient}
					class="mt-1 block w-full rounded-md border-gray-300 shadow-sm focus:border-green-500 focus:ring-green-500"
					required
				/>
			</div>
			<div class="w-32">
				<label class="block text-sm font-medium text-gray-700">Amount</label>
				<input
					type="number"
					bind:value={newItem.amount}
					class="mt-1 block w-full rounded-md border-gray-300 shadow-sm focus:border-green-500 focus:ring-green-500"
					required
				/>
			</div>
			<button type="submit" class="bg-green-600 text-white px-4 py-2 rounded-md hover:bg-green-700">
				Add Item
			</button>
		</form>
	</div>

	{#if loading}
		<p>Loading...</p>
	{:else}
		<div class="bg-white shadow-md rounded-lg overflow-hidden">
			<table class="min-w-full divide-y divide-gray-200">
				<thead class="bg-gray-50">
					<tr>
						<th class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase"
							>Ingredient</th
						>
						<th class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase">Amount</th>
						<th class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase"
							>Last Updated</th
						>
						<th class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase">Actions</th>
					</tr>
				</thead>
				<tbody class="bg-white divide-y divide-gray-200">
					{#each fridgeItems as item}
						<tr>
							<td class="px-6 py-4">{item.ingredient}</td>
							<td class="px-6 py-4">{item.amount}</td>
							<td class="px-6 py-4">{new Date(item.lastUpdated).toLocaleDateString()}</td>
							<td class="px-6 py-4">
								<button
									on:click={() => deleteItem(item.id)}
									class="text-red-600 hover:text-red-900"
								>
									Delete
								</button>
							</td>
						</tr>
					{/each}
				</tbody>
			</table>
		</div>
	{/if}
</div>
