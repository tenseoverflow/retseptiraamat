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
			error = 'Retseptide laadimine ebaõnnestus';
		} finally {
			loading = false;
		}
	});
</script>

<div class="container mx-auto p-4">
	<div class="mb-6 flex items-center justify-between">
		<h1 class="text-2xl font-bold">Retseptid</h1>
		<a
			href="/recipes/add"
			class="bg-primary text-secondary rounded-md px-4 py-2 font-semibold hover:text-gray-700 hover:transition"
		>
			Lisa retsept
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
		<p>Laadin...</p>
	{:else if recipes.length === 0}
		<div class="rounded-md bg-gray-50 p-6 text-center">
			<p class="mb-2 text-gray-500">Ühtegi retsepti ei leitud</p>
			<p class="text-gray-500">Alusta oma esimese retsepti lisamisega</p>
		</div>
	{:else}
		<div class="grid grid-cols-1 gap-6 md:grid-cols-2 lg:grid-cols-3">
			{#each recipes as recipe}
				<div class="flex h-full flex-col overflow-hidden rounded-lg bg-white shadow-md">
					<div class="flex flex-grow flex-col p-6">
						<h2 class="mb-3 text-xl font-semibold">{recipe.title}</h2>

						<div class="flex-grow">
							<p class="mb-4 text-gray-600">
								{recipe.description.length > 100
									? recipe.description.slice(0, 100) + '...'
									: recipe.description}
							</p>

							<div class="mb-4">
								<h3 class="mb-2 font-medium">Koostisosad:</h3>
								<ul class="list-inside list-disc">
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
						</div>

						<div class="mt-auto flex items-center justify-between pt-4">
							<div class="text-sm text-gray-500">
								<p>Loodud: {new Date(recipe.createdAt).toLocaleDateString()}</p>
							</div>

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
