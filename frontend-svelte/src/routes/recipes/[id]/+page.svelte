<script lang="ts">
	import { page } from '$app/stores';
	import { goto } from '$app/navigation';
	import { onMount } from 'svelte';
	import { getRecipe, getFridgeItems, removeRecipeIngredientsFromFridge } from '$lib/api';
	import { showError, showSuccess } from '$lib/toast';

	const recipeId = parseInt($page.params.id);
	let recipe: any = null;
	let loading = true;
	let isRecommended = false;
	let fridgeItems: any[] = [];
	let isRemoving = false;

	function canMakeRecipe(recipe: any, fridgeItems: any[]): boolean {
		if (!recipe || !fridgeItems.length) return false;

		// Create a map of ingredients from fridge for easier lookup
		const fridgeIngredients = fridgeItems.reduce(
			(map, item) => {
				map[item.ingredient.toLowerCase()] =
					(map[item.ingredient.toLowerCase()] || 0) + item.amount;
				return map;
			},
			{} as Record<string, number>
		);

		// Check if all recipe ingredients are available in sufficient quantities
		for (const [ingredient, amount] of Object.entries(recipe.ingredients)) {
			const ingredientLower = ingredient.toLowerCase();
			// Cast amount to number as Object.entries returns unknown for values
			const numericAmount = amount as number;
			if (
				!fridgeIngredients[ingredientLower] ||
				fridgeIngredients[ingredientLower] < numericAmount
			) {
				return false;
			}
		}

		return true;
	}

	onMount(async () => {
		try {
			// Load recipe details
			recipe = await getRecipe(recipeId);

			// Load fridge items to check if recipe can be made
			fridgeItems = await getFridgeItems();

			// Check if recipe can be made with available ingredients
			isRecommended = canMakeRecipe(recipe, fridgeItems);
		} catch (e) {
			showError(e instanceof Error ? e.message : 'Retsepti laadimine ebaõnnestus');
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

		try {
			const result = await removeRecipeIngredientsFromFridge(recipe.ingredients);
			const updatedCount = result.updated.length;
			const removedCount = result.removed.length;

			if (updatedCount > 0 || removedCount > 0) {
				let message = '';

				if (updatedCount > 0) {
					message += `${updatedCount} koostisosa kogust vähendati. `;
				}

				if (removedCount > 0) {
					message += `${removedCount} koostisosa eemaldati täielikult külmkapist.`;
				}

				showSuccess(message);

				// Refresh fridge items to update isRecommended status
				fridgeItems = await getFridgeItems();
				isRecommended = recipe ? canMakeRecipe(recipe, fridgeItems) : false;
			} else {
				showSuccess('Külmkapis ei olnud ühtegi vastava retsepti koostisosa.');
			}
		} catch (e) {
			showError('Koostisosade eemaldamine ebaõnnestus.');
		} finally {
			isRemoving = false;

			// Redirect to recipes after short delay
			if (!isRecommended) {
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
			class="text-secondary flex items-center rounded-md px-2 py-1 hover:bg-gray-100 hover:text-gray-700"
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

	{#if loading}
		<div class="flex justify-center p-12">
			<p class="text-gray-500">Laadin retsepti...</p>
		</div>
	{:else if recipe}
		<div class="overflow-hidden rounded-lg bg-white p-6 shadow-md">
			<div class="mb-4 flex items-center justify-between">
				<h1 class="text-3xl font-bold">{recipe.title}</h1>
				<div class="flex items-center space-x-2">
					{#if isRecommended}
						<span
							class="flex items-center rounded-full bg-green-100 px-3 py-1 text-sm text-green-800"
							title="Sul on olemas kõik koostisosad selle retsepti jaoks"
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
									d="M5 13l4 4L19 7"
								/>
							</svg>
							Saad teha
						</span>
					{/if}

					<a
						href="/recipes/edit/{recipeId}"
						class="text-secondary flex items-center rounded-md px-3 py-2 hover:bg-gray-100 hover:text-gray-700 hover:transition"
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
								d="M11 5H6a2 2 0 00-2 2v11a2 2 0 002 2h11a2 2 0 002-2v-5m-1.414-9.414a2 2 0 112.828 2.828L11.828 15H9v-2.828l8.586-8.586z"
							/>
						</svg>
						Muuda
					</a>
				</div>
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
								class="bg-primary text-secondary rounded-md px-4 py-2 text-sm font-medium shadow-sm hover:cursor-pointer hover:text-gray-700 hover:transition"
							>
								{isRemoving ? 'Eemaldan...' : 'Eemalda koostisosad külmkapist'}
							</button>
						</div>
					{/if}
				</div>
			</div>
		</div>
	{:else}
		<div class="flex justify-center">
			<p class="text-red-500">Retsepti ei leitud</p>
		</div>
	{/if}
</div>
