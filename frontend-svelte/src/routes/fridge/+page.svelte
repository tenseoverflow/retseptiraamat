<script lang="ts">
	import { onMount } from 'svelte';
	import { getFridgeItems, createFridgeItem, deleteFridgeItem, updateFridgeItem } from '$lib/api';
	import { showError, showSuccess } from '$lib/toast';

	interface FridgeItem {
		id: number;
		ingredient: string;
		amount: number;
		lastUpdated: string;
	}

	let fridgeItems: FridgeItem[] = [];
	let newItem = { ingredient: '', amount: 0 };
	let loading = true;
	let editingItem: FridgeItem | null = null;
	let editAmount: number = 0;

	onMount(async () => {
		try {
			fridgeItems = await getFridgeItems();
		} catch (e) {
			showError('Külmkapi sisu laadimine ebaõnnestus');
		} finally {
			loading = false;
		}
	});

	async function addItem() {
		try {
			// Check if amount is zero
			if (newItem.ingredient && newItem.amount === 0) {
				showError(`Koostisosa "${newItem.ingredient}" kogus ei saa olla 0`);
				return;
			}

			const itemToAdd = { ...newItem, lastUpdated: new Date().toISOString() };
			const addedItem = await createFridgeItem(itemToAdd);
			fridgeItems = [...fridgeItems, addedItem];
			newItem = { ingredient: '', amount: 0 };
			showSuccess('Koostisosa lisatud');
		} catch (e) {
			// Check if error contains a duplicate ingredient message
			if (e instanceof Error && e.message.includes('already exists')) {
				showError(e.message);
			} else {
				showError('Koostisosa lisamine ebaõnnestus');
			}
		}
	}

	async function removeItem(id: number) {
		if (confirm('Kas oled kindel, et soovid selle koostisosa kustutada?')) {
			try {
				await deleteFridgeItem(id);
				fridgeItems = fridgeItems.filter((item) => item.id !== id);
				showSuccess('Koostisosa kustutatud');
			} catch (e) {
				showError('Koostisosa kustutamine ebaõnnestus');
			}
		}
	}

	function startEditing(item: FridgeItem) {
		editingItem = { ...item };
		editAmount = item.amount;
	}

	function cancelEditing() {
		editingItem = null;
	}

	async function saveEdit() {
		if (!editingItem) return;

		try {
			// Check if edit amount is zero
			if (editAmount === 0) {
				showError(`Koostisosa "${editingItem.ingredient}" kogus ei saa olla 0`);
				return;
			}

			const updatedItem = {
				...editingItem,
				amount: editAmount,
				lastUpdated: new Date().toISOString()
			};

			await updateFridgeItem(editingItem.id, updatedItem);

			// Update the item in the local array
			fridgeItems = fridgeItems.map((item) => (item.id === editingItem!.id ? updatedItem : item));

			showSuccess('Kogus muudetud');

			// Reset editing state
			editingItem = null;
		} catch (e) {
			showError('Koostisosa muutmine ebaõnnestus');
		}
	}
</script>

<div class="container mx-auto p-4">
	<div class="mb-6 flex items-center justify-between">
		<h1 class="text-3xl font-bold">Külmkapi sisu</h1>
	</div>

	<div class="mb-8 rounded-lg bg-white p-6 shadow-md">
		<h2 class="mb-4 text-xl font-semibold">Lisa uus koostisosa</h2>
		<form class="space-y-4" on:submit|preventDefault={addItem}>
			<div class="grid grid-cols-1 gap-4 md:grid-cols-2">
				<div>
					<label for="ingredient" class="block text-sm font-medium text-gray-700">Nimetus</label>
					<input
						id="ingredient"
						type="text"
						bind:value={newItem.ingredient}
						class="focus:border-primary focus:ring-primary mt-1 block w-full rounded-md border-gray-300 shadow-sm"
						placeholder="Nt. Piim, Munad, Jahu"
						required
					/>
				</div>
				<div>
					<label for="amount" class="block text-sm font-medium text-gray-700">Kogus</label>
					<input
						id="amount"
						type="number"
						bind:value={newItem.amount}
						min="0"
						step="1"
						class="focus:border-primary focus:ring-primary mt-1 block w-full rounded-md border-gray-300 shadow-sm"
						required
					/>
				</div>
			</div>

			<div class="flex justify-end">
				<button
					type="submit"
					class="bg-primary text-secondary rounded-md px-4 py-2 text-sm font-semibold shadow-sm hover:cursor-pointer hover:text-gray-700 hover:transition"
				>
					Lisa külmkappi
				</button>
			</div>
		</form>
	</div>

	{#if loading}
		<div class="flex justify-center">
			<p class="text-gray-500">Laen külmkapi sisu...</p>
		</div>
	{:else if fridgeItems.length === 0}
		<div class="rounded-md bg-gray-50 p-6 text-center">
			<p class="mb-2 text-gray-500">Sinu külmkapp on tühi</p>
			<p class="text-gray-500">Alusta mõne koostisosa lisamisega</p>
		</div>
	{:else}
		<!-- Desktop view - Table -->
		<div class="hidden overflow-hidden rounded-lg bg-white shadow-md md:block">
			<table class="min-w-full divide-y divide-gray-200">
				<thead class="bg-gray-50">
					<tr>
						<th class="w-2/5 px-6 py-3 text-left text-xs font-medium uppercase text-gray-500"
							>Koostisosa</th
						>
						<th class="w-1/6 px-6 py-3 text-left text-xs font-medium uppercase text-gray-500"
							>Kogus</th
						>
						<th class="w-1/6 px-6 py-3 text-left text-xs font-medium uppercase text-gray-500"
							>Lisatud</th
						>
						<th class="w-1/4 px-6 py-3 text-right text-xs font-medium uppercase text-gray-500"
							>Tegevused</th
						>
					</tr>
				</thead>
				<tbody class="divide-y divide-gray-200 bg-white">
					{#each fridgeItems as item}
						<tr>
							<td class="px-6 py-4">{item.ingredient}</td>
							<td class="px-6 py-4">
								{#if editingItem && editingItem.id === item.id}
									<input
										type="number"
										bind:value={editAmount}
										min="0"
										step="1"
										class="focus:border-primary focus:ring-primary w-20 rounded-md border-gray-300 shadow-sm"
									/>
								{:else}
									{item.amount}
								{/if}
							</td>
							<td class="px-6 py-4">{new Date(item.lastUpdated).toLocaleDateString()}</td>
							<td class="flex justify-end space-x-2 px-6 py-4">
								{#if editingItem && editingItem.id === item.id}
									<button
										on:click={saveEdit}
										class="text-secondary hover:cursor-pointer hover:text-gray-700 hover:transition"
									>
										Salvesta
									</button>
									<button
										on:click={cancelEditing}
										class="text-secondary hover:cursor-pointer hover:text-gray-700 hover:transition"
									>
										Tühista
									</button>
								{:else}
									<button
										on:click={() => startEditing(item)}
										class="text-secondary mr-3 hover:cursor-pointer hover:text-gray-700 hover:transition"
									>
										Muuda kogust
									</button>
									<button
										on:click={() => removeItem(item.id)}
										class="text-red-600 hover:cursor-pointer hover:text-red-900 hover:transition"
									>
										Kustuta
									</button>
								{/if}
							</td>
						</tr>
					{/each}
				</tbody>
			</table>
		</div>

		<!-- Mobile view - Cards -->
		<div class="space-y-4 md:hidden">
			{#each fridgeItems as item}
				<div class="rounded-lg bg-white p-4 shadow-md">
					<div class="mb-2 flex justify-between">
						<h3 class="text-lg font-medium">{item.ingredient}</h3>
						<span class="text-sm text-gray-500"
							>{new Date(item.lastUpdated).toLocaleDateString()}</span
						>
					</div>

					<div class="mb-4 flex items-center">
						<span class="mr-2 font-medium">Kogus:</span>
						{#if editingItem && editingItem.id === item.id}
							<input
								type="number"
								bind:value={editAmount}
								min="0"
								step="1"
								class="focus:border-primary focus:ring-primary w-20 rounded-md border-gray-300 shadow-sm"
							/>
						{:else}
							<span>{item.amount}</span>
						{/if}
					</div>

					<div class="flex justify-end space-x-3">
						{#if editingItem && editingItem.id === item.id}
							<button
								on:click={saveEdit}
								class="bg-primary hover:text-secondary rounded-md px-3 py-2 text-sm text-gray-700 hover:cursor-pointer hover:transition"
							>
								Salvesta
							</button>
							<button
								on:click={cancelEditing}
								class="rounded-md bg-gray-100 px-3 py-2 text-sm text-gray-700 hover:cursor-pointer hover:bg-gray-200 hover:transition"
							>
								Tühista
							</button>
						{:else}
							<button
								on:click={() => startEditing(item)}
								class="rounded-md bg-blue-100 px-3 py-2 text-sm text-gray-700 hover:cursor-pointer hover:bg-blue-200 hover:transition"
							>
								Muuda
							</button>
							<button
								on:click={() => removeItem(item.id)}
								class="rounded-md bg-red-100 px-3 py-2 text-sm text-red-800 hover:cursor-pointer hover:bg-red-200 hover:transition"
							>
								Kustuta
							</button>
						{/if}
					</div>
				</div>
			{/each}
		</div>
	{/if}
</div>
