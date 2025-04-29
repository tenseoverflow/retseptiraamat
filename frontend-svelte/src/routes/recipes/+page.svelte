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
	let success = '';

	onMount(async () => {
		try {
			const response = await fetch('http://localhost:5036/api/Recipe');
			recipes = await response.json();
		} catch (e) {
			error = 'Failed to load recipes';
		} finally {
			loading = false;
		}
	});

	async function deleteRecipe(id: number) {
		if (confirm('Are you sure you want to delete this recipe?')) {
			try {
				const response = await fetch(`http://localhost:5036/api/Recipe/${id}`, {
					method: 'DELETE'
				});

				if (response.ok) {
					recipes = recipes.filter((recipe) => recipe.id !== id);
					success = 'Recipe deleted successfully!';

					// Clear success message after 3 seconds
					setTimeout(() => {
						success = '';
					}, 3000);
				} else {
					error = 'Failed to delete recipe';

					// Clear error message after 3 seconds
					setTimeout(() => {
						error = '';
					}, 3000);
				}
			} catch (e) {
				error = 'Failed to delete recipe';

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
		<h1 class="text-2xl font-bold">Recipes</h1>
		<a
			href="/recipes/add"
			class="bg-primary hover:bg-primary-dark rounded-md px-4 py-2 text-gray-800"
		>
			Add Recipe
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
		<p>Loading...</p>
	{:else if recipes.length === 0}
		<div class="rounded-md bg-gray-50 p-6 text-center">
			<p class="mb-2 text-gray-500">No recipes found</p>
			<p class="text-gray-500">Start by adding your first recipe</p>
		</div>
	{:else}
		<div class="grid grid-cols-1 gap-6 md:grid-cols-2 lg:grid-cols-3">
			{#each recipes as recipe}
				<div class="overflow-hidden rounded-lg bg-white shadow-md">
					<div class="p-6">
						<div class="mb-3 flex items-start justify-between">
							<h2 class="text-xl font-semibold">{recipe.title}</h2>
							<div class="flex space-x-2">
								<a
									href={`/recipes/edit/${recipe.id}`}
									class="text-blue-600 hover:text-blue-800"
									title="Edit recipe"
								>
									<svg
										xmlns="http://www.w3.org/2000/svg"
										class="h-5 w-5"
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
								</a>
								<button
									on:click={() => deleteRecipe(recipe.id)}
									class="text-red-600 hover:text-red-800"
									title="Delete recipe"
								>
									<svg
										xmlns="http://www.w3.org/2000/svg"
										class="h-5 w-5"
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
								</button>
							</div>
						</div>

						<p class="mb-4 text-gray-600">
							{recipe.description.length > 100
								? recipe.description.slice(0, 100) + '...'
								: recipe.description}
						</p>

						<div class="mb-4">
							<h3 class="mb-2 font-medium">Ingredients:</h3>
							<ul class="list-inside list-disc">
								{#each Object.entries(recipe.ingredients).slice(0, 3) as [ingredient, amount]}
									<li>{ingredient}: {amount}</li>
								{/each}
								{#if Object.keys(recipe.ingredients).length > 3}
									<li class="text-gray-500">
										+{Object.keys(recipe.ingredients).length - 3} more...
									</li>
								{/if}
							</ul>
						</div>

						<div class="mt-4 flex items-center justify-between">
							<div class="text-sm text-gray-500">
								<p>Created: {new Date(recipe.createdAt).toLocaleDateString()}</p>
							</div>

							<a
								href={`/recipes/${recipe.id}`}
								class="bg-primary hover:bg-primary-dark inline-block rounded-md px-4 py-2 text-sm text-gray-800"
							>
								View Recipe
							</a>
						</div>
					</div>
				</div>
			{/each}
		</div>
	{/if}
</div>
