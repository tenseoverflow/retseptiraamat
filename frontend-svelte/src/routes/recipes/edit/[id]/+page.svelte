<script lang="ts">
	import { goto } from '$app/navigation';
	import { onMount } from 'svelte';
	import { page } from '$app/stores';

	interface Ingredient {
		name: string;
		amount: number;
	}

	let title = '';
	let description = '';
	let ingredients: Ingredient[] = [];
	let error = '';
	let success = '';
	let loading = true;
	const recipeId = $page.params.id;

	onMount(async () => {
		try {
			const response = await fetch(`http://localhost:5036/api/Recipe/${recipeId}`);

			if (response.ok) {
				const recipe = await response.json();
				title = recipe.title;
				description = recipe.description;

				// Convert ingredients object to array format for editing
				ingredients = Object.entries(recipe.ingredients).map(([name, amount]) => ({
					name,
					amount: amount as number
				}));
			} else {
				error = 'Failed to load recipe';
			}
		} catch (e) {
			error = 'Failed to load recipe';
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

	async function updateRecipe() {
		try {
			// Convert ingredients array to object format required by API
			const ingredientsObject = ingredients.reduce(
				(obj, ingredient) => {
					if (ingredient.name) {
						obj[ingredient.name] = ingredient.amount;
					}
					return obj;
				},
				{} as Record<string, number>
			);

			const recipeData = {
				id: parseInt(recipeId),
				title,
				description,
				ingredients: ingredientsObject
			};

			const response = await fetch(`http://localhost:5036/api/Recipe/${recipeId}`, {
				method: 'PUT',
				headers: { 'Content-Type': 'application/json' },
				body: JSON.stringify(recipeData)
			});

			if (response.ok) {
				success = 'Recipe updated successfully!';

				// Navigate to recipes list after 2 seconds
				setTimeout(() => {
					goto('/recipes');
				}, 2000);
			} else {
				error = 'Failed to update recipe';
			}
		} catch (e) {
			error = 'Failed to update recipe';
		}
	}

	async function deleteRecipe() {
		if (confirm('Are you sure you want to delete this recipe?')) {
			try {
				const response = await fetch(`http://localhost:5036/api/Recipe/${recipeId}`, {
					method: 'DELETE'
				});

				if (response.ok) {
					success = 'Recipe deleted successfully!';

					// Navigate to recipes list after a short delay
					setTimeout(() => {
						goto('/recipes');
					}, 2000);
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
	<h1 class="mb-6 text-2xl font-bold">Edit Recipe</h1>

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
		<div class="flex justify-center">
			<p class="text-gray-500">Loading recipe...</p>
		</div>
	{:else}
		<form class="space-y-6" on:submit|preventDefault={updateRecipe}>
			<div>
				<label for="title" class="block text-sm font-medium text-gray-700">Title</label>
				<input
					id="title"
					type="text"
					bind:value={title}
					class="focus:ring-primary focus:border-primary mt-1 block w-full rounded-md border-gray-300 shadow-sm"
					required
				/>
			</div>

			<div>
				<label for="description" class="block text-sm font-medium text-gray-700">Description</label>
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
					<h3 class="text-lg font-medium">Ingredients</h3>
					<button
						type="button"
						on:click={addIngredient}
						class="rounded-md border border-gray-300 px-3 py-1 text-sm shadow-sm hover:cursor-pointer hover:bg-gray-50 hover:transition"
					>
						+ Add Ingredient
					</button>
				</div>

				<div class="mt-3 space-y-4">
					{#each ingredients as ingredient, i}
						<div class="flex items-end gap-4">
							<div class="flex-1">
								<label class="block text-sm font-medium text-gray-700">Ingredient Name</label>
								<input
									type="text"
									bind:value={ingredient.name}
									class="focus:border-primary focus:ring-primary mt-1 block w-full rounded-md border-gray-300 shadow-sm"
									required
								/>
							</div>
							<div class="w-32">
								<label class="block text-sm font-medium text-gray-700">Amount</label>
								<input
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
								Remove
							</button>
						</div>
					{/each}
				</div>
			</div>

			<div class="flex justify-end gap-x-2">
				<button
					on:click={deleteRecipe}
					class="rounded-md bg-red-600 px-4 py-2 text-sm font-medium text-white shadow-sm hover:cursor-pointer hover:bg-red-800 hover:transition"
					title="Delete recipe"
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
					class="bg-primary text-secondary rounded-md px-4 py-2 text-sm font-medium font-semibold shadow-sm hover:cursor-pointer hover:text-gray-700 hover:transition"
				>
					Uuenda
				</button>
			</div>
		</form>
	{/if}
</div>
