<script lang="ts">
	import { goto } from '$app/navigation';
	import { createRecipe } from '$lib/api';
	import { showError, showSuccess } from '$lib/toast';

	let title = '';
	let description = '';
	let ingredients: { name: string; amount: number }[] = [{ name: '', amount: 0 }];
	let isAdding = false;

	function addIngredient() {
		ingredients = [...ingredients, { name: '', amount: 0 }];
	}

	function removeIngredient(index: number) {
		ingredients = ingredients.filter((_, i) => i !== index);
	}

	async function addRecipe() {
		if (isAdding) return; // Prevent multiple submissions

		try {
			isAdding = true;

			// Check for ingredients with zero amount
			const zeroAmountIngredients = ingredients.filter((ing) => ing.name && ing.amount === 0);
			if (zeroAmountIngredients.length > 0) {
				showError(`Koostisosa "${zeroAmountIngredients[0].name}" kogus ei saa olla 0`);
				isAdding = false;
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

			// Create recipe payload
			const recipeData = {
				title,
				description,
				ingredients: ingredientsObject,
				createdAt: new Date().toISOString()
			};

			// Add recipe via API
			await createRecipe(recipeData);

			showSuccess('Retsept lisatud');

			// Redirect to recipes list after a short delay
			setTimeout(() => {
				goto('/recipes');
			}, 1000);
		} catch (e) {
			showError('Retsepti lisamine ebaõnnestus');
		} finally {
			isAdding = false;
		}
	}
</script>

<div class="container mx-auto p-4">
	<h1 class="mb-6 text-2xl font-bold">Lisa uus retsept</h1>

	<form class="space-y-6" on:submit|preventDefault={addRecipe}>
		<div>
			<label for="title" class="block text-sm font-medium text-gray-700">Pealkiri</label>
			<input
				id="title"
				type="text"
				bind:value={title}
				class="focus:border-primary focus:ring-primary mt-1 block w-full rounded-md border-gray-300 shadow-sm"
				required
			/>
		</div>

		<div>
			<label for="description" class="block text-sm font-medium text-gray-700">Kirjeldus</label>
			<textarea
				id="description"
				bind:value={description}
				rows="4"
				class="focus:border-primary focus:ring-primary mt-1 block w-full rounded-md border-gray-300 shadow-sm"
				required
			></textarea>
		</div>

		<div>
			<div class="flex items-center justify-between">
				<h3 class="text-lg font-medium">Koostisosad</h3>
				<button
					type="button"
					on:click={addIngredient}
					class="bg-primary text-secondary rounded-md px-3 py-1 text-sm font-semibold hover:text-gray-700 hover:transition"
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
						{#if ingredients.length > 1}
							<button
								type="button"
								on:click={() => removeIngredient(i)}
								class="rounded-md bg-red-100 px-3 py-2 text-sm text-red-800 hover:bg-red-200"
							>
								Eemalda
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
				Tühista
			</a>
			<button
				type="submit"
				disabled={isAdding}
				class="bg-primary text-secondary rounded-md px-4 py-2 text-sm font-semibold shadow-sm hover:text-gray-700 hover:transition {isAdding
					? 'cursor-not-allowed opacity-70'
					: ''}"
			>
				{isAdding ? 'Lisan...' : 'Lisa retsept'}
			</button>
		</div>
	</form>
</div>
