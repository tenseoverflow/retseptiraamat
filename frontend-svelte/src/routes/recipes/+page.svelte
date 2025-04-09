<script lang="ts">
	import { onMount } from 'svelte';

	interface Recipe {
		id: number;
		title: string;
		description: string;
		ingredients: Record<string, number>;
		createdAt: string;
		updatedAt: string | null;
	}

	let recipes: Recipe[] = [];
	let loading = true;
	let error = '';

	onMount(async () => {
		try {
			const response = await fetch('http://localhost:5000/api/Recipe');
			recipes = await response.json();
		} catch (e) {
			error = 'Failed to load recipes';
		} finally {
			loading = false;
		}
	});
</script>

<div class="container mx-auto p-4">
	<h1 class="text-2xl font-bold mb-6">Recipes</h1>

	{#if error}
		<div class="bg-red-100 border border-red-400 text-red-700 px-4 py-3 rounded mb-4">
			{error}
		</div>
	{/if}

	{#if loading}
		<p>Loading...</p>
	{:else}
		<div class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6">
			{#each recipes as recipe}
				<div class="bg-white rounded-lg shadow-md overflow-hidden">
					<div class="p-6">
						<h2 class="text-xl font-semibold mb-2">{recipe.title}</h2>
						<p class="text-gray-600 mb-4">{recipe.description}</p>

						<h3 class="font-medium mb-2">Ingredients:</h3>
						<ul class="list-disc list-inside mb-4">
							{#each Object.entries(recipe.ingredients) as [ingredient, amount]}
								<li>{ingredient}: {amount}</li>
							{/each}
						</ul>

						<div class="text-sm text-gray-500">
							<p>Created: {new Date(recipe.createdAt).toLocaleDateString()}</p>
							{#if recipe.updatedAt}
								<p>Updated: {new Date(recipe.updatedAt).toLocaleDateString()}</p>
							{/if}
						</div>
					</div>
				</div>
			{/each}
		</div>
	{/if}
</div>
