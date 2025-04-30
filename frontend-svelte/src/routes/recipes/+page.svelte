<script lang="ts">
	import { onMount } from 'svelte';
	import { getRecipes } from '$lib/api';
	import { showError } from '$lib/toast';

	interface Recipe {
		id: number;
		title: string;
		description: string;
		ingredients: Record<string, number>;
		createdAt: string;
	}

	let recipes: Recipe[] = [];
	let allRecipes: Recipe[] = []; // Store all recipes for filtering
	let loading = true;
	let searchTerm = '';

	// Filter recipes based on search term
	$: filteredRecipes = searchTerm
		? allRecipes.filter(
				(recipe) =>
					recipe.title.toLowerCase().includes(searchTerm.toLowerCase()) ||
					recipe.description.toLowerCase().includes(searchTerm.toLowerCase()) ||
					Object.keys(recipe.ingredients || {}).some((ingredient) =>
						ingredient.toLowerCase().includes(searchTerm.toLowerCase())
					)
			)
		: allRecipes;

	onMount(async () => {
		try {
			allRecipes = await getRecipes();
			recipes = allRecipes;
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

	<!-- Search input -->
	<div class="mb-6">
		<div class="relative">
			<input
				type="text"
				placeholder="Otsi retsepte nimetuse, kirjelduse või koostisosade järgi..."
				bind:value={searchTerm}
				class="focus:border-primary focus:ring-primary w-full rounded-md border-gray-300 py-2 pl-10 shadow-sm"
			/>
			<div class="absolute inset-y-0 left-0 flex items-center pl-3">
				<svg
					xmlns="http://www.w3.org/2000/svg"
					class="h-5 w-5 text-gray-400"
					fill="none"
					viewBox="0 0 24 24"
					stroke="currentColor"
				>
					<path
						stroke-linecap="round"
						stroke-linejoin="round"
						stroke-width="2"
						d="M21 21l-6-6m2-5a7 7 0 11-14 0 7 7 0 0114 0z"
					/>
				</svg>
			</div>
		</div>
	</div>

	{#if loading}
		<p>Laen andmeid...</p>
	{:else if filteredRecipes.length === 0}
		<div class="rounded-md bg-gray-50 p-6 text-center">
			{#if searchTerm}
				<p class="mb-2 text-gray-500">Otsingule "{searchTerm}" ei leitud ühtegi retsepti</p>
				<button
					on:click={() => (searchTerm = '')}
					class="text-primary font-medium hover:cursor-pointer hover:underline"
				>
					Tühjenda otsing
				</button>
			{:else}
				<p class="mb-2 text-gray-500">Ühtegi retsepti ei leitud</p>
				<p class="text-gray-500">Alusta oma esimese retsepti lisamisega</p>
			{/if}
		</div>
	{:else}
		<div class="grid grid-cols-1 gap-6 md:grid-cols-2 lg:grid-cols-3">
			{#each filteredRecipes as recipe}
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
