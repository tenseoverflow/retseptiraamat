<!-- filepath: /home/hen/Dokumendid/Projektid/retseptiraamat/frontend-svelte/src/routes/recipes/add/+page.svelte -->
<script lang="ts">
	import { goto } from '$app/navigation';

	interface Ingredient {
		name: string;
		amount: number;
	}

	let title = '';
	let description = '';
	let ingredients: Ingredient[] = [{ name: '', amount: 0 }];
	let error = '';
	let success = '';

	function addIngredient() {
		ingredients = [...ingredients, { name: '', amount: 0 }];
	}

	function removeIngredient(index: number) {
		ingredients = ingredients.filter((_, i) => i !== index);
	}

	async function addRecipe() {
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
				title,
				description,
				ingredients: ingredientsObject
			};

			const response = await fetch('http://localhost:5036/api/Recipe', {
				method: 'POST',
				headers: { 'Content-Type': 'application/json' },
				body: JSON.stringify(recipeData)
			});

			if (response.ok) {
				success = 'Recipe added successfully!';
				// Reset form
				title = '';
				description = '';
				ingredients = [{ name: '', amount: 0 }];

				// Navigate to recipes list after 2 seconds
				setTimeout(() => {
					goto('/recipes');
				}, 2000);
			} else {
				error = 'Failed to add recipe';
			}
		} catch (e) {
			error = 'Failed to add recipe';
		}
	}
</script>

<div class="container mx-auto p-4">
	<h1 class="mb-6 text-2xl font-bold">Add New Recipe</h1>

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

	<form class="space-y-6" on:submit|preventDefault={addRecipe}>
		<div>
			<label for="title" class="block text-sm font-medium text-gray-700">Title</label>
			<input
				id="title"
				type="text"
				bind:value={title}
				class="mt-1 block w-full rounded-md border-gray-300 shadow-sm focus:border-green-500 focus:ring-green-500"
				required
			/>
		</div>

		<div>
			<label for="description" class="block text-sm font-medium text-gray-700">Description</label>
			<textarea
				id="description"
				bind:value={description}
				rows="4"
				class="mt-1 block w-full rounded-md border-gray-300 shadow-sm focus:border-green-500 focus:ring-green-500"
				required
			></textarea>
		</div>

		<div>
			<div class="flex items-center justify-between">
				<h3 class="text-lg font-medium">Ingredients</h3>
				<button
					type="button"
					on:click={addIngredient}
					class="rounded-md bg-green-100 px-3 py-1 text-sm text-green-800 hover:bg-green-200"
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
								class="mt-1 block w-full rounded-md border-gray-300 shadow-sm focus:border-green-500 focus:ring-green-500"
								required
							/>
						</div>
						<div class="w-32">
							<label class="block text-sm font-medium text-gray-700">Amount</label>
							<input
								type="number"
								bind:value={ingredient.amount}
								class="mt-1 block w-full rounded-md border-gray-300 shadow-sm focus:border-green-500 focus:ring-green-500"
								required
							/>
						</div>
						{#if ingredients.length > 1}
							<button
								type="button"
								on:click={() => removeIngredient(i)}
								class="rounded-md bg-red-100 px-3 py-2 text-sm text-red-800 hover:bg-red-200"
							>
								Remove
							</button>
						{/if}
					</div>
				{/each}
			</div>
		</div>

		<div class="flex justify-end">
			<a
				href="/recipes"
				class="mr-2 rounded-md border border-gray-300 bg-white px-4 py-2 text-sm font-medium text-gray-700 shadow-sm hover:bg-gray-50"
			>
				Cancel
			</a>
			<button
				type="submit"
				class="rounded-md bg-green-600 px-4 py-2 text-sm font-medium text-white shadow-sm hover:bg-green-700"
			>
				Add Recipe
			</button>
		</div>
	</form>
</div>
