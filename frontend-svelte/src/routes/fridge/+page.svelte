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
	let success = '';

	onMount(async () => {
		try {
			const response = await fetch('http://localhost:5036/api/Fridge');
			fridgeItems = await response.json();
		} catch (e) {
			error = 'Failed to load fridge items';
		} finally {
			loading = false;
		}
	});

	async function addItem() {
		try {
			const response = await fetch('http://localhost:5036/api/Fridge', {
				method: 'POST',
				headers: { 'Content-Type': 'application/json' },
				body: JSON.stringify({ ...newItem, lastUpdated: new Date().toISOString() })
			});

			if (response.ok) {
				const addedItem = await response.json();
				fridgeItems = [...fridgeItems, addedItem];
				newItem = { ingredient: '', amount: 0 };
				success = 'Item added successfully!';

				// Clear success message after 3 seconds
				setTimeout(() => {
					success = '';
				}, 3000);
			}
		} catch (e) {
			error = 'Failed to add item';
			// Clear error message after 3 seconds
			setTimeout(() => {
				error = '';
			}, 3000);
		}
	}

	async function deleteItem(id: number) {
		if (confirm('Are you sure you want to delete this item?')) {
			try {
				const response = await fetch(`http://localhost:5036/api/Fridge/${id}`, {
					method: 'DELETE'
				});

				if (response.ok) {
					fridgeItems = fridgeItems.filter((item) => item.id !== id);
					success = 'Item deleted successfully!';

					// Clear success message after 3 seconds
					setTimeout(() => {
						success = '';
					}, 3000);
				} else {
					error = `Failed to delete item: ${response.status} ${response.statusText}`;

					// Clear error message after 3 seconds
					setTimeout(() => {
						error = '';
					}, 3000);
				}
			} catch (e) {
				error = 'Failed to delete item: Network error';
				// Clear error message after 3 seconds
				setTimeout(() => {
					error = '';
				}, 3000);
			}
		}
	}
</script>

<div class="container mx-auto p-4">
	<div class="mb-6 flex items-center justify-between">
		<h1 class="text-2xl font-bold">My Fridge</h1>
	</div>

	{#if error}
		<div class="mb-4 rounded border border-red-400 bg-red-100 px-4 py-3 text-red-700">
			{error}
		</div>
	{/if}

	{#if success}
		<div class="mb-4 rounded border border-green-400 bg-green-100 px-4 py-3 text-green-700">
			{success}
		</div>
	{/if}

	<div class="mb-8 rounded-lg bg-white p-6 shadow-md">
		<h2 class="mb-4 text-xl font-semibold">Add New Item</h2>
		<form class="space-y-4" on:submit|preventDefault={addItem}>
			<div class="grid grid-cols-1 gap-4 md:grid-cols-2">
				<div>
					<label for="ingredient" class="block text-sm font-medium text-gray-700">Ingredient</label>
					<input
						id="ingredient"
						type="text"
						bind:value={newItem.ingredient}
						class="mt-1 block w-full rounded-md border-gray-300 shadow-sm focus:border-green-500 focus:ring-green-500"
						placeholder="E.g., Milk, Eggs, Flour"
						required
					/>
				</div>
				<div>
					<label for="amount" class="block text-sm font-medium text-gray-700">Amount</label>
					<input
						id="amount"
						type="number"
						bind:value={newItem.amount}
						min="0"
						step="0.1"
						class="mt-1 block w-full rounded-md border-gray-300 shadow-sm focus:border-green-500 focus:ring-green-500"
						required
					/>
				</div>
			</div>

			<div class="flex justify-end">
				<button
					type="submit"
					class="rounded-md bg-green-600 px-4 py-2 text-sm font-medium text-white shadow-sm hover:bg-green-700"
				>
					Add to Fridge
				</button>
			</div>
		</form>
	</div>

	{#if loading}
		<div class="flex justify-center">
			<p class="text-gray-500">Loading your fridge inventory...</p>
		</div>
	{:else if fridgeItems.length === 0}
		<div class="rounded-md bg-gray-50 p-6 text-center">
			<p class="mb-2 text-gray-500">Your fridge is empty</p>
			<p class="text-gray-500">Start by adding some ingredients</p>
		</div>
	{:else}
		<div class="overflow-hidden rounded-lg bg-white shadow-md">
			<table class="min-w-full divide-y divide-gray-200">
				<thead class="bg-gray-50">
					<tr>
						<th class="px-6 py-3 text-left text-xs font-medium uppercase text-gray-500"
							>Ingredient</th
						>
						<th class="px-6 py-3 text-left text-xs font-medium uppercase text-gray-500">Amount</th>
						<th class="px-6 py-3 text-left text-xs font-medium uppercase text-gray-500"
							>Last Updated</th
						>
						<th class="px-6 py-3 text-left text-xs font-medium uppercase text-gray-500">Actions</th>
					</tr>
				</thead>
				<tbody class="divide-y divide-gray-200 bg-white">
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
