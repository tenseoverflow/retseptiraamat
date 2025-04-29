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
				success = 'Retsept lisatud edukalt!';
				// Reset form
				title = '';
				description = '';
				ingredients = [{ name: '', amount: 0 }];

				// Navigate to recipes list after 2 seconds
				setTimeout(() => {
					goto('/recipes');
				}, 2000);
			} else {
				error = 'Retsepti lisamine ebaõnnestus';
			}
		} catch (e) {
			error = 'Retsepti lisamine ebaõnnestus';
		}
	}
</script>

<div class="container mx-auto p-4">
	<h1 class="mb-6 text-2xl font-bold">Lisa uus retsept</h1>

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
							<label class="block text-sm font-medium text-gray-700">Nimetus</label>
							<input
								type="text"
								bind:value={ingredient.name}
								class="focus:border-primary focus:ring-primary mt-1 block w-full rounded-md border-gray-300 shadow-sm"
								required
							/>
						</div>
						<div class="w-32">
							<label class="block text-sm font-medium text-gray-700">Kogus</label>
							<input
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
				class="bg-primary text-secondary rounded-md px-4 py-2 text-sm font-semibold shadow-sm hover:text-gray-700 hover:transition"
			>
				Lisa retsept
			</button>
		</div>
	</form>
</div>
