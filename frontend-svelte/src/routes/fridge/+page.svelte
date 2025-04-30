<script lang="ts">
	import { onMount } from 'svelte';
	import { getFridgeItems, createFridgeItem, deleteFridgeItem, updateFridgeItem } from '$lib/api';

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
	let editingItem: FridgeItem | null = null;
	let editAmount: number = 0;

	onMount(async () => {
		try {
			fridgeItems = await getFridgeItems();
		} catch (e) {
			error = 'Külmkapi sisu laadimine ebaõnnestus';
		} finally {
			loading = false;
		}
	});

	async function addItem() {
		try {
			const itemToAdd = { ...newItem, lastUpdated: new Date().toISOString() };
			const addedItem = await createFridgeItem(itemToAdd);
			fridgeItems = [...fridgeItems, addedItem];
			newItem = { ingredient: '', amount: 0 };
			success = 'Koostisosa lisatud edukalt!';

			// Clear success message after 3 seconds
			setTimeout(() => {
				success = '';
			}, 3000);
		} catch (e) {
			// Check if error contains a duplicate ingredient message
			if (e instanceof Error && e.message.includes('already exists')) {
				error = e.message;
			} else {
				error = 'Koostisosa lisamine ebaõnnestus';
			}

			// Clear error message after 5 seconds
			setTimeout(() => {
				error = '';
			}, 5000);
		}
	}

	async function removeItem(id: number) {
		if (confirm('Kas oled kindel, et soovid selle koostisosa kustutada?')) {
			try {
				await deleteFridgeItem(id);
				fridgeItems = fridgeItems.filter((item) => item.id !== id);
				success = 'Koostisosa kustutatud edukalt!';

				// Clear success message after 3 seconds
				setTimeout(() => {
					success = '';
				}, 3000);
			} catch (e) {
				error = 'Koostisosa kustutamine ebaõnnestus';
				// Clear error message after 3 seconds
				setTimeout(() => {
					error = '';
				}, 3000);
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
			const updatedItem = {
				...editingItem,
				amount: editAmount,
				lastUpdated: new Date().toISOString()
			};

			await updateFridgeItem(editingItem.id, updatedItem);

			// Update the item in the local array
			fridgeItems = fridgeItems.map((item) => (item.id === editingItem!.id ? updatedItem : item));

			success = 'Kogus muudetud edukalt!';

			// Clear success message after 3 seconds
			setTimeout(() => {
				success = '';
			}, 3000);

			// Reset editing state
			editingItem = null;
		} catch (e) {
			error = 'Koostisosa muutmine ebaõnnestus';

			// Clear error message after 5 seconds
			setTimeout(() => {
				error = '';
			}, 5000);
		}
	}
</script>

<div class="container mx-auto p-4">
	<div class="mb-6 flex items-center justify-between">
		<h1 class="text-3xl font-bold">Külmkapi sisu</h1>
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
		<div class="overflow-hidden rounded-lg bg-white shadow-md">
			<table class="min-w-full divide-y divide-gray-200">
				<thead class="bg-gray-50">
					<tr>
						<th class="px-6 py-3 text-left text-xs font-medium uppercase text-gray-500"
							>Koostisosa</th
						>
						<th class="px-6 py-3 text-left text-xs font-medium uppercase text-gray-500">Kogus</th>
						<th class="px-6 py-3 text-left text-xs font-medium uppercase text-gray-500">Lisatud</th>
						<th class="px-6 py-3 text-left text-xs font-medium uppercase text-gray-500"
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
							<td class="flex space-x-2 px-6 py-4">
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
	{/if}
</div>
