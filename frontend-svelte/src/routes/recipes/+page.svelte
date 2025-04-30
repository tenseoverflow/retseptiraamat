<script lang="ts">
	import { onMount } from 'svelte';
	import { getRecipes } from '$lib/api';
	import { showError } from '$lib/toast';

	let recipes = [];
	let loading = true;

	onMount(async () => {
		try {
			recipes = await getRecipes();
		} catch (e) {
			showError('Retseptide laadimine ebaõnnestus.');
		} finally {
			loading = false;
		}
	});
</script>

<div class="container mx-auto p-4">
	<div class="mb-6 flex items-center justify-between">
		<h1 class="text-3xl font-bold">Kõik retseptid</h1>
		<a
			href="/recipes/add"
			class="bg-primary text-secondary rounded-md px-4 py-2 font-semibold hover:text-gray-700 hover:transition"
		>
			Lisa uus retsept
		</a>
	</div>

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
				<a
					href="/recipes/{recipe.id}"
					class="flex h-full flex-col overflow-hidden rounded-lg bg-white shadow-md transition-shadow hover:shadow-lg"
				>
					<div class="flex-grow p-5">
						<h2 class="mb-2 text-xl font-semibold">{recipe.title}</h2>
						<p class="mb-4 line-clamp-2 text-gray-600">
							{recipe.description}
						</p>

						<div class="mt-auto">
							<div class="mt-4 flex flex-wrap gap-2">
								{#each Object.keys(recipe.ingredients || {}).slice(0, 3) as ingredient}
									<span class="rounded-full bg-gray-100 px-2 py-1 text-xs text-gray-700">
										{ingredient}
									</span>
								{/each}
								{#if Object.keys(recipe.ingredients || {}).length > 3}
									<span class="rounded-full bg-gray-100 px-2 py-1 text-xs text-gray-700">
										+{Object.keys(recipe.ingredients || {}).length - 3} veel
									</span>
								{/if}
							</div>
							<div class="mt-4 text-xs text-gray-500">
								{new Date(recipe.createdAt).toLocaleDateString()}
							</div>
						</div>
					</div>
				</a>
			{/each}
		</div>
	{/if}
</div>
