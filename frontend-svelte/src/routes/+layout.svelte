<script lang="ts">
	import '../app.css';

	let { children } = $props();
	let isMenuOpen = false;

	function toggleMenu() {
		isMenuOpen = !isMenuOpen;
	}
</script>

<div class="flex min-h-screen flex-col">
	<nav class="bg-green-700 text-white shadow-lg">
		<div class="mx-auto max-w-7xl px-4 sm:px-6 lg:px-8">
			<div class="flex h-16 justify-between">
				<!-- Logo/Brand -->
				<div class="flex items-center">
					<a href="/" class="flex flex-shrink-0 items-center">
						<span class="text-xl font-bold">Recipe Book</span>
					</a>
				</div>

				<!-- Desktop Navigation -->
				<div class="hidden items-center space-x-4 md:flex">
					<a href="/" class="rounded-md px-3 py-2 text-sm font-medium hover:bg-green-600">Home</a>
					<a href="/recipes" class="rounded-md px-3 py-2 text-sm font-medium hover:bg-green-600"
						>Recipes</a
					>
					<a href="/fridge" class="rounded-md px-3 py-2 text-sm font-medium hover:bg-green-600"
						>My Fridge</a
					>
					<a href="/about" class="rounded-md px-3 py-2 text-sm font-medium hover:bg-green-600"
						>About</a
					>
				</div>

				<!-- Mobile menu button -->
				<div class="flex items-center md:hidden">
					<button
						on:click={toggleMenu}
						type="button"
						class="inline-flex items-center justify-center rounded-md p-2 hover:bg-green-600"
						aria-expanded="false"
					>
						<span class="sr-only">Open main menu</span>
						<svg class="h-6 w-6" stroke="currentColor" fill="none" viewBox="0 0 24 24">
							{#if isMenuOpen}
								<path
									stroke-linecap="round"
									stroke-linejoin="round"
									stroke-width="2"
									d="M6 18L18 6M6 6l12 12"
								/>
							{:else}
								<path
									stroke-linecap="round"
									stroke-linejoin="round"
									stroke-width="2"
									d="M4 6h16M4 12h16M4 18h16"
								/>
							{/if}
						</svg>
					</button>
				</div>
			</div>
		</div>

		<!-- Mobile Navigation Menu -->
		{#if isMenuOpen}
			<div class="md:hidden">
				<div class="space-y-1 px-2 pb-3 pt-2">
					<a href="/" class="block rounded-md px-3 py-2 text-base font-medium hover:bg-green-600"
						>Home</a
					>
					<a
						href="/recipes"
						class="block rounded-md px-3 py-2 text-base font-medium hover:bg-green-600">Recipes</a
					>
					<a
						href="/fridge"
						class="block rounded-md px-3 py-2 text-base font-medium hover:bg-green-600">My Fridge</a
					>
					<a
						href="/about"
						class="block rounded-md px-3 py-2 text-base font-medium hover:bg-green-600">About</a
					>
				</div>
			</div>
		{/if}
	</nav>

	<main class="flex-grow">
		<div class="mx-auto max-w-7xl px-4 py-6 sm:px-6 lg:px-8">
			{@render children()}
		</div>
	</main>

	<footer class="bg-gray-100 py-4">
		<div class="mx-auto max-w-7xl px-4 sm:px-6 lg:px-8">
			<p class="text-center text-sm text-gray-500">
				© {new Date().getFullYear()} Recipe Book. All rights reserved.
			</p>
		</div>
	</footer>
</div>
