<script lang="ts">
	import { onMount } from 'svelte';
	import { getRecipes, getFridgeItems } from '$lib/api';

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

	let recipes: Recipe[] = [];
	let fridgeItems: FridgeItem[] = [];
	let availableRecipes: Recipe[] = [];
	let loading = true;
	let error = '';

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
		try {
			// Fetch all recipes and fridge items using our API utility
			recipes = await getRecipes();
			fridgeItems = await getFridgeItems();

			// Filter recipes that can be made with the current fridge inventory
			availableRecipes = recipes.filter((recipe) => canMakeRecipe(recipe, fridgeItems));
		} catch (e) {
			error = 'Andmete laadimine ebaõnnestus';
		} finally {
			loading = false;
		}
	});
</script>

<div class="container mx-auto p-4">
	<h1 class="mb-6 text-3xl font-bold">Soovitatud retseptid</h1>

	{#if error}
		<div class="mb-4 rounded border border-red-400 bg-red-100 px-4 py-3 text-red-700">
			{error}
		</div>
	{/if}

	{#if loading}
		<div class="flex justify-center py-12">
			<p class="text-gray-500">Laen retsepte...</p>
		</div>
	{:else}
		<div class="grid grid-cols-1 gap-6 md:grid-cols-12">
			<!-- Main content with recipes -->
			<div class="md:col-span-12">
				{#if availableRecipes.length === 0}
					<div class="rounded-lg bg-white p-8 text-center shadow-md">
						<h2 class="mb-2 text-xl font-semibold">Pole ühtegi retsepti, mida saaksid teha</h2>
						<p class="mb-4 text-gray-600">Mine ostlema!</p>
						<div
							class="flex flex-col justify-center space-y-3 sm:flex-row sm:space-x-4 sm:space-y-0"
						>
							<a
								href="/fridge"
								class="bg-primary text-secondary rounded-md px-4 py-2 text-center font-semibold hover:text-gray-700 hover:transition"
							>
								Lisa koostisosi külmkappi
							</a>
							<a
								href="/recipes"
								class="rounded-md border border-gray-300 bg-white px-4 py-2 text-center text-gray-700 hover:bg-gray-50 hover:transition"
							>
								Kõik retseptid
							</a>
						</div>
					</div>
				{:else}
					<div class="grid grid-cols-1 gap-6 sm:grid-cols-2 lg:grid-cols-3">
						{#each availableRecipes as recipe}
							<div
								class="flex h-full flex-col overflow-hidden rounded-lg bg-white shadow-md transition-shadow hover:shadow-lg"
							>
								<div class="flex flex-grow flex-col p-6">
									<div class="flex-grow">
										<h3 class="mb-2 text-xl font-semibold">{recipe.title}</h3>
										<p class="mb-4 line-clamp-3 whitespace-pre-line text-gray-600">
											{recipe.description.length > 100
												? recipe.description.slice(0, 100) + '...'
												: recipe.description}
										</p>

										<h4 class="mb-2 font-medium">Koostisosad:</h4>
										<ul class="mb-4 list-inside list-disc">
											{#each Object.entries(recipe.ingredients).slice(0, 3) as [ingredient, amount]}
												<li>{ingredient}: {amount}</li>
											{/each}
											{#if Object.keys(recipe.ingredients).length > 3}
												<li class="text-gray-500">
													+{Object.keys(recipe.ingredients).length - 3} veel...
												</li>
											{/if}
										</ul>
									</div>

									<div class="mt-auto flex justify-end pt-4">
										<a
											href={`/recipes/${recipe.id}`}
											class="bg-primary text-secondary inline-block rounded-md px-4 py-2 font-semibold hover:text-gray-700 hover:transition"
										>
											Vaata retsepti
										</a>
									</div>
								</div>
							</div>
						{/each}
					</div>
				{/if}
			</div>
		</div>
	{/if}
</div>
