<script lang="ts">
	import { onMount } from 'svelte';
	import { page } from '$app/stores';

	interface Recipe {
		id: number;
		title: string;
		description: string;
		ingredients: Record<string, number>;
		createdAt: string;
		updatedAt: string | null;
	}

	let recipe: Recipe | null = null;
	let loading = true;
	let error = '';

	onMount(async () => {
		const id = $page.params.id;
		try {
			const response = await fetch(`http://localhost:5036/api/Recipe/${id}`);
			if (!response.ok) {
				throw new Error(`Retsepti ei leitud (${response.status})`);
			}
			recipe = await response.json();
		} catch (e) {
			error = e instanceof Error ? e.message : 'Retsepti laadimine ebaõnnestus';
		} finally {
			loading = false;
		}
	});
</script>

<div class="container mx-auto p-4">
	<div class="mb-6">
		<a href="/recipes" class="text-primary-dark hover:text-primary inline-flex items-center">
			<svg
				xmlns="http://www.w3.org/2000/svg"
				class="mr-1 h-5 w-5"
				fill="none"
				viewBox="0 0 24 24"
				stroke="currentColor"
			>
				<path
					stroke-linecap="round"
					stroke-linejoin="round"
					stroke-width="2"
					d="M10 19l-7-7m0 0l7-7m-7 7h18"
				/>
			</svg>
			Tagasi retseptide juurde
		</a>
	</div>

	{#if error}
		<div class="mb-4 rounded border border-red-400 bg-red-100 px-4 py-3 text-red-700">
			{error}
		</div>
	{/if}

	{#if loading}
		<div class="flex h-64 items-center justify-center">
			<p class="text-lg">Laadin retsepti...</p>
		</div>
	{:else if recipe}
		<div class="overflow-hidden rounded-lg bg-white shadow-lg">
			<div class="p-8">
				<h1 class="mb-4 text-3xl font-bold">{recipe.title}</h1>

				<div class="mb-8 text-gray-600">
					<p class="whitespace-pre-line text-lg">{recipe.description}</p>
				</div>

				<div class="mb-6">
					<h2 class="mb-3 text-xl font-semibold">Koostisosad</h2>
					<div class="rounded-md bg-gray-50 p-4">
						<ul class="list-inside list-disc space-y-2">
							{#each Object.entries(recipe.ingredients) as [ingredient, amount]}
								<li class="text-gray-700">{ingredient}: {amount}</li>
							{/each}
						</ul>
					</div>
				</div>

				<div class="mt-6 border-t border-gray-200 pt-4">
					<div class="text-sm text-gray-500">
						<p>Loodud: {new Date(recipe.createdAt).toLocaleDateString()}</p>
						{#if recipe.updatedAt}
							<p>Uuendatud: {new Date(recipe.updatedAt).toLocaleDateString()}</p>
						{/if}
					</div>
				</div>
			</div>
		</div>
	{/if}
</div>
