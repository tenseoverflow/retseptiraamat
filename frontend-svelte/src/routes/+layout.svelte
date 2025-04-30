<script lang="ts">
	import '../app.css';
	import { page } from '$app/stores';
	import { writable } from 'svelte/store';
	import { Toaster } from 'svelte-french-toast';

	let { children } = $props();
	const isMenuOpen = writable(false);
</script>

<div class="flex min-h-screen flex-col">
	<nav class="text-secondary">
		<div class="mx-auto max-w-7xl px-4 sm:px-6 lg:px-8">
			<div class="flex h-24 items-center justify-between">
				<!-- Logo/Brand for desktop -->
				<div class="hidden items-center space-x-4 md:flex">
					<a
						href="/"
						class="hover:bg-primary-dark rounded-md px-3 py-2 text-sm font-medium {$page.url
							.pathname === '/'
							? 'text-gray-700'
							: ''}"
					>
						Kodu
					</a>
					<a
						href="/recipes"
						class="hover:bg-primary-dark rounded-md px-3 py-2 text-sm font-medium {$page.url.pathname.startsWith(
							'/recipes'
						)
							? 'text-gray-700'
							: ''}"
					>
						Retseptid
					</a>
					<a
						href="/fridge"
						class="hover:bg-primary-dark rounded-md px-3 py-2 text-sm font-medium {$page.url
							.pathname === '/fridge'
							? 'text-gray-700'
							: ''}"
					>
						Külmkapi sisu
					</a>
				</div>

				<!-- Logo/Brand for mobile -->
				<div class="md:hidden">
					<a href="/" class="text-xl font-bold">Retseptiraamat</a>
				</div>

				<!-- Desktop Navigation -->
				<div class="hidden items-center space-x-4 md:flex">
					<a
						href="/recipes/add"
						class="flex flex-shrink-0 items-center rounded-md px-3 py-2 hover:bg-gray-200 hover:text-gray-700 hover:transition {$page
							.url.pathname === '/recipes/add'
							? 'text-gray-700'
							: ''}"
					>
						<span class="font-bold">Lisa retsept</span>
					</a>
					<a
						href="/fridge"
						class="{$page.url.pathname === '/fridge'
							? 'text-gray-700'
							: ''} bg-primary flex flex-shrink-0 items-center rounded-md px-3 py-2 hover:text-gray-700 hover:transition"
					>
						<span class="font-bold">Lisa külmkappi sisu</span>
					</a>
				</div>

				<!-- Mobile menu button -->
				<div class="flex items-center md:hidden">
					<button
						type="button"
						class="hover:bg-primary-dark inline-flex cursor-pointer items-center justify-center rounded-md p-2"
						aria-expanded={$isMenuOpen}
						on:click={() => isMenuOpen.update((value) => !value)}
					>
						<span class="sr-only">Ava menüü</span>
						<svg class="h-6 w-6" stroke="currentColor" fill="none" viewBox="0 0 24 24">
							{#if $isMenuOpen}
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
		{#if $isMenuOpen}
			<div class="md:hidden">
				<div class="space-y-1 px-2 pb-3 pt-2">
					<a
						href="/"
						class="hover:bg-primary-dark block rounded-md px-3 py-2 text-base font-medium {$page.url
							.pathname === '/'
							? 'text-primary-dark'
							: ''}"
						on:click={() => isMenuOpen.set(false)}
					>
						Kodu
					</a>
					<a
						href="/recipes"
						class="hover:bg-primary-dark block rounded-md px-3 py-2 text-base font-medium {$page.url.pathname.startsWith(
							'/recipes'
						)
							? 'text-primary-dark'
							: ''}"
						on:click={() => isMenuOpen.set(false)}
					>
						Retseptid
					</a>
					<a
						href="/fridge"
						class="hover:bg-primary-dark block rounded-md px-3 py-2 text-base font-medium {$page.url
							.pathname === '/fridge'
							? 'text-primary-dark'
							: ''}"
						on:click={() => isMenuOpen.set(false)}
					>
						Külmkapp
					</a>
					<a
						href="/recipes/add"
						class="hover:bg-primary-dark block rounded-md px-3 py-2 text-base font-medium {$page.url
							.pathname === '/recipes/add'
							? 'bg-primary-dark'
							: ''}"
						on:click={() => isMenuOpen.set(false)}
					>
						Lisa retsept
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

<Toaster />
