<script lang="ts">
	import { onMount } from 'svelte';
	import { goto } from '$app/navigation';
	import { page } from '$app/stores';
	import { getRecipe, getFridgeItems, removeRecipeIngredientsFromFridge } from '$lib/api';

	interface Recipe {
		id: number;
		title: string;
		description: string;
		ingredients: Record<string, number>;
		createdAt: string;
		updatedAt: string | null;
	}

	interface FridgeItem {
		id: number;
		ingredient: string;
		amount: number;
		lastUpdated: string;
	}

	let recipe: Recipe | null = null;
	let fridgeItems: FridgeItem[] = [];
	let isRecommended = false;
	let loading = true;
	let error = '';
	let success = '';
	let isRemoving = false;

	// Function to determine if a recipe can be made with available ingredients
	function canMakeRecipe(recipe: Recipe, fridgeItems: FridgeItem[]): boolean {
		// Create a dictionary of ingredients in fridge with amounts
		const fridgeIngredients = fridgeItems.reduce(
			(acc, item) => {
				acc[item.ingredient.toLowerCase()] = item.amount;
				return acc;
			},
			{} as Record<string, number>
		);

		// Check if all ingredients from the recipe are available in sufficient quantity
		for (const [ingredient, requiredAmount] of Object.entries(recipe.ingredients)) {
			const ingredientName = ingredient.toLowerCase();
			if (
				!fridgeIngredients[ingredientName] ||
				fridgeIngredients[ingredientName] < requiredAmount
			) {
				return false;
			}
		}

		return true;
	}

	onMount(async () => {
		const id = parseInt($page.params.id);
		try {
			// Load both recipe and fridge items
			recipe = await getRecipe(id);
			fridgeItems = await getFridgeItems();

			// Check if this recipe is recommended (can be made with current fridge inventory)
			if (recipe) {
				isRecommended = canMakeRecipe(recipe, fridgeItems);
			}
		} catch (e) {
			error = e instanceof Error ? e.message : 'Retsepti laadimine ebaõnnestus';
		} finally {
			loading = false;
		}
	});

	async function removeIngredientsFromFridge() {
		if (!recipe) return;

		if (
			!confirm('Kas oled kindel, et soovid eemaldada kõik selle retsepti koostisosad külmkapist?')
		) {
			return;
		}

		isRemoving = true;
		error = '';
		success = '';

		try {
			const removedItems = await removeRecipeIngredientsFromFridge(recipe.ingredients);
			if (removedItems.length > 0) {
				success = `Eemaldatud ${removedItems.length} koostisosa külmkapist.`;
			} else {
				success = 'Külmkapis ei olnud ühtegi vastava retsepti koostisosa.';
			}
		} catch (e) {
			error = 'Koostisosade eemaldamine ebaõnnestus.';
		} finally {
			isRemoving = false;

			// Clear success message after 5 seconds
			if (success) {
				setTimeout(() => {
					goto('/recipes');
				}, 2000);
			}
		}
	}
</script>

<div class="container mx-auto p-4">
	<div class="mb-6 flex items-center justify-between">
		<a
			href="/recipes"
			class="text-secondary inline-flex items-center hover:text-gray-700 hover:transition"
		>
			<svg
				xmlns="http://www.w3.org/2000/svg"
				class="mr-1 h-5 w-5"
				fill="none"
				viewBox="0 0 24 24"
				stroke="currentColor"
			>
				<path
					stroke-linecap="round"
					stroke-linejoin="round"
					stroke-width="2"
					d="M10 19l-7-7m0 0l7-7m-7 7h18"
				/>
			</svg>
			Tagasi retseptide juurde
		</a>
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

	{#if loading}
		<div class="flex h-64 items-center justify-center">
			<p class="text-lg">Laen retsepti...</p>
		</div>
	{:else if recipe}
		<div class="overflow-hidden rounded-lg bg-white shadow-lg">
			<div class="p-8">
				<div class="mb-4 flex items-center justify-between">
					<h1 class="text-3xl font-bold">{recipe.title}</h1>
					<a
						href={`/recipes/edit/${recipe.id}`}
						class="bg-primary text-secondary flex items-center rounded-md px-4 py-2 text-sm font-semibold shadow-sm hover:text-gray-700 hover:transition"
					>
						<svg
							xmlns="http://www.w3.org/2000/svg"
							class="mr-1 h-4 w-4"
							fill="none"
							viewBox="0 0 24 24"
							stroke="currentColor"
						>
							<path
								stroke-linecap="round"
								stroke-linejoin="round"
								stroke-width="2"
								d="M11 5H6a2 2 0 00-2 2v11a2 2 0 002 2h11a2 2 0 002-2v-5m-1.414-9.414a2 2 0 112.828 2.828L11.828 15H9v-2.828l8.586-8.586z"
							/>
						</svg>
						Muuda
					</a>
				</div>

				<div class="mb-8 text-gray-600">
					<p class="whitespace-pre-line text-lg">{recipe.description}</p>
				</div>

				<div class="mb-6">
					<h2 class="mb-3 text-xl font-semibold">Koostisosad</h2>
					<div class="rounded-md bg-gray-50 p-4">
						<ul class="list-inside list-disc space-y-2">
							{#each Object.entries(recipe.ingredients) as [ingredient, amount]}
								<li class="text-gray-700">{ingredient}: {amount}</li>
							{/each}
						</ul>
					</div>
				</div>

				<div class="border-t border-gray-200 pt-4">
					<div class="flex flex-row justify-between">
						<div class="text-sm text-gray-500">
							<p>Loodud: {new Date(recipe.createdAt).toLocaleDateString()}</p>
							{#if recipe.updatedAt}
								<p>Uuendatud: {new Date(recipe.updatedAt).toLocaleDateString()}</p>
							{/if}
						</div>

						{#if isRecommended}
							<div>
								<button
									on:click={removeIngredientsFromFridge}
									disabled={isRemoving}
									class="flex items-center rounded-md bg-red-600 px-4 py-2 text-sm font-semibold text-white shadow-sm hover:cursor-pointer hover:bg-red-800 hover:transition disabled:opacity-50"
									title="Eemalda kõik retsepti koostisosad külmkapist"
								>
									<svg
										xmlns="http://www.w3.org/2000/svg"
										class="mr-1 h-4 w-4"
										fill="none"
										viewBox="0 0 24 24"
										stroke="currentColor"
									>
										<path
											stroke-linecap="round"
											stroke-linejoin="round"
											stroke-width="2"
											d="M19 7l-.867 12.142A2 2 0 0116.138 21H7.862a2 2 0 01-1.995-1.858L5 7m5 4v6m4-6v6m1-10V4a1 1 0 00-1-1h-4a1 1 0 00-1 1v3M4 7h16"
										/>
									</svg>
									{#if isRemoving}
										Eemaldan...
									{:else}
										Eemalda koostisosad külmkapist
									{/if}
								</button>
							</div>
						{/if}
					</div>
				</div>
			</div>
		</div>
	{/if}
</div>
