<script lang="ts">
	import '../app.css';
	import { page } from '$app/stores';

	let { children } = $props();
	let isMenuOpen = false;

	function toggleMenu() {
		isMenuOpen = !isMenuOpen;
	}
</script>

<div class="flex min-h-screen flex-col">
	<nav class=" text-gray-800">
		<div class="mx-auto max-w-7xl px-4 sm:px-6 lg:px-8">
			<div class="flex h-24 justify-between">
				<!-- Logo/Brand -->
				<div class="hidden items-center space-x-4 md:flex">
					<a
						href="/"
						class="hover:bg-primary-dark rounded-md px-3 py-2 text-sm font-medium {$page.url
							.pathname === '/'
							? 'text-secondary'
							: ''}"
					>
						Kodu
					</a>
					<a
						href="/recipes"
						class="hover:bg-primary-dark rounded-md px-3 py-2 text-sm font-medium {$page.url.pathname.startsWith(
							'/recipes'
						)
							? 'text-secondary'
							: ''}"
					>
						Retseptid
					</a>
					<a
						href="/fridge"
						class="hover:bg-primary-dark rounded-md px-3 py-2 text-sm font-medium {$page.url
							.pathname === '/fridge'
							? 'text-secondary'
							: ''}"
					>
						Külmkapi sisu
					</a>
				</div>

				<!-- Desktop Navigation -->
				<div class="hidden items-center space-x-4 md:flex">
					<a
						href="/recipes/add"
						class="hover:bg-primary-dark flex flex-shrink-0 items-center rounded-md px-3 py-2"
					>
						<span class="font-bold">Lisa retsept</span>
					</a>
					<a
						href="/fridge"
						class="hover:bg-primary-dark bg-primary flex flex-shrink-0 items-center rounded-md px-3 py-2"
					>
						<span class="font-bold">Lisa külmkappi sisu</span>
					</a>
				</div>

				<!-- Mobile menu button -->
				<div class="flex items-center md:hidden">
					<button
						on:click={toggleMenu}
						type="button"
						class="hover:bg-primary-dark inline-flex items-center justify-center rounded-md p-2"
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
					<a
						href="/"
						class="hover:bg-primary-dark block rounded-md px-3 py-2 text-base font-medium {$page.url
							.pathname === '/'
							? 'text-primary-dark'
							: ''}"
					>
						Home
					</a>
					<a
						href="/recipes"
						class="hover:bg-primary-dark block rounded-md px-3 py-2 text-base font-medium {$page.url.pathname.startsWith(
							'/recipes'
						)
							? 'text-primary-dark'
							: ''}"
					>
						Recipes
					</a>
					<a
						href="/fridge"
						class="hover:bg-primary-dark block rounded-md px-3 py-2 text-base font-medium {$page.url
							.pathname === '/fridge'
							? 'text-primary-dark'
							: ''}"
					>
						My Fridge
					</a>
					<a
						href="/about"
						class="hover:bg-primary-dark block rounded-md px-3 py-2 text-base font-medium {$page.url
							.pathname === '/about'
							? 'bg-primary-dark'
							: ''}"
					>
						About
					</a>
				</div>
			</div>
		{/if}
	</nav>

	<main class="flex-grow">
		<div class="mx-auto max-w-7xl px-4 py-2 sm:px-6 lg:px-8">
			{@render children()}
		</div>
	</main>
</div>
