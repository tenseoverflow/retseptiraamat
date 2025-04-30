<script lang="ts">
	import { page } from '$app/stores';
	import { goto } from '$app/navigation';
	import { onMount } from 'svelte';
	import { getRecipe, updateRecipe, deleteRecipe as deleteRecipeAPI } from '$lib/api';
	import { showError, showSuccess } from '$lib/toast';

	const recipeId = parseInt($page.params.id);
	let title = '';
	let description = '';
	let ingredients: { name: string; amount: number }[] = [];
	let loading = true;
	let isUpdating = false;
	let isDeleting = false;

	onMount(async () => {
		try {
			const recipe = await getRecipe(recipeId);

			title = recipe.title;
			description = recipe.description;

			// Convert ingredients object to array for editing
			ingredients = Object.entries(recipe.ingredients).map(([name, amount]) => ({
				name,
				amount: amount as number
			}));
		} catch (e) {
			showError('Retsepti laadimine ebaõnnestus');
		} finally {
			loading = false;
		}
	});

	function addIngredient() {
		ingredients = [...ingredients, { name: '', amount: 0 }];
	}

	function removeIngredient(index: number) {
		ingredients = ingredients.filter((_, i) => i !== index);
	}

	async function handleUpdateRecipe() {
		if (isUpdating) return; // Prevent multiple simultaneous submissions

		try {
			isUpdating = true;

			// Check for ingredients with zero amount
			const zeroAmountIngredients = ingredients.filter((ing) => ing.name && ing.amount === 0);
			if (zeroAmountIngredients.length > 0) {
				showError(`Koostisosa "${zeroAmountIngredients[0].name}" kogus ei saa olla 0`);
				isUpdating = false;
				return;
			}

			// Convert ingredients array to object format required by API
			const ingredientsObject = ingredients.reduce(
				(obj, ing) => {
					if (ing.name) {
						obj[ing.name] = ing.amount;
					}
					return obj;
				},
				{} as Record<string, number>
			);

			// Update recipe payload
			const recipeData = {
				id: recipeId, // Include the ID to match backend expectation
				title,
				description,
				ingredients: ingredientsObject,
				updatedAt: new Date().toISOString()
			};

			await updateRecipe(recipeId, recipeData);
			showSuccess('Retsept uuendatud');

			// Redirect to recipe view after a short delay
			setTimeout(() => {
				goto(`/recipes/${recipeId}`);
			}, 1000);
		} catch (e) {
			showError('Retsepti uuendamine ebaõnnestus');
		} finally {
			isUpdating = false;
		}
	}

	async function deleteRecipe() {
		if (confirm('Kas oled kindel, et soovid selle retsepti kustutada?')) {
			if (!isDeleting) {
				try {
					isDeleting = true;

					await deleteRecipeAPI(recipeId);
					showSuccess('Retsept kustutatud');

					// Use a shorter timeout for navigation to avoid disconnected port issues
					setTimeout(() => {
						goto('/recipes');
					}, 800);
				} catch (e) {
					console.error('Delete failed:', e);
					showError('Retsepti kustutamine ebaõnnestus');
				} finally {
					isDeleting = false;
				}
			}
		}
	}
</script>

<div class="container mx-auto p-4">
	<h1 class="mb-6 text-2xl font-bold">Muuda retsepti</h1>

	{#if loading}
		<div class="flex justify-center">
			<p class="text-gray-500">Laadin retsepti...</p>
		</div>
	{:else}
		<form class="space-y-6" on:submit|preventDefault={handleUpdateRecipe}>
			<div>
				<label for="title" class="block text-sm font-medium text-gray-700">Pealkiri</label>
				<input
					id="title"
					type="text"
					bind:value={title}
					class="focus:ring-primary focus:border-primary mt-1 block w-full rounded-md border-gray-300 shadow-sm"
					required
				/>
			</div>

			<div>
				<label for="description" class="block text-sm font-medium text-gray-700">Kirjeldus</label>
				<textarea
					id="description"
					bind:value={description}
					rows="4"
					class="focus:ring-primary focus:border-primary mt-1 block w-full rounded-md border-gray-300 shadow-sm"
					required
				></textarea>
			</div>

			<div>
				<div class="flex items-center justify-between">
					<h3 class="text-lg font-medium">Koostisosad</h3>
					<button
						type="button"
						on:click={addIngredient}
						class="rounded-md border border-gray-300 px-3 py-1 text-sm shadow-sm hover:cursor-pointer hover:bg-gray-50 hover:transition"
					>
						+ Lisa koostisosa
					</button>
				</div>

				<div class="mt-3 space-y-4">
					{#each ingredients as ingredient, i}
						<div class="flex items-end gap-4">
							<div class="flex-1">
								<label for="ingredient-name-{i}" class="block text-sm font-medium text-gray-700"
									>Nimetus</label
								>
								<input
									id="ingredient-name-{i}"
									type="text"
									bind:value={ingredient.name}
									class="focus:border-primary focus:ring-primary mt-1 block w-full rounded-md border-gray-300 shadow-sm"
									required
								/>
							</div>
							<div class="w-32">
								<label for="ingredient-amount-{i}" class="block text-sm font-medium text-gray-700"
									>Kogus</label
								>
								<input
									id="ingredient-amount-{i}"
									type="number"
									bind:value={ingredient.amount}
									class="focus:border-primary focus:ring-primary mt-1 block w-full rounded-md border-gray-300 shadow-sm"
									required
								/>
							</div>
							<button
								type="button"
								on:click={() => removeIngredient(i)}
								class="rounded-md bg-red-100 px-3 py-2 text-sm text-red-800 hover:cursor-pointer hover:bg-red-200 hover:transition"
							>
								Eemalda
							</button>
						</div>
					{/each}
				</div>
			</div>

			<div class="flex justify-end gap-x-2">
				<button
					type="button"
					on:click={deleteRecipe}
					class="rounded-md bg-red-600 px-4 py-2 text-sm font-medium text-white shadow-sm hover:cursor-pointer hover:bg-red-800 hover:transition"
					title="Kustuta retsept"
				>
					Kustuta
				</button>
				<a
					href="/recipes"
					class="text-secondary rounded-md border border-gray-300 bg-white px-4 py-2 text-sm font-medium shadow-sm hover:bg-gray-50 hover:text-gray-700 hover:transition"
				>
					Tühista
				</a>
				<button
					type="submit"
					class="bg-primary text-secondary rounded-md px-4 py-2 text-sm font-semibold shadow-sm hover:cursor-pointer hover:text-gray-700 hover:transition"
				>
					Uuenda
				</button>
			</div>
		</form>
	{/if}
</div>
